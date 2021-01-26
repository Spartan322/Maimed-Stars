using Godot;
using MSG.Script.UI.Base;

namespace MSG.Script.UI.Game
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

        [Export] public NodePath TopWindowDecorationPath;
        public TopWindowDecoration TopWindowDecoration;

        public override void _Ready()
        {
            ShipClassLabel = GetNode<Label>(ShipClassLabelPath);
            SpecificationVBox = GetNode<VBoxContainer>(SpecificationVBoxPath);
            ShipClassEdit = GetNode<LineEdit>(ShipClassEditPath);
            ClassListVBox = GetNode<VBoxContainer>(ClassListVBoxPath);
            TopWindowDecoration = GetNode<TopWindowDecoration>(TopWindowDecorationPath);
            TopWindowDecoration.OnButtonPressed += (decor, args) =>
            {
                if (args.ButtonType.IsExit()) OnQuitButtonPressed();
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