using System.Collections.Generic;
using System.Linq;
using Godot;
using MSG.Script.Unit;
using SpartansLib.Extensions;

namespace MSG.Script.UI.Game
{
    public partial class SelectionMenu
    {
        public static SelectableGroup CreateGroup(string name)
        {
            var unit = SelectableGroup.Scene.Instance<SelectableGroup>();
            unit.UnitName = name;
            return unit;
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
                foreach (var unit in SelectList)
                    _AddUnitItem(unit, count++);
            _UpdateSubtitle();
            _UpdateSelection();
        }

        private void _TryCreateGroup()
        {
            if (string.IsNullOrWhiteSpace(TypedText))
                return; // TODO: game error, group must be named
            var group = CreateGroup(TypedText);
            this[0].Manager.RegisterUnit(group);
            group.AddRange(this);
            this[0].AddChild(group);
            FocusUnit = group;
        }

        private void _TryUpdateUnitName()
        {
            var group = FocusUnit as SelectableGroup;
            if (!string.IsNullOrWhiteSpace(TypedText))
                FocusUnit.UnitName = TypedText;
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
                Visible = false;
                return;
            }
            Visible = true;
            SelectedPanel.Visible = Count > 1;
        }
    }
}