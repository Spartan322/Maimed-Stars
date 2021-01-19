using System.Reflection;
using Godot;
using SpartansLib.Extensions;
using static MSG.Global.GlobalScript;

namespace MSG.Global.Attribute
{
    public class MSGBaseGlobalAttribute : System.Attribute
    {
        public bool IsInEditor = false;

        public virtual void HandleFor(MethodInfo info)
        {
        }
    }

    public class ReadyAttribute : MSGBaseGlobalAttribute
    {
        public override void HandleFor(MethodInfo info) => OnReady += info.CreateDelegate<GlobalScriptEvent>();
    }

    public class InitAttribute : MSGBaseGlobalAttribute
    {
        public override void HandleFor(MethodInfo info) => OnInit += info.CreateDelegate<GlobalScriptEvent>();
    }

    public class DrawAttribute : MSGBaseGlobalAttribute
    {
        public override void HandleFor(MethodInfo info) => OnDraw += info.CreateDelegate<GlobalScriptEvent>();
    }

    public class EnterTreeAttribute : MSGBaseGlobalAttribute
    {
        public override void HandleFor(MethodInfo info) => OnEnterTree += info.CreateDelegate<GlobalScriptEvent>();
    }

    public class ExitTreeAttribute : MSGBaseGlobalAttribute
    {
        public override void HandleFor(MethodInfo info) => OnExitTree += info.CreateDelegate<GlobalScriptEvent>();
    }

    public class InputAttribute : MSGBaseGlobalAttribute
    {
        public override void HandleFor(MethodInfo info) =>
            OnInput += info.CreateDelegate<GlobalScriptEvent<InputEvent>>();
    }

    public class UnhandledInputAttribute : MSGBaseGlobalAttribute
    {
        public override void HandleFor(MethodInfo info) =>
            OnUnhandledInput += info.CreateDelegate<GlobalScriptEvent<InputEvent>>();
    }

    public class UnhandledKeyInputAttribute : MSGBaseGlobalAttribute
    {
        public override void HandleFor(MethodInfo info) =>
            OnUnhandledKeyInput += info.CreateDelegate<GlobalScriptEvent<InputEventKey>>();
    }

    public class ProcessAttribute : MSGBaseGlobalAttribute
    {
        public override void HandleFor(MethodInfo info) => OnProcess += info.CreateDelegate<GlobalScriptEvent<float>>();
    }

    public class PhysicsProcessAttribute : MSGBaseGlobalAttribute
    {
        public override void HandleFor(MethodInfo info) =>
            OnPhysicsProcess += info.CreateDelegate<GlobalScriptEvent<float>>();
    }

    public class NotificationAttribute : MSGBaseGlobalAttribute
    {
        public override void HandleFor(MethodInfo info) =>
            OnNotification += info.CreateDelegate<GlobalScriptEvent<int>>();
    }

    public class IdleFrameAttribute : MSGBaseGlobalAttribute
    {
        public override void HandleFor(MethodInfo info) => OnIdleFrame += info.CreateDelegate<GlobalScriptEvent>();
    }
}