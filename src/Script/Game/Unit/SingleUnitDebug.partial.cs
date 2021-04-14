using Godot;

namespace MSG.Script.Game.Unit
{
    public partial class SingleUnit
    {
        private static readonly Vector2 _debugLabelRectSize = new Vector2(50, 25);

        private static readonly Label _debugLabelTemplate = new Label
        {
            Visible = false,
            Uppercase = true,
            Valign = Label.VAlign.Center,
            Align = Label.AlignEnum.Center,
            RectPosition = -(_debugLabelRectSize / 2),
            RectSize = _debugLabelRectSize,
            RectPivotOffset = _debugLabelRectSize / 2
        };

        private readonly Label _debugLabel = (Label)_debugLabelTemplate.Duplicate();

        public string DebugString
            => $"{UnitName}\n{ZIndex}:{GetIndex()}";

        private void _HandeDebugReady()
        {
            _debugLabel.Visible = Nation.World.Domain.DebugMode;
            Nation.World.Domain.OnDebugModeChange += (global, debugMode)
                => _debugLabel.Visible = debugMode;
            AddChild(_debugLabel);

        }

        private void _HandleDebugProcess(float delta)
        {
            if (!_debugLabel.Visible) return;
            _debugLabel.SetRotation(-Rotation);
            _debugLabel.Text = DebugString;
        }
    }
}