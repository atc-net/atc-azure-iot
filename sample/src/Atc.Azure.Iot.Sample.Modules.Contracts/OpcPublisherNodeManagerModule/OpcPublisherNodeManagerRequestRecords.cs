namespace Atc.Azure.Iot.Sample.Modules.Contracts.OpcPublisherNodeManagerModule;

public record ConfigurationFileEntry(Uri? EndpointUrl)
{
    public bool? UseSecurity { get; set; }

    public string? OpcAuthenticationMode { get; set; }

    public string? OpcAuthenticationUsername { get; set; }

    public string? OpcAuthenticationPassword { get; set; }

    public List<OpcNodeOnEndpoint>? OpcNodes { get; set; }
}

public record NodeToUpdate(
    string NodeId,
    int OpcSamplingInterval,
    int OpcPublishingInterval);

public record NodeToRemove(string NodeId);

public record OpcNodeOnEndpoint(
    string Id,
    string DisplayName,
    bool? SkipFirst = null)
{
    public int? OpcSamplingInterval { get; set; }

    public int? OpcPublishingInterval { get; set; }
}

public record AddEndpointRequest(
    string EndpointUrl,
    string? UserName,
    string? Password);

public record AddNodeToEndpointRequest(
    string EndpointUrl,
    OpcNodeOnEndpoint OpcNode);

public record AddNodesToEndpointRequest(
    string EndpointUrl,
    IList<OpcNodeOnEndpoint> Nodes);

public record GetEndpointsRequest(int PageIndex);

public record GetEndpointWithNodesRequest(
    string EndpointUrl,
    int PageIndex);

public record UpdateNodeOnEndpointRequest(
    string EndpointUrl,
    NodeToUpdate Node);

public record UpdateNodesOnEndpointRequest(
    string EndpointUrl,
    IList<NodeToUpdate> Nodes);

public record RemoveNodeFromEndpointRequest(
    string EndpointUrl,
    string NodeId);

public record RemoveAllNodesFromEndpointRequest(string EndpointUrl);

public record RemoveNodesFromEndpointRequest(
    string EndpointUrl,
    IList<NodeToRemove> Nodes);

public record RemoveEndpointRequest(string EndpointUrl);