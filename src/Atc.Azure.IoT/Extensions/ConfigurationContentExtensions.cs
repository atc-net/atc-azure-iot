namespace Atc.Azure.IoT.Extensions;

/// <summary>
/// Convenience helpers for composing IoT Edge <see cref="ConfigurationContent"/> objects.
/// All methods return the same instance to allow fluent chaining.
/// </summary>
public static class ConfigurationContentExtensions
{
    /// <summary>
    /// Adds the edgeAgent desired-properties section to <paramref name="configurationContent"/>.
    /// </summary>
    /// <param name="configurationContent">The configuration being built.</param>
    /// <param name="edgeAgentDesiredProperties">The desired properties for the edgeAgent.</param>
    /// <returns>The same <see cref="ConfigurationContent"/> instance.</returns>
    public static ConfigurationContent SetEdgeAgent(
        this ConfigurationContent configurationContent,
        EdgeAgentDesiredProperties edgeAgentDesiredProperties)
    {
        configurationContent.ModulesContent ??= new Dictionary<string, IDictionary<string, object>>(StringComparer.Ordinal);
        configurationContent.ModulesContent.Add(EdgeAgentConstants.ModuleId, new Dictionary<string, object>(StringComparer.Ordinal)
        {
            [PropertyNames.Manifest.PropertiesDesired] = GetEdgeAgentConfiguration(edgeAgentDesiredProperties),
        });

        return configurationContent;
    }

    /// <summary>
    /// Adds the edgeHub desired-properties section to <paramref name="configurationContent"/>.
    /// </summary>
    /// <param name="configurationContent">The configuration being built.</param>
    /// <param name="edgeHubDesiredProperties">The desired properties for the edgeHub.</param>
    /// <returns>The same <see cref="ConfigurationContent"/> instance.</returns>
    public static ConfigurationContent SetEdgeHub(
        this ConfigurationContent configurationContent,
        EdgeHubDesiredProperties edgeHubDesiredProperties)
    {
        var routes = new Dictionary<string, object>(StringComparer.Ordinal);
        edgeHubDesiredProperties.Routes?.ForEach(x =>
        {
            if (!x.Priority.HasValue && !x.TimeToLiveSecs.HasValue)
            {
                // Shorthand format
                routes[x.Name] = x.Value;
                return;
            }

            var dict = new Dictionary<string, object>(StringComparer.Ordinal)
            {
                [PropertyNames.Manifest.Route] = x.Value,
            };

            if (x.Priority.HasValue)
            {
                dict[PropertyNames.Manifest.Priority] = x.Priority.Value;
            }

            if (x.TimeToLiveSecs.HasValue)
            {
                dict[PropertyNames.Manifest.TimeToLiveSecs] = x.TimeToLiveSecs.Value;
            }

            routes[x.Name] = dict;
        });

        configurationContent.ModulesContent ??= new Dictionary<string, IDictionary<string, object>>(StringComparer.Ordinal);
        configurationContent.ModulesContent.Add(EdgeHubConstants.ModuleId, new Dictionary<string, object>(StringComparer.Ordinal)
        {
            [PropertyNames.Manifest.PropertiesDesired] = new Dictionary<string, object>(StringComparer.Ordinal)
            {
                [PropertyNames.Manifest.SchemaVersion] = edgeHubDesiredProperties.SchemaVersion,
                [PropertyNames.Manifest.Routes] = routes,
                [PropertyNames.Manifest.StoreAndForwardConfiguration] = new Dictionary<string, object>(StringComparer.Ordinal)
                {
                    [PropertyNames.Manifest.TimeToLiveSecs] = edgeHubDesiredProperties.StoreAndForwardTimeToLiveSecs,
                },
            },
        });

        return configurationContent;
    }

    /// <summary>
    /// Adds a regular module’s desired properties to <paramref name="configurationContent"/>.
    /// </summary>
    /// <param name="configurationContent">The configuration being built.</param>
    /// <param name="moduleSpecificationDesiredProperties">The module specification.</param>
    /// <returns>The same <see cref="ConfigurationContent"/> instance.</returns>
    public static ConfigurationContent SetModuleDesiredProperty(
        this ConfigurationContent configurationContent,
        ModuleSpecificationDesiredProperties moduleSpecificationDesiredProperties)
    {
        configurationContent.ModulesContent ??= new Dictionary<string, IDictionary<string, object>>(StringComparer.Ordinal);
        configurationContent.ModulesContent.Add(moduleSpecificationDesiredProperties.Name, new Dictionary<string, object>(StringComparer.Ordinal)
        {
            [PropertyNames.Manifest.PropertiesDesired] = moduleSpecificationDesiredProperties.DesiredProperties,
        });

        return configurationContent;
    }

