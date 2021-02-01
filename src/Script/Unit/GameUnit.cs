using System;
using Godot;
using MSG.Game;
using MSG.Game.Unit;
using MSG.Script.World;
using SpartansLib;

namespace MSG.Script.Unit
{
    public abstract partial class GameUnit : Node2D,
        IComparable<GameUnit>,
        IComparableOverlap<GameUnit>,
        IEquatable<GameUnit>
    {
        public UnitManager Manager { get; internal set; }
        public GameNation Nation
        {
            get => Manager.Nation;
            set
            {
                if (!Engine.EditorHint)
                {
                    Manager = value?.UnitManager;
                    Manager?.RegisterUnit(this);
                }
            }
        }
        public SelectableGroup Group { get; internal set; }
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
            Manager = GetParent().GetParent<GameNation>().UnitManager;
            if (UnitName == null)
                _unitName = Name;
        }

        public virtual void OnUnitDeleted() { }

        public virtual int CompareTo(GameUnit other)
        {
            if (other == null) return 1;
            if (other == this) return 0;
            var sign = other.ZIndex - ZIndex;
            return -(sign == 0 ? other.GetIndex() - GetIndex() : sign);
        }

        public virtual bool Equals(GameUnit other)
            => GetInstanceId() == other?.GetInstanceId();

        public virtual int CompareOverlap(GameUnit other)
            => CompareTo(other);

        public delegate void NameChangeAction(GameUnit unit, string newName);
        public event NameChangeAction OnNameChange;
    }
}
