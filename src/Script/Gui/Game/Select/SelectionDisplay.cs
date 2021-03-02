using System.Collections.Generic;
using Godot;
using MSG.Engine;
using MSG.Game.Rts.Unit;
using MSG.Global;
using MSG.Script.Gui.Window;
using SpartansLib.Attributes;
using static MSG.Script.Gui.Window.BaseWindow;

namespace MSG.Script.Gui.Game.Select
{
    [Global]
    public partial class SelectionDisplay : VBoxContainer, IReadOnlyList<BaseUnit>
    {
        #region Nodes
        [Node("TitleHList/Name")]
        public LineEdit NameLineEdit;

        [Node("TitleHList/AcceptButton")]
        public Button AcceptButton;

        [Node("SubtitlePanel/Subtitle")]
        public RichTextLabel SubtitleNode;

        [Node("SelectedPanel/SelectMargin/SelectedList")]
        public ItemList SelectedItemList;

        [Node("SelectedPanel")]
        public Control SelectedPanel;

        [Node("TitleHList/DeleteButton")]
        public Button DeleteButton;
        #endregion

        #region Exports
        [Export] public int MinForScrollBar = 5;
        #endregion

        #region Events
        public delegate void SelectedUnitChangeAction(SelectionDisplay _display, BaseUnit unit);
        public event SelectedUnitChangeAction OnSelectedUnitChange;
        #endregion

        #region Public Fields
        public MainSelectList SelectList { get; } = new MainSelectList();

        public BaseUnit FocusUnit
        {
            get => SelectList.FocusUnit;
            set => SelectList.FocusUnit = value;
        }

        public string TypedText
        {
            get => NameLineEdit.Text;
            set => NameLineEdit.Text = value.Trim();
        }

        public string PlaceholderNameText
        {
            get => NameLineEdit.PlaceholderText;
            set => NameLineEdit.PlaceholderText = value.Trim();
        }

        public int SelectedCount => GetSelectedIndices().Count;
        #endregion

        internal BaseWindow Parent;
        public override void _Ready()
        {
            Parent = GetParent<BaseWindow>();
            SelectList.OnSelectListChanged += _OnSelectListChanged;
            SelectList.OnSelectListPostChanged += _OnSelectListPostChanged;
            SelectList.OnFocusUnitChanged += _OnFocusUnitChanged;
            _OnFocusUnitChanged(SelectList, FocusUnit);
            SelectList.OnFocusUnitNameChange += _OnFocusUnitNameChange;
            Parent.OnButtonPressed += (Window, button) =>
            {
                if (button.GetIndex() == (int)WindowButton.Close) OnQuitButtonPressed();
            };
        }

        public override void _Notification(int what)
        {
            switch (what)
            {
                case NotificationVisibilityChanged: // Optimize when not visible
                    if (!Parent.Visible)
                    {
                        ReleaseFocus();
                        if (FocusUnit == null && Count > 0)
                            SelectList.Clear();
                        else Clear();
                    }
                    break;
            }
        }

        public override void _Input(InputEvent e)
        {
            Parent.Visible &= !e.PauseKeyIsJustPressed();

            if (e.UiNextFocusKeyIsJustPressed()
                && Parent.Visible
                && !NameLineEdit.HasFocus())
                NameLineEdit.GrabFocus();

            if (FocusUnit != null && e.SelectionDeleteKeyIsJustPressed())
                OnDestroyButtonPressed();
        }

        [Connect("item_activated", "SelectedPanel/SelectMargin/SelectedList")]
        public void OnSelectedListItemActivated(int index)
        {
            (IList<BaseUnit> list, BaseUnit unit) temp = (null, null);
            if (index > -1) temp.unit = this[index];
            else temp.list = GetSelectedUnits();
            Clear(true);
            SelectedItemList.Clear();
            if (temp.unit != null) Add(temp.unit);
            else AddRange(temp.list);
            ignoreMouseInput |= InputManager.MouseActionPressed;
        }

        [Connect("item_rmb_selected", "SelectedPanel/SelectMargin/SelectedList")]
        public void OnSelectedListRmbSelected(int index, Vector2 click)
            => Remove(this[index]);

        [Connect("button_up", "TitleHList/AcceptButton")]
        public void OnAcceptButtonUp()
        {
            switch (Count)
            {
                case 0: break;
                case 1:
                    _TryUpdateUnitName();
                    break;
                default:
                    if (FocusUnit == null) _TryCreateGroup();
                    else _TryUpdateUnitName();
                    break;
            }

            TypedText = string.Empty;
            NameLineEdit.ReleaseFocus();
        }

        [Connect("pressed", "TitleHList/DeleteButton")]
        public void OnDestroyButtonPressed()
        {
            var delete = FocusUnit;
            FocusUnit = null;
            delete.Manager.DeregisterUnit(delete);
            delete.GetParent().RemoveChild(delete);
            delete.QueueFree();
        }

        [Connect("text_entered", "TitleHList/Name")]
        public void OnNameTextEntered(string newText) => OnAcceptButtonUp();

        public void OnQuitButtonPressed() => Parent.Visible = false;

        private static bool ignoreMouseInput;
        public static void HandleTopLevelInput(GlobalScript global, InputEvent @event)
        {
            if (!@event.IsMouseAction() || !ignoreMouseInput) return;
            ignoreMouseInput = false;
            global.GetTree().SetInputAsHandled();
        }

        static SelectionDisplay()
        {
            GlobalScript.OnInput += HandleTopLevelInput;
        }

        // 🛡

        private void _OnFocusUnitChanged(MainSelectList list, BaseUnit unit)
        {
            DeleteButton.Visible = unit != null;
            if (unit != null)
            {
                _OnFocusUnitNameChange(unit, unit.UnitName);
                AcceptButton.Text = "A";//"✓";
            }
            else
            {
                PlaceholderNameText = "Group Name";
                AcceptButton.Text = "+";
            }
        }

        private void _OnFocusUnitNameChange(BaseUnit unit, string name)
        {
            PlaceholderNameText = name;
        }
    }
}
