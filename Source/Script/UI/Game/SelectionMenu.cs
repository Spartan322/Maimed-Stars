using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MSG.Game.Unit;
using MSG.Game.Unit.Control.Group;
using MSG.Game.Unit.Control.Select;
using MSG.Global;
using MSG.Script.Agent;
using MSG.Script.UI.Base;
using SpartansLib.Attributes;
using SpartansLib.Extensions;

namespace MSG.Script.UI.Game
{
	[Global]
	public class SelectionMenu : VBoxContainer, IReadOnlyList<IUnit>
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

		public delegate void SelectedGroupChangeAction(SelectionMenu menu, IGroup<IUnit> group);
		public event SelectedGroupChangeAction OnSelectedGroupChange;

		#endregion

		#region Public Fields

		public ISelectionList SelectionList { get; } = new SelectionList();

		private IGroup _selectedGroup;

		public IGroup SelectedGroup
		{
			get => _selectedGroup;
			set
			{
				if (_selectedGroup == value) return;
				//_selectedGroup?.SetTopSelection(this, false);
				//value?.SetTopSelection(this, true);
				_OnSelectedGroupChanged(value);
				_selectedGroup = value;
				if (_selectedGroup == null) return;
				_selectedGroup.OnNameChange += _OnSelectedGroupNameChange;
				_OnSelectedGroupNameChange(_selectedGroup, _selectedGroup.Name);

				//else if (Count > 1)
				//{
				//	PlaceholderNameText = "Group Name";
				//	AcceptButton.Text = "+";
				//}
				//else
				//PlaceholderNameText = this[0].Name;
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

		public IUnit this[int index] => SelectionList[index];
		public int Count => SelectedItemList.GetItemCount();

		#endregion

		//private bool selectionDirty;
		public override void _Ready()
		{
			//InterfaceData.Set(this);
			TopWindowDecoration.OnButtonPressed += (sender, args) =>
			{
				if (args.ButtonType.IsExit()) OnQuitButtonPressed();
			};
			//SelectionHandler.ObjectSelected += (handler, selected, index, deselectAll) =>
			//{
			//	if (deselectAll) TopLevelSelection = null;
			//	Active = true;
			//	selectionDirty = true;
			//};
			//SelectionHandler.ObjectDeselected += (handler, selected, index)
			//	=> selectionDirty = true;
			//SelectionHandler.SelectionEmptied += h =>
			//{
			//	TopLevelSelection = null;
			//	Active = false;
			//};
		}

		public override void _Notification(int what)
		{
			switch (what)
			{
				case NotificationVisibilityChanged: // Optimize when not visible
					if (!Visible) ReleaseFocus();
					SetProcess(Visible);
					if (SelectedGroup == null && !Visible && SelectionList.Count > 0)
						SelectionList.Clear();
					break;
			}
		}

		public void Add(IUnit unit, bool clear = false)
		{
			SelectionList.Add(unit, clear);
			_AddUnitItem(unit, Count);
			_UpdateSubtitle();
		}

		public void AddRange(IEnumerable<IUnit> units, bool clear = false)
		{
			var count = Count;
			var enumerable = units as IList<IUnit> ?? units.ToArray();
			SelectionList.AddRange(enumerable, clear);
			foreach (var unit in enumerable)
				_AddUnitItem(unit, count++);
			_UpdateSubtitle();
		}

		public void Remove(IUnit unit)
		{
			var index = SelectionList.IndexOf(unit);
			if (index == -1) return;
			_RemoveUnitItem(index);
			SelectionList.Remove(unit);
		}

		public void Clear()
		{
			SelectionList.Clear();
			SelectedGroup = null;
			_ResetItemList();
		}

		public static IGroup CreateGroup(string name)
		{
			var unit = SelectableGroup.Scene.Instance<SelectableGroup>().Unit;
			unit.Name = name;
			return unit;
		}

		private void _AddUnitItem(IUnit unit, int index)
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
		}

		private void _UpdateGroupSelection()
		{
			SelectedGroup.Clear();
			SelectedGroup.AddRange(SelectionList);
		}

		private void _TryUpdateGroupName()
		{
			if (string.IsNullOrWhiteSpace(TypedText)) return;
			SelectedGroup.Name = TypedText;
		}

		private void _TryCreateGroup()
		{
			if (string.IsNullOrWhiteSpace(TypedText)) return;
			SelectedGroup = CreateGroup(TypedText);
		}

		private void _TryUpdateUnitName()
		{
			if (string.IsNullOrWhiteSpace(TypedText)) return;
			this[0].Name = TypedText;
		}

		private void _UpdateSelection()
		{
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

		public IList<IUnit> GetSelectedUnits()
		{
			var indices = GetSelectedIndices();
			var result = new IUnit[indices.Count];
			for (var i = 0; i < indices.Count; i++)
				result[i] = this[indices[i]];
			return result;
		}

		//public float ItemListScrollBarHeight
		//{
		//	get
		//	{
		//		var vsep = SelectedItemList.GetConstant("vseperation");
		//		var lsep = SelectedItemList.GetConstant("line_seperation");
		//		//SelectedItemList.GetStylebox("bg").GetMinimumSize().y;
		//		return (SelectedItemList.GetFont("font").GetHeight() + vsep + lsep + 5.8f) * (MinForScrollBar);
		//	}
		//}

		public override void _Process(float delta)
		{
			//HandleSelectedList();
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

		public void OnSelectedListItemActivated(int index)
		{
			Clear();
			AddRange(GetSelectedUnits());
			ignoreMouseInput |= InputHandler.MouseActionPressed;
		}

		public void OnSelectedListRmbSelected(int index, Vector2 click)
			=> Remove(this[index]);

		public void OnSelectedListGuiInput(InputEvent @event)
		{
			/*if (Input.IsActionJustPressed("ui_accept"))
				OnSelectedListItemActivated(-1);*/
		}

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

		public void OnDestroyButtonPressed()
		{
			SelectedGroup = null;
			_ResetItemList();
		}

		public void OnNameTextEntered(string newText) => OnAcceptButtonUp();

		public void OnQuitButtonPressed() => Visible = false;

		public void SelectionPanelMouseEnter()
		{
		} //MouseWatcher.HasMouseEvent = this;

		public void SelectionPanelMouseExit()
		{
			//if (MouseWatcher.HasMouseEvent == this)
			//MouseWatcher.HasMouseEvent = null;
		}

		private static bool ignoreMouseInput;

		public static void HandleTopLevelInput(GlobalScript global, InputEvent @event)
		{
			if (!@event.IsMouseAction() || !ignoreMouseInput) return;
			ignoreMouseInput = false;
			global.GetTree().SetInputAsHandled();
		}

		public IEnumerator<IUnit> GetEnumerator()
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
			//if (!selectionDirty || !Visible) return;
			//selectionDirty = false;
			SelectedItemList.Clear();
			SelectionList.Sort();
			switch (SelectionList.Count)
			{
				case 0:
					Visible = false;
					return;
				case 1:
					if (!InputHandler.AddControlKeyPressed && this[0] is IGroup group) // TODO: not agent
						SelectedGroup = group;
					else /* if menu deselected to one unit */
						SelectedGroup = null;
					break;
			}

			// AddUnit(this[0], $"*🛡  {this[0].Name}");
			// foreach (var unit in this.Skip(1))
				// AddUnit(unit);
		}

		private void _OnSelectedGroupChanged(IGroup group)
		{
			if (_selectedGroup != null)
				_selectedGroup.OnNameChange -= _OnSelectedGroupNameChange;
			OnSelectedGroupChange?.Invoke(this, group);
			DeleteButton.Visible = group != null;
			_ResetItemList();
			AddRange(group);
		}

		private void _OnSelectedGroupNameChange(IUnit unit, string name)
			=> PlaceholderNameText = name;
	}
}
