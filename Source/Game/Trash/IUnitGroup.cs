using System.Collections.Generic;
using MSG.Global;
using MSG.Script.UI.Game;

namespace MSG.Game.Unit
{
    public interface IUnitGroup : IList<UnitAgent>, IReadOnlyList<UnitAgent>
    {
        //void OnAddToMenu(SelectionMenu menu);
        //void OnRemoveFromMenu(SelectionMenu menu);
        UnitAgent Commander { get; }
        void SetTopSelection(SelectionMenu menu, bool isTop);
        new bool Add(UnitAgent add);
        bool ReplaceWith(UnitAgent original, UnitAgent @new);
        void Set(IEnumerable<UnitAgent> source);
        bool TryFree();
        new int Count { get; }
        IUnitGroup Sort();
    }

    public static class UnitGroupExtensions
    {
        public static void ExpandSelection(this IUnitGroup group)
        {
            // Remove group from list without alerting SelectionHandler
            if (group is UnitAgent unit)
                SelectionHandler.Singleton.selectedObjects.Remove(unit);
            // Select group as an enumerable
            SelectionHandler.SelectMultiple(group);
        }
    }
}