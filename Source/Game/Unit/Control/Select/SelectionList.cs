using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace MSG.Game.Unit.Control.Select
{
    public class SelectionList : ISelectionList
    {
        private readonly IList<IUnit> _selection;

        public event OnObjectSelectAction<IUnit> OnObjectSelect;
        public event OnObjectDeselectAction<IUnit> OnObjectDeselect;

        public IUnit this[int index]
        {
            get => _selection[index];
            set => _selection[index] = value;
        }

        public int Count => _selection.Count;
        public bool IsReadOnly => false;

        public SelectionList()
        {
            _selection = new List<IUnit>();
        }

        public SelectionList(IList<IUnit> units)
        {
            _selection = units;
        }

        public SelectionList(IEnumerable<IUnit> enumerables)
        {
            if (enumerables is IList<IUnit> list)
                _selection = list;
            else
            {
                _selection = new List<IUnit>();
                AddRange(enumerables);
            }
        }

        public SelectionList(int capacity)
        {
            _selection = new List<IUnit>(capacity);
        }

        public void Add(IUnit obj, bool clear)
        {
            if (clear) Clear();
            if (!CanSelect(obj)) return;
            OnObjectSelect?.Invoke(this, obj);
            obj.SelectionList = this;
            _selection.Add(obj);
        }

        public void AddRange(IEnumerable<IUnit> objs, bool clear = false)
        {
            if (clear) Clear();
            foreach (var obj in objs)
            {
                if (!CanSelect(obj)) continue;
                OnObjectSelect?.Invoke(this, obj);
                obj.SelectionList = this;
                _selection.Add(obj);
            }
        }

        public void Clear()
        {
            foreach (var obj in _selection)
                obj.SelectionList = null;
            _selection.Clear();
        }

        public bool Contains(IUnit obj) => obj?.SelectionList == this;

        public void CopyTo(IUnit[] array, int index) => _selection.CopyTo(array, index);

        public void Move(Vector2 point, bool queue = false, float limit = -1)
        {
            // TODO: Add formation object movement??
            foreach (var unit in this)
            {
                unit.TargetSpeed = limit;
                if (!queue)
                    unit.ClearMovementTargets();
                unit.AddMovementTarget(point);
            }
        }

        public IEnumerator<IUnit> GetEnumerator() => _selection.GetEnumerator();

        public int IndexOf(IUnit obj) => _selection.IndexOf(obj);

        public void Insert(int index, IUnit obj)
        {
            if (obj == null || !CanSelect(obj)) return;
            OnObjectSelect?.Invoke(this, obj);
            obj.SelectionList = this;
            _selection.Insert(index, obj);
        }

        public void InsertRange(int index, IEnumerable<IUnit> objs)
        {
            foreach (var obj in objs)
            {
                if (!CanSelect(obj)) continue;
                OnObjectSelect?.Invoke(this, obj);
                obj.SelectionList = this;
                _selection.Insert(index++, obj);
            }
        }

        public bool Remove(IUnit obj) => Remove(obj, false);

        public bool Remove(IUnit obj, bool ignoreListSet)
        {
            if (!Contains(obj)) return false;
            OnObjectDeselect?.Invoke(this, obj);
            if (!ignoreListSet)
                obj.SelectionList = null;
            return _selection.Remove(obj);
        }

        public void RemoveAt(int index)
        {
            var obj = this[index];
            OnObjectDeselect?.Invoke(this, obj);
            obj.SelectionList = null;
            _selection.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private bool CanSelect(IUnit obj) => obj.SelectionList != this && obj.CanSelect;

        public void Sort() => ArrayList.Adapter((IList)_selection).Sort();

        public void Add(IUnit item) => Add(item, false);
    }
}