using Godot;
using SpartansLib;
using SpartansLib.Attributes;
using SpartansLib.Common;
using SpartansLib.Extensions;

namespace MSG.Script.Gui.Window
{
    [Tool]
    public class BaseWindow : Container
    {
        private string _title;
        [Export]
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                if (TitleLabel != null)
                    TitleLabel.Text = value;
                Update();
            }
        }

        private bool _shouldCenterTitle;
        [Export]
        public bool ShouldCenterTitle
        {
            get => _shouldCenterTitle;
            set
            {
                _shouldCenterTitle = value;
                if (TitleLabel != null)
                    TitleLabel.Align = value ? Label.AlignEnum.Center : Label.AlignEnum.Left;
                Update();
            }
        }

        private bool _hasMinimizeButton;
        [Export]
        public bool HasMinimizeButton
        {
            get => _hasMinimizeButton;
            set
            {
                _hasMinimizeButton = value;
                _SetVisibility(WindowButton.Minimize, value);
            }
        }

        private bool _hasMaximizeButton;
        [Export]
        public bool HasMaximizeButton
        {
            get => _hasMaximizeButton;
            set
            {
                _hasMaximizeButton = value;
                _SetVisibility(WindowButton.Maximize, value);
            }
        }

        private bool _hasCloseButton;
        [Export]
        public bool HasCloseButton
        {
            get => _hasCloseButton;
            set
            {
                _hasCloseButton = value;
                _SetVisibility(WindowButton.Close, value);
            }
        }

        [Export]
        public bool IsMovable { get; set; }

        [Export]
        public bool IsResizeable { get; set; }

        [Export]
        public bool ShouldFreeOnClose { get; set; }

        [Node("WindowContainer/DecorationBar")]
        protected Control Decoration;
        [Node("WindowContainer/DecorationBar/DecorationContent/TitleLabel")]
        protected Label TitleLabel;
        [Node("WindowContainer/DecorationBar/DecorationContent/ButtonList")]
        protected Control DecorationButtonList;
        [Node("WindowContainer/DecorationPanel")]
        protected Control DecorationPanel;

        public enum WindowButton
        {
            Minimize,
            Maximize,
            Close
        }

        protected Button GetDecorationButton(WindowButton button)
        {
            return DecorationButtonList?.GetChild<Button>((int)button);
        }

        private bool _GetVisibility(WindowButton button)
            => GetDecorationButton(button)?.Visible ?? false;

        private void _SetVisibility(WindowButton button, bool value)
        {
            var buttonVal = GetDecorationButton(button);
            if (buttonVal != null)
                buttonVal.Visible = value;
            Update();
        }

        private bool _isMinimized;
        public bool IsMinimized
        {
            get => _isMinimized;
            set
            {
                if (_isMinimized == value) return;
                var windowContent = Content;
                if (_isMinimized)
                {
                    Decoration.SizeFlagsHorizontal |= (int)SizeFlags.Fill;
                }
                else
                {
                    Decoration.SizeFlagsHorizontal &= ~(int)SizeFlags.Fill;
                }
                if (windowContent != null)
                    windowContent.Visible = _isMinimized;
                DecorationPanel.Visible = _isMinimized;
                _isMinimized = value;
            }
        }

        private bool _maximizeMoveCheck;
        private bool _maximizeResizeCheck;
        private Rect2 _prevRect;
        private bool _isMaximized;
        public bool IsMaximized
        {
            get => _isMaximized;
            set
            {
                if (_isMaximized == value) return;
                if (_isMaximized)
                {
                    RectPosition = _prevRect.Position;
                    RectSize = _prevRect.Size;
                    IsMovable = _maximizeMoveCheck;
                    IsResizeable = _maximizeResizeCheck;
                }
                else
                {
                    _prevRect = new Rect2(RectPosition, RectSize);
                    RectPosition = Vector2.Zero;
                    RectSize = GetTree().Root.Size;
                    _maximizeMoveCheck = IsMovable;
                    _maximizeResizeCheck = IsResizeable;
                    IsMovable = false;
                    IsResizeable = false;
                }
                _isMaximized = value;
            }
        }

        public CanvasItem Content => GetChildCount() > 1 ? GetChild<CanvasItem>(1) : null;

        public void CloseWindow()
        {
            if (ShouldFreeOnClose)
            {
                GetParent().RemoveChild(this);
                QueueFree();
            }
            else Visible = false;
        }

        public override void _Ready()
        {
            base._Ready();
            Title = Title ?? TitleLabel.Text;
            HasMinimizeButton = HasMinimizeButton;
            HasMaximizeButton = HasMaximizeButton;
            HasCloseButton = HasCloseButton;
        }

        public override Vector2 _GetMinimumSize()
        {
            var content = (Content as Control);
            return content != null ? content.GetCombinedMinimumSize() + new Vector2(20, 45) : Vector2.Zero;
        }

        private DragType _dragType = DragType.None;
        private Vector2 _dragOffset, _dragOffsetFar;
        public DragType DragHitTest(Vector2 pos)
        {
            if (IsMovable && Decoration.GetGlobalRect().HasPoint(pos)) return DragType.Move;

            if (IsResizeable)
            {
                var drag = DragType.None;
                var consoleContainerRect = DecorationPanel.GetChild<Control>().GetChild<Control>(0).GetGlobalRect();
                if (pos.y < consoleContainerRect.Position.y) drag = DragType.ResizeTop;
                if (pos.y >= consoleContainerRect.End.y) drag = DragType.ResizeBottom;
                if (pos.x >= consoleContainerRect.End.x) drag |= DragType.ResizeRight;
                if (pos.x < consoleContainerRect.Position.x) drag |= DragType.ResizeLeft;
                return drag;
            }

            return DragType.None;
        }

        const int MOUSE_BUTTON = (int)ButtonList.Left;

        public override void _Process(float delta)
        {
            if (Input.IsMouseButtonPressed(MOUSE_BUTTON))
            {
                if (_dragType == DragType.Move)
                    RectGlobalPosition = GetGlobalMousePosition() - _dragOffset;
                else if (_dragType != DragType.None)
                {
                    var rect = GetGlobalRect();
                    var minSize = GetCombinedMinimumSize();
                    var globalPos = GetGlobalMousePosition();
                    if (_dragType.HasFlag(DragType.ResizeTop))
                    {
                        var end = rect.End.y;
                        rect.Position = rect.Position.Y(Mathf.Min(globalPos.y - _dragOffset.y, end - minSize.y));
                        rect.Size = rect.Size.Y(end - rect.Position.y);
                    }
                    else if (_dragType.HasFlag(DragType.ResizeBottom))
                    {
                        rect.Size = rect.Size.Y(globalPos.y - rect.Position.y + _dragOffsetFar.y);
                    }

                    if (_dragType.HasFlag(DragType.ResizeLeft))
                    {
                        var right = rect.End.x;
                        rect.Position = rect.Position.X(Mathf.Min(globalPos.x - _dragOffset.x, right - minSize.x));
                        rect.Size = rect.Size.X(right - rect.Position.x);
                    }
                    else if (_dragType.HasFlag(DragType.ResizeRight))
                    {
                        rect.Size = rect.Size.X(globalPos.x - rect.Position.x + _dragOffsetFar.x);
                    }

                    RectGlobalPosition = rect.Position;
                    RectSize = rect.Size;
                }
            }
            else _dragType = DragType.None;
        }

        public override void _GuiInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton mb)
            {
                if (mb.ButtonMask == MOUSE_BUTTON && mb.IsPressed())
                {
                    // Begin a possible dragging operation.
                    _dragType = DragHitTest(mb.GlobalPosition);
                    if (_dragType != DragType.None)
                        _dragOffset = mb.GlobalPosition - RectGlobalPosition;
                    _dragOffsetFar = GetGlobalRect().End - mb.GlobalPosition;
                }
            }

            if (@event is InputEventMouseMotion mm)
            {
                if (_dragType == DragType.None)
                {
                    // Update the cursor while moving along the borders.
                    CursorShape cursor = CursorShape.Arrow;
                    if (IsResizeable)
                    {
                        var previewDragType = DragHitTest(mm.GlobalPosition);
                        switch (previewDragType)
                        {
                            case DragType.ResizeTop | DragType.ResizeLeft:
                            case DragType.ResizeBottom | DragType.ResizeRight:
                                cursor = CursorShape.Fdiagsize;
                                break;
                            case DragType.ResizeTop | DragType.ResizeRight:
                            case DragType.ResizeBottom | DragType.ResizeLeft:
                                cursor = CursorShape.Bdiagsize;
                                break;
                            case DragType.ResizeTop:
                            case DragType.ResizeBottom:
                                cursor = CursorShape.Vsize;
                                break;
                            case DragType.ResizeLeft:
                            case DragType.ResizeRight:
                                cursor = CursorShape.Hsize;
                                break;
                        }
                    }

                    if (GetCursorShape() != cursor)
                        MouseDefaultCursorShape = cursor;
                }
            }
        }

        private const int DECORATION_MARGIN = 2;
        public override void _Notification(int what)
        {
            switch (what)
            {
                case NotificationSortChildren:
                    FitChildInRect(GetChild<Control>(0), new Rect2(Vector2.Zero, RectSize));
                    if (GetChildCount() == 1) break;

                    StyleBox style;
                    if (HasStylebox("panel")) style = GetStylebox("panel");
                    else style = GetStylebox("panel", nameof(PanelContainer));

                    var rect = new Rect2(0, Decoration.RectSize.y - 5, RectSize);
                    rect = rect.GrowIndividual(0, 0, DECORATION_MARGIN, -Decoration.RectSize.y + 7);
                    if (style != null)
                    {
                        rect.Size -= style.GetMinimumSize();
                        rect.Position += style.GetOffset();
                    }
                    for (var i = 1; i < GetChildCount(); i++)
                    {
                        var c = GetChildOrNull<Control>(i);
                        if (c == null || c.IsSetAsToplevel()) continue;
                        FitChildInRect(c, rect);
                    }
                    break;
                case NotificationMouseExit:
                case NotificationVisibilityChanged:
                case NotificationExitTree:
                    if (IsResizeable && _dragType == DragType.None && MouseDefaultCursorShape != CursorShape.Arrow)
                        MouseDefaultCursorShape = CursorShape.Arrow;
                    break;
            }
        }

        public delegate void OnButtonPressedAction(BaseWindow window, BaseButton button);
        public event OnButtonPressedAction OnButtonPressed;

        [Connect("pressed", "WindowContainer/DecorationBar/DecorationContent/ButtonList/MinimizeButton")]
        [Connect("pressed", "WindowContainer/DecorationBar/DecorationContent/ButtonList/MaximizeButton")]
        [Connect("pressed", "WindowContainer/DecorationBar/DecorationContent/ButtonList/CloseButton")]
        public void _OnButtonPressed(ulong _triggerId)
        {
            var button = (BaseButton)GD.InstanceFromId(_triggerId);
            OnButtonPressed?.Invoke(this, button);
            switch ((WindowButton)button.GetIndex())
            {
                case WindowButton.Minimize:
                    IsMinimized = !IsMinimized;
                    break;
                case WindowButton.Maximize:
                    IsMaximized = !IsMaximized;
                    break;
                case WindowButton.Close:
                    CloseWindow();
                    break;
            }
        }
    }
}
