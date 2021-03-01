using Godot;
using MSG.Script.Gui.Window;
using static MSG.Script.Gui.Window.BaseWindow;

namespace MSG.Script.Gui.Game.Menu
{
    public class ShipTemplateMenu : PanelContainer
    {
        [Export] public NodePath ShipClassLabelPath;
        public Label ShipClassLabel;

        [Export] public NodePath SpecificationVBoxPath;
        public VBoxContainer SpecificationVBox;

        [Export] public NodePath ShipClassEditPath;
        public LineEdit ShipClassEdit;

        [Export] public NodePath ClassListVBoxPath;
        public VBoxContainer ClassListVBox;

        private BaseWindow _parent;

        public override void _Ready()
        {
            ShipClassLabel = GetNode<Label>(ShipClassLabelPath);
            SpecificationVBox = GetNode<VBoxContainer>(SpecificationVBoxPath);
            ShipClassEdit = GetNode<LineEdit>(ShipClassEditPath);
            ClassListVBox = GetNode<VBoxContainer>(ClassListVBoxPath);
            _parent = GetParent<BaseWindow>();
            _parent.OnButtonPressed += (window, button) =>
            {
                if (button.GetIndex() == (int)WindowButton.Close) OnQuitButtonPressed();
            };
        }

        public bool Active
        {
            get => Visible;
            set
            {
                if (Active == value) return;
                Visible = value;
                if (!value) ReleaseFocus();
                //SetProcess(value);
            }
        }

        public void OnQuitButtonPressed()
        {
            Active = false;
        }

        public void OnSaveButtonPressed()
        {
        }

        public void OnClearButtonPressed()
        {
        }

        public void OnCreateClassButtonPressed()
        {
        }
    }
}