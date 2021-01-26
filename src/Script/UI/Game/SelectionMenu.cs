using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MSG.Game.Unit;
using MSG.Global;
using MSG.Script.Agent;
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
		public delegate void SelectedGroupChangeAction(SelectionMenu menu, SelectableGroup group);
		public event SelectedGroupChangeAction OnSelectedGroupChange;
		#endregion

		#region Public Fields
		public SelectionMenuList SelectionList { get; } = new SelectionMenuList();

		private SelectableGroup _selectedGroup;

		public SelectableGroup SelectedGroup
		{
			get => _selectedGroup;
			set
			{
				if (_selectedGroup == value) return;
				_OnSelectedGroupChanged(value);
				_selectedGroup = value;
				if (_selectedGroup == null) return;
				_selectedGroup.OnNameChange += _OnSelectedGroupNameChange;
				_OnSelectedGroupNameChange(_selectedGroup, _selectedGroup.Name);
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
					if (!Visible) ReleaseFocus();
					if (SelectedGroup == null && !Visible && SelectionList.Count > 0)
						SelectionList.ClearInternal();
					break;
			}
		}

		public void Add(GameUnit unit)
		{
			SelectionList.AddInternal(unit);
			_AddUnitItem(unit, Count);
			_UpdateSubtitle();
			_UpdateSelection();
		}

		public void AddRange(IEnumerable<GameUnit> units)
		{
			var count = Count;
			var enumerable = units as ICollection<GameUnit> ?? units.ToArray();
			SelectionList.AddRangeInternal(enumerable);
			foreach (var unit in enumerable)
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
			SelectedGroup = null;
			_ResetItemList();
		}

		public static SelectableGroup CreateGroup(string name)
		{
			var unit = SelectableGroup.Scene.Instance<SelectableGroup>();
			unit.Name = name;
			return unit;
		}

		private void _AddUnitItem(GameUnit unit, int index)
			=> SelectedItemList.AddItem($"{(index == 0 ? "*" : "")}{index}. {unit.Name}");

		private void _RemoveUnitItem(int index)
			=> SelectedItemList.RemoveItem(index);

		private void _UpdateSubtitle()
			=> SubtitleNode.Text = $"Selection: {Count}";

		private void _ResetItemList()
		{
			SelectedItemList.Clear();
			var count = 0;
			foreach (var unit in SelectionList)
				_AddUnitItem(unit, count++);
			_UpdateSubtitle();
			_UpdateSelection();
		}

		private void _TryUpdateGroupName()
		{
			if (string.IsNullOrWhiteSpace(TypedText)) return;
			SelectedGroup.Name = TypedText;
		}

		private void _TryCreateGroup()
		{
			if (string.IsNullOrWhiteSpace(TypedText)) return; // TODO: game error, group must be named
			SelectedGroup = CreateGroup(TypedText);
		}

		private void _TryUpdateUnitName()
		{
			if (string.IsNullOrWhiteSpace(TypedText)) return; // TODO: game error, unit must be named
			this[0].UnitName = TypedText;
		}

		private void _UpdateSelection()
		{
			if (Count == 0)
			{
				if(Visible) Visible = false;
				return;
			}
			if(!Visible) Visible = true;
			SelectedPanel.Visible = Count > 1;
			AcceptButton.Text = "✓";
			if (SelectedGroup == null && Count > 1)
			{
				PlaceholderNameText = "Group Name";
				AcceptButton.Text = "+";
			}
			else
				PlaceholderNameText = this[0].Name;
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

			if (SelectedGroup != null && e.SelectionDeleteKeyIsJustPressed())
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
					if (SelectedGroup == null)
						_TryCreateGroup();
					else _TryUpdateGroupName();
					break;
			}

			TypedText = string.Empty;
			NameLineEdit.ReleaseFocus();
		}

		[Connect("pressed", "SelectionPanel/VBoxContainer/TopMargin/VList/ControlButtonList/DeleteButton")]
		public void OnDestroyButtonPressed()
		{
			SelectedGroup = null;
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
						SelectedGroup = group;
					else /* if menu deselected to one unit */
						SelectedGroup = null;
					break;
			}
			// 🛡
		}

		private void _OnSelectedGroupChanged(SelectableGroup group)
		{
			if (_selectedGroup != null)
				_selectedGroup.OnNameChange -= _OnSelectedGroupNameChange;
			OnSelectedGroupChange?.Invoke(this, group);
			DeleteButton.Visible = group != null;
			_ResetItemList();
			AddRange(group);
		}

		private void _OnSelectedGroupNameChange(GameUnit unit, string name)
			=> PlaceholderNameText = name;
	}
}