# TriplePlay IPTV Server Plugin (c) 2021
# *** UNDER DEVELOPMENT ***

## License

Provided under MIT license

## Device Specific Information

Currently underdevelopment

### Communication Settings

| Setting      | Value       |
|--------------|-------------|
| Default IP   | 169.254.1.1 |
| Default Port | 23          |
| Username     | admin       |
| Password     | password    |

#### Plugin Valid Communication methods

```c#
Http
Https
```

### Plugin Configuration Object

Update the configuration object as needed for the plugin being developed.

```json
{
	"devices": [
		{
			"key": "essentialsPluginKey",
			"name": "Essentials Plugin Name",
			"type": "essentialsPluginTypeName",
			"group": "pluginDevices",
			"properties": {
				"control": {
					"method": "See PepperDash.Core.eControlMethod for valid control methods",
					"controlPortDevKey": "exampleControlPortDevKey",
					"controlPortNumber": 1,
					"comParams": {
						"baudRate": 9600,
						"dataBits": 8,
						"stopBits": 1,
						"parity": "None",
						"protocol": "RS232",
						"hardwareHandshake": "None",
						"softwareHandshake": "None"
					},
					"tcpSshProperties": {
						"address": "172.22.0.101",
						"port": 22,
						"username": "admin",
						"password": "password",
						"autoReconnect": true,
						"autoReconnectIntervalMs": 10000
					}
				},
				"pollTimeMs": 30000,
				"warningTimeoutMs": 180000,
				"errorTimeoutMs": 300000,
				"pluginCollection": {
					"item1": {
						"name": "Item 1",
						"value": 1
					},
					"item2": {
						"name": "Item 2",
						"value": 2
					}
				}
			}
		}		
	]
}
```

### Plugin Bridge Configuration Object

Update the bridge configuration object as needed for the plugin being developed.

```json
{
	"devices": [
		{
			"key": "essentialsPluginBridgeKey",
			"name": "Essentials Plugin Bridge Name",
			"group": "api",
			"type": "eiscApiAdvanced",
			"properties": {
				"control": {
					"ipid": "1A",
					"tcpSshProperties": {
						"address": "127.0.0.2",
						"port": 0
					}
				},
				"devices": [
					{
						"deviceKey": "essentialsPluginKey",
						"joinStart": 1
					}
				]
			}
		}
	]
}
```

### SiMPL EISC Bridge Map

The selection below documents the digital, analog, and serial joins used by the SiMPL EISC. Update the bridge join maps as needed for the plugin being developed.

#### Digitals
| dig-o (Input/Triggers)                | I/O | dig-i (Feedback) |
|---------------------------------------|-----|------------------|
|                                       | 1   | Is Online        |
| Connect (Held) / Disconnect (Release) | 2   | Connected        |
|                                       | 3   |                  |
|                                       | 4   |                  |
|                                       | 5   |                  |
#### Analogs
| an_o (Input/Triggers) | I/O | an_i (Feedback) |
|-----------------------|-----|-----------------|
|                       | 1   | Socket Status   |
|                       | 2   | Monitor Status  |
|                       | 3   |                 |
|                       | 4   |                 |
|                       | 5   |                 |


#### Serials
| serial-o (Input/Triggers) | I/O | serial-i (Feedback) |
|---------------------------|-----|---------------------|
|                           | 1   | Device Name         |
|                           | 2   |                     |
|                           | 3   |                     |
|                           | 4   |                     |
|                           | 5   |                     |

