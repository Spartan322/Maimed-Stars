using System;
using Godot;
using MSG.Script.Game.Unit;
using MSG.Script.Game.World;

namespace MSG.Game.Rts.Unit
{
    public abstract partial class BaseUnit : Node2D,
        IComparable<BaseUnit>,
        //IComparableOverlap<GameUnit>,
        IEquatable<BaseUnit>
    {
        public UnitManager Manager { get; internal set; }
        public GameNation Nation
        {
            get => Manager.Nation;
            set
            {
                if (Godot.Engine.EditorHint) return;
                if (value == null) GD.PushWarning($"{nameof(BaseUnit)} '{UnitName}' Nation being set to null.");
                Manager = value?.UnitManager;
                Manager?.RegisterUnit(this);
            }
        }
        public GroupUnit Group { get; internal set; }
        public Vector2? AimTarget;
        public float TargetAngle;
        public virtual bool CanProcessAim { get; set; } = true;

        private string _unitName;
        [Export]
        public string UnitName
        {
            get => _unitName;
            set
            {
                if (value == null)
                {
                    // TODO: game error: can't leave unit names blank
                    return;
                }
                OnNameChange?.Invoke(this, value);
                _unitName = value;
            }
        }
        public float MaximumMoveSpeed { get; } = 100;

        public override void _Ready()
        {
            if (UnitName == null)
                _unitName = Name;
            Nation = FindNation();
        }

        protected virtual GameNation FindNation() { return null; }

        public virtual void OnUnitDeleted() { }

        public virtual int CompareTo(BaseUnit other)
        {
            if (other == null) return 1;
            if (other == this) return 0;
            var sign = other.ZIndex - ZIndex;
            return -(sign == 0 ? other.GetIndex() - GetIndex() : sign);
        }

        public virtual bool Equals(BaseUnit other)
            => GetInstanceId() == other?.GetInstanceId();

        public virtual int CompareOverlap(BaseUnit other)
            => CompareTo(other);

        public delegate void NameChangeAction(BaseUnit unit, string newName);
        public event NameChangeAction OnNameChange;
    }
}
