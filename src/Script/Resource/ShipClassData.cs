using Godot;
using SpartansLib.Structure;

namespace MSG.Script.Resource
{
    public class ShipClassData : Godot.Resource
    {
        [Export] public string ClassName = "";

        [Export] public float MaxSpeed = 100;
        [Export] public float MaxAcceleration = 50;
        [Export] public float MaxAngularSpeedDegree = 180;
        [Export] public float Mass = 1;
        [Export] public float BrakingRadius = 50;
        [Export] public float GoalThreshold = 0.9f;
        [Export] public float AngularStartThresholdDegree = 7;

        public Angle MaxAngularSpeed => new Angle(MaxAngularSpeedDegree);
        public Angle AngularStartThreshold => new Angle(AngularStartThresholdDegree);

        public ShipClassData()
        {
        }
    }
}