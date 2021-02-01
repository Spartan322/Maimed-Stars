using Godot;
using SpartansLib.Structure;

namespace MSG.Script.Unit
{
    public partial class SelectableGroup
    {
        public GameUnit SlowestUnit { get; private set; }

        public override void AddMovementTarget(Vector2 target)
        {
            // TODO: define a manner to allow units to ignore slowest unit or stay in formation as well?
            MaximumSpeedLimit = Mathf.Min(MaximumSpeedLimit, SlowestUnit.MaximumMoveSpeed);
            // TODO: generate formation?
            if (Formation != null)
                Formation.QueueFormationMove(new Offset(target, TargetAngle), this);
            else foreach(var unit in this)
            {
                if (MaximumSpeedLimit > 0)
                    unit.MaximumSpeedLimit = MaximumSpeedLimit;
                unit.MoveTo(target);
            }
        }

        public override void ClearMovementTargets()
        {
            foreach(var unit in this)
                unit.ClearMovementTargets();
        }
    }
}