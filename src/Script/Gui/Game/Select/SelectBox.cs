using System.Collections.Generic;
using Godot;
using MSG.Engine;
using MSG.Game.Rts.Unit;
using SpartansLib;
using SpartansLib.Attributes;
using SpartansLib.Extensions;

namespace MSG.Script.Gui.Game.Select
{
    // TODO: convert SelectionBox to use its Area2D child
    [Global]
    public class SelectBox : Control
    {
        [Node] public Area2D Area2D;

        private Color _color = new Color(1, 1, 1);

        [Export]
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                Update();
            }
        }

        private float _width = 1;

        [Export]
        public float Width
        {
            get => _width;
            set
            {
                _width = value;
                Update();
            }
        }

        private bool _antialiased;

        [Export]
        public bool Antialiased
        {
            get => _antialiased;
            set
            {
                _antialiased = value;
                Update();
            }
        }

        [Export] public float SelectionStartThreshold = 20;

        internal Rect2 selectGlobal;
        public Rect2 SelectGlobal => selectGlobal.Abs();

        private bool _isActivelySelecting;

        public bool IsActivelySelecting
        {
            get => _isActivelySelecting;
            set
            {
                SetPhysicsProcess(value);
                _isActivelySelecting = value;
            }
        }

        public bool HasValidSelectionArea =>
            Visible || SelectGlobal.Size.LengthSquared() >= Mathf.Pow(SelectionStartThreshold, 2);

        private void UpdateRect()
        {
            RectSize = SelectGlobal.Size;
            RectPosition = SelectGlobal.Position;
            var areaValue = RectSize / 2;
            Area2D.Position = areaValue;
            Area2D.Scale = areaValue;
        }

        public void Reset()
        {
            Visible = false;
            IsActivelySelecting = false;
            selectGlobal = new Rect2();
            UpdateRect();
        }

        private SelectionDisplay _selectionDisplay;
        public override void _Ready()
        {
            _selectionDisplay = NodeRegistry.Get<SelectionDisplay>();
            IsActivelySelecting = IsActivelySelecting;
        }

        public override void _PhysicsProcess(float delta)
        {
            if (Godot.Engine.EditorHint || GetTree().IsInputHandled() && !IsActivelySelecting) return;
            selectGlobal.End = GetGlobalMousePosition();
            Visible = HasValidSelectionArea;
            UpdateRect();
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event.LeftMouseIsJustPressed())
            {
                selectGlobal.Position = GetGlobalMousePosition();
                IsActivelySelecting = true;
            }
            else if (IsActivelySelecting && @event.LeftMouseIsJustReleased())
            {
                Visible = false;
                IsActivelySelecting = false;
                //InputHandler.SelectionHandled = false;
                if (!HasValidSelectionArea)
                {
                    return;
                }

                if (!InputManager.AddControlKeyPressed)
                    _selectionDisplay.Clear();
                if (_areaSelected.Count > 0)
                    _selectionDisplay.AddRange(_areaSelected);
                AcceptEvent();
            }
        }

        private readonly List<BaseUnit> _areaSelected = new List<BaseUnit>();

        [Connect("area_entered", "Area2D")]
        public void OnAreaEntered(Area2D area)
        {
            if (area.GetParent() is BaseUnit unit)
            {
                _areaSelected.Add(unit);
            }
        }

        [Connect("area_exited", "Area2D")]
        public void OnAreaExited(Area2D area)
        {
            if (area.GetParent() is BaseUnit unit)
            {
                _areaSelected.Remove(unit);
            }
        }

        public override void _Draw()
        {
            var topLeft = new Vector2() * RectScale;
            var bottomLeft = topLeft + Vector2.Down * RectSize * RectScale;
            var topRight = topLeft + Vector2.Right * RectSize * RectScale;
            var bottomRight = topRight + Vector2.Down * RectSize * RectScale;
            var modulateForWidth = Width > 1 ? Width / 2 : 0;
            DrawLine(topLeft, topRight.AddX(modulateForWidth), Color, Width, Antialiased);
            DrawLine(topRight, bottomRight.AddY(modulateForWidth), Color, Width, Antialiased);
            DrawLine(bottomRight, bottomLeft.SubX(modulateForWidth), Color, Width, Antialiased);
            DrawLine(bottomLeft, topLeft.SubY(modulateForWidth), Color, Width, Antialiased);
        }
    }
}
