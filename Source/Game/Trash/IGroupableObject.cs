using System;

namespace MSG.Game.Unit.Control.Group
{
    public delegate void OnGroupChangeAction(IGroupableObject obj, IGroup group);

    public interface IGroupableObject : IComparable<IGroupableObject>
    {
        event OnGroupChangeAction OnGroupChange;
        IGroup Group { get; }
        void AddToGroup(IGroup group);
        void RemoveFromGroup();
    }

    public delegate void OnGroupChangeAction<T>(T obj, IGroup<T> group)
        where T : IUnit;

    public interface IGroupableObject<T> : IGroupableObject
        where T : IUnit
    {
        new event OnGroupChangeAction<T> OnGroupChange;
        new IGroup<T> Group { get; }
    }
}