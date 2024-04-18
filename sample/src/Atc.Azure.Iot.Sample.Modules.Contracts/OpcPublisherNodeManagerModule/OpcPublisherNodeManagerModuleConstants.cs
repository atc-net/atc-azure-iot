namespace Atc.Azure.Iot.Sample.Modules.Contracts.OpcPublisherNodeManagerModule;

public static class OpcPublisherNodeManagerModuleConstants
{
    public const string ModuleId = "PublishedNodeManager";

    public const string DirectMethodGetEndpoints = "GetEndpoints";

    public const string DirectMethodGetEndpointWithNodes = "GetEndpointWithNodes";

    public const string DirectMethodGetEndpointsWithEmptyOpcNodesList = "GetEndpointsWithEmptyOpcNodesList";

    public const string DirectMethodAddEndpoint = "AddEndpoint";

    public const string DirectMethodAddNodeToEndpoint = "AddNodeToEndpoint";

    public const string DirectMethodAddNodesToEndpoint = "AddNodesToEndpoint";

    public const string DirectMethodUpdateNodeOnEndpoint = "UpdateNodeOnEndpoint";

    public const string DirectMethodUpdateNodesOnEndpoint = "UpdateNodesOnEndpoint";

    public const string DirectMethodRemoveEndpoint = "RemoveEndpoint";

    public const string DirectMethodRemoveNodeFromEndpoint = "RemoveNodeFromEndpoint";

    public const string DirectMethodRemoveNodesFromEndpoint = "RemoveNodesFromEndpoint";

    public const string DirectMethodRemoveAllNodesFromEndpoint = "RemoveAllNodesFromEndpoint";
}