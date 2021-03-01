using System.Collections;
using System.Collections.Generic;
using MSG.Game.Rts.Unit;

namespace MSG.Script.Gui.Game.Select
{
    public partial class SelectionDisplay
    {
        public BaseUnit this[int index] => SelectList[index];
        public int Count => SelectList.Count;

        public void Add(BaseUnit unit) => SelectList.Add(unit);

        public void AddRange(IEnumerable<BaseUnit> units) => SelectList.AddRange(units);

        public void Remove(BaseUnit unit) => SelectList.Remove(unit);

        private bool _ignoreItemList;
        public void Clear(bool ignoreItemList)
        {
            _ignoreItemList = ignoreItemList;
            SelectList.Clear();
        }

        public void Clear() => Clear(false);
        public IEnumerator<BaseUnit> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private bool _updateListShouldWait;
        private void _OnSelectListChanged(MainSelectList list, BaseUnit singleAdd, BaseUnit[] multiAdd, BaseUnit sub)
        {
            if (singleAdd != null)
            {
                _AddUnitItem(singleAdd, Count + 1);
            }
            else if (sub != null)
            {
                _RemoveUnitItem(list.IndexOf(sub));
            }
            else if (multiAdd != null)
            {
                _updateListShouldWait = true;
            }
        }

        private void _OnSelectListPostChanged(MainSelectList list, BaseUnit singleAdd, BaseUnit[] multiAdd, BaseUnit sub)
        {
            if (singleAdd == null && multiAdd == null && sub == null) // if cleared
            {
                if (!_ignoreItemList) _ResetItemList();
                return;
            }
            if (_updateListShouldWait && multiAdd == null) return;
            _UpdateSubtitle();
            _UpdateSelection();
            _updateListShouldWait = false;
        }
    }
}