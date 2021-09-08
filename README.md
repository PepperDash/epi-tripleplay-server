# Internal Essentials Plugin Template (c) 2020

## License

Provided under MIT license

## Overview

Use this repo as a template when creating a new plugin for Essentials. For more information about plugins, refer to the Essentials Wiki [Plugins](https://github.com/PepperDash/Essentials/wiki/Plugins) article.

## Nuget

You must have nuget.exe installed and in the PATH environment variable to use the following command. Nuget.exe is available at nuget.org.  It is recommended to use [Scoop](https://scoop.sh/) to install nuget using the command: 

```
scoop install nuget
```

### Manually Installing Dependencies

To install dependencies once nuget.exe is installed, after cloning the template or creating your template repo, run the following command: 

```
nuget install .\packages.config -OutputDirectory .\packages -excludeVersion
```

Verify you are using the proper "\\" or "/" per the console used.  To verify that the packages installed correctly, open Essentials and make sure that all references are found, then try and build it.  **This issue will be found when using WSL on Windows10.**

Once the nuget package has been installed locally you will need to update your project references.
1. Right click on **References**
2. Select **Add Reference**
3. Browse to the **packages** folder 
4. Select the required references.

### Installing Different versions of PepperDash Essentials

If you need a different version of PepperDash Essentials, use the command:
```
nuget install PepperDashEssentials -OutputDirectory .\packages -excludeVersion -Version {versionToGet}
```

Omitting the **-Version** option will pull the latest version available.

## Github Actions

Github Action workflow Templates will build this project automatically. Any branches named `feature/*`, `release/*`, `hotfix/*` or `development` will automatically be built with the action and create a release in the repository with a version number based on the latest release on the main branch. If there are no releases yet, the version number will be 0.0.1. The version number will be modified based on what branch triggered the build:

- `feature` branch builds will be tagged with an `alpha` descriptor, with the Action run appended: `0.0.1-alpha-1`
- `development` branch builds will be tagged with a `beta` descriptor, with the Action run appended: `0.0.1-beta-2`
- `release` branches will be tagged with an `rc` descriptor, with the Action run appended: `0.0.1-rc-3`
- `hotfix` branch builds will be tagged with a `hotfix` descriptor, with the Action run appended: `0.0.1-hotfix-4`

Builds on the Main branch will ONLY be triggered by manually creating a release using the web interface in the repository. They will be versioned with the tag that is created when the release is created. The tags MUST take the form `major.minor.revision` to be compatible with the build process. A tag like `v0.1.0-alpha` is NOT compatabile and may result in the build process failing.

To trigger a Main branch build follow the steps:
1. Click ***Releases*** on the left of the repo file window.
2. On the Releases page, click the ***Draft*** a new release" button on the right.
3. Enter a ***Tag version*** in the form `major.minor`.
4. Select the ***Target***, typically this will be the `Main` branch.
5. Enter a ***Release title***, typically this will be the same as the ***Tag version***.
6. Click ***Publish Release***

## Intial steps to build a plugin 

When building a plugin the following steps can be followed to get you up and running quickly.  The steps below assume you have cloned the template repo and installed the necessary NuGet packages.

1. In GitHub click the ***Use This Template*** to create a new repo from the template.
2. In GitHub click the ***Actions*** tab at the top of the repo created.
3. From GitHub ***Actions*** page, click ***New Workflow*** to setup build actions.
4. From GitHub ***Actions Workflow Template*** page, find ***Workflows created by PepperDash-Engineering***, there should be 2 actions:
	- **Essentials Plugins Beta Builds Workflow**
	- **Essentials Plugins Release Builds Workflow**.
5. Click ***Set up this workflow*** for both workflow actions
6. Clone the newly created template repo to begin working locally.
7. Rename the ***EssentialsPluginTemplate*** folder to represent the plugin being built
8. Rename the ****.sln*** and the ****.nuspec*** files to represent the plugin being built
9. Install Essentials as a Nuget package.
	- You can click on the ***GetPackages.bat*** file to automate installation of the nuget packages.
10. Update the ***.nuspec*** file.  The file contains comments providing additional directions on what updates are required.
11. Open the solution and resolve the reference issues
	- Right click on ***References*** 
	- Select ***Add References***
	- Select ***Browse***
	- Navigate the the ***packages*** folder created when installing the NuGet packages
	- Select the necessary ****.dll's*** to resolve all reference warnings
12. Review the ***EssentialsPluginFactoryTemplate.cs*** and remove the unused classes and associated ****.cs*** files from the solution.
13. Rename the classes to represent the device plugin being built.
	
	***Plugin Template***
	```
	EssentialsPluginFactoryTemplate.cs
	```
	***Plugin Example***
	```
	TacoTuesdayCalculatorFactory.cs
	```
14. Follow the ***TODO [ ]*** comments found within the template solution.
15. Update the readme.md information below to help document your plugin.
16. When development is complete, commit the changes and push back to GitHub.



## Device Specific Information

Update the below readme as needed to support documentation of the plugin

### Communication Settings

Update the communication settings as needed for the plugin being developed.

| Setting      | Value       |
|--------------|-------------|
| Baud rate    | 9600        |
| Data bits    | 8           |
| Stop bits    | 1           |
| Parity       | None        |
| Flow Control | None        |
| Delimiter    | "\r"        |
| Default IP   | 169.254.1.1 |
| Default Port | 23          |
| Username     | admin       |
| Password     | password    |

#### Plugin Valid Communication methods

Reference PepperDash Core eControlMethods Enum for valid values, (currently listed below).  Update to reflect the valid values for the plugin device being developed.

```c#
Cec
Comm
Cresnet
IpId
IpIdTcp
IR
None
Ssh
Tcpip
Telnet
Udp
```

##### Communication Method Note - ***DELETE WHEN UPDATING PLUGIN README***

As of PepperDash Core 1.0.41, HTTP and HTTPS are not valid control mehtods and will throw an exception in the plugin factory if not properly handled.  The plugin template is currently setup to check the method before attempting to create the comms for the device.  When using HTTP or HTTPS you will need to create a custom comm object and modify the constructor as needed.

For reference see the [Watt Box PDU Plugin](https://github.com/PepperDash-Engineering/epi-pdu-wattbox) as a working example of implementing both HTTP and standard socket communications.

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

