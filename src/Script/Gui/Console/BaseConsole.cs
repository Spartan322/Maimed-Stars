using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MSG.Engine;
using MSG.Engine.Command;
using MSG.Game.Gui;
using SpartansLib.Attributes;
using SpartansLib.Structure;

namespace MSG.Script.Gui.Console
{
    // TODO: enable properly drawing indentation on wrapping
    [Global]
    public class BaseConsole : MarginContainer, ICommandInterface
    {
        public event Action<CommandArguments> OnExecute;

        [Export] public string ExecutionLineIndicator { get; private set; } = "$";

        [Export]                                                           // rr gg bb
        public Color CommandColor { get; private set; } = ColorExt.FromRGB8(0xff_ff_66);

        [Export] public string CommandSplit { get; private set; } = ";";

        [Export] public int MaxHistorySize { get; private set; } = 10;

        [Node("ConsoleContainer/VBoxContainer/ConsoleTextPanel/ConsoleText")]
        public RichTextLabel ConsoleText;

        [Node("ConsoleContainer/VBoxContainer/ConsoleLine")]
        public LineEdit ConsoleLine;

        [Node("ConsoleContainer")]
        public VBoxContainer ConsoleContainer;

        private Control _parent;
        private Rect2 defaultRect;
        private readonly Queue<string> history = new Queue<string>();

        public override void _Ready()
        {
            _parent = GetParent<Control>();
            defaultRect = _parent.GetGlobalRect();

            var v = Godot.Engine.GetVersionInfo();
            PrintLine(ProjectSettings.GetSetting("application/config/name")
                      + $" (Godot {v["major"]}.{v["minor"]}.{v["patch"]} {v["status"]})\n"
                      + $"Type {CommandHandler.GetCommand("help").FormatName()} to get more information about usage");
        }

        #region Console Implementation
        public override void _Input(InputEvent @event)
        {
            var focusOwner = GetFocusOwner();
            if (focusOwner != null && focusOwner != ConsoleLine) return;
            if (@event.PauseKeyIsJustPressed() && _parent.Visible)
            {
                _parent.Visible = false;
                AcceptEvent();
                return;
            }

            if (@event.IsActionPressed("console_toggle"))
            {
                _parent.Visible = !_parent.Visible;
                if (_parent.Visible) ConsoleLine.CallDeferred("grab_focus");
            }

            if (focusOwner == ConsoleLine && InputManager.LeftMouseJustPressed &&
                !GetRect().Grow(4).HasPoint(GetViewport().GetMousePosition()))
                ConsoleLine.ReleaseFocus();
        }

        public override void _Notification(int what)
        {
            if (what == NotificationVisibilityChanged)
            {
                _parent.RectGlobalPosition = defaultRect.Position;
                _parent.RectSize = defaultRect.Size;
            }
        }
        #endregion

        public void Print(string str)
            => ConsoleText.AppendBbcode(str);

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

        [Connect("meta_clicked", "ConsoleContainer/VBoxContainer/ConsoleTextPanel/ConsoleText")]
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

        [Connect("text_entered", "ConsoleContainer/VBoxContainer/ConsoleLine")]
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

        [Connect("gui_input", "ConsoleContainer/VBoxContainer/ConsoleLine")]
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
            => ConsoleLine.Text.Split(new[] { CommandSplit }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();

        public string TryFindNearestCommandNameFor(string input)
        {
            if (input == null) input = "";
            var cmd = CommandHandler.GetAllCommands().FirstOrDefault(pair =>
                pair.Name.StartsWith(input, StringComparison.CurrentCultureIgnoreCase));
            if (cmd?.Name != null)
                return cmd.Name;
            return "";
        }

        public T CastTo<T>()
            where T : class
            => this as T;
    }
}
