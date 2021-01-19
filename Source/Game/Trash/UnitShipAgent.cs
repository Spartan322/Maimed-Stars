using System.Collections.Generic;
using System.Collections.ObjectModel;
using Godot;
using MSG.Script.Agent;
using MSG.Utility;

namespace MSG.Game.Unit
{
    public class UnitShipAgent : UnitAgent<Ship>
    {
        private readonly Queue<Vector2> moveGoals = new Queue<Vector2>();

        public Vector2 Velocity { get; set; }

        private static readonly List<UnitShipAgent> allShipUnits = new List<UnitShipAgent>();

        public static IReadOnlyList<UnitShipAgent> AllShipUnits
            => new ReadOnlyCollection<UnitShipAgent>(allShipUnits);

        public UnitShipAgent(Ship node, UnitGroupAgent parent = null) : base(node, parent)
        {
        }

        internal override void Create()
        {
            base.Create();
            allShipUnits.Add(this);
            if (string.IsNullOrWhiteSpace(Name)) Name = Node.Name;
        }

        internal override void Destroy()
        {
            base.Destroy();
            allShipUnits.Remove(this);
        }

        public override void OnSelected(bool selected)
            => Node.SetSelectedDisplay(selected);

        protected override void OnSetSelectionOffset(int offset)
        {
            Node.ZIndex = offset < 0
                ? 0
                : AllShipUnits.Count * 2 - offset;
        }

        private float speedLimit = -1;

        public float SpeedLimit
        {
            get
            {
                if (speedLimit < 0) speedLimit = MaxSpeed;
                return speedLimit;
            }
            set => speedLimit = Mathf.Clamp(value, 0, MaxSpeed);
        }

        public override Rect2 SelectRect => new Rect2(
            Node.Position - Vector2.One * (Node.SelectionRectSize / 2),
            Vector2.One * Node.SelectionRectSize
        );

        public override float? RotateTo
        {
            get
            {
                if (AimTo != null) AimTo.Value.AngleToPoint(Node.Position);
                if (base.RotateTo != null) return base.RotateTo;
                return null;
            }
            set
            {
                if (base.RotateTo == null) AimTo = null;
                base.RotateTo = value;
            }
        }

        public override float MaxSpeed
        {
            get => Node.ClassData.MaxSpeed;
            set => Node.ClassData.MaxSpeed = value;
        }

        public override string Name
        {
            get => Node.InstanceData.Name;
            set => Node.InstanceData.Name = value;
        }

        public override void ClearMoves() => moveGoals.Clear();
        public override Vector2 DequeueMove() => moveGoals.Dequeue();

        public override bool QueueMove(Vector2 m, bool addGoals = false, float speedLimit = -1)
        {
            var oldCount = moveGoals.Count;
            if (speedLimit >= 0) SpeedLimit = speedLimit;
            if (!addGoals) moveGoals.Clear();
            moveGoals.Enqueue(m);
            return moveGoals.Count != oldCount;
        }

        protected override void OnMoveProcess(float delta)
        {
            if (moveGoals.Count == 0)
            {
                if (RotateTo != null)
                {
                    var rotation = MoveFunc.RotateLerpChecked(Node.Rotation,
                        RotateTo.Value,
                        Node.ClassData.MaxAngularSpeed,
                        Node.ClassData.AngularStartThreshold,
                        delta);
                    if (rotation != null) Node.Rotation = rotation.Value;
                }

                return;
            }

            var first = moveGoals.Peek();
            var dist = Node.Position.DistanceTo(first);
            if (moveGoals.Count > 1 && (dist < Node.ClassData.BrakingRadius)
                || dist < Node.ClassData.GoalThreshold)
            {
                DequeueMove();
                Velocity = Vector2.Zero;
                return;
            }

            (Velocity, Node.Rotation) = MoveFunc.Arrive(
                (Node.Position, Node.Rotation),
                first,
                Velocity,
                SpeedLimit,
                MaxSpeed,
                Node.ClassData.MaxAcceleration,
                Node.ClassData.Mass,
                Node.ClassData.BrakingRadius,
                Node.ClassData.MaxAngularSpeed,
                delta
            );
            Node.Position += Velocity * delta;
        }
    }
}