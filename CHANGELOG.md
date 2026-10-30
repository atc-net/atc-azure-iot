# Changelog

## [3.0.0](https://github.com/atc-net/atc-azure-iot/compare/v2.0.0...v3.0.0) (2026-10-30)


### ⚠ BREAKING CHANGES

* **iotedge:** each MockModuleClientWrapper has its own twin, copied from MockTwinProperties when it is created; UpdateReportedPropertiesAsync merges the reported properties instead of replacing them, and no longer calls the desired property callback.
* **iotedge:** MethodResponseFactory writes JSON without indentation by default. Pass JsonSerializerOptions to change it.
* **iotedge:** the TransportSettingsFactory methods require a ServerCertificateValidation. Pass ServerCertificateValidation.AcceptAny to keep accepting any certificate.
* **iotedge:** BuildMqttTransportSettings connects over TCP, port 8883. Use BuildMqttWebSocketTransportSettings to keep WebSockets.
* **iot:** EdgeAgentDesiredProperties.SystemModuleVersion defaults to 1.6 instead of 1.5. It is only used when no system modules are given; pass SystemModuleVersion: "1.5" to keep 1.5.
* **iot:** both GetConfigurationContent overloads return Task<ConfigurationContent> instead of (ConfigurationContent?, string? ErrorMessage), and throw on failure.
* **dps:** CreateIndividualTpmEnrollment returns the enrollment and throws ProvisioningServiceClientHttpException on failure, instead of returning (Enrollment, ErrorMessage). GetIndividualEnrollment throws on failure instead of returning null, and DeleteIndividualEnrollment throws instead of returning false. GetIndividualEnrollments returns IReadOnlyCollection<IndividualEnrollment>. The ErrorResponse record is removed.
* **iothub:** GetDeviceRegistryStatistics, CreateDevice, UpdateDeviceTwin, AddModuleToDevice, UpdateModule and UpdateDesiredProperties return the result and throw on failure, instead of a nullable result or a (Succeeded, ...) tuple. ApplyConfigurationContentOnDevice and AddNewModules return Task and throw instead of (Succeeded, ErrorMessage). GetModulesOnIotEdgeDevice returns IReadOnlyCollection<Module>. DeleteDevice and RemoveModuleFromDevice return false only when the device or module doesn't exist, and throw on failure. GetDevice, GetDeviceTwin, GetModuleTwin and GetDeviceConnectionString throw on failure instead of returning null.
* **iotedge:** IotEdgeExecutionMode.Emulator and IsEmulatorMode() are removed. Use the Standalone environment with MockModuleClientWrapper to run a module locally without IoT Hub.

### New features

* **iot:** default the IoT Edge system modules to 1.6 ([16f672a](https://github.com/atc-net/atc-azure-iot/commit/16f672a3845e343f4994d3a766105ea9fc9d8d6a))
* **iotedge:** choose how server certificates are validated ([4c46b91](https://github.com/atc-net/atc-azure-iot/commit/4c46b91367905f41894b03e90d98098b0b62aafa))
* **iotedge:** connect MQTT over TCP and add WebSocket transport settings ([2f90c54](https://github.com/atc-net/atc-azure-iot/commit/2f90c54f2603618ea8bc78f025ac251af6473920))
* **iotedge:** make MockModuleClientWrapper usable in tests ([4f03001](https://github.com/atc-net/atc-azure-iot/commit/4f03001b6d16d23c0cb49a1bfc2948e43adc9c1a))
* **iotedge:** remove the Docker IoT Edge device emulator ([d41f0e1](https://github.com/atc-net/atc-azure-iot/commit/d41f0e1a0aecc86b1e0b90600683c311ba3a11b9))
* **iothub:** add GetModuleConnectionString ([e3ec1a9](https://github.com/atc-net/atc-azure-iot/commit/e3ec1a91c940e07ea49fa0de86a7bcd8a3c1d762))
* **iot:** support the on-failure and on-unhealthy restart policies ([3de4a8e](https://github.com/atc-net/atc-azure-iot/commit/3de4a8e09a11528ef41b86fff9259fba042e5ded))


### Bug fixes

* **dps:** throw DPS errors instead of returning null ([e899e23](https://github.com/atc-net/atc-azure-iot/commit/e899e2377078a679277b939a2932b0bcfe40f287))
* **iotedge:** match the Standalone environment as IsEnvironment does ([3f7426c](https://github.com/atc-net/atc-azure-iot/commit/3f7426c409ab1d4608e591e643398e655468737c))
* **iotedge:** write compact JSON in direct method responses ([0b51e00](https://github.com/atc-net/atc-azure-iot/commit/0b51e0041e6bd466c5227c191de70c075b1aa21b))
* **iot:** escape the message in IotHubException.ToJsonPayload ([182f0a0](https://github.com/atc-net/atc-azure-iot/commit/182f0a0fbc811718826a1c65d775afa0f1ce12bd))
* **iothub:** remove the unreachable device branch from CallMethod ([0e4d2e1](https://github.com/atc-net/atc-azure-iot/commit/0e4d2e1ed0ff0f078fd7208bca48be7e145a6c04))
* **iothub:** throw IoT Hub errors instead of returning null ([43d04be](https://github.com/atc-net/atc-azure-iot/commit/43d04be27a21d077a1aca5f2ff7b3000c3913e12))
* **iot:** read edgeHub routes as text or object with System.Text.Json ([773491f](https://github.com/atc-net/atc-azure-iot/commit/773491f5677e69c5d6700c76ddded51fb53f3071))
* **iot:** register the manifest provider with the IoT Hub services ([636a51a](https://github.com/atc-net/atc-azure-iot/commit/636a51a1c07d5aa1facc4e6cbb4b9c8252f7b1f2))
* **iot:** throw when a deployment manifest can't be read ([f341433](https://github.com/atc-net/atc-azure-iot/commit/f34143332cf34a9da84c3411779ac87d81a45c72))

## [2.0.0](https://github.com/atc-net/atc-azure-iot/compare/v1.0.182...v2.0.0) (2026-10-25)


### ⚠ BREAKING CHANGES

* Atc.Azure.IoTEdge.DeviceEmulator is no longer published.
* the packages and the CLI tool now target net10.0 only, so apps on .NET 9 can no longer use them.

### chore

* stop publishing Atc.Azure.IoTEdge.DeviceEmulator ([beb6e5a](https://github.com/atc-net/atc-azure-iot/commit/beb6e5af21f2e6ae1867aa99cfcb1c24ea42060a))
* upgrade to .NET 10 incl. nuget updates ([6e2bfc9](https://github.com/atc-net/atc-azure-iot/commit/6e2bfc91e2bf5006dd55f28afe98f6b190d25c21))


### New features

* **iothub:** add AddModuleToDevice and UpdateModule to IIoTHubService ([d7e581e](https://github.com/atc-net/atc-azure-iot/commit/d7e581e95a0dced2643cbca5bc1fd6397bbff8f4))
* **options:** add SectionName to IotHubOptions and DeviceProvisioningServiceOptions and redact their connection strings ([84e91de](https://github.com/atc-net/atc-azure-iot/commit/84e91de9b2ef6c2dbec988047792ad3d4a5fe28a))


### Bug fixes

* **cli:** make the command ExecuteAsync overrides public ([570ba4e](https://github.com/atc-net/atc-azure-iot/commit/570ba4ead3650759fb31ada16d17746401736dae))
* **iothub:** keep the connection string out of the InvalidConfigurationException message ([c8569e3](https://github.com/atc-net/atc-azure-iot/commit/c8569e369cec6bd39e5521b8dcbe66382349c55c))
