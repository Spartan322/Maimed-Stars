using System;
using Godot;
using Godot.Collections;
using GStringDic = Godot.Collections.Dictionary<string, string>;

namespace MSG.Engine
{
    public static class SettingsManager
    {
        public static event Action<string, object> OnSettingChanged;

        public static string[] ActionDisplayBlacklist
            => (string[]) ProjectSettings.GetSetting("extra_settings/control_settings/action_rebind_blacklist");

        public static GStringDic ActionDescriptions
            => new GStringDic(
                (Dictionary) ProjectSettings.GetSetting("extra_settings/control_settings/action_descriptions"));

        public static float CameraMovementMultiplier
        {
            get => (float) ProjectSettings.GetSetting("extra_settings/control_settings/camera_movement_multiplier");
            set
            {
                ProjectSettings.SetSetting("extra_settings/control_settings/camera_movement_multiplier", value);
                OnSettingChanged?.Invoke(nameof(CameraMovementMultiplier), value);
            }
        }

        public static bool EdgeScroll
        {
            get => (bool) ProjectSettings.GetSetting("extra_settings/control_settings/edge_scroll");
            set
            {
                ProjectSettings.SetSetting("extra_settings/control_settings/edge_scroll", value);
                OnSettingChanged?.Invoke(nameof(EdgeScroll), value);
            }
        }
    }
}