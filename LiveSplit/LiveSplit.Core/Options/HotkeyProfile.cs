﻿using LiveSplit.Model.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using static LiveSplit.UI.SettingsHelper;

namespace LiveSplit.Options
{
    public class HotkeyProfile : ICloneable
    {
        public KeyOrButton SplitKey { get; set; }
        public KeyOrButton ResetKey { get; set; }
        public KeyOrButton SkipKey { get; set; }
        public KeyOrButton UndoKey { get; set; }
        public KeyOrButton PauseKey { get; set; }
        public KeyOrButton ToggleGlobalHotkeys { get; set; }
        public KeyOrButton SwitchComparisonPrevious { get; set; }
        public KeyOrButton SwitchComparisonNext { get; set; }
        public KeyOrButton NextBackground { get; set; }

        public float HotkeyDelay { get; set; }
        public bool GlobalHotkeysEnabled { get; set; }
        public bool DeactivateHotkeysForOtherPrograms { get; set; }
        public bool DoubleTapPrevention { get; set; }
        public bool AllowGamepadsAsHotkeys { get; set; }

        public const string DefaultHotkeyProfileName = "Default";

        public static HotkeyProfile FromXml(XmlElement element, Version version)
        {
            var hotkeyProfile = new HotkeyProfile();

            var keyStart = element["SplitKey"];
            if (!string.IsNullOrEmpty(keyStart.InnerText))
                hotkeyProfile.SplitKey = new KeyOrButton(keyStart.InnerText);
            else
                hotkeyProfile.SplitKey = null;

            var keyReset = element["ResetKey"];
            if (!string.IsNullOrEmpty(keyReset.InnerText))
                hotkeyProfile.ResetKey = new KeyOrButton(keyReset.InnerText);
            else
                hotkeyProfile.ResetKey = null;

            var keySkip = element["SkipKey"];
            if (!string.IsNullOrEmpty(keySkip.InnerText))
                hotkeyProfile.SkipKey = new KeyOrButton(keySkip.InnerText);
            else
                hotkeyProfile.SkipKey = null;

            var keyUndo = element["UndoKey"];
            if (!string.IsNullOrEmpty(keyUndo.InnerText))
                hotkeyProfile.UndoKey = new KeyOrButton(keyUndo.InnerText);
            else
                hotkeyProfile.UndoKey = null;

            if (version >= new Version(1, 0))
            {
                var keyPause = element["PauseKey"];
                if (!string.IsNullOrEmpty(keyPause.InnerText))
                    hotkeyProfile.PauseKey = new KeyOrButton(keyPause.InnerText);
                else
                    hotkeyProfile.PauseKey = null;

                var keyToggle = element["ToggleGlobalHotkeys"];
                if (!string.IsNullOrEmpty(keyToggle.InnerText))
                    hotkeyProfile.ToggleGlobalHotkeys = new KeyOrButton(keyToggle.InnerText);
                else
                    hotkeyProfile.ToggleGlobalHotkeys = null;

                if (version >= new Version(1, 3))
                {
                    var keySwitchPrevious = element["SwitchComparisonPrevious"];
                    if (!string.IsNullOrEmpty(keySwitchPrevious.InnerText))
                        hotkeyProfile.SwitchComparisonPrevious = new KeyOrButton(keySwitchPrevious.InnerText);
                    else
                        hotkeyProfile.SwitchComparisonPrevious = null;

                    var keySwitchNext = element["SwitchComparisonNext"];
                    if (!string.IsNullOrEmpty(keySwitchNext.InnerText))
                        hotkeyProfile.SwitchComparisonNext = new KeyOrButton(keySwitchNext.InnerText);
                    else
                        hotkeyProfile.SwitchComparisonNext = null;

                    var keyNextBG = element["NextBackground"];
                    if (!string.IsNullOrEmpty(keyNextBG.InnerText))
                        hotkeyProfile.NextBackground = new KeyOrButton(keyNextBG.InnerText);
                    else
                        hotkeyProfile.NextBackground = null;
                }
            }

            hotkeyProfile.GlobalHotkeysEnabled = ParseBool(element["GlobalHotkeysEnabled"], false);
            hotkeyProfile.DeactivateHotkeysForOtherPrograms = ParseBool(element["DeactivateHotkeysForOtherPrograms"], false);
            hotkeyProfile.DoubleTapPrevention = ParseBool(element["DoubleTapPrevention"], true);
            hotkeyProfile.HotkeyDelay = ParseFloat(element["HotkeyDelay"], 0f);

            hotkeyProfile.AllowGamepadsAsHotkeys = ParseBool(element["AllowGamepadsAsHotkeys"], false);
            if (version < new Version(1, 8, 17) && !hotkeyProfile.AnyGamepadKeys())
            {
                hotkeyProfile.AllowGamepadsAsHotkeys = false;
            }

            return hotkeyProfile;
        }

