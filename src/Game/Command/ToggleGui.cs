using Godot;
using MSG.Engine.Command;
using MSG.Script.Gui.Game.Frontend;
using SpartansLib;

namespace MSG.Game.Command
{
    public class ToggleGui : BaseCommand
    {
        public override string Name => "toggle_gui";
        public override string Description => "Toggles the visibility of the general game GUI.";

        public override CompiledCommand GetCompiledCommand(ArgList args)
        {
            var cc = new CompiledCommandFilled();
            cc.OnInvoke += script => { Execute(null, null); };
            return cc;
        }

        protected override void Execute(ICommandInterface console, ArgList arguments)
        {
            var coreInterface = NodeRegistry.Get<Control>(nameof(GameFrontend));
            coreInterface.Visible = !coreInterface.Visible;
        }
    }
}