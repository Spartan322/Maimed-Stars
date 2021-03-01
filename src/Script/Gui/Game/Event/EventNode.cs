using Godot;
using MSG.Game.Rts.Event;

namespace MSG.Script.Gui.Game.Event
{
    public class EventNode : Control
    {
        protected readonly EventData Data;

        protected Label TitleLabel { get; }
        protected TextureRect Image { get; }
        protected RichTextLabel DescriptionLabel { get; }
        protected Container ChoiceList { get; }

    }
}