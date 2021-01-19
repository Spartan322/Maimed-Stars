using System.Collections.Generic;
using MSG.Game.Unit.Control.Select;

namespace MSG.Game.Unit.Control.Group
{
    //public interface IGroup : IGroupableObject, IList<IGroupableObject>
    //{
    //	bool SearchFor(IGroupableObject obj, out IGroup parentGroup);
    //	bool SearchFor(IGroupableObject obj);
    //	void AddRange(IEnumerable<IGroupableObject> objs);
    //	void InsertRange(int index, IEnumerable<IGroupableObject> objs);
    //}

    public interface IGroup<T> : IReadOnlySelectionList<T>
    {
        T SlowestUnit { get; }
        bool SearchFor(T obj, out IGroup parentGroup);
        bool SearchFor(T obj);
    }

    public interface IGroup : IGroup<IUnit>, IUnit, IList<IUnit>
    {
        void AddRange(IEnumerable<IUnit> objs);
        void InsertRange(int index, IEnumerable<IUnit> objs);
    }
}