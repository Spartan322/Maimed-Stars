using Godot;
using MSG.Global.Attribute;

namespace MSG.Global
{
    public static class MouseWatcher
    {
        public static event GlobalScript.GlobalScriptEvent<(Vector2 prevOrigin, Vector2 currentOrigin)> MouseGlobalMove;
        public static event GlobalScript.GlobalScriptEvent<(Vector2 prevOrigin, Vector2 currentOrigin)> MouseLocalMove;

        public static Vector2 MouseOriginGlobal { get; private set; }
        public static Vector2 MouseOriginLocal { get; private set; }
        public static Vector2 PrevMouseOriginGlobal { get; private set; }
        public static Vector2 PrevMouseOriginLocal { get; private set; }

        [Process]
        public static void OnProcess(GlobalScript global, float delta)
        {
            PrevMouseOriginGlobal = MouseOriginGlobal;
            PrevMouseOriginLocal = MouseOriginLocal;
            MouseOriginGlobal = global.GetGlobalMousePosition();
            MouseOriginLocal = global.GetViewport().GetMousePosition();
            if (PrevMouseOriginGlobal != MouseOriginGlobal)
                MouseGlobalMove?.Invoke(global, (PrevMouseOriginGlobal, MouseOriginGlobal));
            if (PrevMouseOriginLocal != MouseOriginLocal)
                MouseLocalMove?.Invoke(global, (PrevMouseOriginLocal, MouseOriginLocal));
        }
    }
}