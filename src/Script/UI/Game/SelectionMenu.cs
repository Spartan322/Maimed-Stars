using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MSG.Game.Unit;
using MSG.Global;
using MSG.Script.Unit;
using MSG.Script.UI.Base;
using SpartansLib.Attributes;
using SpartansLib.Extensions;

namespace MSG.Script.UI.Game
{
    [Global]
    public partial class SelectionMenu : VBoxContainer, IReadOnlyList<GameUnit>
    {
        #region Nodes
        [Node("SelectionPanel/VBoxContainer/TopMargin/VList/TitleHList/Name")]
        public LineEdit NameLineEdit;

        [Node("SelectionPanel/VBoxContainer/TopMargin/VList/TitleHList/AcceptButton")]
        public Button AcceptButton;

        [Node("SelectionPanel/VBoxContainer/TopMargin/VList/SubtitlePanel/Subtitle")]
        public RichTextLabel SubtitleNode;

        [Node("SelectionPanel/VBoxContainer/TopMargin/VList/SelectedPanel/SelectMargin/SelectedList")]
        public ItemList SelectedItemList;

        [Node("SelectionPanel/VBoxContainer/TopMargin/VList/SelectedPanel")]
        public Control SelectedPanel;

        [Node("SelectionPanel/VBoxContainer/TopMargin/VList/TitleHList/DeleteButton")]
        public Button DeleteButton;

        [Node("SelectionPanel/VBoxContainer/TopWindowDecoration")]
        public TopWindowDecoration TopWindowDecoration;
        #endregion

        #region Exports
        [Export] public int MinForScrollBar = 5;
        #endregion

        #region Events
        public delegate void SelectedUnitChangeAction(SelectionMenu menu, GameUnit unit);
        public event SelectedUnitChangeAction OnSelectedUnitChange;
        #endregion

        #region Public Fields
        public MainSelectList SelectList { get; } = new MainSelectList();

        private GameUnit _selectedUnit;

        public GameUnit SelectedUnit
        {
            get => _selectedUnit;
            set
            {
                if (Object.ReferenceEquals(SelectedUnit, value)) return;
                if (SelectedUnit != null)
                    SelectedUnit.OnNameChange -= _OnSelectedUnitNameChange;
                OnSelectedUnitChange?.Invoke(this, value);
                DeleteButton.Visible = value != null;
                if (value is IEnumerable<GameUnit> range)
                    AddRange(range);
                if (value != null)
                    PlaceholderNameText = value.UnitName;
                _selectedUnit = value;
                if (SelectedUnit == null) return;
                SelectedUnit.OnNameChange += _OnSelectedUnitNameChange;
            }
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

        public override void _Ready()
        {
            TopWindowDecoration.OnButtonPressed += (sender, args) =>
            {
                if (args.ButtonType.IsExit()) OnQuitButtonPressed();
            };
        }

        public override void _Notification(int what)
        {
            switch (what)
            {
                case NotificationVisibilityChanged: // Optimize when not visible
                    if (!Visible)
                    {
                        ReleaseFocus();
                        if (SelectedUnit == null && Count > 0)
                            SelectList.ClearInternal();
                        else Clear();
                    }
                    break;
            }
        }

        public override void _Input(InputEvent e)
        {
            Visible &= !e.PauseKeyIsJustPressed();

            if (e.UiNextFocusKeyIsJustPressed()
                && Visible
                && !NameLineEdit.HasFocus())
                NameLineEdit.GrabFocus();

            if (SelectedUnit != null && e.SelectionDeleteKeyIsJustPressed())
                OnDestroyButtonPressed();
        }

        [Connect("item_activated", "SelectionPanel/VBoxContainer/TopMargin/VList/SelectedPanel/SelectMargin/SelectedList")]
        public void OnSelectedListItemActivated(int index)
        {
            (IList<GameUnit> list, GameUnit unit) temp = (null, null);
            if (index > -1) temp.unit = this[index];
            else temp.list = GetSelectedUnits();
            Clear(true);
            SelectedItemList.Clear();
            if (temp.unit != null) Add(temp.unit);
            else AddRange(temp.list);
            ignoreMouseInput |= InputHandler.MouseActionPressed;
        }

        [Connect("item_rmb_selected", "SelectionPanel/VBoxContainer/TopMargin/VList/SelectedPanel/SelectMargin/SelectedList")]
        public void OnSelectedListRmbSelected(int index, Vector2 click)
            => Remove(this[index]);

        [Connect("button_up", "SelectionPanel/VBoxContainer/TopMargin/VList/TitleHList/AcceptButton")]
        public void OnAcceptButtonUp()
        {
            switch (Count)
            {
                case 0: break;
                case 1:
                    _TryUpdateUnitName();
                    break;
                default:
                    if (SelectedUnit == null) _TryCreateGroup();
                    else _TryUpdateUnitName();
                    break;
            }

            TypedText = string.Empty;
            NameLineEdit.ReleaseFocus();
        }

        [Connect("pressed", "SelectionPanel/VBoxContainer/TopMargin/VList/TitleHList/DeleteButton")]
        public void OnDestroyButtonPressed()
        {
            var delete = SelectedUnit;
            delete.Manager.DeregisterUnit(delete);
            delete.GetParent().RemoveChild(delete);
            delete.QueueFree();
        }

        [Connect("text_entered", "SelectionPanel/VBoxContainer/TopMargin/VList/TitleHList/Name")]
        public void OnNameTextEntered(string newText) => OnAcceptButtonUp();

        public void OnQuitButtonPressed() => Visible = false;

        private static bool ignoreMouseInput;
        public static void HandleTopLevelInput(GlobalScript global, InputEvent @event)
        {
            if (!@event.IsMouseAction() || !ignoreMouseInput) return;
            ignoreMouseInput = false;
            global.GetTree().SetInputAsHandled();
        }

        static SelectionMenu()
        {
            GlobalScript.OnInput += HandleTopLevelInput;
        }

        // 🛡

        private void _OnSelectedUnitNameChange(GameUnit unit, string name)
            => PlaceholderNameText = name;
    }
}
