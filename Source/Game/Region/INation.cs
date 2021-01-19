using System;
using System.Collections.Generic;
using MSG.Game.Controller;
using MSG.Game.Unit;
using MSG.Game.Unit.Control.Group;

namespace MSG.Game.Region
{
    public interface INation :
        IEnumerable<IUnit>,
        IEnumerable<BaseController>,
        IEquatable<INation>
    {
        GameWorld World { get; }
        int ControllerCount { get; }
        int UnitCount { get; }
        int GroupCount { get; }
        int SingularUnitCount { get; }
        IUnit this[int index] { get; }
        BaseController MainController { get; set; }
        int Identifier { get; }
        bool IsActive { get; }
        bool HasPlayer { get; }

        void Add(BaseController controller);
        void Add(IUnit unit);
        void AddRange(ICollection<IUnit> units);
        void Remove(BaseController controller);
        void Remove(IUnit unit);
        BaseController GetController(int index);
        IUnit GetUnit(int index);
        IGroup GetGroup(int index);
        IUnit GetSingularUnit(int index);
        bool Contains(IUnit unit);
        bool Contains(BaseController controller);
    }

    public interface INation<T> :
        IEquatable<INation<T>>,
        INation
    {
    }
}