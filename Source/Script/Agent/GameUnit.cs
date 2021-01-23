using System;
using Godot;
using MSG.Game;
using MSG.Game.Unit;

namespace MSG.Script.Agent
{
    public abstract partial class GameUnit : Node2D,
        IComparable<GameUnit>,
		IComparableOverlap<GameUnit>,
		IEquatable<GameUnit>
    {
        public UnitManager Manager { get; internal set; }
        public GameNation Nation => Manager.Nation;
        public SelectableGroup Group { get; internal set; }
        public Vector2? AimTarget;
        public float TargetAngle;
		public virtual bool CanAim { get; set; }

        public float Mass { get; }
		public float MaximumMoveSpeed { get; }
		public float MaximumAngularSpeed { get; }
		public float MaximumAcceleration { get; }

		public virtual int CompareTo(GameUnit other)
		{
			if (other == null) return 1;
			if (other == this) return 0;
			var sign = Math.Sign(other.ZIndex - ZIndex);
			return sign == 0 ? Math.Sign(other.GetIndex() - GetIndex()) : sign;
		}

		public virtual bool Equals(GameUnit other)
			=> GetInstanceId() == other?.GetInstanceId();

		public virtual int CompareOverlap(GameUnit other)
		{
			if (other == null) return 1;
			if (other == this) return 0;
			var sign = Math.Sign(ZIndex - other.ZIndex);
			return sign == 0 ? Math.Sign(GetIndex() - other.GetIndex()) : sign;
		}
    }
}