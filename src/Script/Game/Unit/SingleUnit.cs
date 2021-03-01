using System;
using Godot;
using MSG.Game.Rts.Unit;
using MSG.Script.Game.World;
using SpartansLib;
using SpartansLib.Attributes;
using SpartansLib.Structure;

namespace MSG.Script.Game.Unit
{
    public partial class SingleUnit :
        BaseUnit,
        IComparable<SingleUnit>,
        //IComparableOverlap<Ship>,
        IEquatable<SingleUnit>
    {
        public static readonly PackedScene Scene = GD.Load<PackedScene>("res://asset/godot-scene/Game/Unit/SingleUnit.tscn");

        public static SingleUnit MouseOver { get; set; }

        public float Mass { get; } = 1;

        #region Nodes
        [Node] public Node2D SelectionBorder;
        [Node] public Node2D Border;
        [Node] public CollisionObject2D SelectionArea;
        #endregion

        #region Exports
        [Export] public float SelectionRectSize = 6;

        [Export]                                        // rr gg bb aa
        public Color SelectionColor = ColorExt.FromRGBA8(0xff_00_00_be/*0xdc_d4_2d_0c*/);

        public Color UnselectedColor { get; private set; }
        /*
                public ShipClassData ClassData = new ShipClassData();

                [Export] public Godot.Resource InstanceDataResource;
                public ShipInstanceData InstanceData => (ShipInstanceData)InstanceDataResource; */
        #endregion

        #region Signal Callbacks
        [Connect("input_event", "SelectionArea")]
        public void SelectionColliderInputEvent(Node viewport, InputEvent @event, int shapeIdx)
        {
            if (MouseOver == null || CompareOverlap(MouseOver) > 0)
                MouseOver = this;
        }

        [Connect("mouse_exited", "SelectionArea")]
        public void SelectionColliderMouseExited()
        {
            if (MouseOver == this)
                MouseOver = null;
        }
        #endregion

        public override void _Ready()
        {
            base._Ready();
            _HandeDebugReady();
            UnselectedColor = Border.GetColorFor();
        }

        public override void _Process(float delta)
        {
            _HandleDebugProcess(delta);
        }

        public override void _PhysicsProcess(float delta)
        {
            _HandleAimPhysicsProcess(delta);
            _HandleMovementPhysicsProcess(delta);
            Position += Velocity * delta;
        }

        public override void _ExitTree()
        {
            if (MouseOver == this) MouseOver = null;
        }

        protected override GameNation FindNation()
            => GetParent().GetParent<GameNation>();

        public int CompareTo(SingleUnit other)
        {
            if (other == null) return 1;
            if (other == this) return 0;
            var sign = other.ZIndex - ZIndex;
            return -(sign == 0 ? other.GetIndex() - GetIndex() : sign);
        }

        public bool Equals(SingleUnit other)
            => GetInstanceId() == other?.GetInstanceId();

        public int CompareOverlap(SingleUnit other)
        {
            return base.CompareOverlap(other);
        }

        public override int CompareTo(BaseUnit other)
        {
            if (other is SingleUnit ship) return CompareTo(ship);
            return -1;
        }

        public override int CompareOverlap(BaseUnit other)
        {
            if (other == null) return 1;
            if (other == this) return 0;
            if (other is SingleUnit ship) return CompareOverlap(ship);
            return 0;
        }

        public override void SelectUpdate(InternalSelectList nextSelector)
        {
            ((Polygon2D)Border).Color = nextSelector != null ? SelectionColor : UnselectedColor;
        }
    }
}
