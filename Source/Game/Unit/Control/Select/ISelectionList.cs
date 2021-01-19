using System.Collections.Generic;
using Godot;

namespace MSG.Game.Unit.Control.Select
{
    public delegate void OnObjectSelectAction<T>(ISelectionList<T> selectionList, T obj);
    public delegate void OnObjectDeselectAction<T>(ISelectionList<T> selectionList, T obj);

    public interface ISelectionList<T> : IReadOnlySelectionList<T>, IList<T>
    {
        event OnObjectSelectAction<T> OnObjectSelect;
        event OnObjectDeselectAction<T> OnObjectDeselect;
        void Add(T obj, bool clear = false);
        void AddRange(IEnumerable<T> objs, bool clear = false);
        void InsertRange(int index, IEnumerable<T> objs);
        bool Remove(T obj, bool ignoreListSet);
        void Sort();
        new int Count { get; }
        new T this[int index] { get; set; }
        new bool Contains(T obj);
        new int IndexOf(T obj);
        new void CopyTo(T[] array, int index);
    }

    public interface IReadOnlySelectionList<T> : IReadOnlyList<T>
    {
        bool Contains(T obj);
        int IndexOf(T obj);
        void CopyTo(T[] array, int index);

        void Move(Vector2 point, bool queue = false, float limit = -1);
    }

    public interface ISelectionList : ISelectionList<IUnit>
    {
    }
}