using System.Linq;

namespace MSG.Game.Rts.Event
{
    public static class Extension
    {
        public static void EvaluateAll(this EffectData[] effects, EventContext ctx, object masterScope)
        {
            foreach (var e in effects)
                e.Evaluate(ctx, masterScope);
        }

        public static bool EvaluateAll(this ConditionData[] conditions, EventContext ctx, object masterScope)
        {
            return conditions.All(c => c.Evaluate(ctx, masterScope));
        }
    }
}