using System.Collections.Generic;

namespace MSG.Utility
{
    public interface ISimpleList<T> : IReadOnlyList<T>, ICollection<T>
    {
        int IndexOf(T item);
        void RemoveAt(int index);
    }
}