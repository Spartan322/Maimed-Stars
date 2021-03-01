using Godot;
using MSG.Script.Gui.Game.Event;
using SpartansLib.Extensions;

namespace MSG.Game.Rts.Event
{
    public class ChoiceData
    {
        public readonly string ChoiceId;

        /// <summary>
        /// PackedScene that will be instantiated to manage the ChoiceData
        /// </summary>
        public PackedScene ChoiceScene;

        public string Text;
        public ConditionData[] Conditions;
        public EffectData[] Effects;

        public ChoiceData(string choiceId)
        {
            ChoiceId = choiceId;
        }

        public bool EvaluateChoices(EventContext ctx, object scope)
        {
            return Conditions.EvaluateAll(ctx, scope);
        }

        public void EvaluateEffects(EventContext ctx, object scope)
        {
            Effects.EvaluateAll(ctx, scope);
        }

        public ChoiceNode CreateNode()
        {
            return ChoiceScene.Instance<ChoiceNode>();
        }
    }
}