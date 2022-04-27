using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DeviceSupport;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;

namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer
{
    /// <summary>
    /// Plugin device
    /// </summary>
    public class TriplePlayIptvDevice : EssentialsBridgeableDevice
    {
        // request path for all commands
        private const string RequestPath = "triplecare/JsonRpcHandler.php";        

        // generic http/https client
        private readonly IRestfulComms _comms;

        // preset dictionary
        private Dictionary<int, TriplePlayServicesPresetsConfig> _presets;

        // settop box id (stbId) field
        private int _stbId;
        private string _hostIp;

        /// <summary>
        /// settop box id (stbId) property
        /// </summary>
        public int StbId
        {
            get { return _stbId; }
            set
            {
                if (value == _stbId) return;
                _stbId = value;
                StbIdFeedback.FireUpdate();
            }
        }

        /// <summary>
        /// settop box id (stbId) int feedback
        /// </summary>
        public IntFeedback StbIdFeedback { get; set; }


        // max count of allowed favorites, should max index defined in bridge join map
        private const int PresetsMaxAllowedDefault = 24;

        /// <summary>
        /// Presets max allowed property
        /// </summary>
        public int PresetsMaxAllowed { get; set; }

        /// <summary>
        /// Preset enabled bool feedback
        /// </summary>
        public Dictionary<uint, BoolFeedback> PresetEnabledFeedbacks { get; set; }

        /// <summary>
        /// Preset channel int feedback
        /// </summary>
        public Dictionary<uint, IntFeedback> PresetChannelFeedbacks { get; set; }

        /// <summary>
        /// Preset name string feedback
        /// </summary>
        public Dictionary<uint, StringFeedback> PresetNameFeedbacks { get; set; }

        /// <summary>
        /// Preset name icon url feedback
        /// </summary>
        public Dictionary<uint, StringFeedback> PresetIconPathFeedbacks { get; set; }

        // response code field
        private int _responseCode;

        /// <summary>
        /// Response code property
        /// </summary>
        public int ResponseCode
        {
            get { return _responseCode; }
            set
            {
                if (value == _responseCode) return;
                _responseCode = value;
                ResponseCodeFeedback.FireUpdate();
            }
        }

        /// <summary>
        /// Client response code int feedback
        /// </summary>
        public IntFeedback ResponseCodeFeedback { get; set; }

        // response content string field
        private string _responseContentString;

        /// <summary>
        /// Client response content string property
        /// </summary>
        public string ResponseContentString
        {
            get { return _responseContentString; }
            set
            {
                if (value == _responseContentString) return;
                _responseContentString = value;
                ResponseContentStringFeedback.FireUpdate();
            }
        }

        /// <summary>
        /// Client response content string feedback
        /// </summary>
        public StringFeedback ResponseContentStringFeedback { get; set; }

        // respose error field
        private string _responseError;

        /// <summary>
        /// Response Error property
        /// </summary>
        public string ResponseError
        {
            get { return _responseError; }
            set
            {
                if (value == _responseError) return;
                _responseError = value;
                ResponseErrorFeedback.FireUpdate();
            }
        }

        /// <summary>
        /// client response error feedback
        /// </summary>
        public StringFeedback ResponseErrorFeedback { get; set; }

        /// <summary>
        /// Plugin device constructor
        /// </summary>
        /// <param name="key">device key</param>
        /// <param name="name">device name</param>
        /// <param name="config">device configuration object</param>
        /// <param name="client"></param>
        public TriplePlayIptvDevice(string key, string name, TriplePlayIptvConfig config, IRestfulComms client)
            : base(key, name)
        {
            Debug.Console(TriplePlayDebug.Info, this, "Constructing new {0} instance", name);

            if (config == null || config.Control == null)
            {
                Debug.Console(TriplePlayDebug.Info, this, "Configuration or control object is null, unable to construct new {0} instance.  Check configuration.", name);
                return;
            }

            _hostIp = config.Control.TcpSshProperties.Address;
            _comms = client;
            if (_comms == null)
            {
                Debug.Console(TriplePlayDebug.Info, this, Debug.ErrorLogLevel.Error, "Failed to construct GenericClient using method {0}", config.Control.Method);
                return;
            }

            _comms.ResponseReceived += _comms_ResponseRecieved;

            StbIdFeedback = new IntFeedback(() => StbId);

            PresetEnabledFeedbacks = new Dictionary<uint, BoolFeedback>();
            PresetChannelFeedbacks = new Dictionary<uint, IntFeedback>();
            PresetNameFeedbacks = new Dictionary<uint, StringFeedback>();
            PresetIconPathFeedbacks = new Dictionary<uint, StringFeedback>();

            ResponseCodeFeedback = new IntFeedback(() => ResponseCode);
            ResponseContentStringFeedback = new StringFeedback(() => ResponseContentString);
            ResponseErrorFeedback = new StringFeedback(() => ResponseError);

            StbId = config.StbId;

            // TODO [ ] Issue #6 - Preset indexing
            PresetsMaxAllowed = config.PresetsMaxAllowed ?? PresetsMaxAllowedDefault;
            _presets = new Dictionary<int, TriplePlayServicesPresetsConfig>();
            for (var p = 1; p <= PresetsMaxAllowed; p++)
            {
                var preset = p;
                var emptyPreset = new TriplePlayServicesPresetsConfig {Id = (uint)preset};

                _presets.Add(p, emptyPreset);
                PresetEnabledFeedbacks.Add((uint)preset, new BoolFeedback(() => _presets[preset].IsWatchable));
                PresetNameFeedbacks.Add((uint)preset, new StringFeedback(() => _presets[preset].Name));
                PresetChannelFeedbacks.Add((uint)preset, new IntFeedback(() => (int) _presets[preset].ChannelNumber));
                PresetIconPathFeedbacks.Add((uint)preset, new StringFeedback(() => _presets[preset].IconPath));
            }

            Debug.Console(TriplePlayDebug.Info, this, "Constructing new {0} instance complete", name);
            Debug.Console(TriplePlayDebug.Verbose, new string('*', 80));
            Debug.Console(TriplePlayDebug.Verbose, new string('*', 80));
        }

        #region Overrides of EssentialsBridgeableDevice

        /// <summary>
        /// Links the plugin device to the EISC bridge
        /// </summary>
        /// <param name="trilist"></param>
        /// <param name="joinStart"></param>
        /// <param name="joinMapKey"></param>
        /// <param name="bridge"></param>
        public override void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
        {
            var joinMap = new TriplePlayIptvBridgeJoinMap(joinStart);

            // This adds the join map to the collection on the bridge
            if (bridge != null)
            {
                bridge.AddJoinMap(Key, joinMap);
            }

            var customJoins = JoinMapHelper.TryGetJoinMapAdvancedForDevice(joinMapKey);

            if (customJoins != null)
            {
                joinMap.SetCustomJoinData(customJoins);
            }

            Debug.Console(TriplePlayDebug.Info, this, "Linking to Trilist '{0}'", trilist.ID.ToString("X"));
            Debug.Console(TriplePlayDebug.Info, this, "Linking to Bridge Type {0}", GetType().Name);

            // links to bridge
            trilist.SetString(joinMap.DeviceName.JoinNumber, Name);           

            // TODO [ ] If connection state is managed by Essentials, delete the following.  If connection is managed by SiMPL, uncomment the following
            //trilist.SetBoolSigAction(joinMap.Connect.JoinNumber, sig => Connect = sig);
            //ConnectFeedback.LinkInputSig(trilist.BooleanInput[joinMap.Connect.JoinNumber]);

            //SocketStatusFeedback.LinkInputSig(trilist.UShortInput[joinMap.SocketStatus.JoinNumber]);
            //OnlineFeedback.LinkInputSig(trilist.BooleanInput[joinMap.IsOnline.JoinNumber]);

            ResponseCodeFeedback.LinkInputSig(trilist.UShortInput[joinMap.ResponseCode.JoinNumber]);
            ResponseContentStringFeedback.LinkInputSig(trilist.StringInput[joinMap.ResponseContent.JoinNumber]);
            ResponseErrorFeedback.LinkInputSig(trilist.StringInput[joinMap.ResponseError.JoinNumber]);

            // stb controls
            trilist.SetSigTrueAction(joinMap.RebootClient.JoinNumber, RebootClient);
            trilist.SetSigTrueAction(joinMap.GetAllServices.JoinNumber, GetAllServices);
            trilist.SetSigTrueAction(joinMap.ChannelUp.JoinNumber, ChannelUp);
            trilist.SetSigTrueAction(joinMap.ChannelDown.JoinNumber, ChannelDown);
            trilist.SetSigTrueAction(joinMap.VolumeUp.JoinNumber, VolumeUp);
            trilist.SetSigTrueAction(joinMap.VolumeDown.JoinNumber, VolumeDown);
            trilist.SetSigTrueAction(joinMap.VolumeMuteToggle.JoinNumber, VolumeMuteToggle);
            trilist.SetSigTrueAction(joinMap.VolumeMuteOn.JoinNumber, VolumeMuteOn);
            trilist.SetSigTrueAction(joinMap.VolumeMuteOff.JoinNumber, VolumeMuteOff);

            trilist.SetSigTrueAction(joinMap.RcuKpPower.JoinNumber, () => RcuKeyPress(RcuKeys.Power));
            trilist.SetSigTrueAction(joinMap.RcuKpChannelUp.JoinNumber, () => RcuKeyPress(RcuKeys.ChannelUp));
            trilist.SetSigTrueAction(joinMap.RcuKpChannelDown.JoinNumber, () => RcuKeyPress(RcuKeys.ChannelDown));
            trilist.SetSigTrueAction(joinMap.RcuKpGuide.JoinNumber, () => RcuKeyPress(RcuKeys.Guide));
            trilist.SetSigTrueAction(joinMap.RcuKpTv.JoinNumber, () => RcuKeyPress(RcuKeys.Tv));
            trilist.SetSigTrueAction(joinMap.RcuKpHome.JoinNumber, () => RcuKeyPress(RcuKeys.Home));
            trilist.SetSigTrueAction(joinMap.RcuKpUp.JoinNumber, () => RcuKeyPress(RcuKeys.Up));
            trilist.SetSigTrueAction(joinMap.RcuKpDown.JoinNumber, () => RcuKeyPress(RcuKeys.Down));
            trilist.SetSigTrueAction(joinMap.RcuKpLeft.JoinNumber, () => RcuKeyPress(RcuKeys.Left));
            trilist.SetSigTrueAction(joinMap.RcuKpRight.JoinNumber, () => RcuKeyPress(RcuKeys.Right));
            trilist.SetSigTrueAction(joinMap.RcuKpOk.JoinNumber, () => RcuKeyPress(RcuKeys.Ok));
            trilist.SetSigTrueAction(joinMap.RcuKpRed.JoinNumber, () => RcuKeyPress(RcuKeys.Red));
            trilist.SetSigTrueAction(joinMap.RcuKpGreen.JoinNumber, () => RcuKeyPress(RcuKeys.Green));
            trilist.SetSigTrueAction(joinMap.RcuKpYellow.JoinNumber, () => RcuKeyPress(RcuKeys.Yellow));
            trilist.SetSigTrueAction(joinMap.RcuKpBlue.JoinNumber, () => RcuKeyPress(RcuKeys.Blue));
            trilist.SetSigTrueAction(joinMap.RcuKpPlay.JoinNumber, () => RcuKeyPress(RcuKeys.Play));
            trilist.SetSigTrueAction(joinMap.RcuKpStop.JoinNumber, () => RcuKeyPress(RcuKeys.Stop));
            trilist.SetSigTrueAction(joinMap.RcuKpPause.JoinNumber, () => RcuKeyPress(RcuKeys.Pause));
            trilist.SetSigTrueAction(joinMap.RcuKpRewind.JoinNumber, () => RcuKeyPress(RcuKeys.Rewind));
            trilist.SetSigTrueAction(joinMap.RcuKpForward.JoinNumber, () => RcuKeyPress(RcuKeys.Forward));
            trilist.SetSigTrueAction(joinMap.RcuKpBack.JoinNumber, () => RcuKeyPress(RcuKeys.Back));
            trilist.SetSigTrueAction(joinMap.RcuKpInfo.JoinNumber, () => RcuKeyPress(RcuKeys.Info));
            trilist.SetSigTrueAction(joinMap.RcuKpPvr.JoinNumber, () => RcuKeyPress(RcuKeys.Pvr));
            trilist.SetSigTrueAction(joinMap.RcuKpRecord.JoinNumber, () => RcuKeyPress(RcuKeys.Record));
            trilist.SetSigTrueAction(joinMap.RcuKpTitles.JoinNumber, () => RcuKeyPress(RcuKeys.Titles));
            trilist.SetSigTrueAction(joinMap.RcuKpSource.JoinNumber, () => RcuKeyPress(RcuKeys.Source));
            trilist.SetSigTrueAction(joinMap.RcuKpPageUp.JoinNumber, () => RcuKeyPress(RcuKeys.PageUp));
            trilist.SetSigTrueAction(joinMap.RcuKpPageDown.JoinNumber, () => RcuKeyPress(RcuKeys.PageDown));

            // stb rcu keypad numbers
            for (var n = 0; n < joinMap.RcuKpNumbers.JoinSpan; n++)
            {
                var join = joinMap.RcuKpNumbers.JoinNumber + n;
                var number = n;
                // TODO [ ] Update to pass index of keypad number pressed
                trilist.SetSigTrueAction((uint)join, () => RcuKpNumbers(number));
            }

            // stb presets
            for (var p = 1; p < joinMap.Presets.JoinSpan; p++)
            {
                var presetJoin = (uint)(joinMap.Presets.JoinNumber + p - 1);
                var presetPathJoin = (uint)(joinMap.PresetIconPaths.JoinNumber + p - 1);
                var preset = (uint)p;

                // TODO [ ] Update to pass index of preset select pressed
                trilist.SetSigTrueAction(presetJoin, () => PresetSelect(preset));

                PresetEnabledFeedbacks[preset].LinkInputSig(trilist.BooleanInput[presetJoin]);
                PresetNameFeedbacks[preset].LinkInputSig(trilist.StringInput[presetJoin]);
                PresetChannelFeedbacks[preset].LinkInputSig(trilist.UShortInput[presetJoin]);
                PresetIconPathFeedbacks[preset].LinkInputSig(trilist.StringInput[presetPathJoin]);
            }

            // tv control
            trilist.SetSigTrueAction(joinMap.TvPowerOn.JoinNumber, TvPowerOn);
            trilist.SetSigTrueAction(joinMap.TvPowerOff.JoinNumber, TvPowerOff);
            trilist.SetSigTrueAction(joinMap.TvInputHdmi1.JoinNumber, () => TvInputSelect(TvControlInputs.Hdmi1));
            trilist.SetSigTrueAction(joinMap.TvInputHdmi2.JoinNumber, () => TvInputSelect(TvControlInputs.Hdmi2));
            trilist.SetSigTrueAction(joinMap.TvInputHdmi3.JoinNumber, () => TvInputSelect(TvControlInputs.Hdmi3));
            trilist.SetSigTrueAction(joinMap.TvInputHdmi4.JoinNumber, () => TvInputSelect(TvControlInputs.Hdmi4));
            trilist.SetSigTrueAction(joinMap.TvInputHdmi.JoinNumber, () => TvInputSelect(TvControlInputs.Hdmi));
            trilist.SetSigTrueAction(joinMap.TvInputDvi.JoinNumber, () => TvInputSelect(TvControlInputs.Dvi));
            trilist.SetSigTrueAction(joinMap.TvInputDisplayPort.JoinNumber, () => TvInputSelect(TvControlInputs.DisplayPort));
            trilist.SetSigTrueAction(joinMap.TvInputPc.JoinNumber, () => TvInputSelect(TvControlInputs.Pc));
            trilist.SetSigTrueAction(joinMap.TvInputVga.JoinNumber, () => TvInputSelect(TvControlInputs.Vga));
            trilist.SetSigTrueAction(joinMap.TvInputMedia.JoinNumber, () => TvInputSelect(TvControlInputs.Media));

            // stb id
            trilist.SetUShortSigAction(joinMap.StbId.JoinNumber, id => ChangeStbId(id));
            StbIdFeedback.LinkInputSig(trilist.UShortInput[joinMap.StbId.JoinNumber]);

            // channel direct tune
            trilist.SetUShortSigAction(joinMap.ChannelSelect.JoinNumber, ch => ChannelSet(ch));

            // volume direct set (0-65536)
            trilist.SetUShortSigAction(joinMap.Volume.JoinNumber, vol => VolumeSet(vol));

            // tv volume direct set (0-65535)
            trilist.SetUShortSigAction(joinMap.TvVolume.JoinNumber, vol => TvVolumeSet(vol));

            UpdateFeedbacks();

            trilist.OnlineStatusChange += (sender, args) =>
            {
                if (args.DeviceOnLine == false) return;

                Debug.Console(TriplePlayDebug.Verbose, this, "{0}", new String('*', 50));
                Debug.Console(TriplePlayDebug.Verbose, this, "LinkToApi args.DeviceOnline: {0}", args.DeviceOnLine);
                Debug.Console(TriplePlayDebug.Verbose, this, "LinkToApi Name: {0}", Name);
                Debug.Console(TriplePlayDebug.Verbose, this, "{0}", new String('*', 50));

                trilist.SetString(joinMap.DeviceName.JoinNumber, Name);
                UpdateFeedbacks();
            };
        }

        private void UpdateFeedbacks()
        {
            // TODO [ ] Update as needed for the plugin being developed
            //ConnectFeedback.FireUpdate();
            //OnlineFeedback.FireUpdate();
            //SocketStatusFeedback.FireUpdate();

            StbIdFeedback.FireUpdate();

            foreach (var item in PresetEnabledFeedbacks)
                item.Value.FireUpdate();

            foreach (var item in PresetNameFeedbacks)
                item.Value.FireUpdate();

            foreach (var item in PresetIconPathFeedbacks)
                item.Value.FireUpdate();

            ResponseCodeFeedback.FireUpdate();
            ResponseContentStringFeedback.FireUpdate();
            ResponseErrorFeedback.FireUpdate();
        }

        #endregion Overrides of EssentialsBridgeableDevice

        private bool IsValidJson(string contentString)
        {
            if (string.IsNullOrEmpty(contentString)) return false;

            contentString = contentString.Trim();
            if ((!contentString.StartsWith("{") || !contentString.EndsWith("}")) &&
                (!contentString.StartsWith("[") || !contentString.EndsWith("]"))) return false;

            try
            {
                var obj = JToken.Parse(contentString);
                return true;
            }
            catch (JsonReaderException jex)
            {
                Debug.Console(TriplePlayDebug.Debug, this, "IsValidJson Exception Message: {0}", jex.Message);
                Debug.Console(TriplePlayDebug.Verbose, this, "IsValidJson Inner Exception: {0}", jex.InnerException);
                Debug.Console(TriplePlayDebug.Verbose, this, "IsValidJson Stack Trace: {0}", jex.StackTrace);
                return false;
            }
            catch (Exception ex)
            {
                Debug.Console(TriplePlayDebug.Debug, this, "IsValidJson Exception Message: {0}", ex.Message);
                Debug.Console(TriplePlayDebug.Verbose, this, "IsValidJson Inner Exception: {0}", ex.InnerException);
                Debug.Console(TriplePlayDebug.Verbose, this, "IsValidJson Stack Trace: {0}", ex.StackTrace);
                return false;
            }
        }

        private void _comms_ResponseRecieved(object sender, GenericClientResponseEventArgs args)
        {
            try
            {
                Debug.Console(TriplePlayDebug.Debug, this, "_comms_ResponseRecieved: Code = {0} | ContentString = {1}", args.Code, args.ContentString);

                ResponseCode = args.Code;

                if (string.IsNullOrEmpty(args.ContentString)) return;

                // {"jsonrpc":"2.0","id":null,"result":true}
                // {"jsonrpc":"2.0","id":null,"error":{"code":-32601,"message":"server error. method not found.\n\nGetPowerStatus"}}
                // {"jsonrpc":"2.0","id":null,"result":[{"id":1,"channelNumber":1,"name":"CP24","isFavourite":false,"iconPath":"/portalImages/services/1_v2.png","accessControl":false,"serviceAccessControl":"1","recordingType":"NwPvr","type":1,"typeSpecificData":{"ipAddress":"239.0.8.2","port":2802,"videoCodec":"mpeg2","encryptionType":"None","encryptionId":""},"multipleTypeData":{"None":{"ipAddress":"239.0.8.2","port":2802,"videoCodec":"mpeg2","encryptionType":"None","encryptionId":""}},"geographicalArea":"0","isWatchable":true,"liveTrickplayRunning":false}]}
                if (IsValidJson(args.ContentString))
                {
                    var jObject = JObject.Parse(args.ContentString);
                    var token = jObject.SelectToken("result");
                    var tokenType = token.Type;
                    if (tokenType != JTokenType.Array)
                    {
                        ResponseContentString = args.ContentString;
                        return;
                    }

                    // if array, desrialize message
                    var services = JsonConvert.DeserializeObject<ServicesResponse>(args.ContentString);
                    if (services == null || services.Results == null) return;
                    ProcessResultsObject(services.Results);
                }
                else
                {
                    ResponseContentString = args.ContentString;
                }
            }
            catch (Exception ex)
            {
                Debug.Console(TriplePlayDebug.Debug, this, Debug.ErrorLogLevel.Error, "_comms_ResponseRecieved Exception Message: {0}", ex.Message);
                Debug.Console(TriplePlayDebug.Verbose, this, Debug.ErrorLogLevel.Error, "_comms_ResponseRecieved Inner Exception {0}", ex.InnerException);
                Debug.Console(TriplePlayDebug.Verbose, this, Debug.ErrorLogLevel.Error, "_comms_ResponseRecieved Stack Trace: {0}", ex.StackTrace);
            }
        }

        // TODO [ ] Issue #6 - Preset indexing
        private void ProcessResultsObject(IList<ResultsObject> results)
        {
            if (results == null) return;

            var count = results.Count > PresetsMaxAllowed ? PresetsMaxAllowed : results.Count;

            // go through array and convert results to feedbacks to pass back to bridge
            // id: bridge preset index
            // channelNumber
            // name
            // iconPath
            // isFavourite
            //foreach (var result in results)
            for(var i = 0; i < count; i++)
            {
                var key = i + 1;

                if (_presets.ContainsKey(key))
                {
                    _presets[key].Id = results[i].Id;
                    _presets[key].ChannelNumber = results[i].ChannelNumber;
                    _presets[key].Name = results[i].Name;
                    _presets[key].IsFavorite = results[i].IsFavourite;
                    _presets[key].IconPath = String.Format("http://{0}{1}", _hostIp, results[i].IconPath);
                    _presets[key].IsWatchable = results[i].IsWatchable;
                }
                else
                {
                    _presets.Add(key, new TriplePlayServicesPresetsConfig
                    {
                        Id = results[i].Id,
                        ChannelNumber = results[i].ChannelNumber,
                        Name = results[i].Name,
                        IsFavorite = results[i].IsFavourite,
                        IconPath = results[i].IconPath,
                        IsWatchable = results[i].IsWatchable
                    });
                }

                UpdatePresetFeedbacks(key, _presets[key]);
            }
        }

        // TODO [ ] Issue #6 - Preset indexing
        private void UpdatePresetFeedbacks(int index, TriplePlayServicesPresetsConfig preset)
        {

            if (preset == null)
            {
                Debug.Console(TriplePlayDebug.Debug, this, "Preset is null");
                return;
            }
            Debug.Console(TriplePlayDebug.Debug, this, "Preset Id: {0}", preset.Id);

            var key = (uint)index;

            // preset name
            if (PresetNameFeedbacks.ContainsKey(key))
                PresetNameFeedbacks[key].FireUpdate();

            // preset channel number
            if (PresetChannelFeedbacks.ContainsKey(key))
                PresetChannelFeedbacks[key].FireUpdate();

            // preset icon path
            if (PresetIconPathFeedbacks.ContainsKey(key))
                PresetIconPathFeedbacks[key].FireUpdate();

            // preset enable
            if (PresetEnabledFeedbacks.ContainsKey(key))
                PresetEnabledFeedbacks[key].FireUpdate();
        }

        /// <summary>
        /// Method to print known presets 
        /// </summary>
        public void GetPresets()
        {
            Debug.Console(TriplePlayDebug.Info, this, "{0}", new string('*', 80));
            foreach (var preset in _presets)
            {
                Debug.Console(TriplePlayDebug.Info, this, "preset[{0}].Id", preset.Key, preset.Value.Id);
                Debug.Console(TriplePlayDebug.Info, this, "preset[{0}].Name: {1}", preset.Key, preset.Value.Name);
                Debug.Console(TriplePlayDebug.Info, this, "preset[{0}].ChannelNumber: {1}", preset.Key, preset.Value.ChannelNumber);
                Debug.Console(TriplePlayDebug.Info, this, "preset[{0}].IconPath: {1}", preset.Key, preset.Value.IconPath);
                Debug.Console(TriplePlayDebug.Info, this, "preset[{0}].IsFavorite: {1}", preset.Key, preset.Value.IsFavorite);
                Debug.Console(TriplePlayDebug.Info, this, "preset[{0}].IsWatchable: {1}", preset.Key, preset.Value.IsWatchable);
                Debug.Console(TriplePlayDebug.Info, this, "{0}", new string('-', 80));
            }
            Debug.Console(TriplePlayDebug.Info, this, "{0}", new string('*', 80));
        }

        /// <summary>
        /// Sends commands to the device
        /// </summary>
        /// <param name="method"></param>
        public void SendText(string method)
        {
            Debug.Console(TriplePlayDebug.Debug, this, "SendText: method = {0}", method);

            if (_comms == null) return;

            var query = BuildQuery(StbId, method, null, null, null);
            if (string.IsNullOrEmpty(query)) return;

            _comms.SendRequest(String.Format("{0}?{1}",RequestPath, query), String.Empty);
        }

        /// <summary>
        /// Sends commands to the device
        /// </summary>
        /// <param name="method"></param>
        /// <param name="boolParam"></param>
        public void SendText(string method, bool boolParam)
        {
            Debug.Console(TriplePlayDebug.Debug, this, "SendText: method = {0} | strParam = {1}", method, boolParam);

            if (_comms == null) return;

            var query = BuildQuery(StbId, method, null, null, boolParam);
            if (string.IsNullOrEmpty(query)) return;

            _comms.SendRequest(String.Format("{0}?{1}", RequestPath, query), String.Empty);
        }

        /// <summary>
        /// Sends commands to the device
        /// </summary>
        /// <param name="method"></param>
        /// <param name="strParam"></param>
        public void SendText(string method, string strParam)
        {
            Debug.Console(TriplePlayDebug.Debug, this, "SendText: method = {0} | strParam = {1}", method, strParam);

            if (_comms == null) return;

            var query = BuildQuery(StbId, method, strParam, null, null);
            if (string.IsNullOrEmpty(query)) return;

            _comms.SendRequest(String.Format("{0}?{1}", RequestPath, query), String.Empty);
        }

        /// <summary>
        /// Sends commands to the device
        /// </summary>
        /// <param name="method"></param>
        /// <param name="intParam"></param>
        public void SendText(string method, int? intParam)
        {
            Debug.Console(TriplePlayDebug.Debug, this, "SendText: method = {0} | intParam = {1}", method, intParam);

            if (_comms == null) return;

            var query = BuildQuery(StbId, method, null, intParam, null);
            if (string.IsNullOrEmpty(query)) return;

            _comms.SendRequest(String.Format("{0}?{1}", RequestPath, query), String.Empty);
        }

        /// <summary>
        /// Sends commands to the device
        /// </summary>
        /// <param name="method"></param>
        /// <param name="strParam"></param>
        /// <param name="intParam"></param>
        public void SendText(string method, string strParam, int? intParam)
        {
            Debug.Console(TriplePlayDebug.Debug, this, "SendText: method = {0} | strParam = {1} | intParam = {2}", method, strParam, intParam);

            if (_comms == null) return;

            var query = BuildQuery(StbId, method, strParam, intParam, null);
            if (string.IsNullOrEmpty(query)) return;

            _comms.SendRequest(String.Format("{0}?{1}", RequestPath, query), String.Empty);
        }        

        // builds query with string and/or int parameter
        private string BuildQuery(int stbId, string method, string strParam, int? intParam, bool? boolParam)
        {
            if ((stbId <= 0) || string.IsNullOrEmpty(method))
            {
                Debug.Console(TriplePlayDebug.Debug, this, "BuildQuery: invalid stbId {0}, unable to build query.", stbId);
                return null;
            }

            var array = new JArray
            {
                stbId
            };
            if (!string.IsNullOrEmpty(strParam))
                array.Add(strParam);
            if (intParam != null)
                array.Add(intParam);
            if(boolParam != null)
                array.Add(boolParam);

            var obj = new JObject
            {
                {"jsonrpc", "2.0"}, 
                {"method", method}
            };
            obj["params"] = array;

            var call = JsonConvert.SerializeObject(obj, Formatting.None);
            Debug.Console(TriplePlayDebug.Verbose, this, "BuildQuery: call = {0}", call);
            return string.Format("call={0}", call);
        }       

        /// <summary>
        /// Allows bridge to change the settop box id (stbId) to be controlled
        /// </summary>
        /// <param name="id"></param>
        public void ChangeStbId(int id)
        {
            if (id <= 0) return;
            StbId = id;
        }

        /// <summary>
        /// Polls the device
        /// </summary>
        /// <remarks>
        /// Poll method is used by the communication monitor.  Update the poll method as needed for the plugin being developed
        /// </remarks>
        public void Poll()
        {
            SendText("GetAllServices");
        }

        /// <summary>
        /// Reboots the client using the settop box Id (stbId)
        /// </summary>
        public void RebootClient()
        {
            SendText("Reboot");
        }

        /// <summary>
        /// Gets the client services
        /// </summary>
        public void GetAllServices()
        {
            SendText("GetAllServices");
        }

        /// <summary>
        /// client channel up
        /// </summary>
        public void ChannelUp()
        {
            SendText("ChannelUp");
        }

        /// <summary>
        /// client channel down
        /// </summary>
        public void ChannelDown()
        {
            SendText("ChannelDown");
        }

        /// <summary>
        /// client channel direct tune by channel number
        /// </summary>
        /// <param name="channel"></param>
        public void ChannelSet(int channel)
        {
            if (channel <= 0) return;
            SendText("SelectChannel", channel);
        }

        /// <summary>
        /// client volume up
        /// </summary>
        public void VolumeUp()
        {
            //throw new NotImplementedException();
            SendText("HandleKeyPress", RcuKeys.VolumeUp.ToString());
        }

        /// <summary>
        /// client volume down
        /// </summary>
        public void VolumeDown()
        {
            //throw new NotImplementedException();
            SendText("HandleKeyPress", RcuKeys.VolumeDown.ToString());
        }

        /// <summary>
        /// client volume level set (0-65535 scaled to 0-100)
        /// </summary>
        /// <param name="level">0-65535</param>
        public void VolumeSet(int level)
        {
            if (level < 0) return;
            SendText("SetVolume", CrestronEnvironment.ScaleWithLimits(level, 65535, 0, 100, 0));
        }

        /// <summary>
        /// client mute toggle
        /// </summary>
        public void VolumeMuteToggle()
        {
            //throw new NotImplementedException()
            SendText("HandleKeyPress", RcuKeys.Mute.ToString());
        }

        /// <summary>
        /// client volume mute on
        /// </summary>
        public void VolumeMuteOn()
        {
            SendText("SetMute", true);
        }

        /// <summary>
        /// client volume mute off
        /// </summary>
        public void VolumeMuteOff()
        {
            SendText("SetMute", false);
        }

        /// <summary>
        /// queries client for current mute state
        /// </summary>
        public void GetVolumeMuteState()
        {
            SendText("GetAudioState");
        }

        #region rcu kp presses


        /// <summary>
        /// rcu key press enum
        /// </summary>
        public enum RcuKeys
        {
            /// <summary>
            /// RcuKeys number 0
            /// </summary>
            Number0 = 0,
            /// <summary>
            /// RcuKeys number 1
            /// </summary>
            Number1 = 1,
            /// <summary>
            /// RcuKeys number 2
            /// </summary>
            Number2 = 2,
            /// <summary>
            /// RcuKeys number 3
            /// </summary>
            Number3 = 3,
            /// <summary>
            /// RcuKeys number 4
            /// </summary>
            Number4 = 4,
            /// <summary>
            /// RcuKeys number 5
            /// </summary>
            Number5 = 5,
            /// <summary>
            /// RcuKeys number 6
            /// </summary>
            Number6 = 6,
            /// <summary>
            /// RcuKeys number 7
            /// </summary>
            Number7 = 7,
            /// <summary>
            /// RcuKeys number 8
            /// </summary>
            Number8 = 8,
            /// <summary>
            /// RcuKeys number 9
            /// </summary>
            Number9 = 9,
            /// <summary>
            /// RcuKeys Channel Up
            /// </summary>
            ChannelUp = 10,
            /// <summary>
            /// RcuKeys Channel Down
            /// </summary>
            ChannelDown = 11,
            /// <summary>
            /// RcuKeys Page Up
            /// </summary>
            PageUp = 12,
            /// <summary>
            /// RcuKeys Page Down
            /// </summary>
            PageDown = 13,
            /// <summary>
            /// RcuKeys Dpad Up
            /// </summary>
            Up = 14,
            /// <summary>
            /// RcuKeys Dpad Down
            /// </summary>
            Down = 15,
            /// <summary>
            /// RcuKeys Dpad Left
            /// </summary>
            Left = 16,
            /// <summary>
            /// RcuKeys Dpad Right
            /// </summary>
            Right = 17,
            /// <summary>
            /// RcuKeys red
            /// </summary>
            Red = 18,
            /// <summary>
            /// RcuKeys green
            /// </summary>
            Green = 19,
            /// <summary>
            /// RcuKeys yellow
            /// </summary>
            Yellow = 20,
            /// <summary>
            /// RcuKeys blue
            /// </summary>
            Blue = 21,
            /// <summary>
            /// RcuKeys Dpad Ok
            /// </summary>
            Ok = 22,
            /// <summary>
            /// RcuKeys Home
            /// </summary>
            Home = 23,
            /// <summary>
            /// RcuKeys Back
            /// </summary>
            Back = 24,
            /// <summary>
            /// RcuKeys Power
            /// </summary>
            Power = 25,
            /// <summary>
            /// RcuKeys Volume Up
            /// </summary>
            VolumeUp = 26,
            /// <summary>
            /// RcuKeys Volume Down
            /// </summary>
            VolumeDown = 27,
            /// <summary>
            /// RcuKeys Mute toggle
            /// </summary>
            Mute = 28,
            /// <summary>
            /// RcuKeys Source toggle
            /// </summary>
            Source = 29,
            /// <summary>
            /// RcuKeys rewind
            /// </summary>
            Rewind = 30,
            /// <summary>
            /// RcuKeys play
            /// </summary>
            Play = 31,
            /// <summary>
            /// RcuKeys pause
            /// </summary>
            Pause = 32,
            /// <summary>
            /// RcuKeys forward
            /// </summary>
            Forward = 33,
            /// <summary>
            /// RcuKeys stop
            /// </summary>
            Stop = 34,
            /// <summary>
            /// RcuKeys Record
            /// </summary>
            Record = 35,
            /// <summary>
            /// RcuKeys Guide
            /// </summary>
            Guide = 36,
            /// <summary>
            /// RcuKeys TV
            /// </summary>
            Tv = 37,
            /// <summary>
            /// RcuKeys Info
            /// </summary>
            Info = 38,
            /// <summary>
            /// RcuKeys Movies
            /// </summary>
            Movies = 39,
            /// <summary>
            /// RcuKeys PVR
            /// </summary>
            Pvr = 40,
            /// <summary>
            /// RcuKeys Music
            /// </summary>
            Music = 41,
            /// <summary>
            /// RcuKeys Titles (aka closed captions)
            /// </summary>
            Titles = 42
        }

        /// <summary>
        /// emulate rcu key presses
        /// </summary>
        /// <param name="key"></param>
        public void RcuKeyPress(RcuKeys key)
        {
            Debug.Console(TriplePlayDebug.Verbose, this, "RcuKeyPress: key = {0}", key);
            SendText("HandleKeyPress", key.ToString());
        }

        /// <summary>
        /// client rcu key press - 0-9 
        /// </summary>
        public void RcuKpNumbers(int number)
        {
            Debug.Console(TriplePlayDebug.Verbose, this, "RcuKpNumbers: number = {0}", number);

            if (number < 0 && number > 9) return;
            SendText("HandleKeyPress", String.Format("Number{0}", number));
        }

        #endregion


        #region presets

        /// <summary>
        /// Preset select
        /// </summary>
        /// <param name="index"></param>
        public void PresetSelect(uint index)
        {
            TriplePlayServicesPresetsConfig preset;
            if (_presets.TryGetValue((int)index, out preset))
                SendText("SelectChannel", (int)preset.ChannelNumber);
        }

        #endregion


        #region tv control

        /// <summary>
        /// client tv inputs
        /// </summary>
        public enum TvControlInputs
        {
            /// <summary>
            /// TV Input Select - HDMI (101)
            /// </summary>
            Hdmi = 101,
            /// <summary>
            /// TV Input Select - DVI (102)
            /// </summary>
            Dvi = 102,
            /// <summary>
            /// TV Input Select - VGA (103)
            /// </summary>
            Vga = 103,
            /// <summary>
            /// TV Input Select - DisplayPort (139)
            /// </summary>
            DisplayPort = 139,
            /// <summary>
            /// TV Input Select - HDMI1 (153)
            /// </summary>
            Hdmi1 = 153,
            /// <summary>
            /// TV Input Select - HDMI2 (154)
            /// </summary>
            Hdmi2 = 154,
            /// <summary>
            /// TV Input Select - HDMI3 (155)
            /// </summary>
            Hdmi3 = 155,
            /// <summary>
            /// TV Input Select - HDMI4 (156)
            /// </summary>
            Hdmi4 = 156,
            /// <summary>
            /// TV Input Select - Media (158)
            /// </summary>
            Media = 158,
            /// <summary>
            /// TV Input Select - PC (160)
            /// </summary>
            Pc = 160
        }

        /// <summary>
        /// client tv control - power on
        /// </summary>
        public void TvPowerOn()
        {
            SendText("PowerOnTv");
        }

        /// <summary>
        /// client tv control - power off
        /// </summary>
        public void TvPowerOff()
        {
            SendText("PowerOffTv");
        }

        /// <summary>
        /// queries client for tv power status
        /// </summary>
        public void GetTvPowerStatus()
        {
            SendText("GetPowerStatus");
        }

        /// <summary>
        /// client tv control - input select
        /// </summary>
        /// <param name="input"></param>
        public void TvInputSelect(TvControlInputs input)
        {
            SendText("SelectTVInput", (int)input);
        }

        /// <summary>
        /// client tv control - volume level set (0-65535 scaled to 0-100)
        /// </summary>
        /// <param name="level">0-65535</param>
        public void TvVolumeSet(int level)
        {
            if (level < 0) return;
            SendText("SetTVVolume", CrestronEnvironment.ScaleWithLimits(level, 65535, 0, 100, 0));
        }

        #endregion
    }
}

