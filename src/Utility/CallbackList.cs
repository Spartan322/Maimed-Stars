using System;
using System.Collections.Generic;

namespace MSG.Utility
{
    public enum ModificationType
    {
        Set,
        Add,
        Clear,
        Insert,
        InsertRange,
        Remove,
        RemoveAll,
        RemoveAt,
        RemoveRange,
        Sort,
        SortIComparer,
        SortComparison,
        SortIndex,
        TrimExcess
    }

    public delegate void ModifyCallback<T>(CallbackList<T> list, ModificationType type, CallbackData<T> data);

    public class CallbackData<T>
    {
        public T Change { get; internal set; }
        public int Index { get; internal set; }
        public int Count { get; internal set; }
        public IEnumerable<T> Range { get; internal set; }
        public Predicate<T> Match { get; internal set; }
        public IComparer<T> Comparer { get; internal set; }
        public Comparison<T> Comparison { get; internal set; }
    }

    public class CallbackList<T> : List<T>
    {
        public event ModifyCallback<T> OnModified;
        public event ModifyCallback<T> OnPostModified;

        public CallbackList()
        {
        }

        public CallbackList(List<T> list) : base(list)
        {
        }

        public CallbackList(IEnumerable<T> collection) : base(collection)
        {
        }

        public CallbackList(int capacity) : base(capacity)
        {
        }

        public new T this[int index]
        {
            get { return base[index]; }
            set
            {
                var d = new CallbackData<T> {Index = index, Change = value};
                OnModified?.Invoke(this, ModificationType.Set, d);
                base[index] = value;
                OnPostModified?.Invoke(this, ModificationType.Set, d);
            }
        }

        public new void Add(T item)
        {
            var d = new CallbackData<T> {Change = item};
            OnModified?.Invoke(this, ModificationType.Add, d);
            base.Add(item);
            OnPostModified?.Invoke(this, ModificationType.Add, d);
        }

        public new void Clear()
        {
            OnModified?.Invoke(this, ModificationType.Clear, null);
            base.Clear();
            OnPostModified?.Invoke(this, ModificationType.Clear, null);
        }

        public new void Insert(int index, T item)
        {
            var d = new CallbackData<T> {Index = index, Change = item};
            OnModified?.Invoke(this, ModificationType.Insert, d);
            base.Insert(index, item);
            OnPostModified?.Invoke(this, ModificationType.Insert, d);
        }

        public new void InsertRange(int index, IEnumerable<T> collection)
        {
            var d = new CallbackData<T> {Index = index, Range = collection};
            OnModified?.Invoke(this, ModificationType.InsertRange, d);
            base.InsertRange(index, collection);
            OnPostModified?.Invoke(this, ModificationType.InsertRange, d);
        }

        public new bool Remove(T item)
        {
            var d = new CallbackData<T> {Change = item};
            OnModified?.Invoke(this, ModificationType.Remove, d);
            var r = base.Remove(item);
            OnPostModified?.Invoke(this, ModificationType.Remove, d);
            return r;
        }

        public new int RemoveAll(Predicate<T> match)
        {
            var d = new CallbackData<T> {Match = match};
            OnModified?.Invoke(this, ModificationType.RemoveAll, d);
            var r = base.RemoveAll(match);
            OnPostModified?.Invoke(this, ModificationType.RemoveAll, d);
            return r;
        }

        public new void RemoveAt(int index)
        {
            var d = new CallbackData<T> {Index = index};
            OnModified?.Invoke(this, ModificationType.RemoveAt, d);
            base.RemoveAt(index);
            OnPostModified?.Invoke(this, ModificationType.RemoveAt, d);
        }

        public new void RemoveRange(int index, int count)
        {
            var d = new CallbackData<T> {Index = index, Count = count};
            OnModified?.Invoke(this, ModificationType.RemoveRange, d);
            base.RemoveRange(index, count);
            OnPostModified?.Invoke(this, ModificationType.RemoveRange, d);
        }

        public new void Sort()
        {
            OnModified?.Invoke(this, ModificationType.Sort, null);
            base.Sort();
            OnPostModified?.Invoke(this, ModificationType.Sort, null);
        }

        public new void Sort(IComparer<T> comparer)
        {
            var d = new CallbackData<T> {Comparer = comparer};
            OnModified?.Invoke(this, ModificationType.SortIComparer, d);
            base.Sort(comparer);
            OnPostModified?.Invoke(this, ModificationType.SortIComparer, d);
        }

        public new void Sort(Comparison<T> comparison)
        {
            var d = new CallbackData<T> {Comparison = comparison};
            OnModified?.Invoke(this, ModificationType.SortComparison, d);
            base.Sort(comparison);
            OnPostModified?.Invoke(this, ModificationType.SortComparison, d);
        }

        public new void Sort(int index, int count, IComparer<T> comparer)
        {
            var d = new CallbackData<T> {Index = index, Count = count, Comparer = comparer};
            OnModified?.Invoke(this, ModificationType.SortIndex, d);
            base.Sort(index, count, comparer);
            OnPostModified?.Invoke(this, ModificationType.SortIndex, d);
        }

        public new void TrimExcess()
        {
            OnModified?.Invoke(this, ModificationType.TrimExcess, null);
            base.TrimExcess();
            OnPostModified?.Invoke(this, ModificationType.TrimExcess, null);
        }
    }
}