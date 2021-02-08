
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MSG.Game.Unit;
using MSG.Script.Unit;
using MSG.Utility;

namespace MSG.Game.Unit
{
    public sealed class UnitSelectList : GameUnit.InternalUnitSelectList
    {
        public UnitSelectList() : base() { }
        public UnitSelectList(int capacity) : base(capacity) { }
        public UnitSelectList(GameUnit.InternalUnitSelectList list) : base(list.Count)
        {
            _listImplementation.AddRange(list);
        }
    }

    public sealed class SelectionMenuList : GameUnit.InternalUnitSelectList
    {
        public SelectionMenuList() : base() { }
        public SelectionMenuList(int capacity) : base(capacity) { }

        protected override bool IsMainSelector => true;

        public new void Add(GameUnit item) { }
        public new void AddRange(ICollection<GameUnit> items) { }
        public new void AddRange(IReadOnlyCollection<GameUnit> items) { }
        public new void AddRange(IEnumerable<GameUnit> items) { }
        public new bool Remove(GameUnit item) { return false; }
        public new void RemoveAt(int index) { }
        public new void Clear() { }

        internal bool AddInternal(GameUnit item)
        {
            var newAdd = !Contains(item);
            base.Add(item);
            return newAdd;
        }
        internal IList<GameUnit> AddRangeInternal(ICollection<GameUnit> items)
        {
            IList<GameUnit> difference = items.Except(this).ToArray();
            base.AddRange(difference);
            return difference;
        }
        internal bool RemoveInternal(GameUnit item) { return base.Remove(item); }
        internal void ClearInternal() { base.Clear(); }
    }
}

namespace MSG.Script.Unit
{
    public partial class GameUnit
    {
        internal InternalUnitSelectList _selector;

        public virtual void SelectUpdate(InternalUnitSelectList nextSelector) { }

        public virtual void Select(InternalUnitSelectList nextSelector)
        {
            if (!CanSelect(nextSelector)) return;
            nextSelector.Add(this);
        }

        public virtual void Deselect()
        {
            _selector?.Remove(this);
        }

        public virtual bool CanSelect(InternalUnitSelectList nextSelector)
        {
            return true;
        }

        public class InternalUnitSelectList : ISimpleList<GameUnit>, IFormationHolder
        {
            protected readonly List<GameUnit> _listImplementation;

            private FormationBase _formation;
            public FormationBase Formation
            {
                get => _formation;
                set
                {
                    _formation = value;
                    _formation.Holder = this;
                }
            }

            protected internal InternalUnitSelectList()
            {
                _listImplementation = new List<GameUnit>();
            }

            protected internal InternalUnitSelectList(int capacity)
            {
                _listImplementation = new List<GameUnit>(capacity);
            }

            protected virtual bool IsMainSelector => false;

            public void QueueMoveForSelection(Vector2 target, bool queue, float speed = -1)
            {
                // TODO: generate formation?
                foreach (var unit in this)
                {
                    if (speed > 0) unit.MaximumSpeedLimit = speed;
                    if (!queue) unit.ClearMovementTargets();
                    unit.MoveTo(target);
                }
            }

            public IEnumerator<GameUnit> GetEnumerator() => _listImplementation.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => _listImplementation.GetEnumerator();

            public void Add(GameUnit item)
            {
                if (!(item?.CanSelect(this) ?? false) || ReferenceEquals(item._selector, this)) return;
                _RemoveOrPreselect(item, this);
                _listImplementation.Add(item);
                if (_listImplementation.Count > 1)
                    _listImplementation.Sort();
                if (IsMainSelector)
                {
                    item._selector = this;
                    item.SelectUpdate(this);
                }
            }

            public void AddRange(IEnumerable<GameUnit> items)
            {
                if (items is ICollection<GameUnit> collection)
                {
                    AddRange(collection);
                    return;
                }
                if (items is IReadOnlyCollection<GameUnit> readonlyCollection)
                {
                    AddRange(readonlyCollection);
                    return;
                }
                _AddRange(items);
            }

            public void AddRange(ICollection<GameUnit> items)
                => _AddRange(items, items.Count);

            public void AddRange(IReadOnlyCollection<GameUnit> items)
                => _AddRange(items, items.Count);

            public void Clear()
            {
                if (IsMainSelector)
                    foreach (var item in this)
                    {
                        item.SelectUpdate(null);
                        item._selector = null;
                    }

                _listImplementation.Clear();
            }

            public bool Contains(GameUnit item) => _listImplementation.Contains(item);

            public void CopyTo(GameUnit[] array, int arrayIndex)
                => _listImplementation.CopyTo(array, arrayIndex);

            public bool Remove(GameUnit item) => _Remove(item, null);

            private bool _Remove(GameUnit item, InternalUnitSelectList nextSelector)
            {
                if (item == null) return false;
                if (IsMainSelector)
                {
                    item.SelectUpdate(nextSelector);
                    item._selector = nextSelector;
                }
                return _listImplementation.Remove(item);
            }

            private void _AddRange(IEnumerable<GameUnit> items, int capacity = -1)
            {
                if (capacity > -1) _listImplementation.Capacity += capacity;
                foreach (var item in items)
                    Add(item);
            }

            private static void _RemoveOrPreselect(GameUnit item, InternalUnitSelectList nextSelector)
            {
                if (item._selector == null) item.SelectUpdate(nextSelector);
                else item._selector._Remove(item, nextSelector);
            }

            public int Count => _listImplementation.Count;
            public bool IsReadOnly => true;
            public int IndexOf(GameUnit item) => _listImplementation.IndexOf(item);

            public void RemoveAt(int index)
            {
                if (index < 0 || index > Count) throw new IndexOutOfRangeException();
                var item = this[index];
                if (IsMainSelector)
                {
                    item.SelectUpdate(null);
                    item._selector = null;
                }
                _listImplementation.RemoveAt(index);
            }

            public GameUnit this[int index] => _listImplementation[index];
        }
    }
}