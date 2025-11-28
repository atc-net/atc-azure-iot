namespace Atc.Azure.Iot.Sample.Modules.Contracts.OpcPublisherNodeManagerModule;

public record GetEndpointsResponse(
    List<string> Endpoints,
    bool HasMoreData);

public record Endpoint(
    string EndpointUrl,
    List<OpcNodeOnEndpoint> OpcNodes);

public record GetEndpointWithNodesResponse(
    Endpoint Endpoint,
    bool HasMoreData);

public record GetEndpointsWithEmptyOpcNodesListResponse(List<string> Endpoints);