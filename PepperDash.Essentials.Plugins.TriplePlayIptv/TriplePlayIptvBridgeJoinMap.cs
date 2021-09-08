using PepperDash.Essentials.Core;

namespace TriplePlayIptvPlugin
{
	/// <summary>
	/// Plugin device Bridge Join Map
	/// </summary>
	public class TriplePlayIptvBridgeJoinMap : JoinMapBaseAdvanced
	{
		#region Digital

		/// <summary>
		/// Plugin online feedback
		/// </summary>
		[JoinName("IsOnline")]
		public JoinDataComplete IsOnline = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 1,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Is online feedback",
				JoinCapabilities = eJoinCapabilities.ToSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// Reboot client
		/// </summary>
		[JoinName("RebootClient")]
		public JoinDataComplete RebootClient = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 6,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Reboot client",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// Get all services
		/// </summary>
		[JoinName("GetAllServices")]
		public JoinDataComplete GetAllServices = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 7,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Get a list of all IPTV services configured",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// Channel up
		/// </summary>
		[JoinName("ChannelUp")]
		public JoinDataComplete ChannelUp = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 11,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Channel up",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// Channel down
		/// </summary>
		[JoinName("ChannelDown")]
		public JoinDataComplete ChannelDown = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 12,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Channel down",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// Volume up
		/// </summary>
		[JoinName("VolumeUp")]
		public JoinDataComplete VolumeUp = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 16,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Volume up",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// Volume down
		/// </summary>
		[JoinName("VolumeDown")]
		public JoinDataComplete VolumeDown = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 17,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Volume down",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// Volume mute toggle
		/// </summary>
		[JoinName("VolumeMuteToggle")]
		public JoinDataComplete VolumeMuteToggle = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 18,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Volume mute toggle",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// Volume mute on
		/// </summary>
		[JoinName("VolumeMuteOn")]
		public JoinDataComplete VolumeMuteOn = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 19,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Volume mute on set and feedback",
				JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// Volume mute off
		/// </summary>
		[JoinName("VolumeMuteOff")]
		public JoinDataComplete VolumeMuteOff = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 20,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Volume mute off set and feedback",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// RCU key press - power
		/// </summary>
		[JoinName("RcuKpPower")]
		public JoinDataComplete RcuKpPower = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 21,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "RCU key press Power",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// RCU key press - channel up
		/// </summary>
		[JoinName("RcuKpChannelUp")]
		public JoinDataComplete RcuKpChannelUp = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 22,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "RCU key press Channel Up",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// RCU key press - channel down
		/// </summary>
		[JoinName("RcuKpChannelDown")]
		public JoinDataComplete RcuKpChannelDown = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 23,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "RCU key press Channel Down",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// RCU key press - Guide
		/// </summary>
		[JoinName("RcuKpGuide")]
		public JoinDataComplete RcuKpGuide = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 24,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "RCU key press Guide",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// RCU key press - TV
		/// </summary>
		[JoinName("RcuKpTv")]
		public JoinDataComplete RcuKpTv = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 25,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "RCU key press TV",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// RCU key press - menu
		/// </summary>
		[JoinName("RcuKpMenu")]
		public JoinDataComplete RcuKpMenu = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 26,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "RCU key press Menu",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// RCU key press - navigation up
		/// </summary>
		[JoinName("RcuKpUp")]
		public JoinDataComplete RcuKpUp = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 27,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "RCU key press navigation up",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// RCU key press - navigation down
		/// </summary>
		[JoinName("RcuKpDown")]
		public JoinDataComplete RcuKpDown = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 28,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "RCU key press navigation down",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// RCU key press - navigation left
		/// </summary>
		[JoinName("RcuKpLeft")]
		public JoinDataComplete RcuKpLeft = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 29,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "RCU key press navigation left",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// RCU key press - navigation right
		/// </summary>
		[JoinName("RcuKpRight")]
		public JoinDataComplete RcuKpRight = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 30,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "RCU key press navigation right",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// RCU key press - navigation ok
		/// </summary>
		[JoinName("RcuKpOk")]
		public JoinDataComplete RcuKpOk = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 31,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "RCU key press navigation OK",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// RCU key press - red
		/// </summary>
		[JoinName("RcuKpRed")]
		public JoinDataComplete RcuKpRed = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 32,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "RCU key press red",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// RCU key press - green
		/// </summary>
		[JoinName("RcuKpGreen")]
		public JoinDataComplete RcuKpGreen = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 33,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "RCU key press green",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// RCU key press - yellow
		/// </summary>
		[JoinName("RcuKpYellow")]
		public JoinDataComplete RcuKpYellow = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 34,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "RCU key press yellow",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// RCU key press - blue
		/// </summary>
		[JoinName("RcuKpBlue")]
		public JoinDataComplete RcuKpBlue = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 35,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "RCU key press blue",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// RCU key press - play
		/// </summary>
		[JoinName("RcuKpPlay")]
		public JoinDataComplete RcuKpPlay = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 36,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "RCU key press green",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// RCU key press - stop
		/// </summary>
		[JoinName("RcuKpStop")]
		public JoinDataComplete RcuKpStop = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 37,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "RCU key press stop",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// RCU key press - pause
		/// </summary>
		[JoinName("RcuKpPause")]
		public JoinDataComplete RcuKpPause = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 38,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "RCU key press pause",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// RCU key press - rewind
		/// </summary>
		[JoinName("RcuKpRewind")]
		public JoinDataComplete RcuKpRewind = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 39,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "RCU key press rewind",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// RCU key press - forward
		/// </summary>
		[JoinName("RcuKpForward")]
		public JoinDataComplete RcuKpForward = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 40,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "RCU key press forward",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});
		

		/// <summary>
		/// RCU key press - 0-9
		/// </summary>
		[JoinName("RcuKpNumbers")]
		public JoinDataComplete RcuKpNumbers = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 50,
				JoinSpan = 10
			},
			new JoinMetadata
			{
				Description = "RCU key press 0-10",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// Preset select, 1-24
		/// </summary>
		[JoinName("Presets")]
		public JoinDataComplete Presets = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 71,
				JoinSpan = 24
			},
			new JoinMetadata
			{
				Description = "Presets, 1-24, select and feedback (icon and label)",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.DigitalAnalogSerial
			});


		/// <summary>
		/// TV control - power on
		/// </summary>
		[JoinName("TvPowerOn")]
		public JoinDataComplete TvPowerOn = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 101,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "TV control power on",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// TV control - power off
		/// </summary>
		[JoinName("TvPowerOff")]
		public JoinDataComplete TvPowerOff = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 102,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "TV control power off",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// TV control - input hdmi 1
		/// </summary>
		[JoinName("TvInputHdmi1")]
		public JoinDataComplete TvInputHdmi1 = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 111,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "TV control input hdmi 1",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// TV control - input hdmi 2
		/// </summary>
		[JoinName("TvInputHdmi2")]
		public JoinDataComplete TvInputHdmi2 = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 112,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "TV control input hdmi 2",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// TV control - input hdmi 3
		/// </summary>
		[JoinName("TvInputHdmi3")]
		public JoinDataComplete TvInputHdmi3 = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 113,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "TV control input hdmi 4",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// TV control - input hdmi 4
		/// </summary>
		[JoinName("TvInputHdmi4")]
		public JoinDataComplete TvInputHdmi4 = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 114,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "TV control input hdmi 4",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// TV control - input hdmi
		/// </summary>
		[JoinName("TvInputHdmi")]
		public JoinDataComplete TvInputHdmi = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 115,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "TV control input hdmi",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// TV control - input DVI
		/// </summary>
		[JoinName("TvInputDvi")]
		public JoinDataComplete TvInputDvi = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 116,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "TV control input DVI",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// TV control - input dsipay port
		/// </summary>
		[JoinName("TvInputDisplayPort")]
		public JoinDataComplete TvInputDisplayPort = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 117,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "TV control input Display Port",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		/// <summary>
		/// TV control - input PC
		/// </summary>
		[JoinName("TvInputPc")]
		public JoinDataComplete TvInputPc = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 118,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "TV control input PC",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

        /// <summary>
        /// TV control - input vga
        /// </summary>
        [JoinName("TvInputVga")]
        public JoinDataComplete TvInputVga = new JoinDataComplete(
            new JoinData
            {
                JoinNumber = 119,
                JoinSpan = 1
            },
            new JoinMetadata
            {
                Description = "TV control input vga",
                JoinCapabilities = eJoinCapabilities.FromSIMPL,
                JoinType = eJoinType.Digital
            });

		/// <summary>
		/// TV control - input media
		/// </summary>
		[JoinName("TvInputMedia")]
		public JoinDataComplete TvInputMedia = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 120,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "TV control input media",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		#endregion


		#region Analog

		/// <summary>
		/// Plugin socket status join map
		/// </summary>
		[JoinName("SocketStatus")]
		public JoinDataComplete SocketStatus = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 1,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Socket status feedback",
				JoinCapabilities = eJoinCapabilities.ToSIMPL,
				JoinType = eJoinType.Analog
			});

		/// <summary>
		/// Plugin monitor status join map
		/// </summary>
		[JoinName("MonitorStatus")]
		public JoinDataComplete MonitorStatus = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 2,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Monitor status feedback",
				JoinCapabilities = eJoinCapabilities.ToSIMPL,
				JoinType = eJoinType.Analog
			});

		/// <summary>
		/// Stb ID set and feedback
		/// </summary>
		[JoinName("StbId")]
		public JoinDataComplete StbId = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 6,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Settop box ID set and feedback",
				JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
				JoinType = eJoinType.Analog
			});

		/// <summary>
		/// Response code feedback
		/// </summary>
		[JoinName("ResponseCode")]
		public JoinDataComplete ResponseCode = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 7,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Response code feedback",
				JoinCapabilities = eJoinCapabilities.ToSIMPL,
				JoinType = eJoinType.Analog
			});

		/// <summary>
		/// Channel set and feedback
		/// </summary>
		[JoinName("ChannelSelect")]
		public JoinDataComplete ChannelSelect = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 11,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Channel direct select",
				JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
				JoinType = eJoinType.Analog
			});

		/// <summary>
		/// Volume set and feedback
		/// </summary>
		[JoinName("Volume")]
		public JoinDataComplete Volume = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 16,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Volume direct set",
				JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
				JoinType = eJoinType.Analog
			});

		/// <summary>
		/// TV control - volume set and feedback
		/// </summary>
		[JoinName("TvVolume")]
		public JoinDataComplete TvVolume = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 101,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "TV Volume set and feedback",
				JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
				JoinType = eJoinType.Analog
			});

		#endregion


		#region Serial

		/// <summary>
		/// Plugin device name
		/// </summary>
		[JoinName("DeviceName")]
		public JoinDataComplete DeviceName = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 1,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Device Name",
				JoinCapabilities = eJoinCapabilities.ToSIMPL,
				JoinType = eJoinType.Serial
			});

		/// <summary>
		/// Response content feedback
		/// </summary>
		[JoinName("ResponseContent")]
		public JoinDataComplete ResponseContent = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 7,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Response content feedback",
				JoinCapabilities = eJoinCapabilities.ToSIMPL,
				JoinType = eJoinType.Serial
			});

		/// <summary>
		/// Response error feedback
		/// </summary>
		[JoinName("ResponseError")]
		public JoinDataComplete ResponseError = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 8,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Response error feedback",
				JoinCapabilities = eJoinCapabilities.ToSIMPL,
				JoinType = eJoinType.Serial
			});

        /// <summary>
        /// Preset Icon Path 
        /// </summary>
        [JoinName("PresetIconPaths")]
        public JoinDataComplete PresetIconPaths = new JoinDataComplete(
            new JoinData
            {
                JoinNumber = 51,
                JoinSpan = 24
            },
            new JoinMetadata
            {
                Description = "Preset Icon Paths",
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Serial
            });

		#endregion

		/// <summary>
		/// Plugin device BridgeJoinMap constructor
		/// </summary>
		/// <param name="joinStart">This will be the join it starts on the EISC bridge</param>
		public TriplePlayIptvBridgeJoinMap(uint joinStart)
			: base(joinStart, typeof(TriplePlayIptvBridgeJoinMap))
		{
		}
	}
}
