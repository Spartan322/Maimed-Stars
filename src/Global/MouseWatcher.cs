using Godot;
using MSG.Global.Attribute;

namespace MSG.Global
{
    public static class MouseWatcher
    {
        public static event GlobalScript.GlobalScriptEvent<(Vector2 prevOrigin, Vector2 currentOrigin)> MouseGlobalMove;
        public static Vector2 MouseOriginGlobal { get; private set; }
        public static Vector2 PrevMouseOriginGlobal { get; private set; }

        [Process]
        public static void OnProcess(GlobalScript global, float delta)
        {
            PrevMouseOriginGlobal = MouseOriginGlobal;
            MouseOriginGlobal = global.GetGlobalMousePosition();
            if (PrevMouseOriginGlobal != MouseOriginGlobal)
                MouseGlobalMove?.Invoke(global, (PrevMouseOriginGlobal, MouseOriginGlobal));
        }
    }
}