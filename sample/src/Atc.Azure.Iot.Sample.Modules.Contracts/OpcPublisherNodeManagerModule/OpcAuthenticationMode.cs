namespace Atc.Azure.Iot.Sample.Modules.Contracts.OpcPublisherNodeManagerModule;

/// <summary>
/// Enum that defines the authentication method.
/// </summary>
/// <remarks>
/// Ref: https://raw.githubusercontent.com/Azure/Industrial-IoT/72bede29649073e53c7d38ab7d44c0ea6a1eff16/components/opc-ua/src/Microsoft.Azure.IIoT.OpcUa/src/Publisher/Models/OpcAuthenticationMode.cs .
/// </remarks>
public enum OpcAuthenticationMode
{
    /// <summary>
    /// Anonymous authentication.
    /// </summary>
    Anonymous,

    /// <summary>
    /// Username/Password authentication.
    /// </summary>
    UsernamePassword,
}