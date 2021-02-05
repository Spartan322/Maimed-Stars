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
    public class SelectionMenu : VBoxContainer, IReadOnlyList<GameUnit>
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

        [Node("SelectionPanel/VBoxContainer/TopMargin/VList/ControlButtonList/DeleteButton")]
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
        public SelectionMenuList SelectionList { get; } = new SelectionMenuList();

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

        public GameUnit this[int index] => SelectionList[index];
        public int Count => SelectionList.Count;
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
                            SelectionList.ClearInternal();
                        else Clear();
                    }
                    break;
            }
        }

        public void Add(GameUnit unit)
        {
            if (!SelectionList.AddInternal(unit)) return;
            if (Count == 1)
            {
                SelectedUnit = unit;
                if (SelectedUnit is IEnumerable<GameUnit> group)
                {
                    SelectionList.RemoveInternal(unit);
                    unit.SelectUpdate(SelectionList);
                    _ResetItemList();
                    return;
                }
            }
            else if (SelectedUnit != null && SelectionList.Contains(SelectedUnit))
                SelectedUnit = null;
            _AddUnitItem(unit, Count);
            _UpdateSubtitle();
            _UpdateSelection();
        }

        public void AddRange(IEnumerable<GameUnit> units)
        {
            if (Count > 0 && SelectedUnit == this[0])
                SelectedUnit = null;
            var count = Count + 1;
            var enumerable = units as ICollection<GameUnit> ?? units.ToArray();
            var diff = SelectionList.AddRangeInternal(enumerable);
            if (Count == 1)
                SelectedUnit = this[0];
            foreach (var unit in diff)
                _AddUnitItem(unit, count++);
            _UpdateSubtitle();
            _UpdateSelection();

        }

        public void Remove(GameUnit unit)
        {
            var index = SelectionList.IndexOf(unit);
            if (index == -1) return;
            _RemoveUnitItem(index);
            SelectionList.RemoveInternal(unit);
            _UpdateSubtitle();
            _UpdateSelection();
        }

        public void Clear()
        {
            SelectionList.ClearInternal();
            SelectedUnit = null;
            _ResetItemList();
        }

        public static SelectableGroup CreateGroup(string name)
        {
            var unit = SelectableGroup.Scene.Instance<SelectableGroup>();
            unit.UnitName = name;
            return unit;
        }

        private void _AddUnitItem(GameUnit unit, int displayIndex)
        {
            SelectedItemList.AddItem($"{(displayIndex == 1 ? "*" : "")}{displayIndex}. {unit.UnitName}");
        }

        private void _RemoveUnitItem(int index)
            => SelectedItemList.RemoveItem(index);

        private void _UpdateSubtitle()
            => SubtitleNode.Text = $"Selection: {Count}";

        private void _ResetItemList(bool noAdd = false)
        {
            SelectedItemList.Clear();
            var count = 1;
            if (!noAdd)
                foreach (var unit in SelectionList)
                    _AddUnitItem(unit, count++);
            _UpdateSubtitle();
            _UpdateSelection();
        }

        private void _TryCreateGroup()
        {
            if (string.IsNullOrWhiteSpace(TypedText))
                return; // TODO: game error, group must be named
            var group = CreateGroup(TypedText);
            group.AddRange(this);
            this[0].Manager.RegisterUnit(group);
            SelectedUnit = group;
            this[0].AddChild(SelectedUnit);
        }

        private void _TryUpdateUnitName()
        {
            var group = SelectedUnit as SelectableGroup;
            if (!string.IsNullOrWhiteSpace(TypedText))
                SelectedUnit.UnitName = TypedText;
            else if (group == null)
                return; // TODO: game error, unit must be named)
            if (group != null)
            {
                group.Clear();
                group.AddRange(this);
            }
        }

        private void _UpdateSelection()
        {
            if (Count == 0)
            {
                if (Visible) Visible = false;
                return;
            }
            if (!Visible) Visible = true;
            SelectedPanel.Visible = Count > 1;
            AcceptButton.Text = "✓";
            if (SelectedUnit == null && Count > 1)
            {
                PlaceholderNameText = "Group Name";
                AcceptButton.Text = "+";
            }
        }

        public IList<int> GetSelectedIndices() => SelectedItemList.GetSelectedItems();

        public IList<GameUnit> GetSelectedUnits()
        {
            var indices = GetSelectedIndices();
            var result = new GameUnit[indices.Count];
            for (var i = 0; i < indices.Count; i++)
                result[i] = this[indices[i]];
            return result;
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
            Clear();
            AddRange(GetSelectedUnits());
            ignoreMouseInput |= InputHandler.MouseActionPressed;
        }

        [Connect("item_rmb_selected", "SelectionPanel/VBoxContainer/TopMargin/VList/SelectedPanel/SelectMargin/SelectedList")]
        public void OnSelectedListRmbSelected(int index, Vector2 click)
            => Remove(this[index]);

        public void OnSelectedListGuiInput(InputEvent @event)
        {
            /*if (Input.IsActionJustPressed("ui_accept"))
                OnSelectedListItemActivated(-1);*/
        }

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

        [Connect("pressed", "SelectionPanel/VBoxContainer/TopMargin/VList/ControlButtonList/DeleteButton")]
        public void OnDestroyButtonPressed()
        {
            SelectedUnit = null;
            _ResetItemList();
        }

        [Connect("text_entered", "SelectionPanel/VBoxContainer/TopMargin/VList/TitleHList/Name")]
        public void OnNameTextEntered(string newText) => OnAcceptButtonUp();

        public void OnQuitButtonPressed() => Visible = false;

        public void SelectionPanelMouseEnter()
        {
        }

        public void SelectionPanelMouseExit()
        {
        }

        private static bool ignoreMouseInput;

        public static void HandleTopLevelInput(GlobalScript global, InputEvent @event)
        {
            if (!@event.IsMouseAction() || !ignoreMouseInput) return;
            ignoreMouseInput = false;
            global.GetTree().SetInputAsHandled();
        }

        public IEnumerator<GameUnit> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        static SelectionMenu()
        {
            GlobalScript.OnInput += HandleTopLevelInput;
        }

        private void HandleSelectedList()
        {
            SelectedItemList.Clear();
            switch (SelectionList.Count)
            {
                case 0:
                    Visible = false;
                    return;
                case 1:
                    if (!InputHandler.AddControlKeyPressed && this[0] is SelectableGroup group) // TODO: not agent
                        SelectedUnit = group;
                    else /* if menu deselected to one unit */
                        SelectedUnit = null;
                    break;
            }
            // 🛡
        }

        private void _OnSelectedUnitNameChange(GameUnit unit, string name)
            => PlaceholderNameText = name;
    }
}
