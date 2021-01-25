using System;
using Godot;
using SpartansLib;
using SpartansLib.Common;
using SpartansLib.Extensions;

namespace MSG.Script.UI.Base
{
    [Tool]
    public class TopWindowDecoration : HBoxContainer
    {
        public class ButtonArgs : EventArgs
        {
            public StandardButton ButtonType;
            public BaseButton Button;
        }

        public event EventHandler<TopWindowDecoration, ButtonArgs> OnButtonPressed;

        public Label Label;

        public HBoxContainer ButtonList;

        public Button MinimizeButton;

        public Button MaximizeButton;

        public Button ExitButton;

        [Export]
        public string WindowName
        {
            get => _windowName;
            set
            {
                _windowName = value;
                if (Label != null)
                    Label.Text = value;
                Update();
            }
        }

        private string _windowName;


        [Export]
        public bool CanMinimize
        {
            get => _canMinimize;
            set
            {
                _canMinimize = value;
                if (MinimizeButton != null)
                    MinimizeButton.Visible = value;
                Update();
            }
        }

        private bool _canMinimize;

        [Export]
        public bool CanMaximize
        {
            get => _canMaximize;
            set
            {
                _canMaximize = value;
                if (MaximizeButton != null)
                    MaximizeButton.Visible = value;
                Update();
            }
        }

        private bool _canMaximize;

        [Export]
        public bool CanExit
        {
            get => _canExit;
            set
            {
                _canExit = value;
                if (ExitButton != null)
                    ExitButton.Visible = value;
                Update();
            }
        }

        private bool _canExit;

        public override void _Ready()
        {
            Label = GetNode<Label>(nameof(Label));
            ButtonList = GetNode<HBoxContainer>(nameof(ButtonList));
            MinimizeButton = ButtonList.GetNode<Button>(nameof(MinimizeButton));
            MaximizeButton = ButtonList.GetNode<Button>(nameof(MaximizeButton));
            ExitButton = ButtonList.GetNode<Button>(nameof(ExitButton));
            var i = 0;
            foreach (var b in ButtonList.GetChildren<BaseButton>())
            {
                b.Connect(Signal.VisibilityChanged, this, nameof(OnButtonVisibilityChanged), i);
                if (!Engine.EditorHint)
                    b.Connect(Signal.Pressed, this, nameof(OnButtonPressedSignal), b, i);
                i++;
            }

            WindowName = WindowName ?? Label.Text;
            CanMinimize = CanMinimize;
            CanMaximize = CanMaximize;
            CanExit = CanExit;
        }

        public void AddButton(BaseButton button, int? index = null)
        {
            ButtonList?.AddChild(button);
            if (index != null)
                ButtonList?.MoveChild(button, index.Value);
            if (!Engine.EditorHint)
                button.Connect(Signal.Pressed, this, nameof(OnButtonPressedSignal), button, -1);
        }

        public void RemoveButton(BaseButton button) => ButtonList?.RemoveChild(button);
        public void RemoveButton(int index) => ButtonList?.GetChild(index).RemoveAndSkip();

        private void OnButtonPressedSignal(BaseButton button, int buttonType)
            => OnButtonPressed?.Invoke(this,
                new ButtonArgs {Button = button, ButtonType = (StandardButton) buttonType});

        public void OnButtonVisibilityChanged(int val)
        {
            switch ((StandardButton) val)
            {
                case StandardButton.Minimize:
                    CanMinimize = MinimizeButton.Visible;
                    break;
                case StandardButton.Maximize:
                    CanMaximize = MaximizeButton.Visible;
                    break;
                case StandardButton.Exit:
                    CanExit = ExitButton.Visible;
                    break;
            }
        }
    }
}
