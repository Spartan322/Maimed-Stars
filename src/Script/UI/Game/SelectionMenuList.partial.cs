using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MSG.Script.Unit;

namespace MSG.Script.UI.Game
{
    public partial class SelectionMenu
    {
        public GameUnit this[int index] => SelectList[index];
        public int Count => SelectList.Count;

        public void Add(GameUnit unit)
        {
            if (!SelectList.AddInternal(unit)) return;
            if (Count == 1)
            {
                SelectedUnit = unit;
                if (SelectedUnit is IEnumerable<GameUnit> group)
                {
                    SelectList.RemoveInternal(unit);
                    unit.SelectUpdate(SelectList);
                    _ResetItemList();
                    return;
                }
            }
            else if (SelectedUnit != null && SelectList.Contains(SelectedUnit))
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
            var diff = SelectList.AddRangeInternal(enumerable);
            if (Count == 1)
                SelectedUnit = this[0];
            foreach (var unit in diff)
                _AddUnitItem(unit, count++);
            _UpdateSubtitle();
            _UpdateSelection();

        }

        public void Remove(GameUnit unit)
        {
            var index = SelectList.IndexOf(unit);
            if (index == -1) return;
            _RemoveUnitItem(index);
            SelectList.RemoveInternal(unit);
            _UpdateSubtitle();
            _UpdateSelection();
        }

        public void Clear(bool ignoreItemList)
        {
            SelectList.ClearInternal();
            SelectedUnit = null;
            if (!ignoreItemList) _ResetItemList();
        }

        public void Clear() => Clear(false);
        public IEnumerator<GameUnit> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}