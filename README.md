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
				"presetsMaxAllowed": 24,	// if not defined, plugin will default to 24 to match the bridge
				"presets": {
					"1": {
						"name": "Preset 1",
						"icon": 1,
						"channel": 101
					},
					"24": {
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
<!-- START Minimum Essentials Framework Versions -->
### Minimum Essentials Framework Versions

- 2.12.1
<!-- END Minimum Essentials Framework Versions -->
<!-- START Config Example -->
### Config Example

```json
{
    "key": "GeneratedKey",
    "uid": 1,
    "name": "GeneratedName",
    "type": "tripleplay",
    "group": "Group",
    "properties": {
        "control": "SampleValue",
        "pollTimeMs": 0,
        "warningTimeoutMs": 0,
        "errorTimeoutMs": 0,
        "stbId": 0,
        "presetsMaxAllowed": 0,
        "Presets": {
            "SampleValue": {
                "Id": "SampleValue",
                "ChannelNumber": "SampleValue",
                "Name": "SampleString",
                "IsFavorite": true,
                "IconPath": "SampleString",
                "IsWatchable": true
            }
        }
    }
}
```
<!-- END Config Example -->
<!-- START Supported Types -->
### Supported Types

- tripleplay
- tripleplayiptv
<!-- END Supported Types -->
<!-- START Join Maps -->
### Join Maps

#### Digitals

| Join | Type (RW) | Description |
| --- | --- | --- |
| 1 | R | Is online feedback |
| 6 | R | Reboot client |
| 7 | R | Get a list of all IPTV services configured |
| 11 | R | Channel up |
| 12 | R | Channel down |
| 16 | R | Volume up |
| 17 | R | Volume down |
| 18 | R | Volume mute toggle |
| 19 | R | Volume mute on set and feedback |
| 20 | R | Volume mute off set and feedback |
| 21 | R | RCU key press Power |
| 22 | R | RCU key press Channel Up |
| 23 | R | RCU key press Channel Down |
| 24 | R | RCU key press Guide |
| 25 | R | RCU key press TV |
| 26 | R | RCU key press Home |
| 27 | R | RCU key press navigation up |
| 28 | R | RCU key press navigation down |
| 29 | R | RCU key press navigation left |
| 30 | R | RCU key press navigation right |
| 31 | R | RCU key press navigation OK |
| 32 | R | RCU key press red |
| 33 | R | RCU key press green |
| 34 | R | RCU key press yellow |
| 35 | R | RCU key press blue |
| 36 | R | RCU key press green |
| 37 | R | RCU key press stop |
| 38 | R | RCU key press pause |
| 39 | R | RCU key press rewind |
| 40 | R | RCU key press forward |
| 41 | R | RCU key press back |
| 42 | R | RCU key press info |
| 43 | R | RCU key press PVR |
| 44 | R | RCU key press record |
| 45 | R | RCU key press titles |
| 46 | R | RCU key press source |
| 48 | R | RCU key press page up |
| 49 | R | RCU key press page down |
| 50 | R | RCU key press 0-10 |
| 71 | R | Presets, 1-24, select and feedback (icon and label) |
| 101 | R | TV control power on |
| 102 | R | TV control power off |
| 111 | R | TV control input hdmi 1 |
| 112 | R | TV control input hdmi 2 |
| 113 | R | TV control input hdmi 4 |
| 114 | R | TV control input hdmi 4 |
| 115 | R | TV control input hdmi |
| 116 | R | TV control input DVI |
| 117 | R | TV control input Display Port |
| 118 | R | TV control input PC |
| 119 | R | TV control input vga |
| 120 | R | TV control input media |

#### Analogs

| Join | Type (RW) | Description |
| --- | --- | --- |
| 1 | R | Socket status feedback |
| 2 | R | Monitor status feedback |
| 6 | R | Settop box ID set and feedback |
| 7 | R | Response code feedback |
| 11 | R | Channel direct select |
| 16 | R | Volume direct set |
| 101 | R | TV Volume set and feedback |

#### Serials

| Join | Type (RW) | Description |
| --- | --- | --- |
| 1 | R | Device Name |
| 7 | R | Response content feedback |
| 8 | R | Response error feedback |
| 41 | R | Preset Icon Paths |
<!-- END Join Maps -->
<!-- START Interfaces Implemented -->
### Interfaces Implemented

- IRestfulComms
<!-- END Interfaces Implemented -->
<!-- START Base Classes -->
### Base Classes

- EssentialsBridgeableDevice
- JoinMapBaseAdvanced
- EventArgs
<!-- END Base Classes -->
<!-- START Public Methods -->
### Public Methods

- public void GetPresets()
- public void SendText(string method)
- public void SendText(string method, bool boolParam)
- public void SendText(string method, string strParam)
- public void SendText(string method, int? intParam)
- public void SendText(string method, string strParam, int? intParam)
- public void ChangeStbId(int id)
- public void Poll()
- public void RebootClient()
- public void GetAllServices()
- public void ChannelUp()
- public void ChannelDown()
- public void ChannelSet(int channel)
- public void VolumeUp()
- public void VolumeDown()
- public void VolumeSet(int level)
- public void VolumeMuteToggle()
- public void VolumeMuteOn()
- public void VolumeMuteOff()
- public void GetVolumeMuteState()
- public void RcuKeyPress(RcuKeys key)
- public void RcuKpNumbers(int number)
- public void PresetSelect(uint index)
- public void TvPowerOn()
- public void TvPowerOff()
- public void GetTvPowerStatus()
- public void TvInputSelect(TvControlInputs input)
- public void TvVolumeSet(int level)
- public void SendRequest(string requestType, string path, string content)
- public void SendRequest(string request, string contentString)
- public void SendRequest(string requestType, string path, string content)
- public void SendRequest(string request, string contentString)
<!-- END Public Methods -->
<!-- START Bool Feedbacks -->

<!-- END Bool Feedbacks -->
<!-- START Int Feedbacks -->
### Int Feedbacks

- StbIdFeedback
- ResponseCodeFeedback
<!-- END Int Feedbacks -->
<!-- START String Feedbacks -->
### String Feedbacks

- ResponseContentStringFeedback
- ResponseErrorFeedback
<!-- END String Feedbacks -->
