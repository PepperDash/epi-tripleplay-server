# TriplePlay IPTV Server Plugin (c) 2021

## License

Provided under MIT license

## Device Specific Information

All commands are sent to the TriplePlay server.  The server will pass the command to the IPTV client associated with the `stbId` as defined.

### Communication Settings

| Setting      | Value           |
| ------------ | --------------- |
| Default IP   | server specific |
| Default Port | 80              |
| Username     | admin           |
| Password     | password        |

### Plugin Valid Type Names

```c#
tripleplayiptv
tripleplay
```
#### Plugin Valid Communication methods

```c#
http
https
```

### Plugin Configuration Object

Update the configuration object as needed for the plugin being developed.

```json
{
	"devices": [
		{
			"key": "iptv-server-1",
			"name": "TriplePlay IPTV Server",
			"type": "tripleplayiptv",
			"group": "pluginDevices",
			"properties": {
				"control": {
					"method": "http",
					"tcpSshProperties": {
						"address": "10.1.2.3",
						"port": 80,
						"username": "",
						"password": "",
						"autoReconnect": false,
						"autoReconnectIntervalMs": 10000
					}
				},
				"stbId": 3
			}
		}		
	]
}
```

#### Optional configuration propeties
The following properties are optional and do not need to be defined.  

The `presets` dictionary will be automatically populated by the `GetAllServices` query, but can be 
defined via config if needed.

```json
{
	"devices": [
		{
			"key": "iptv-server-1",
			"name": "TriplePlay IPTV Server",
			"type": "tripleplayiptv",
			"group": "pluginDevices",
			"properties": {				
				// if input ID's are not defined, they default to the API documented values (captured below and plugin as an ENUM)
				"inputVgaId": 103,
				"inputHdmiId": 101,
				"inputHdmi1Id": 153,
				"inputHdmi2Id": 154,
				"inputHdmi3Id": 155,
				"inputHdmi4Id": 156,
				"inputDviId": 102,
				"inputDisplayPortId": 139,
				"inputPcId": 160,
				"inputMediaId": 158,		
				// if presetMaxAllowed is not defined, plugin will default to 24 to match the bridge span
				"presetsMaxAllowed": 24,
				// presets object is optionial, polling GetAllServices will return presets ** IF DEFINED ON THE SERVER **	
				"presets": {
					"1": {
						"id": 1,
						"name": "Preset 1",
						"icon": 1,
						"channel": 101
					},
					"24": {
						"id": 24,
						"name": "Preset 24",
						"icon": 24,
						"channel": 124
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
			"key": "device-bridge-1",
			"name": "Essentials Device Bridge",
			"group": "api",
			"type": "eiscApiAdvanced",
			"properties": {
				"control": {
					"ipid": "B0",
					"tcpSshProperties": {
						"address": "127.0.0.2",
						"port": 0
					}
				},
				"devices": [
					{
						"deviceKey": "iptv-server-1",
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
| dig-o (Input/Triggers) | I/O     | dig-i (Feedback)      |
| ---------------------- | ------- | --------------------- |
|                        | 1       | Is Online             |
| Reboot Client          | 6       |                       |
| Get All Services       | 7       |                       |
| Channel Up             | 11      |                       |
| Channel Down           | 12      |                       |
| Volume Up              | 16      |                       |
| Volume Down            | 17      |                       |
| Volume Mute Toggle     | 18      |                       |
| Volume Mute On         | 19      |                       |
| Volume Mute Off        | 20      |                       |
| RCU Power              | 21      |                       |
| RCU Channel Up         | 22      |                       |
| RCU Channel Down       | 23      |                       |
| RCU Guide              | 24      |                       |
| RCU TV                 | 25      |                       |
| RCU Home               | 26      |                       |
| RCU D-Pad Up           | 27      |                       |
| RCU D-Pad Down         | 28      |                       |
| RCU D-Pad Left         | 29      |                       |
| RCU D-Pad Right        | 30      |                       |
| RCU D-Pad Ok           | 31      |                       |
| RCU Red                | 32      |                       |
| RCU Green              | 33      |                       |
| RCU Yellow             | 34      |                       |
| RCU Blue               | 35      |                       |
| RCU Play               | 36      |                       |
| RCU Stop               | 37      |                       |
| RCU Pause              | 38      |                       |
| RCU Rewind             | 39      |                       |
| RCU Forward            | 40      |                       |
| RCU Back               | 41      |                       |
| RCU Info               | 42      |                       |
| RCU PVR                | 43      |                       |
| RCU Record             | 44      |                       |
| RCU Titles             | 45      |                       |
| RCU Source             | 46      |                       |
| RCU Page Up            | 48      |                       |
| RCU Page Down          | 49      |                       |
| RCU KP 0               | 50      |                       |
| RCU KP 1               | 51      |                       |
| RCU KP 2               | 52      |                       |
| RCU KP 3               | 53      |                       |
| RCU KP 4               | 54      |                       |
| RCU KP 5               | 55      |                       |
| RCU KP 6               | 56      |                       |
| RCU KP 7               | 57      |                       |
| RCU KP 8               | 58      |                       |
| RCU KP 9               | 59      |                       |
| Preset (1-24) Select   | 71 - 95 | Preset (1-24) Enabled |
| TV Power On            | 101     |                       |
| TV Power Off           | 102     |                       |
| TV Input HDMI 1        | 111     |                       |
| TV Input HDMI 2        | 112     |                       |
| TV Input HDMI 3        | 113     |                       |
| TV Input HDMI 4        | 114     |                       |
| TV Input HDMI          | 115     |                       |
| TV Input DVI           | 116     |                       |
| TV Input Display Port  | 117     |                       |
| TV Input PC            | 118     |                       |
| TV Input VGA           | 119     |                       |
| TV Input Media         | 120     |                       |

#### Analogs
| an_o (Input/Triggers)       | I/O     | an_i (Feedback)                  |
| --------------------------- | ------- | -------------------------------- |
|                             | 1       | Socket Status                    |
|                             | 2       | Monitor Status                   |
| Stb ID (Set-top-box ID) set | 6       | Stb ID (Set-top-box ID) Feedback |
|                             | 7       | Response Code                    |
| Channel Set                 | 11      | Channel Feedback                 |
| Volume Set                  | 16      | Volume Feedback                  |
|                             | 71 - 95 | Preset (1-24) Channel Number     |
| TV Volume Set               | 101     | TV Volume Feedback               |


#### Serials
| serial-o (Input/Triggers) | I/O    | serial-i (Feedback)         |
| ------------------------- | ------ | --------------------------- |
|                           | 1      | Device Name                 |
|                           | 7      | Response content String     |
|                           | 8      | Response Error String       |
|                           | 41- 65 | Preset (1-24) Icon URL Path |
|                           | 71-95  | Preset (1-24) Name          |
