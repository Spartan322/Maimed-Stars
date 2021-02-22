using System;
using System.Reflection;
using Godot;
using MSG.Global.Attribute;
using SpartansLib;
using SpartansLib.Extensions;

namespace MSG.Global
{
    //[Tool]
    public class GlobalScript : Control
    {
        public static GlobalScript Singleton { get; private set; }
        bool _initalized = false;

        public delegate void GlobalScriptEvent(GlobalScript global);

        public delegate void GlobalScriptEvent<in T>(GlobalScript global, T arg1);

        public static event GlobalScriptEvent OnInit;
        public static event GlobalScriptEvent OnReady;
        public static event GlobalScriptEvent OnDraw;
        public static event GlobalScriptEvent OnEnterTree;
        public static event GlobalScriptEvent OnExitTree;
        public static event GlobalScriptEvent<InputEvent> OnInput;
        public static event GlobalScriptEvent<InputEvent> OnUnhandledInput;
        public static event GlobalScriptEvent<InputEventKey> OnUnhandledKeyInput;
        public static event GlobalScriptEvent<float> OnProcess;
        public static event GlobalScriptEvent<float> OnPhysicsProcess;
        public static event GlobalScriptEvent<int> OnNotification;
        public static event GlobalScriptEvent OnIdleFrame;
        public static event UnhandledExceptionEventHandler OnUnhandledException;

        static GlobalScript()
        {
            AppDomain.CurrentDomain.UnhandledException += CaptureUnhandledException;
            foreach (var t in Assembly.GetExecutingAssembly().GetExportedTypes())
            {
                if (!(t.IsAbstract && t.IsSealed))
                    continue;
                //RuntimeHelpers.RunClassConstructor(t.TypeHandle);
                var inEditor = Engine.EditorHint;
                foreach (var m in t.GetMethods(BindingFlags.Public | BindingFlags.Static))
                    foreach (var attrib in m.GetAllAttributesOf<MSGBaseGlobalAttribute>())
                        attrib.HandleFor(m);
            }
        }

        public override void _Ready()
        {
            PauseMode = PauseModeEnum.Process;
            if (!_initalized)
                GetTree().Connect("idle_frame", this, nameof(_OnIdleFrame));
            OnReady?.Invoke(this);
            _initalized = true;
        }

        public GlobalScript()
        {
            if (_initalized) return;
            Singleton = this;
            OnInit?.Invoke(this);
        }

        public GlobalScript(Node parent) : this()
        {
            parent.AddChild(this);
        }

        public override void _Draw() => OnDraw?.Invoke(this);
        public override void _EnterTree() => OnEnterTree?.Invoke(this);
        public override void _ExitTree() => OnExitTree?.Invoke(this);
        public override void _Input(InputEvent @event) => OnInput?.Invoke(this, @event);
        public override void _UnhandledInput(InputEvent @event) => OnUnhandledInput?.Invoke(this, @event);
        public override void _UnhandledKeyInput(InputEventKey @event) => OnUnhandledKeyInput?.Invoke(this, @event);
        public override void _Process(float delta) => OnProcess?.Invoke(this, delta);
        public override void _PhysicsProcess(float delta) => OnPhysicsProcess?.Invoke(this, delta);
        public override void _Notification(int what) => OnNotification?.Invoke(this, what);
        public void _OnIdleFrame() => OnIdleFrame?.Invoke(this);

        private static void CaptureUnhandledException(object sender, UnhandledExceptionEventArgs arg)
        {
            OnUnhandledException?.Invoke(sender, arg);
            Logger.Fatal(
                $"Unhandled Mono Exception Thrown '{arg.ExceptionObject.GetType().Name}':\n───────────────\n{arg.ExceptionObject}\n───────────────");
            if (arg.IsTerminating) Logger.Fatal("Exception Terminating");
        }
    }
}