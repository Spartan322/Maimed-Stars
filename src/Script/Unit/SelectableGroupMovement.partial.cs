using Godot;
using SpartansLib.Structure;

namespace MSG.Script.Unit
{
    public partial class SelectableGroup
    {
        public GameUnit SlowestUnit { get; private set; }

        public void AddMovementTarget(Vector2 target, bool ignoreSlowest)
        {
            if (!ignoreSlowest)
                MaximumSpeedLimit = Mathf.Min(MaximumSpeedLimit, SlowestUnit.MaximumMoveSpeed);
            if (Formation != null)
                Formation.QueueFormationMove(new Offset(target, TargetAngle), this);
            else foreach (var unit in this)
                {
                    if (MaximumSpeedLimit > 0)
                        unit.MaximumSpeedLimit = MaximumSpeedLimit;
                    unit.MoveTo(target);
                }
        }

        public override void AddMovementTarget(Vector2 target)
            => AddMovementTarget(target, false);

        public override void ClearMovementTargets()
        {
            foreach (var unit in this)
                unit.ClearMovementTargets();
        }
    }
}