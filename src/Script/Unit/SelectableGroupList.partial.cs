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
            if(unit.Group == null) return;
            _OnRemoveUnit(unit, this);
            unit.Group._units.Remove(unit);
            if(unit.Manager != Manager) unit.Manager = Manager;
        }

        private void _OnRemoveUnit(GameUnit unit, SelectableGroup newGroup)
        {
            unit.Group = newGroup;
        }

        public GameUnit this[int index]
        {
            get => _units[index];
            set
            {
                if(Contains(value)) return;
                _OnAddUnit(value);
                _units[index] = value;
            }
        }

        public int IndexOf(GameUnit item) => _units.IndexOf(item);

        public void Insert(int index, GameUnit item)
        {
            _OnAddUnit(item);
            _units.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _OnRemoveUnit(this[index], null);
            _units.RemoveAt(index);
        }

        public void Add(GameUnit item)
        {
            _OnAddUnit(item);
            _units.Add(item);
        }

        public void Clear()
        {
            foreach(var unit in _units)
                _OnRemoveUnit(unit, null);
            _units.Clear();
        }

        public bool Contains(GameUnit item) => item.Group == this;

        public void CopyTo(GameUnit[] array, int arrayIndex) => _units.CopyTo(array, arrayIndex);

        public bool Remove(GameUnit item)
        {
            _OnRemoveUnit(item, null);
            return _units.Remove(item);
        }

        public IEnumerator<GameUnit> GetEnumerator() => _units.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}