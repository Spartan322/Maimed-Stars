using Godot;
using MSG.Game.Rts.Event;

namespace MSG.Script.Gui.Game.Event
{
    public class ChoiceNode : Control
    {
        protected ChoiceData Data;

        protected Label TextLabel { get; }
    }
}