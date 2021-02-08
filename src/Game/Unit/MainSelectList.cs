using System.Collections.Generic;
using System.Linq;
using MSG.Script.Unit;

namespace MSG.Game.Unit
{
    public sealed class MainSelectList : GameUnit.InternalSelectList
    {
        public MainSelectList() : base() { }
        public MainSelectList(int capacity) : base(capacity) { }

        protected override bool IsMainSelector => true;

        public new void Add(GameUnit item) { }
        public new void AddRange(ICollection<GameUnit> items) { }
        public new void AddRange(IReadOnlyCollection<GameUnit> items) { }
        public new void AddRange(IEnumerable<GameUnit> items) { }
        public new bool Remove(GameUnit item) { return false; }
        public new void RemoveAt(int index) { }
        public new void Clear() { }

        internal bool AddInternal(GameUnit item)
        {
            var newAdd = !Contains(item);
            base.Add(item);
            return newAdd;
        }
        internal IList<GameUnit> AddRangeInternal(ICollection<GameUnit> items)
        {
            IList<GameUnit> difference = items.Except(this).ToArray();
            base.AddRange(difference);
            return difference;
        }
        internal bool RemoveInternal(GameUnit item) { return base.Remove(item); }
        internal void ClearInternal() { base.Clear(); }
    }
}