        public XmlElement ToXml(XmlDocument document, string name = "HotkeyProfile")
        {
            var parent = document.CreateElement(name);

            var splitKey = document.CreateElement("SplitKey");
            if (SplitKey != null)
                splitKey.InnerText = SplitKey.ToString();
            parent.AppendChild(splitKey);

            var resetKey = document.CreateElement("ResetKey");
            if (ResetKey != null)
                resetKey.InnerText = ResetKey.ToString();
            parent.AppendChild(resetKey);

            var skipKey = document.CreateElement("SkipKey");
            if (SkipKey != null)
                skipKey.InnerText = SkipKey.ToString();
            parent.AppendChild(skipKey);

            var undoKey = document.CreateElement("UndoKey");
            if (UndoKey != null)
                undoKey.InnerText = UndoKey.ToString();
            parent.AppendChild(undoKey);

            var pauseKey = document.CreateElement("PauseKey");
            if (PauseKey != null)
                pauseKey.InnerText = PauseKey.ToString();
            parent.AppendChild(pauseKey);

            var toggleKey = document.CreateElement("ToggleGlobalHotkeys");
            if (ToggleGlobalHotkeys != null)
                toggleKey.InnerText = ToggleGlobalHotkeys.ToString();
            parent.AppendChild(toggleKey);

            var switchComparisonPrevious = document.CreateElement("SwitchComparisonPrevious");
            if (SwitchComparisonPrevious != null)
                switchComparisonPrevious.InnerText = SwitchComparisonPrevious.ToString();
            parent.AppendChild(switchComparisonPrevious);

            var switchComparisonNext = document.CreateElement("SwitchComparisonNext");
            if (SwitchComparisonNext != null)
                switchComparisonNext.InnerText = SwitchComparisonNext.ToString();
            parent.AppendChild(switchComparisonNext);

            var nextBackground = document.CreateElement("NextBackground");
            if (NextBackground != null)
                nextBackground.InnerText = NextBackground.ToString();
            parent.AppendChild(nextBackground);

            CreateSetting(document, parent, "GlobalHotkeysEnabled", GlobalHotkeysEnabled);
            CreateSetting(document, parent, "DeactivateHotkeysForOtherPrograms", DeactivateHotkeysForOtherPrograms);
            CreateSetting(document, parent, "DoubleTapPrevention", DoubleTapPrevention);
            CreateSetting(document, parent, "HotkeyDelay", HotkeyDelay);
            CreateSetting(document, parent, "AllowGamepadsAsHotkeys", AllowGamepadsAsHotkeys);

            return parent;
        }

        public object Clone()
        {
            return new HotkeyProfile()
            {
                SplitKey = SplitKey,
                ResetKey = ResetKey,
                SkipKey = SkipKey,
                UndoKey = UndoKey,
                PauseKey = PauseKey,
                ToggleGlobalHotkeys = ToggleGlobalHotkeys,
                SwitchComparisonPrevious = SwitchComparisonPrevious,
                SwitchComparisonNext = SwitchComparisonNext,
                NextBackground = NextBackground,
                HotkeyDelay = HotkeyDelay,
                GlobalHotkeysEnabled = GlobalHotkeysEnabled,
                DeactivateHotkeysForOtherPrograms = DeactivateHotkeysForOtherPrograms,
                DoubleTapPrevention = DoubleTapPrevention,
                AllowGamepadsAsHotkeys = AllowGamepadsAsHotkeys
            };
        }

        private bool AnyGamepadKeys()
        {
            var buttons = new List<KeyOrButton> {
                SplitKey,
                ResetKey,
                SkipKey,
                UndoKey,
                PauseKey,
                ToggleGlobalHotkeys,
                SwitchComparisonPrevious,
                SwitchComparisonNext,
                NextBackground
            };

            return buttons.Any(button => button?.IsButton ?? false);
        }
    }
}
