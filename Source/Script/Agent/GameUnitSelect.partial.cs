
using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using MSG.Game.Unit;

namespace MSG.Game.Unit
{
    public sealed class UnitSelectList : Script.Agent.GameUnit.InternalUnitSelectList
    {
        public UnitSelectList() : base() {}
        public UnitSelectList(int capacity) : base(capacity) {}
    }
}

namespace MSG.Script.Agent
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

        public class InternalUnitSelectList : IList<GameUnit>, IFormationHolder
        {
            protected readonly IList<GameUnit> _listImplementation;

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
                item._selector = this;
            }

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

            public void CopyTo(GameUnit[] array, int arrayIndex) =>
                _listImplementation.CopyTo(array, arrayIndex);

            public bool Remove(GameUnit item) => _Remove(item, null);

            private bool _Remove(GameUnit item, InternalUnitSelectList nextSelector)
            {
                if (item == null) return false;
                item.SelectUpdate(nextSelector);
                item._selector = null;
                return _listImplementation.Remove(item);
            }

            private static void _RemoveOrPreselect(GameUnit item, InternalUnitSelectList nextSelector)
            {
                if (item._selector == null) item.SelectUpdate(nextSelector);
                else item._selector._Remove(item, nextSelector);
            }

            public int Count => _listImplementation.Count;
            public bool IsReadOnly => _listImplementation.IsReadOnly;
            public int IndexOf(GameUnit item) => _listImplementation.IndexOf(item);

            public void Insert(int index, GameUnit item)
            {
                if (!(item?.CanSelect(this) ?? false) || ReferenceEquals(item._selector, this)) return;
                _RemoveOrPreselect(item, this);
                _listImplementation.Insert(index, item);
                item._selector = this;
            }

            public void RemoveAt(int index)
            {
                if (index < 0 || index > Count) throw new IndexOutOfRangeException();
                var item = this[index];
                item.SelectUpdate(null);
                item._selector = null;
                _listImplementation.RemoveAt(index);
            }

            public GameUnit this[int index]
            {
                get => _listImplementation[index];
                set
                {
                    if (index < 0 || index > Count + 1) throw new IndexOutOfRangeException();
                    if (!(value?.CanSelect(this) ?? false) || ReferenceEquals(value._selector, this)) return;
                    _RemoveOrPreselect(value, this);
                    _listImplementation[index] = value;
                    value._selector = this;
                }
            }
        }
    }
}