using System;
using System.Collections;
using System.Collections.Generic;
using MSG.Game.Controller;
using MSG.Game.Unit;
using MSG.Game.Unit.Control.Group;

namespace MSG.Game.Region
{
    public class DummyNation : INation
    {
        private readonly List<BaseController> _controllers = new List<BaseController>();
        private readonly List<IGroup> _nationGroups = new List<IGroup>();
        private readonly List<IUnit> _nationUnits = new List<IUnit>();

        public IUnit this[int index]
            => index >= _nationGroups.Count ? _nationUnits[index - _nationGroups.Count] : _nationGroups[index];

        public int ControllerCount => _controllers.Count;
        public int UnitCount => _nationGroups.Count + _nationUnits.Count;

        public BaseController MainController
        {
            get => null;
            set => throw new NotSupportedException();
        }

        public int Identifier => -1;
        public bool IsActive => true;
        public bool HasPlayer { get; private set; }
        public GameWorld World { get; }

        public int GroupCount => _nationGroups.Count;
        public int SingularUnitCount => _nationUnits.Count;

        public DummyNation(GameWorld world)
        {
            World = world;
        }

        public void Add(BaseController controller)
        {
            controller.AddToNation(this);
            _controllers.Add(controller);
        }

        public void Add(IUnit unit)
        {
            unit.AddToNation(this);
            if (unit is IGroup group)
                _nationGroups.Add(group);
            else _nationUnits.Add(unit);
        }

        public void AddRange(ICollection<IUnit> units)
        {
            _nationUnits.Capacity += units.Count;
            foreach (var obj in units)
            {
                obj.AddToNation(this);
                if (obj is IGroup group)
                    _nationGroups.Add(group);
                else _nationUnits.Add(obj);
            }
        }

        public bool Contains(IUnit unit)
            => unit.Nation == this;

        public bool Contains(BaseController controller)
            => controller.Nation == this;

        public bool Equals(INation other)
            => false;

        public BaseController GetController(int index)
            => _controllers[index];

        public IEnumerator<IUnit> GetEnumerator()
            => _nationUnits.GetEnumerator();

        public IUnit GetUnit(int index)
            => _nationUnits[index];

        public void Remove(BaseController controller)
        {
            if (!Contains(controller)) return;
            controller.RemoveFromNation();
            _controllers.Remove(controller);
        }

        public void Remove(IUnit unit)
        {
            if (!Contains(unit)) return;
            unit.RemoveFromNation();
            if (unit is IGroup group)
                _nationGroups.Remove(group);
            else _nationUnits.Remove(unit);
        }

        IEnumerator<BaseController> IEnumerable<BaseController>.GetEnumerator()
            => _controllers.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public IGroup GetGroup(int index)
            => _nationGroups[index];

        public IUnit GetSingularUnit(int index)
            => _nationUnits[index];
    }
}