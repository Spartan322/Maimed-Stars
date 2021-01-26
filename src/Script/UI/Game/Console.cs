using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MSG.Command;
using MSG.Global;
using SpartansLib;
using SpartansLib.Attributes;
using SpartansLib.Extensions;
using SpartansLib.Structure;

namespace MSG.Script.UI.Game
{
    public class Console : PanelContainer, ICommandInterface
    {
        public event Action<CommandArguments> OnExecute;

        [Export] public bool Resizable { get; private set; } = true;

        [Export] public string ExecutionLineIndicator { get; private set; } = "$";

        [Export] //  r  g  b
        public Color CommandColor { get; private set; } = ColorExt.FromRGB8(0xff_ff_66);

        [Export] public string CommandSplit { get; private set; } = ";";

        [Export] public int MaxHistorySize { get; private set; } = 10;

        [Export] public NodePath ConsoleTitlePath;
        [Node] public Label ConsoleTitle;

        [Export] public NodePath ConsoleTextPath;
        [Node] public RichTextLabel ConsoleText;

        [Export] public NodePath ConsoleLinePath;
        [Node] public LineEdit ConsoleLine;

        [Export] public NodePath ConsoleContainerPath;
        [Node] public VBoxContainer ConsoleContainer;

        private DragType dragType = DragType.None;
        private Vector2 dragOffset, dragOffsetFar;
        private Rect2 defaultRect;
        private readonly Queue<string> history = new Queue<string>();

        public override void _Ready()
        {
            defaultRect = GetGlobalRect();

            var v = Engine.GetVersionInfo();
            PrintLine(ProjectSettings.GetSetting("application/config/name")
                      + $" (Godot {v["major"]}.{v["minor"]}.{v["patch"]} {v["status"]})\n"
                      + $"Type {CommandHandler.GetCommand("help").FormatName()} to get more information about usage");

            ConsoleText.Connect("meta_clicked", this, nameof(OnMetaClicked));
            ConsoleLine.Connect("text_entered", this, nameof(OnConsoleLineTextEntered));
            ConsoleLine.Connect("gui_input", this, nameof(OnConsoleLineGuiInput));
        }

        #region Console Manipulatable Impls

        public DragType DragHitTest(Vector2 pos)
        {
            if (ConsoleTitle.GetGlobalRect().HasPoint(pos)) return DragType.Move;

            if (Resizable)
            {
                var drag = DragType.None;
                var consoleContainerRect = ConsoleContainer.GetGlobalRect();
                if (pos.y < consoleContainerRect.Position.y) drag = DragType.ResizeTop;
                if (pos.y >= consoleContainerRect.End.y) drag = DragType.ResizeBottom;
                if (pos.x >= consoleContainerRect.End.x) drag |= DragType.ResizeRight;
                if (pos.x < consoleContainerRect.Position.x) drag |= DragType.ResizeLeft;
                return drag;
            }

            return DragType.None;
        }

        const int MOUSE_BUTTON = (int) ButtonList.Left;

        public override void _Process(float delta)
        {
            if (Input.IsMouseButtonPressed(MOUSE_BUTTON))
            {
                if (dragType == DragType.Move)
                    RectGlobalPosition = GetGlobalMousePosition() - dragOffset;
                else if (dragType != DragType.None)
                {
                    var rect = GetGlobalRect();
                    var minSize = GetCombinedMinimumSize();
                    var globalPos = GetGlobalMousePosition();
                    if (dragType.HasFlag(DragType.ResizeTop))
                    {
                        var end = rect.End.y;
                        rect.Position = rect.Position.Y(Mathf.Min(globalPos.y - dragOffset.y, end - minSize.y));
                        rect.Size = rect.Size.Y(end - rect.Position.y);
                    }
                    else if (dragType.HasFlag(DragType.ResizeBottom))
                    {
                        rect.Size = rect.Size.Y(globalPos.y - rect.Position.y + dragOffsetFar.y);
                    }

                    if (dragType.HasFlag(DragType.ResizeLeft))
                    {
                        var right = rect.End.x;
                        rect.Position = rect.Position.X(Mathf.Min(globalPos.x - dragOffset.x, right - minSize.x));
                        rect.Size = rect.Size.X(right - rect.Position.x);
                    }
                    else if (dragType.HasFlag(DragType.ResizeRight))
                    {
                        rect.Size = rect.Size.X(globalPos.x - rect.Position.x + dragOffsetFar.x);
                    }

                    RectGlobalPosition = rect.Position;
                    RectSize = rect.Size;
                }
            }
            else dragType = DragType.None;
        }

        public override void _Input(InputEvent @event)
        {
            var focusOwner = GetFocusOwner();
            if (focusOwner != null && focusOwner != ConsoleLine) return;
            if (@event.PauseKeyIsJustPressed() && Visible)
            {
                Visible = false;
                AcceptEvent();
                return;
            }

            if (focusOwner != ConsoleLine && @event.IsActionPressed("console_toggle"))
            {
                Visible = !Visible;
                if (Visible) ConsoleLine.CallDeferred("grab_focus");
            }

            if (focusOwner == ConsoleLine && InputHandler.LeftMouseJustPressed &&
                !GetRect().Grow(4).HasPoint(MouseWatcher.MouseOriginLocal))
                ConsoleLine.ReleaseFocus();
        }

        public override void _GuiInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton mb)
            {
                if (mb.ButtonMask == MOUSE_BUTTON && mb.IsPressed())
                {
                    // Begin a possible dragging operation.
                    dragType = DragHitTest(mb.GlobalPosition);
                    if (dragType != DragType.None)
                        dragOffset = mb.GlobalPosition - RectGlobalPosition;
                    dragOffsetFar = GetGlobalRect().End - mb.GlobalPosition;
                }
            }

