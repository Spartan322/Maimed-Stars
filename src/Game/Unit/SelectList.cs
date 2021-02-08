using MSG.Script.Unit;

namespace MSG.Game.Unit
{
    public sealed class SelectList : GameUnit.InternalSelectList
    {
        public SelectList() : base() { }
        public SelectList(int capacity) : base(capacity) { }
        public SelectList(GameUnit.InternalSelectList list) : base(list.Count)
        {
            _listImplementation.AddRange(list);
        }
    }
}