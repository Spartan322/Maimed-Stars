using Godot;
using MSG.Script.Gui.Game.Event;
using SpartansLib.Extensions;

namespace MSG.Game.Rts.Event
{
    /// <summary>
    /// Events only pertain to interactions with the players,
    /// game initialization, continious function execution
    /// and checks, or any other operations that don't
    /// require player interaction are irrelevant to the
    /// eventing system.
    /// </summary>
    public class EventData
    {
        /// <summary>
        /// Clear reference id for cheap and easy access
        /// </summary>
        public readonly string EventId;

        /// <summary>
        /// PackedScene that will be instantiated to manage the EventData
        /// </summary>
        public PackedScene EventScene; // TODO: default to standard EventNode scene

        private object _scope;

        private ConditionData[] _activateConditions;
        private DelayData _mtthData;
        private EffectData[] _prefireEffects;
        private ChoiceData[] _choices;
        private EffectData[] _postfireEffects;
        private string _title;
        private string _description;
        private Texture _image;

        public EventData(string eventId, object scope)
        {
            EventId = eventId;
            _scope = scope;
        }

        public EventData(string eventId, object scope, string loadFile) : this(eventId, scope)
        {
            LoadFrom(loadFile);
        }

        public void LoadFrom(string file)
        {
            // TODO: create a human-readable form for events, load them here
        }

        public void Evaluate(EventContext ctx)
        {
            if (ctx.PassedPreEvaluation(this) || (_activateConditions.EvaluateAll(ctx, _scope) && _mtthData.ShouldFire(ctx, _scope)))
            {
                _prefireEffects.EvaluateAll(ctx, _scope);
                // TODO: evaluate viable ChoiceData and save them
            }
        }

        public void SelectChoice(EventContext ctx, int index)
        {
            _choices[index].EvaluateEffects(ctx, _scope);
            _postfireEffects.EvaluateAll(ctx, _scope);
        }

        public EventNode CreateNode()
        {
            return EventScene.Instance<EventNode>();
        }
    }
}