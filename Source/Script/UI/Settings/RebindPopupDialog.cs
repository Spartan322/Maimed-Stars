using Godot;
using MSG.Script.UI.Settings.Tab;
using SpartansLib;
using SpartansLib.Attributes;

namespace MSG.Script.UI.Settings
{
    public class RebindPopupDialog : PanelContainer
    {
        [Node("CenterContainer/HBoxContainer/VBoxContainer/NameLabel")]
        public Label NameLabel;

        [Node("CenterContainer/HBoxContainer/VBoxContainer/DescriptionLabel")]
        public Label DescriptionLabel;

        [Node("CenterContainer/HBoxContainer/VBoxContainer/NewBindLabel")]
        public Label NewBindLabel;

        [Node("CenterContainer/HBoxContainer/VBoxContainer/HBoxContainer/AcceptButton")]
        public Button AcceptButton;

        [Node("CenterContainer/HBoxContainer/VBoxContainer/HBoxContainer/CancelButton")]
        public Button CancelButton;

        private InputEvent actionToRebind;

        public InputEvent ActionToRebind
        {
            get => actionToRebind;
            set
            {
                actionToRebind = value;
                NewBindLabel.Text = "Rebind: " + Controls.ConvertButtonText(value);
            }
        }

        private string actionNameToRebind;

        public string ActionNameToRebind
        {
            get => actionNameToRebind;
            set
            {
                actionNameToRebind = value;
                NameLabel.Text = Controls.ConvertActionName(value);
            }
        }

        private InputEvent eventToReplaceActionWith;

        public InputEvent EventToReplaceActionWith
        {
            get => eventToReplaceActionWith;
            set
            {
                eventToReplaceActionWith = value;
                NewBindLabel.Text = "Rebind: " + Controls.ConvertButtonText(value);
            }
        }

        public override void _Ready()
        {
            SetProcessInput(false);
        }

        public bool Active
        {
            get => Visible;
            set
            {
                if (Active == value) return;
                Visible = value;
                SetProcessInput(value);
            }
        }

        public override void _Input(InputEvent @event)
        {
            if (AcceptButton.IsHovered() || CancelButton.IsHovered()
                                         || @event is InputEventMouseMotion
                                         || !@event.IsPressed()) return;
            EventToReplaceActionWith = @event;
            AcceptEvent();
        }

        public void OnAcceptPressed()
        {
            Active = false;
            if (EventToReplaceActionWith == null)
                return;
            InputMap.ActionEraseEvent(ActionNameToRebind, ActionToRebind);
            InputMap.ActionAddEvent(ActionNameToRebind, EventToReplaceActionWith);
            Logger.Info($"{ActionNameToRebind} rebound to {EventToReplaceActionWith.AsText()}");
            ((Button) Controls.NameToRowControl[ActionNameToRebind].InputObject).Text =
                NewBindLabel.Text.Substring("Rebind: ".Length);
        }

        public void OnCancelPressed()
        {
            Active = false;
        }
    }
}