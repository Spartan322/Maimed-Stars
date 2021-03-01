using System.Collections;
using System.Collections.Generic;
using MSG.Script.Game.World;

namespace MSG.Game.Rts.Unit
{
    public class UnitManager : IReadOnlyList<BaseUnit>
    {
        private readonly List<BaseUnit> _units = new List<BaseUnit>();

        public readonly GameNation Nation;

        public UnitManager(GameNation gameNation)
        {
            Nation = gameNation;
        }

        public void RegisterUnit(BaseUnit unit)
        {
            _DeregisterUnit(unit, this);
            _units.Add(unit);
        }

        public void DeregisterUnit(BaseUnit unit)
        {
            if (unit is IList<BaseUnit> group)
                group.Clear();
            unit.Group?.Remove(unit);
            unit._selector?.Remove(unit);
            _DeregisterUnit(unit, null);
        }

        private static void _DeregisterUnit(BaseUnit unit, UnitManager nextManager)
        {
            unit.Manager?._units.Remove(unit);
            unit.Manager = nextManager;
        }

        public IEnumerator<BaseUnit> GetEnumerator() => _units.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public int Count => _units.Count;
        public BaseUnit this[int index] => _units[index];
    }
}
