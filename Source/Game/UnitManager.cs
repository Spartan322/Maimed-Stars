using System.Collections;
using System.Collections.Generic;
using MSG.Game.Unit;
using MSG.Script.Agent;

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
            unit.Manager._DeregisterUnit(unit, this);
            _units.Add(unit);
        }

        public void DeregisterUnit(GameUnit unit)
        {
            if(unit.Group != null)
            {
                // TODO: remove from group
            }
            _DeregisterUnit(unit, null);
        }

        private void _DeregisterUnit(GameUnit unit, UnitManager nextManager)
        {
            _units.Remove(unit);
            unit.Manager = nextManager;
        }

        public IEnumerator<GameUnit> GetEnumerator() => _units.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public int Count => _units.Count;
        public GameUnit this[int index] => _units[index];
    }
}