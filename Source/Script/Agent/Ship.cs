using System;
using Godot;
using MSG.Game;
using MSG.Game.Unit;
using MSG.Global;
using MSG.Script.Resource;
using MSG.Utility;
using SpartansLib;
using SpartansLib.Attributes;
using SpartansLib.Common;
using SpartansLib.Extensions;
using SpartansLib.Extensions.Linq;
using SpartansLib.Structure;

namespace MSG.Script.Agent
{
	public partial class Ship :
		GameUnit,
		IComparable<Ship>,
		IComparableOverlap<Ship>,
		IEquatable<Ship>
	{
		public static readonly PackedScene Scene = GD.Load<PackedScene>("res://Scenes/Ship.tscn");

		public static Ship MouseOverShip { get; set; }

		#region Nodes

		[Node] public Node2D SelectionBorder;
		[Node] public Node2D Border;
		[Node] public CollisionObject2D SelectionArea;

		#endregion

		#region Script Exports

		private int _nationId;

		[Export]
		public int NationId
		{
			get => _nationId;
			set
			{
				_nationId = value;
				if (!Engine.EditorHint)
				{
					Manager.RegisterUnit(this);
				}
			}
		}

		[Export] public float SelectionRectSize = 6;

		[Export]                                        // rr gg bb aa
		public Color SelectionColor = ColorExt.FromRGBA8(0xdc_d4_2d_0c);

		public Color UnselectedColor { get; private set; }

		public ShipClassData ClassData = new ShipClassData();

		[Export] public Godot.Resource InstanceDataResource;
		public ShipInstanceData InstanceData => (ShipInstanceData) InstanceDataResource;

		#endregion

		[Connect("input_event", "SelectionArea")]
		public void SelectionColliderInputEvent(Node viewport, InputEvent @event, int shapeIdx)
		{
			if (MouseOverShip == null || CompareOverlap(MouseOverShip) > 0)
				MouseOverShip = this;
		}

		[Connect("mouse_exited", "SelectionArea")]
		public void SelectionColliderMouseExited()
		{
			if (MouseOverShip == this)
				MouseOverShip = null;
		}

		public Node2D NodeObject => this;

		public override void _Ready()
		{
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
			if (MouseOverShip == this) MouseOverShip = null;
		}

		public int CompareTo(Ship other)
		{
			if (other == null) return 1;
			if (other == this) return 0;
			var sign = Math.Sign(other.ZIndex - ZIndex);
			return sign == 0 ? Math.Sign(other.GetIndex() - GetIndex()) : sign;
		}

		public bool Equals(Ship other)
			=> GetInstanceId() == other?.GetInstanceId();

		public int CompareOverlap(Ship other)
		{
			if (other == null) return 1;
			if (other == this) return 0;
			var sign = Math.Sign(ZIndex - other.ZIndex);
			return sign == 0 ? Math.Sign(GetIndex() - other.GetIndex()) : sign;
		}

		public override int CompareTo(GameUnit other)
		{
			if (other is Ship ship) return CompareTo(ship);
			return base.CompareTo(other);
		}

		public override int CompareOverlap(GameUnit other)
		{
			if (other == null) return 1;
			if (other == this) return 0;
			if (other is Ship ship) return CompareOverlap(ship);
			return 0;
		}

		public override void SelectUpdate(InternalUnitSelectList nextSelector)
		{
			((Polygon2D)Border).Color = nextSelector != null ? SelectionColor : UnselectedColor;
		}
	}
}
