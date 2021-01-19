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
	public class Ship :
		Node2D,
		IComparable<Ship>,
		IComparableOverlap<Ship>,
		IEquatable<Ship>,
		IUnitController
	{
		public static readonly PackedScene Scene = GD.Load<PackedScene>("res://Scenes/Ship.tscn");

		private readonly UnitModuleStorage _moduleStorage;

		public float Mass { get; }
		public float MaximumMoveSpeed { get; }
		public float MaximumAngularSpeed { get; }
		public float MaximumAcceleration { get; }
		
		public Ship()
		{
			_moduleStorage = new UnitModuleStorage(this);
		}

		void IUnitController.OnSelectChange(bool isToBeSelected)
			=> ((Polygon2D)Border).Color = isToBeSelected ? SelectionColor : UnselectedColor;

		//public UnitShipAgent Instance { get; private set; }

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
					_moduleStorage.GetClass<GameNation>("Nation").UnitManager.RegisterUnit(this);
					//Instance.State = universe.GetNation(_nationId);
					//if (Instance.State == null)
					//universe.Add(Instance);
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
			foreach(var m in _moduleStorage) m.Ready();
			UnselectedColor = Border.GetColorFor();
			//Instance = new UnitShipAgent(this);
			//Instance.Create();
		}

		public override void _Process(float delta)
		{
			foreach(var m in _moduleStorage) m.Process(delta);
		}

		public override void _PhysicsProcess(float delta)
		{
			foreach(var m in _moduleStorage) m.PhysicsProcess(delta);
			/*if (Unit.CurrentMovementTarget == null)
			{
				if (Unit.CurrentRotationTarget == null) return;
				var rotation = MoveFunc.RotateLerpChecked(Rotation,
					Unit.CurrentRotationTarget.Value.AngleToPoint(Position),
					ClassData.MaxAngularSpeed,
					ClassData.AngularStartThreshold,
					delta);
				if (rotation != null)
					Rotation = rotation.Value;

				return;
			}

			var first = Unit.CurrentMovementTarget;
			if (first != null)
			{
				var dist = Position.DistanceTo(first.Value);
				if (Unit.HasSecondaryTarget && (dist < ClassData.BrakingRadius)
					|| dist < ClassData.GoalThreshold)
				{
					Unit.PopMovementTarget();
					Velocity = Vector2.Zero;
					return;
				}

				var maxSpeed = Mathf.Min(Unit.MaxSpeed, ClassData.MaxSpeed);

				(Velocity, Rotation) = MoveFunc.Arrive(
					(Position, Rotation),
					first.Value,
					Velocity,
					SpeedLimit,
					maxSpeed,
					ClassData.MaxAcceleration,
					ClassData.Mass,
					ClassData.BrakingRadius,
					ClassData.MaxAngularSpeed,
					delta
				);
			}*/

			Position += _moduleStorage.GetStruct<Vector2>("Velocity") * delta;
		}

		public override void _EnterTree()
		{
			foreach(var m in _moduleStorage) m.EnterTree();
		}

		public override void _ExitTree()
		{
			foreach(var m in _moduleStorage) m.ExitTree();
			if (MouseOverShip == this) MouseOverShip = null;
			//Instance.Destroy();
		}

		public override void _Draw()
		{
			foreach(var m in _moduleStorage) m.Draw();
		}

		public override void _Input(InputEvent @event)
		{
			foreach(var m in _moduleStorage) m.Input(@event);
		}

		public override void _UnhandledInput(InputEvent @event)
		{
			foreach(var m in _moduleStorage) m.UnhandledInput(@event);
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
	}
}
