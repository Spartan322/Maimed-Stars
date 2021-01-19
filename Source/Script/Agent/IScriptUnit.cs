using MSG.Game.Unit;

namespace MSG.Script.Agent
{
    public interface IScriptUnit
    {
        IUnit Unit { get; }
    }

    public interface IScriptUnit<out UnitT> : IScriptUnit
        where UnitT : IUnit
    {
        new UnitT Unit { get; }
    }
}