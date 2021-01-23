using Godot;

namespace MSG.Script.Agent
{
    public partial class Ship
    {
        private static readonly Vector2 _debugLabelRectSize = new Vector2(50, 25);

        private static readonly Label _debugLabelTemplate = new Label
        {
            Visible = false,
            Uppercase = true,
            Valign = Label.VAlign.Center,
            Align = Label.AlignEnum.Center,
            RectPosition = -(_debugLabelRectSize/2),
            RectSize = _debugLabelRectSize,
            RectPivotOffset = _debugLabelRectSize/2
        };

        private readonly Label _debugLabel = (Label)_debugLabelTemplate.Duplicate();

        public string DebugString
            => //Instance.Name + "\n" + ZIndex + ":" + GetIndex();
                $"{Name}\n{ZIndex}:{GetIndex()}";

        private void _HandeDebugReady()
        {
            _debugLabel.Visible = Global.DebugHandler.DebugMode;
            Global.DebugHandler.DebugModeModified += (global, debugMode)
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