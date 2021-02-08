using System.Collections;
using System.Collections.Generic;
using Godot;
using MSG.Game.Unit;
using MSG.Script.Unit;
using MSG.Script.World;

namespace MSG.Game
{
    public class UnitManager : IReadOnlyList<GameUnit>
    {
        private readonly List<GameUnit> _units = new List<GameUnit>();

        public readonly GameNation Nation;

        public UnitManager(GameNation gameNation)
        {
            Nation = gameNation;
        }

        public void RegisterUnit(GameUnit unit)
        {
            _DeregisterUnit(unit, this);
            _units.Add(unit);
        }

        public void DeregisterUnit(GameUnit unit)
        {
            if (unit is IList<GameUnit> group)
                group.Clear();
            unit.Group?.Remove(unit);
            unit._selector?.Remove(unit);
            _DeregisterUnit(unit, null);
        }

        private static void _DeregisterUnit(GameUnit unit, UnitManager nextManager)
        {
            unit.Manager?._units.Remove(unit);
            unit.Manager = nextManager;
        }

        public IEnumerator<GameUnit> GetEnumerator() => _units.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public int Count => _units.Count;
        public GameUnit this[int index] => _units[index];
    }
}
