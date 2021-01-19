using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace MSG.Game.Unit
{
    public sealed class UnitSelectList : UnitSelectModule.InternalUnitSelectList
    {
    }

    public class UnitSelectModule : UnitBaseModule
    {
        private InternalUnitSelectList _selector;
        public UnitSelectList Selector => (UnitSelectList)_selector;

        public UnitSelectModule(IUnitController unitController)
            : base(unitController)
        {
        }

        public void Select(InternalUnitSelectList nextSelector)
        {
            if (!CanSelect(nextSelector)) return;
            nextSelector.Add(this);
        }

        public void Deselect()
        {
            _selector?.Remove(this);
        }

        public bool CanSelect(InternalUnitSelectList nextSelector)
        {
            return true;
        }

        internal void OnPreSelect(IEnumerable nextSelector)
        {
            UnitController.OnSelectChange(nextSelector != null);
        }

        public class InternalUnitSelectList : IList<UnitSelectModule>
        {
            private readonly IList<UnitSelectModule> _listImplementation = new List<UnitSelectModule>();

            protected InternalUnitSelectList() {}

            public void QueueMoveForSelection(Vector2 target, bool queue, float speed = -1)
            {
                // TODO: generate formation
                foreach (var moveModule in this
                    .Select(unit => unit.UnitController.GetModule<UnitMoveModule>(ModuleIndex.Move)))
                {
                    if (speed > 0) moveModule.MaximumSpeedLimit = speed;
                    if (!queue) moveModule.ClearTargets();
                    moveModule.AddTarget(target);
                }
            }

            public IEnumerator<UnitSelectModule> GetEnumerator() => _listImplementation.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => _listImplementation.GetEnumerator();

            public void Add(UnitSelectModule item)
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
                    item.OnPreSelect(null);
                    item._selector = null;
                }

                _listImplementation.Clear();
            }

            public bool Contains(UnitSelectModule item) => _listImplementation.Contains(item);

            public void CopyTo(UnitSelectModule[] array, int arrayIndex) =>
                _listImplementation.CopyTo(array, arrayIndex);

            public bool Remove(UnitSelectModule item) => _Remove(item, null);

            private bool _Remove(UnitSelectModule item, IEnumerable nextSelector)
            {
                if (item == null) return false;
                item.OnPreSelect(nextSelector);
                item._selector = null;
                return _listImplementation.Remove(item);
            }

            private static void _RemoveOrPreselect(UnitSelectModule item, IEnumerable nextSelector)
            {
                if (item._selector == null) item.OnPreSelect(nextSelector);
                else item._selector._Remove(item, nextSelector);
            }

            public int Count => _listImplementation.Count;
            public bool IsReadOnly => _listImplementation.IsReadOnly;
            public int IndexOf(UnitSelectModule item) => _listImplementation.IndexOf(item);

            public void Insert(int index, UnitSelectModule item)
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
                item.OnPreSelect(null);
                item._selector = null;
                _listImplementation.RemoveAt(index);
            }

            public UnitSelectModule this[int index]
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