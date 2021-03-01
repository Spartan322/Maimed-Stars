using System.Collections;
using System.Collections.Generic;
using MSG.Game.Rts.Unit;

namespace MSG.Script.Game.Unit
{
    public partial class GroupUnit
    {
        private List<BaseUnit> _units = new List<BaseUnit>();

        public int Count => _units.Count;

        public bool IsReadOnly => false;

        private void _OnAddUnit(BaseUnit unit)
        {
            if (SlowestUnit == null || unit.MaximumMoveSpeed < SlowestUnit.MaximumMoveSpeed)
                SlowestUnit = unit;
            if (unit.Group == null) return;
            unit.Group.Remove(unit);
            if (unit.Manager != Manager) unit.Manager = Manager;
        }

        private void _OnRemoveUnit(BaseUnit unit, GroupUnit newGroup)
        {
            unit.Group = newGroup;
            if (SlowestUnit == unit)
            {
                SlowestUnit = null;
                foreach (var groupUnit in this)
                {
                    if (SlowestUnit == null) SlowestUnit = groupUnit;
                    else if (SlowestUnit.MaximumMoveSpeed > groupUnit.MaximumMoveSpeed)
                        SlowestUnit = groupUnit;
                }
            }
        }

        public BaseUnit this[int index]
        {
            get => _units[index];
            set
            {
                if (Contains(value)) return;
                _OnAddUnit(value);
                _units[index] = value;
                value.Group = this;
                UpdateData();
            }
        }

        public int IndexOf(BaseUnit item) => _units.IndexOf(item);

        public void Insert(int index, BaseUnit item)
        {
            _OnAddUnit(item);
            _units.Insert(index, item);
            item.Group = this;
            UpdateData();
        }

        public void RemoveAt(int index)
        {
            _OnRemoveUnit(this[index], null);
            _units.RemoveAt(index);
            UpdateData();
        }

        public void Add(BaseUnit item, bool ignoreUpdate)
        {
            _OnAddUnit(item);
            _units.Add(item);
            item.Group = this;
            if (!ignoreUpdate) UpdateData();
        }

        public void Add(BaseUnit item) => Add(item, false);

        public void AddRange(IEnumerable<BaseUnit> items)
        {
            if (items is ICollection<BaseUnit> collection)
                _units.Capacity += collection.Count;
            if (items is IReadOnlyCollection<BaseUnit> read)
                _units.Capacity += read.Count;
            foreach (var unit in items)
                Add(unit, true);
            UpdateData();
        }

        public void Clear(bool ignoreUpdate)
        {
            foreach (var unit in _units)
                _OnRemoveUnit(unit, null);
            _units.Clear();
            if (!ignoreUpdate) UpdateData();
        }

        public void Clear() => Clear(false);

        public bool Contains(BaseUnit item) => item.Group == this;

        public void CopyTo(BaseUnit[] array, int arrayIndex) => _units.CopyTo(array, arrayIndex);

        public bool Remove(BaseUnit item, bool ignoreUpdate)
        {
            _OnRemoveUnit(item, null);
            var result = _units.Remove(item);
            if (Count == 0)
            {
                GetParent().RemoveChild(this);
                QueueFree();
            }
            else if (!ignoreUpdate) UpdateData();
            return result;
        }

        public bool Remove(BaseUnit item) => Remove(item, false);

        public IEnumerator<BaseUnit> GetEnumerator() => _units.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}