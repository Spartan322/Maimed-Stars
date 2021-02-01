
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MSG.Game.Unit;
using MSG.Utility;

namespace MSG.Game.Unit
{
    public sealed class UnitSelectList : Script.Unit.GameUnit.InternalUnitSelectList
    {
        public UnitSelectList() : base() {}
        public UnitSelectList(int capacity) : base(capacity) {}
    }

    public sealed class SelectionMenuList : Script.Unit.GameUnit.InternalUnitSelectList
    {
        public SelectionMenuList() : base() {}
        public SelectionMenuList(int capacity) : base(capacity) {}

        public new void Add(Script.Unit.GameUnit item) { }
        public new void AddRange(ICollection<Script.Unit.GameUnit> items) { }
        public new void AddRange(IReadOnlyCollection<Script.Unit.GameUnit> items) { }
        public new void AddRange(IEnumerable<Script.Unit.GameUnit> items) { }
        public new bool Remove(Script.Unit.GameUnit item) { return false; }
        public new void RemoveAt(int index) { }
        public new void Clear() { }

        internal void AddInternal(Script.Unit.GameUnit item) { base.Add(item); }
        internal void AddRangeInternal(ICollection<Script.Unit.GameUnit> items) { base.AddRange(items); }
        internal bool RemoveInternal(Script.Unit.GameUnit item) { return base.Remove(item); }
        internal void ClearInternal() { base.Clear(); }
    }
}

namespace MSG.Script.Unit
{
    public partial class GameUnit
    {
        private InternalUnitSelectList _selector;
        public UnitSelectList Selector => (UnitSelectList)_selector;

        public virtual void SelectUpdate(InternalUnitSelectList nextSelector) {}

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
                if(_listImplementation.Count > 1)
                    _listImplementation.Sort();
                item._selector = this;
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
                item.SelectUpdate(nextSelector);
                item._selector = nextSelector;
                return _listImplementation.Remove(item);
            }

            private void _AddRange(IEnumerable<GameUnit> items, int capacity = -1)
            {
                if(capacity > -1) _listImplementation.Capacity += capacity;
                foreach(var item in items)
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
                item.SelectUpdate(null);
                item._selector = null;
                _listImplementation.RemoveAt(index);
            }

            public GameUnit this[int index] => _listImplementation[index];
        }
    }
}