            if (@event is InputEventMouseMotion mm)
            {
                if (dragType == DragType.None)
                {
                    // Update the cursor while moving along the borders.
                    CursorShape cursor = CursorShape.Arrow;
                    if (Resizable)
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

        public override void _Notification(int what)
        {
            if (what == NotificationMouseExit)
            {
                if (Resizable && dragType == DragType.None && MouseDefaultCursorShape != CursorShape.Arrow)
                    MouseDefaultCursorShape = CursorShape.Arrow;
            }

            if (what == NotificationVisibilityChanged)
            {
                RectPosition = defaultRect.Position;
                RectSize = defaultRect.Size;
                if (Resizable && dragType == DragType.None && MouseDefaultCursorShape != CursorShape.Arrow)
                    MouseDefaultCursorShape = CursorShape.Arrow;
            }
        }

        #endregion

        public void Print(string str)
            => ConsoleText.BbcodeText += str;

        public void Print(params string[] str)
            => Print(ConsoleFormatHandler.Format(str));

        public void Print(params object[] obj)
            => Print(ConsoleFormatHandler.Format(obj));

        public void PrintLine(string str = "")
            => Print(str + "\n");

        public void PrintLine(params string[] str)
            => PrintLine(ConsoleFormatHandler.Format(str));

        public void PrintLine(params object[] obj)
            => PrintLine(ConsoleFormatHandler.Format(obj));

        public void Clear() => ConsoleText.BbcodeText = "";

        public void OnMetaClicked(object meta)
        {
            if (meta is string str)
            {
                if (Uri.TryCreate(str, UriKind.Absolute, out var uri))
                {
                    if (uri.IsFile) OnFileClicked(str);
                    else if (uri.IsLoopback) OnUrlClicked(str);
                }
                else OnTextLinkClicked(str);
            }
        }

        public virtual void OnFileClicked(string path)
        {
        }

        public virtual void OnUrlClicked(string path)
        {
        }

        public virtual void OnTextLinkClicked(string text)
        {
            var txt = ConsoleLine.Text;
            if (txt.Length > 0)
            {
                var lastCmd = TryFindNearestCommandNameFor(GetLastLine());
                if (lastCmd != "")
                    SetLineTextLast(lastCmd);
                if (txt.LastIndexOf(CommandSplit, StringComparison.Ordinal) != txt.Length - 1 - CommandSplit.Length)
                    ConsoleLine.Text += CommandSplit;
            }

            SetLineTextLast(text);
        }

        public void SetLineText(string text, bool grabFocus = true, bool caretToEnd = true)
        {
            ConsoleLine.Text = text;
            if (grabFocus) ConsoleLine.GrabFocus();
            if (caretToEnd) ConsoleLine.CaretPosition = text.Length;
        }

        public void SetLineTextLast(string text, bool grabFocus = true, bool caretToEnd = true)
        {
            var lastIndex = ConsoleLine.Text.LastIndexOf(CommandSplit, StringComparison.Ordinal);
            if (lastIndex > -1 && lastIndex < ConsoleLine.Text.Length - 1)
                SetLineText(ConsoleLine.Text.Remove(lastIndex + 1) + text, grabFocus, caretToEnd);
            else if (lastIndex > -1) SetLineText(ConsoleLine.Text + text, grabFocus, caretToEnd);
            else SetLineText(text, grabFocus, caretToEnd);
        }

        public void AddHistory(string input)
        {
            if (history.Count == MaxHistorySize)
                history.Dequeue();
            history.Enqueue(input);
        }

        public string GetHistory(int index)
        {
            if (index < 0 || index > history.Count) return "";
            if (index == history.Count - 1) return history.Peek();
            return history.ElementAt(history.Count - 1 - index);
        }

        public void OnConsoleLineTextEntered(string text)
        {
            currentHistoryIndex = -1;
            if (!string.IsNullOrWhiteSpace(text))
            {
                AddHistory(text);
                ConsoleLine.Clear();
                Execute(text);
            }
        }

        public void Execute(string input)
        {
            PrintLine($"[color=#{CommandColor.ToHtml()}]{ExecutionLineIndicator}[/color]{input}");
            foreach (var argList in CommandHandler.GenerateCommandList(input))
                if (CommandHandler.ExecuteCommand(this, argList) == null)
                    PrintLine("Command Unknown.");
        }

        public void OnCommandExecute(CommandArguments cmdArgs)
        {
            OnExecute?.Invoke(cmdArgs);
        }

        private int currentHistoryIndex = -1;

        public void OnConsoleLineGuiInput(InputEvent e)
        {
            var prevHistoryIndex = currentHistoryIndex;
            if (e.IsActionPressed("ui_up"))
                currentHistoryIndex = Mathf.Min(++currentHistoryIndex, history.Count - 1);
            if (e.IsActionPressed("ui_down"))
                currentHistoryIndex = Mathf.Max(--currentHistoryIndex, -1);

            if (currentHistoryIndex != prevHistoryIndex)
                SetLineText(GetHistory(currentHistoryIndex), false);

            if (e.IsActionPressed("ui_focus_next"))
            {
                SetLineTextLast(TryFindNearestCommandNameFor(GetLastLine()));
            }
        }

        public string GetLastLine()
            => ConsoleLine.Text.Split(new[] {CommandSplit}, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();

        public string TryFindNearestCommandNameFor(string input)
        {
            if (input == null) input = "";
            var cmd = CommandHandler.GetAllCommands().FirstOrDefault(pair =>
                pair.Name.StartsWith(input, StringComparison.CurrentCultureIgnoreCase));
            if (cmd.Name != null)
                return cmd.Name;
            return "";
        }

        public T CastTo<T>()
            where T : class
            => this as T;
    }
}