using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using MSG.Game.Controller;
using MSG.Game.Unit;
using MSG.Game.Unit.Control.Group;

namespace MSG.Game.Region
{
    public class NationSettings
    {
        public int? Identifier;
    }

    public class Nation : Reference, INation<Script.Game>
    {
        private static int _identifierCounter = 1;

        private readonly List<BaseController> _controllers = new List<BaseController>();
        private readonly List<IUnit> _nationUnits = new List<IUnit>();
        private readonly List<IGroup> _nationGroups = new List<IGroup>();

        public GameWorld World { get; }

        public NationSettings Settings { get; }
        public int Identifier { get; private set; }

        public bool IsActive => MainController.IsActive;
        public bool HasPlayer { get; private set; }

        private int _mainControllerIndex = -1;

        public BaseController MainController
        {
            get => GetController(_mainControllerIndex);
            set
            {
                var index = _controllers.IndexOf(value);
                if (index == -1)
                    throw new ArgumentException($"{nameof(BaseController)} not contained in {nameof(Nation)}",
                        nameof(value));
                _mainControllerIndex = index;
            }
        }

        public int ControllerCount => _controllers.Count;
        public int UnitCount => _nationGroups.Count + _nationUnits.Count;
        public int GroupCount => _nationGroups.Count;
        public int SingularUnitCount => _nationUnits.Count;

        public IUnit this[int index]
            => index >= _nationGroups.Count ? _nationUnits[index - _nationGroups.Count] : _nationGroups[index];

        public Nation(GameWorld world, NationSettings settings = default)
        {
            World = world;
            Settings = settings;
        }

        public void Initialize(Script.Game node)
        {
            if (Settings.Identifier == null)
                Identifier = _identifierCounter++;
            else Identifier = Settings.Identifier.Value;
        }

        public override bool Equals(object obj)
            => obj is Nation nation && Equals(nation);

        public override int GetHashCode()
            => Identifier.GetHashCode();

        public void Add(BaseController controller)
        {
            controller.AddToNation(this);
            _controllers.Add(controller);
        }

        public void Add(IUnit unit)
        {
            if (!CanAdd(unit)) return;
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
                if (CanAdd(obj))
                {
                    obj.AddToNation(this);
                    if (obj is IGroup group)
                        _nationGroups.Add(group);
                    else _nationUnits.Add(obj);
                }
            }
        }

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

        public BaseController GetController(int index)
            => _controllers[index];

        public IUnit GetUnit(int index)
            => this[index];

        public IGroup GetGroup(int index)
            => _nationGroups[index];

        public IUnit GetSingularUnit(int index)
            => _nationUnits[index];

        public IEnumerator<IUnit> GetEnumerator()
            => _nationUnits.GetEnumerator();

        IEnumerator<BaseController> IEnumerable<BaseController>.GetEnumerator()
            => _controllers.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public bool Equals(INation<Script.Game> other)
            => Identifier == other.Identifier;

        public bool Contains(IUnit unit)
            => unit.Nation == this;

        public bool Contains(BaseController controller)
            => controller.Nation == this;

        private bool CanAdd(IUnit unit)
            => !Contains(unit);

        public bool Equals(INation other)
            => Identifier == other.Identifier;
    }
}