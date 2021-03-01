namespace MSG.Game.Rts.Unit
{
    public sealed class SelectList : BaseUnit.InternalSelectList
    {
        public SelectList() : base() { }
        public SelectList(int capacity) : base(capacity) { }
        public SelectList(BaseUnit.InternalSelectList list) : base(list.Count)
        {
            _listImplementation.AddRange(list);
        }
    }
}