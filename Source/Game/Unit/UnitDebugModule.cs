using Godot;
using MSG.Global;

namespace MSG.Game.Unit
{
    public class UnitDebugModule : UnitBaseModule
    {
        private readonly Label _debugLabel = new Label
        {
            Visible = DebugHandler.DebugMode,
            Uppercase = true,
            Valign = Label.VAlign.Center,
            Align = Label.AlignEnum.Center,
            RectPosition = new Vector2(-25, -12.5f),
            RectSize = new Vector2(50,25)
        };

        public string DebugString
            => //Instance.Name + "\n" + ZIndex + ":" + GetIndex();
                $"{UnitController.NodeObject.Name}\n{UnitController.NodeObject.ZIndex}:{UnitController.NodeObject.GetIndex()}";

        public UnitDebugModule(IUnitController unitController) : base(unitController)
        {
            _debugLabel.RectPivotOffset = _debugLabel.RectSize / 2;
            DebugHandler.DebugModeModified += (global, debugMode) => _debugLabel.Visible = debugMode;
        }

        public override void Ready()
        {
            UnitController.NodeObject.AddChild(_debugLabel.Duplicate());
        }

        public override void Process(float delta)
        {
            if (!_debugLabel.Visible) return;
            _debugLabel.SetRotation(-UnitController.NodeObject.Rotation);
            _debugLabel.Text = DebugString;
        }
    }
}