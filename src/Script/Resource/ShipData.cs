using Godot;
using SpartansLib.Structure;

namespace MSG.Script.Resource
{
    public class ShipData : Godot.Resource
    {
        [Export] public string Name = "";
        [Export] public float MaxSpeed = 100;
        [Export] public float MaxAcceleration = 50;
        [Export] public float MaxAngularSpeedDegree = 180;
        [Export] public float Mass = 1;
        [Export] public float BrakingRadius = 50;
        [Export] public float GoalThreshold = 0.9f;
        [Export] public float AngularStartThresholdDegree = 7;

        public Angle MaxAngularSpeed => new Angle(MaxAngularSpeedDegree);
        public Angle AngularStartThreshold => new Angle(AngularStartThresholdDegree);

        public ShipData()
        {
        }

        public ShipData(string name,
            float maxSpeed = 100,
            float maxAccel = 50,
            float maxAngularSpeed = 180,
            float mass = 1,
            float brakingRadius = 50,
            float goalReached = 0.9f,
            float rotationAim = 7)
        {
            Name = name;
            MaxSpeed = maxSpeed;
            MaxAcceleration = maxAccel;
            MaxAngularSpeedDegree = maxAngularSpeed;
            Mass = mass;
            BrakingRadius = brakingRadius;
            GoalThreshold = goalReached;
            AngularStartThresholdDegree = rotationAim;
        }

        public bool GetData<T>(string name, out T data)
        {
            bool success = false;
            data = default;
            switch (name)
            {
                case nameof(Name):
                    if (Name is T t)
                    {
                        data = t;
                        success = true;
                    }

                    break;
                case nameof(MaxSpeed):
                    if (MaxSpeed is T tms)
                    {
                        data = tms;
                        success = true;
                    }

                    break;
                case nameof(MaxAcceleration):
                    if (MaxAcceleration is T tma)
                    {
                        data = tma;
                        success = true;
                    }

                    break;
                case nameof(MaxAngularSpeedDegree):
                    if (MaxAngularSpeedDegree is T tmasd)
                    {
                        data = tmasd;
                        success = true;
                    }

                    break;
                case nameof(Mass):
                    if (Mass is T tm)
                    {
                        data = tm;
                        success = true;
                    }

                    break;
                case nameof(BrakingRadius):
                    if (BrakingRadius is T tbr)
                    {
                        data = tbr;
                        success = true;
                    }

                    break;
                case nameof(GoalThreshold):
                    if (GoalThreshold is T tgrt)
                    {
                        data = tgrt;
                        success = true;
                    }

                    break;
                case nameof(AngularStartThresholdDegree):
                    if (AngularStartThresholdDegree is T tratd)
                    {
                        data = tratd;
                        success = true;
                    }

                    break;
            }

            return success;
        }

        public bool SetData<T>(string name, T data)
        {
            switch (name)
            {
                case nameof(Name):
                    if (data is string n)
                    {
                        Name = n;
                        return true;
                    }

                    break;
                default:
                    if (data is float f)
                        switch (name)
                        {
                            case nameof(MaxSpeed):
                                MaxSpeed = f;
                                return true;
                            case nameof(MaxAcceleration):
                                MaxAcceleration = f;
                                return true;
                            case nameof(MaxAngularSpeedDegree):
                                MaxAngularSpeedDegree = f;
                                return true;
                            case nameof(Mass):
                                Mass = f;
                                return true;
                            case nameof(BrakingRadius):
                                BrakingRadius = f;
                                return true;
                            case nameof(GoalThreshold):
                                GoalThreshold = f;
                                return true;
                            case nameof(AngularStartThresholdDegree):
                                AngularStartThresholdDegree = f;
                                return true;
                        }

                    break;
            }

            return false;
        }
    }
}