    private static Dictionary<string, object> GetEdgeAgentConfiguration(
        EdgeAgentDesiredProperties edgeAgentDesiredProperties)
    {
        var manifestRegistryCredentials = edgeAgentDesiredProperties.RegistryCredentials.ToDictionary(
            x => x.Name,
            x => new Dictionary<string, string>(StringComparer.Ordinal)
            {
                [PropertyNames.Manifest.Address] = x.Address,
                [PropertyNames.Manifest.Username] = x.UserName,
                [PropertyNames.Manifest.Password] = x.Password,
            },
            StringComparer.Ordinal);

        var modules = edgeAgentDesiredProperties.EdgeModuleSpecifications.ToDictionary(
            x => x.Name,
            BuildDockerModule,
            StringComparer.Ordinal);

        var config = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            [PropertyNames.Manifest.SchemaVersion] = edgeAgentDesiredProperties.SchemaVersion,
            [PropertyNames.Runtime] = new Dictionary<string, object>(StringComparer.Ordinal)
            {
                [PropertyNames.Manifest.Type] = PropertyNames.Manifest.Docker,
                [PropertyNames.ModuleSettings] = new Dictionary<string, object>(StringComparer.Ordinal)
                {
                    [PropertyNames.Manifest.LoggingOptions] = string.Empty,
                    [PropertyNames.Manifest.MinDockerVersion] = edgeAgentDesiredProperties.MinDockerVersion,
                    [PropertyNames.Manifest.RegistryCredentials] = manifestRegistryCredentials,
                },
            },
            [PropertyNames.SystemModules] = GetSystemModuleSpecification(edgeAgentDesiredProperties),
        };

        if (modules.Count > 0)
        {
            config[PropertyNames.Modules] = modules;
        }

        return config;
    }

    private static Dictionary<string, object> GetSystemModuleSpecification(
        EdgeAgentDesiredProperties edgeAgentDesiredProperties)
    {
        if (edgeAgentDesiredProperties.EdgeSystemModuleSpecifications.Count == 0)
        {
            // No custom system-module specs → compose defaults and return
            return new Dictionary<string, object>(StringComparer.Ordinal)
            {
                [PropertyNames.Manifest.EdgeAgent] = new Dictionary<string, object>(StringComparer.Ordinal)
                {
                    [PropertyNames.Manifest.Type] = PropertyNames.Manifest.Docker,
                    [PropertyNames.ModuleSettings] = new Dictionary<string, object>(StringComparer.Ordinal)
                    {
                        [PropertyNames.Manifest.Image] = $"mcr.microsoft.com/azureiotedge-agent:{edgeAgentDesiredProperties.SystemModuleVersion}",
                        [PropertyNames.Manifest.CreateOptions] = edgeAgentDesiredProperties.EdgeAgentCreateOptions,
                    },
                },
                [PropertyNames.Manifest.EdgeHub] = new Dictionary<string, object>(StringComparer.Ordinal)
                {
                    [PropertyNames.Manifest.Type] = PropertyNames.Manifest.Docker,
                    [PropertyNames.Manifest.Status] = ModuleStatus.Running.ToStringLowerCase(),
                    [PropertyNames.Manifest.RestartPolicy] = RestartPolicy.Always.ToStringLowerCase(),
                    [PropertyNames.ModuleSettings] = new Dictionary<string, object>(StringComparer.Ordinal)
                    {
                        [PropertyNames.Manifest.Image] = $"mcr.microsoft.com/azureiotedge-hub:{edgeAgentDesiredProperties.SystemModuleVersion}",
                        [PropertyNames.Manifest.CreateOptions] = edgeAgentDesiredProperties.EdgeHubCreateOptions,
                    },
                },
            };
        }

        return edgeAgentDesiredProperties.EdgeSystemModuleSpecifications.ToDictionary<EdgeModuleSpecification, string, object>(
            x => x.Name,
            BuildDockerModule,
            StringComparer.Ordinal);
    }

    private static Dictionary<string, object> BuildDockerModule(
        EdgeModuleSpecification edgeModuleSpecification)
    {
        var module = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            [PropertyNames.Manifest.Version] = edgeModuleSpecification.Version,
            [PropertyNames.Manifest.Type] = PropertyNames.Manifest.Docker,
            [PropertyNames.Manifest.Status] = edgeModuleSpecification.Status.ToString().ToLowerInvariant(),
            [PropertyNames.Manifest.RestartPolicy] = edgeModuleSpecification.RestartPolicy.ToString().ToLowerInvariant(),
        };

        var settings = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            [PropertyNames.Manifest.Image] = edgeModuleSpecification.Image,
        };

        if (!string.IsNullOrWhiteSpace(edgeModuleSpecification.CreateOptions))
        {
            try
            {
                // Try to parse; use JToken so the serializer keeps it as JSON
                settings[PropertyNames.Manifest.CreateOptions] = Newtonsoft.Json.Linq.JToken.Parse(edgeModuleSpecification.CreateOptions);
            }
            catch
            {
                // not valid JSON → keep the raw string
                settings[PropertyNames.Manifest.CreateOptions] = edgeModuleSpecification.CreateOptions;
            }
        }

        module[PropertyNames.ModuleSettings] = settings;

        if (edgeModuleSpecification.StartupOrder.HasValue)
        {
            module[PropertyNames.ModuleStartupOrder] = edgeModuleSpecification.StartupOrder.Value;
        }

        var env = BuildEnvironmentVariables(edgeModuleSpecification.EnvironmentVariables);
        if (env.Count > 0)
        {
            module[PropertyNames.ModuleEnvironment] = env;
        }

        return module;
    }

    private static Dictionary<string, object> BuildEnvironmentVariables(
        IReadOnlyCollection<EnvironmentVariable> variables)
        => variables.ToDictionary(
            x => x.Name,
            object (x) => new { value = x.Value },
            StringComparer.Ordinal);
}