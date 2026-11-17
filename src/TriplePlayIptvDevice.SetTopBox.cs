using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Presets;

namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer
{
    /// <summary>
    /// ISetTopBoxControls, ITvPresetsProvider and routing-source members. Each button sends one
    /// TriplePlay HandleKeyPress on press and ignores the release.
    /// </summary>
    public partial class TriplePlayIptvDevice
    {
        // Runs first in the constructor so TvPresets and OutputPorts are never null:
        // Mobile Control's presets messenger reads TvPresets without a null check.
        private void InitializeSetTopBox(string presetsList)
        {
            HasDpad = true;
            HasNumeric = true;
            HasDvr = false;
            HasPresets = !string.IsNullOrEmpty(presetsList);

            LoadPresets(presetsList ?? string.Empty);

            AnyVideoOut = new RoutingOutputPort(RoutingPortNames.AnyVideoOut, eRoutingSignalType.AudioVideo,
                eRoutingPortConnectionType.Hdmi, null, this);
            OutputPorts = new RoutingPortCollection<RoutingOutputPort> { AnyVideoOut };
        }

        private void PressKey(bool pressRelease, RcuKeys key)
        {
            if (pressRelease) RcuKeyPress(key);
        }

        #region IRoutingSource Members

        /// <summary>
        /// Audio/video output of the set-top box, for tie lines
        /// </summary>
        public RoutingOutputPort AnyVideoOut { get; private set; }

        /// <summary>
        /// Output ports
        /// </summary>
        public RoutingPortCollection<RoutingOutputPort> OutputPorts { get; private set; }

        #endregion

        #region ISetTopBoxControls Members

        /// <summary>
        /// Show DVR controls
        /// </summary>
        public bool HasDvr { get; private set; }

        /// <summary>
        /// Show preset controls; true when a presetsList file is configured
        /// </summary>
        public bool HasPresets { get; private set; }

        /// <summary>
        /// Show number pad controls
        /// </summary>
        public bool HasNumeric { get; private set; }

        /// <summary>
        /// Show D-pad controls
        /// </summary>
        public bool HasDpad { get; private set; }

        /// <summary>
        /// TV presets loaded from the configured presetsList file (empty when none is configured)
        /// </summary>
        public DevicePresetsModel TvPresets { get; private set; }

        /// <summary>
        /// Loads presets from a file in the processor's presets list folder
        /// </summary>
        /// <param name="filePath">file name, relative to /html/presets/lists/</param>
        public void LoadPresets(string filePath)
        {
            TvPresets = new DevicePresetsModel(Key + "-presets", this, filePath);
            DeviceManager.AddDevice(TvPresets);
        }

        /// <summary>
        /// DVR list (TriplePlay PVR key)
        /// </summary>
        public void DvrList(bool pressRelease) { PressKey(pressRelease, RcuKeys.Pvr); }

        /// <summary>
        /// Replay; TriplePlay has no replay key, so this does nothing
        /// </summary>
        public void Replay(bool pressRelease) { }

        #endregion

        #region IUiDisplayInfo Members

        /// <summary>
        /// UI type for set-top box pages
        /// </summary>
        public uint DisplayUiType { get { return DisplayUiConstants.TypeDirecTv; } }

        #endregion

        #region IChannel Members

        /// <summary>
        /// Channel up
        /// </summary>
        public void ChannelUp(bool pressRelease)
        {
            if (pressRelease) ChannelUp();
        }

        /// <summary>
        /// Channel down
        /// </summary>
        public void ChannelDown(bool pressRelease)
        {
            if (pressRelease) ChannelDown();
        }

        /// <summary>
        /// Last channel; TriplePlay has no last-channel key, so this does nothing
        /// </summary>
        public void LastChannel(bool pressRelease) { }

        /// <summary>
        /// Guide
        /// </summary>
        public void Guide(bool pressRelease) { PressKey(pressRelease, RcuKeys.Guide); }

        /// <summary>
        /// Info
        /// </summary>
        public void Info(bool pressRelease) { PressKey(pressRelease, RcuKeys.Info); }

        /// <summary>
        /// Exit (TriplePlay Back key); shared by IChannel and IDPad
        /// </summary>
        public void Exit(bool pressRelease) { PressKey(pressRelease, RcuKeys.Back); }

        #endregion

        #region IColor Members

        /// <summary>
        /// Red
        /// </summary>
        public void Red(bool pressRelease) { PressKey(pressRelease, RcuKeys.Red); }

        /// <summary>
        /// Green
        /// </summary>
        public void Green(bool pressRelease) { PressKey(pressRelease, RcuKeys.Green); }

        /// <summary>
        /// Yellow
        /// </summary>
        public void Yellow(bool pressRelease) { PressKey(pressRelease, RcuKeys.Yellow); }

        /// <summary>
        /// Blue
        /// </summary>
        public void Blue(bool pressRelease) { PressKey(pressRelease, RcuKeys.Blue); }

        #endregion

        #region IDPad Members

        /// <summary>
        /// D-pad up
        /// </summary>
        public void Up(bool pressRelease) { PressKey(pressRelease, RcuKeys.Up); }

        /// <summary>
        /// D-pad down
        /// </summary>
        public void Down(bool pressRelease) { PressKey(pressRelease, RcuKeys.Down); }

        /// <summary>
        /// D-pad left
        /// </summary>
        public void Left(bool pressRelease) { PressKey(pressRelease, RcuKeys.Left); }

        /// <summary>
        /// D-pad right
        /// </summary>
        public void Right(bool pressRelease) { PressKey(pressRelease, RcuKeys.Right); }

        /// <summary>
        /// D-pad select (TriplePlay OK key)
        /// </summary>
        public void Select(bool pressRelease) { PressKey(pressRelease, RcuKeys.Ok); }

        /// <summary>
        /// Menu (TriplePlay Home key)
        /// </summary>
        public void Menu(bool pressRelease) { PressKey(pressRelease, RcuKeys.Home); }

        #endregion

        #region ISetTopBoxNumericKeypad Members

        /// <summary>Digit 0</summary>
        public void Digit0(bool pressRelease) { PressKey(pressRelease, RcuKeys.Number0); }

        /// <summary>Digit 1</summary>
        public void Digit1(bool pressRelease) { PressKey(pressRelease, RcuKeys.Number1); }

        /// <summary>Digit 2</summary>
        public void Digit2(bool pressRelease) { PressKey(pressRelease, RcuKeys.Number2); }

        /// <summary>Digit 3</summary>
        public void Digit3(bool pressRelease) { PressKey(pressRelease, RcuKeys.Number3); }

        /// <summary>Digit 4</summary>
        public void Digit4(bool pressRelease) { PressKey(pressRelease, RcuKeys.Number4); }

        /// <summary>Digit 5</summary>
        public void Digit5(bool pressRelease) { PressKey(pressRelease, RcuKeys.Number5); }

        /// <summary>Digit 6</summary>
        public void Digit6(bool pressRelease) { PressKey(pressRelease, RcuKeys.Number6); }

        /// <summary>Digit 7</summary>
        public void Digit7(bool pressRelease) { PressKey(pressRelease, RcuKeys.Number7); }

        /// <summary>Digit 8</summary>
        public void Digit8(bool pressRelease) { PressKey(pressRelease, RcuKeys.Number8); }

        /// <summary>Digit 9</summary>
        public void Digit9(bool pressRelease) { PressKey(pressRelease, RcuKeys.Number9); }

        /// <summary>
        /// Dash; TriplePlay has no dash key, so this does nothing
        /// </summary>
        public void Dash(bool pressRelease) { }

        /// <summary>
        /// Keypad enter (TriplePlay OK key). Preset recall dials the digits and then calls this.
        /// </summary>
        public void KeypadEnter(bool pressRelease) { PressKey(pressRelease, RcuKeys.Ok); }

        /// <summary>No accessory button 1</summary>
        public bool HasKeypadAccessoryButton1 { get { return false; } }

        /// <summary>No accessory button 1</summary>
        public string KeypadAccessoryButton1Label { get { return string.Empty; } }

        /// <summary>No accessory button 1</summary>
        public void KeypadAccessoryButton1(bool pressRelease) { }

        /// <summary>No accessory button 2</summary>
        public bool HasKeypadAccessoryButton2 { get { return false; } }

        /// <summary>No accessory button 2</summary>
        public string KeypadAccessoryButton2Label { get { return string.Empty; } }

        /// <summary>No accessory button 2</summary>
        public void KeypadAccessoryButton2(bool pressRelease) { }

        #endregion

        #region ITransport Members

        /// <summary>Play</summary>
        public void Play(bool pressRelease) { PressKey(pressRelease, RcuKeys.Play); }

        /// <summary>Pause</summary>
        public void Pause(bool pressRelease) { PressKey(pressRelease, RcuKeys.Pause); }

        /// <summary>Rewind</summary>
        public void Rewind(bool pressRelease) { PressKey(pressRelease, RcuKeys.Rewind); }

        /// <summary>Fast forward</summary>
        public void FFwd(bool pressRelease) { PressKey(pressRelease, RcuKeys.Forward); }

        /// <summary>
        /// Chapter back; TriplePlay has no chapter keys, so this does nothing
        /// </summary>
        public void ChapMinus(bool pressRelease) { }

        /// <summary>
        /// Chapter forward; TriplePlay has no chapter keys, so this does nothing
        /// </summary>
        public void ChapPlus(bool pressRelease) { }

        /// <summary>Stop</summary>
        public void Stop(bool pressRelease) { PressKey(pressRelease, RcuKeys.Stop); }

        /// <summary>Record</summary>
        public void Record(bool pressRelease) { PressKey(pressRelease, RcuKeys.Record); }

        #endregion
    }
}
