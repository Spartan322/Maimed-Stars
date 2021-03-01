using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MSG.Engine;
using SpartansLib.Structure;

namespace MSG.Game.Rts.Unit
{
    public partial class BaseUnit
    {
        internal InternalSelectList _selector;

        public virtual void SelectUpdate(InternalSelectList nextSelector) { }

        public virtual void Select(InternalSelectList nextSelector)
        {
            if (!CanSelect(nextSelector)) return;
            nextSelector.Add(this);
        }

        public virtual void Deselect()
        {
            _selector?.Remove(this);
        }

        public virtual bool CanSelect(InternalSelectList nextSelector)
        {
            return true;
        }

        public class InternalSelectList : ISimpleList<BaseUnit>, IFormationHolder
        {
            protected readonly List<BaseUnit> _listImplementation;

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

            public float MaximumSpeedLimit = -1;

            protected internal InternalSelectList()
            {
                _listImplementation = new List<BaseUnit>();
            }

            protected internal InternalSelectList(int capacity)
            {
                _listImplementation = new List<BaseUnit>(capacity);
            }

            protected virtual bool IsMainSelector => false;

            public void QueueMoveForSelection(Vector2 target, bool queue)
            {
                if (Formation != null)
                    Formation.QueueFormationMove(new Offset(target, this.FirstOrDefault()?.Position.AngleToPoint(target) ?? 0), this);
                else foreach (var unit in this)
                    {
                        if (MaximumSpeedLimit > 0) unit.MaximumSpeedLimit = MaximumSpeedLimit;
                        if (!queue) unit.ClearMovementTargets();
                        unit.MoveTo(target);
                    }
            }

            public IEnumerator<BaseUnit> GetEnumerator() => _listImplementation.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => _listImplementation.GetEnumerator();

            public virtual void Add(BaseUnit item)
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

            public virtual void AddRange(IEnumerable<BaseUnit> items)
            {
                if (items is ICollection<BaseUnit> collection)
                {
                    AddRange(collection);
                    return;
                }
                if (items is IReadOnlyCollection<BaseUnit> readonlyCollection)
                {
                    AddRange(readonlyCollection);
                    return;
                }
                _AddRange(items);
            }

            public void AddRange(ICollection<BaseUnit> items)
                => _AddRange(items, items.Count);

            public void AddRange(IReadOnlyCollection<BaseUnit> items)
                => _AddRange(items, items.Count);

            public virtual void Clear()
            {
                if (IsMainSelector)
                    foreach (var item in this)
                    {
                        item.SelectUpdate(null);
                        item._selector = null;
                    }

                _listImplementation.Clear();
            }

            public virtual bool Contains(BaseUnit item) => _listImplementation.Contains(item);

            public virtual void CopyTo(BaseUnit[] array, int arrayIndex)
                => _listImplementation.CopyTo(array, arrayIndex);

            public bool Remove(BaseUnit item) => _Remove(item, null);

            protected virtual bool _Remove(BaseUnit item, InternalSelectList nextSelector)
            {
                if (item == null) return false;
                if (IsMainSelector)
                {
                    item.SelectUpdate(nextSelector);
                    item._selector = nextSelector;
                }
                return _listImplementation.Remove(item);
            }

            protected void _AddRange(IEnumerable<BaseUnit> items, int capacity = -1)
            {
                if (capacity > -1)
                    _listImplementation.Capacity = Count + capacity;
                foreach (var item in items)
                    Add(item);
            }

            protected static void _RemoveOrPreselect(BaseUnit item, InternalSelectList nextSelector)
            {
                if (item._selector == null) item.SelectUpdate(nextSelector);
                else item._selector._Remove(item, nextSelector);
            }

            public int Count => _listImplementation.Count;
            public bool IsReadOnly => true;
            public virtual int IndexOf(BaseUnit item) => _listImplementation.IndexOf(item);

            public virtual void RemoveAt(int index)
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

            public virtual BaseUnit this[int index] => _listImplementation[index];
        }
    }
}