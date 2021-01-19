namespace MSG.Game.Unit
{
    public interface IComparableOverlap<T>
    {
        int CompareOverlap(T other);
    }
}