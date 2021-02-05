using System.Collections;
using System.Collections.Generic;

namespace MSG.Script.Unit
{
    public partial class SelectableGroup
    {
        private List<GameUnit> _units = new List<GameUnit>();

        public int Count => _units.Count;

        public bool IsReadOnly => false;

        private void _OnAddUnit(GameUnit unit)
        {
            if (SlowestUnit == null || unit.MaximumMoveSpeed < SlowestUnit.MaximumMoveSpeed)
                SlowestUnit = unit;
            if (unit.Group == null) return;
            _OnRemoveUnit(unit, this);
            unit.Group._units.Remove(unit);
            if (unit.Manager != Manager) unit.Manager = Manager;
        }

        private void _OnRemoveUnit(GameUnit unit, SelectableGroup newGroup)
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

        public GameUnit this[int index]
        {
            get => _units[index];
            set
            {
                if (Contains(value)) return;
                _OnAddUnit(value);
                _units[index] = value;
                UpdateData();
            }
        }

        public int IndexOf(GameUnit item) => _units.IndexOf(item);

        public void Insert(int index, GameUnit item)
        {
            _OnAddUnit(item);
            _units.Insert(index, item);
            UpdateData();
        }

        public void RemoveAt(int index)
        {
            _OnRemoveUnit(this[index], null);
            _units.RemoveAt(index);
            UpdateData();
        }

        public void Add(GameUnit item, bool ignoreUpdate)
        {
            _OnAddUnit(item);
            _units.Add(item);
            if (!ignoreUpdate) UpdateData();
        }

        public void Add(GameUnit item) => Add(item, false);

        public void AddRange(IEnumerable<GameUnit> items)
        {
            if (items is ICollection<GameUnit> collection)
                _units.Capacity += collection.Count;
            if (items is IReadOnlyCollection<GameUnit> read)
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

        public bool Contains(GameUnit item) => item.Group == this;

        public void CopyTo(GameUnit[] array, int arrayIndex) => _units.CopyTo(array, arrayIndex);

        public bool Remove(GameUnit item)
        {
            _OnRemoveUnit(item, null);
            var result = _units.Remove(item);
            UpdateData();
            return result;
        }

        public IEnumerator<GameUnit> GetEnumerator() => _units.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}