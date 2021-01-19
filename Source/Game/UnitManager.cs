using System.Collections;
using System.Collections.Generic;
using MSG.Game.Unit;

namespace MSG.Game
{
    public class UnitManager : IReadOnlyList<UnitNationModule>
    {
        private readonly List<UnitNationModule> _units = new List<UnitNationModule>();

        public readonly GameNation Nation;

        public UnitManager(GameNation gameNation)
        {
            Nation = gameNation;
        }

        public void RegisterUnit(IUnitController unitController)
        {
            var module = unitController.GetModule<UnitNationModule>(ModuleIndex.Nation);
            module.Manager._DeregisterUnit(unitController, this);
            _units.Add(module);
        }

        public void DeregisterUnit(IUnitController unitController) => _DeregisterUnit(unitController, null);

        private void _DeregisterUnit(IUnitController unitController, UnitManager nextManager)
        {
            var module = unitController.GetModule<UnitNationModule>(ModuleIndex.Nation);
            module.OnPreRegister(nextManager);
            _units.Remove(module);
            module.Manager = nextManager;
        }

        public IEnumerator<UnitNationModule> GetEnumerator() => _units.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public int Count => _units.Count;
        public UnitNationModule this[int index] => _units[index];
    }
}