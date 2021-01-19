using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MSG.Game.Unit.Control.Group;

namespace MSG.Game.Unit
{
    public class BaseUnitGroup<SceneObjectT> :
        BaseUnit<SceneObjectT>,
        IGroup
        where SceneObjectT : IComparable<SceneObjectT>, IComparableOverlap<SceneObjectT>, IEquatable<SceneObjectT>
    {
        private readonly List<IUnit> _units = new List<IUnit>();
        // TODO: FormationObject

        public IUnit SlowestUnit { get; private set; }

        public IUnit this[int index]
        {
            get => _units[index];
            set => _units[index] = value;
        }

        public int Count => _units.Count;
        public bool IsReadOnly => false;

        public void Add(IUnit item)
        {
            if (item == null || !CanAdd(item)) return;
            item.AddToGroup(this);
            _units.Add(item);
            _OnAdd(item);
        }

        public void Clear()
        {
            foreach (var obj in _units.Where(Contains))
                obj.RemoveFromGroup();

            _units.Clear();
        }

        public bool Contains(IUnit item)
            => item != null && ReferenceEquals(item.Group, this);

        public void CopyTo(IUnit[] array, int arrayIndex)
            => _units.CopyTo(array, arrayIndex);

        public void Move(Vector2 point, bool queue = false, float limit = -1)
        {
            if(limit < 0 || limit > SlowestUnit.MaxSpeed)
                limit = SlowestUnit.MaxSpeed;
            MaxSpeed = limit;
            // TODO: Add formation object movement
            foreach (var unit in _units)
            {
                unit.TargetSpeed = limit;
                if (!queue)
                    unit.ClearMovementTargets();
                unit.AddMovementTarget(point);
            }
        }

        public IEnumerator<IUnit> GetEnumerator()
            => _units.GetEnumerator();

        public int IndexOf(IUnit item)
            => _units.IndexOf(item);

        public void Insert(int index, IUnit item)
        {
            if (item == null || !CanAdd(item)) return;
            item.AddToGroup(this);
            _units.Insert(index, item);
            _OnAdd(item);
        }

        public bool Remove(IUnit item)
        {
            if (item == null || !Contains(item)) return false;
            item.RemoveFromGroup();
            _OnRemove(item);
            return _units.Remove(item);
        }

        public void RemoveAt(int index)
        {
            this[index].RemoveFromGroup();
            _OnRemove(this[index]);
            _units.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
            => _units.GetEnumerator();

        public bool SearchFor(IUnit obj)
            => SearchFor(obj, out _);

        public bool SearchFor(IUnit obj, out IGroup parentGroup)
        {
            parentGroup = this;
            foreach (var unit in parentGroup)
            {
                if (unit.Equals(obj)) return true;
                if (unit is IGroup group)
                    return group.SearchFor(obj, out parentGroup);
            }

            parentGroup = null;
            return false;
        }

        private bool CanAdd(IUnit obj)
            => true;

        public void AddRange(IEnumerable<IUnit> objs)
        {
            if (objs is ICollection<IUnit> collection)
                _units.Capacity += collection.Count;
            foreach (var obj in objs)
            {
                if (!CanAdd(obj)) continue;
                obj.AddToGroup(this);
                _units.Add(obj);
                _OnAdd(obj);
            }
        }

        public void InsertRange(int index, IEnumerable<IUnit> objs)
        {
            if (objs is ICollection<IUnit> collection)
                _units.Capacity += collection.Count;
            foreach (var obj in objs)
            {
                if (!CanAdd(obj)) continue;
                obj.AddToGroup(this);
                _units.Insert(index++, obj);
                _OnAdd(obj);
            }
        }

        private void _OnAdd(IUnit item)
        {
            if (SlowestUnit.MaxSpeed > item.MaxSpeed) SlowestUnit = item;
        }

        private void _OnRemove(IUnit item)
        {
            if (!ReferenceEquals(item, SlowestUnit)) return;
            IUnit slowest = null;
            _units.ForEach(unit =>
            {
                if (unit.MaxSpeed < (slowest?.MaxSpeed ?? 0))
                    slowest = unit;
            });
        }
    }
}