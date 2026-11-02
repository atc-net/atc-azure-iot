# Changelog

## [3.0.0](https://github.com/atc-net/atc-azure-iot/compare/v2.0.0...v3.0.0) (2026-11-02)


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

* **iot:** default the IoT Edge system modules to 1.6 ([2bfd741](https://github.com/atc-net/atc-azure-iot/commit/2bfd741ad35315ce8b9f9ba32daf736110511f49))
* **iotedge:** choose how server certificates are validated ([3f0665e](https://github.com/atc-net/atc-azure-iot/commit/3f0665e073d19c4bff194c61279aa15378446a3f))
* **iotedge:** connect MQTT over TCP and add WebSocket transport settings ([5fbc1e0](https://github.com/atc-net/atc-azure-iot/commit/5fbc1e0910248b3107e94bc208eca6c640b6d052))
* **iotedge:** make MockModuleClientWrapper usable in tests ([f16ce15](https://github.com/atc-net/atc-azure-iot/commit/f16ce1598c79cb5ac1e665360dafe68a70068d14))
* **iotedge:** remove the Docker IoT Edge device emulator ([084e125](https://github.com/atc-net/atc-azure-iot/commit/084e1250ce6026c193ca6d2aecbd1c9371a8432c))
* **iothub:** add GetModuleConnectionString ([2158c09](https://github.com/atc-net/atc-azure-iot/commit/2158c090e60fec85c636e91906868b7cd14a4da6))
* **iothub:** read IoT Hub's error message from the SDK exception ([1f8ff82](https://github.com/atc-net/atc-azure-iot/commit/1f8ff82ee14f31412852e3db5ae9af0218bf86db))
* **iot:** support the on-failure and on-unhealthy restart policies ([7613c3c](https://github.com/atc-net/atc-azure-iot/commit/7613c3c94cadd0f08cb24dc587385c8813693f3a))


### Bug fixes

* **dps:** throw DPS errors instead of returning null ([292e39f](https://github.com/atc-net/atc-azure-iot/commit/292e39f1be1fcba422f3a521417050199b1c43ba))
* **iotedge:** match the Standalone environment as IsEnvironment does ([9d6c3c1](https://github.com/atc-net/atc-azure-iot/commit/9d6c3c1027954d90368610986477c57ca83bb6a3))
* **iotedge:** write compact JSON in direct method responses ([83461d6](https://github.com/atc-net/atc-azure-iot/commit/83461d611a0163bcfba7708492e6aa0797fc7788))
* **iot:** escape the message in IotHubException.ToJsonPayload ([782ba0b](https://github.com/atc-net/atc-azure-iot/commit/782ba0b5bc2f90866787ff39ef15dcb0d00b01eb))
* **iothub:** remove the unreachable device branch from CallMethod ([9ff35a4](https://github.com/atc-net/atc-azure-iot/commit/9ff35a43e2694f8abd76915b6eba02de7d6536de))
* **iothub:** throw IoT Hub errors instead of returning null ([daf8f82](https://github.com/atc-net/atc-azure-iot/commit/daf8f825de671a5db3f8d01152c3ca866642b30e))
* **iot:** read edgeHub routes as text or object with System.Text.Json ([2cee34e](https://github.com/atc-net/atc-azure-iot/commit/2cee34e4b2f34b752733b2b52dc95a0399cdfa36))
* **iot:** register the manifest provider with the IoT Hub services ([b7a2cce](https://github.com/atc-net/atc-azure-iot/commit/b7a2ccee3e2ece1eab476fe233de3c25661f4322))
* **iot:** throw when a deployment manifest can't be read ([07f4623](https://github.com/atc-net/atc-azure-iot/commit/07f4623f3894f9b8e2f77829c0dd4646931e874b))
* **sample:** write the temperature with a decimal point ([3177b6e](https://github.com/atc-net/atc-azure-iot/commit/3177b6e7c2998a4c3bbcfc12ace47c9c0b84dd1d))

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
