using System.Collections;
using System.Collections.Generic;

namespace MSG.Game.Rts.Event
{
    public class EventManager : IReadOnlyCollection<EventData>
    {
        private Queue<EventData> _queue;

        public int Count => _queue.Count;

        public void Add(EventData item)
        {
            _queue.Enqueue(item);
        }

        public void Clear()
        {
            _queue.Clear();
        }

        public bool Contains(EventData item)
        {
            return _queue.Contains(item);
        }

        public void CopyTo(EventData[] array, int arrayIndex)
        {
            _queue.CopyTo(array, arrayIndex);
        }

        public IEnumerator<EventData> GetEnumerator() => _queue.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}