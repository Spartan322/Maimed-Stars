using Godot;
using SpartansLib.Common;

namespace MSG.Script.UI.Base
{
    public class MenuRow : HBoxContainer
    {
        public Label Label;
        public Control InputObject;

        public MenuRow() : this(null)
        {
        }

        public MenuRow(Control inputObj)
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

            if (inputObj != null)
            {
                InputObject = this.AddChildAndReturn(inputObj);
                InputObject.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
            }
        }

        public override void _Ready()
        {
            Name = $"{nameof(MenuRow)}+{InputObject.ToString()}";
        }
    }

    public class MenuRow<T> : MenuRow
        where T : Control, new()
    {
        public new T InputObject
        {
            get => (T)base.InputObject;
            set => base.InputObject = value;
        }

        public MenuRow() : base(new T())
        {
        }
    }
}
