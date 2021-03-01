using Godot;
using SpartansLib.Common;

namespace MSG.Script.Gui.Menu.Settings
{
    public class SettingsRow : HBoxContainer
    {
        public Label Label;
        public Control InputObject;

        public SettingsRow() : this(null)
        {
        }

        public SettingsRow(Control inputObj)
        {
            RectMinSize = new Vector2(10, 28);
            SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
            //var hbox = this.AddChildAndReturn(new HBoxContainer());
            //hbox.AnchorRight = 1;
            //hbox.AnchorBottom = 1;
            //hbox.RectSize = new Vector2(20, 28);
            //hbox.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;

            var panel = this.AddChildAndReturn(new Panel());
            //panel.MarginRight = 15;
            //panel.MarginBottom = 28;
            panel.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
            panel.SizeFlagsVertical = (int)SizeFlags.ExpandFill;

            Label = panel.AddChildAndReturn(new Label());
            Label.Align = Label.AlignEnum.Center;
            Label.Valign = Label.VAlign.Center;
            //Label.Autowrap = true;
            Label.MaxLinesVisible = 1;
            //Label.MarginLeft = 7;
            //Label.MarginTop = 7;
            //Label.MarginRight = 8;
            //Label.MarginBottom = 21;
            //Label.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
            //Label.SizeFlagsVertical = (int)SizeFlags.ExpandFill;
            Label.MouseFilter = MouseFilterEnum.Pass;
            Label.SetAnchorsPreset(LayoutPreset.Wide, true);

            var colorRect = this.AddChildAndReturn(new ColorRect());
            colorRect.Color = Colors.White;
            colorRect.RectMinSize = new Vector2(1, 0);

            if (inputObj == null) return;
            InputObject = this.AddChildAndReturn(inputObj);
            InputObject.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
        }

        public override void _Ready()
        {
            Name = $"{nameof(SettingsRow)}+{InputObject}";
        }
    }

    public class SettingsRow<T> : SettingsRow
        where T : Control, new()
    {
        public new T InputObject
        {
            get => (T)base.InputObject;
            set => base.InputObject = value;
        }

        public SettingsRow() : base(new T())
        {
        }
    }
}
