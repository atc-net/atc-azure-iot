# Changelog

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
