using Godot;
using MSG.Global.Attribute;

namespace MSG.Global
{
    public static class DebugHandler
    {
        /* debugActive */
        public static event GlobalScript.GlobalScriptEvent<bool> DebugModeModified;

        public static bool DebugMode
        {
            get => (bool) ProjectSettings.GetSetting("extra_settings/game_settings/debug_mode");
            set => ProjectSettings.SetSetting("extra_settings/game_settings/debug_mode", value);
        }

        [Input]
        public static void OnInput(GlobalScript global, InputEvent @event)
        {
            if (OS.IsDebugBuild() && Input.IsActionJustPressed("debug_enable_debug_mode"))
                DebugModeModified?.Invoke(global, DebugMode = !DebugMode);
        }
    }
}