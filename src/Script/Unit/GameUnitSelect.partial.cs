using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using MSG.Game.Unit;
using MSG.Utility;

namespace MSG.Script.Unit
{
    public partial class GameUnit
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

        public class InternalSelectList : ISimpleList<GameUnit>, IFormationHolder
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

            protected internal InternalSelectList()
            {
                _listImplementation = new List<GameUnit>();
            }

            protected internal InternalSelectList(int capacity)
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

            private bool _Remove(GameUnit item, InternalSelectList nextSelector)
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

            private static void _RemoveOrPreselect(GameUnit item, InternalSelectList nextSelector)
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