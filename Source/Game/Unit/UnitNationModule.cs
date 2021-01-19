namespace MSG.Game.Unit
{
    public class UnitNationModule : UnitBaseModule
    {
        public UnitManager Manager { get; internal set; }
        public GameNation Nation => Manager.Nation;

        public UnitNationModule(IUnitController unitController) : base(unitController) {}

        public override ClassT GetClass<ClassT>(string name)
        {
            if(name == "Nation") return (ClassT)(object)Nation;
            return base.GetClass<ClassT>(name);
        }

        internal void OnPreRegister(UnitManager nextUnitManager)
        {
        }
    }
}