using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MSG.Game.Region;
using MSG.Game.Unit.Control.Group;
using MSG.Game.Unit.Control.Select;

namespace MSG.Game.Unit
{
    public class BaseUnit<SceneT> :
        Reference,
        IUnit,
        IComparable<BaseUnit<SceneT>>,
        IEquatable<BaseUnit<SceneT>>,
        IComparableOverlap<BaseUnit<SceneT>>
        where SceneT : IComparable<SceneT>, IComparableOverlap<SceneT>, IEquatable<SceneT>
    {
        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                OnNameChange?.Invoke(this, value);
                _name = value;
            }
        }

        public GameWorld World { get; set; }

        private INation _nation;

        public INation Nation
        {
            get => _nation;
            private set
            {
                OnNationChange?.Invoke(this, value);
                _nation = value;
            }
        }

        public virtual bool CanSelect => true;
        public bool IsSelected => CanSelect && SelectionList != null;

        public float TargetSpeed { get; set; }
        public virtual float MaxSpeed { get; set; }
        public bool CanMove { get; set; }

        public bool IsMoving => CanMove && MaxSpeed != 0 && CurrentMovementTarget != null;

        public bool IsMovingIndependently { get; private set; }

        private readonly Queue<Vector2> _targets = new Queue<Vector2>();

        public Vector2? CurrentMovementTarget
        {
            get
            {
                if (!CanMove) return null;
                if (Group == null || IsMovingIndependently)
                    return _targets.Peek();
                return Group.CurrentMovementTarget; // TODO: have group assign unit formation
            }
            set
            {
                _targets.Clear();
                if (CanMove && value != null)
                    AddMovementTarget(value.Value);
            }
        }

        public bool HasSecondaryTarget
            => CanMove && _targets.Count > 1 || Group.HasSecondaryTarget;

        private IGroup _group;

        public IGroup Group
        {
            get => _group;
            private set
            {
                if (Group != null)
                {
                    Group.OnNationChange -= OnGroupNationChanged;
                    Group.OnSelectChange -= OnGroupSelectChanged;
                    Group.OnMoveTargetChange -= OnGroupMoveTargetChanged;
                }

                OnGroupChange?.Invoke(this, value);
                _group = value;
                if (Group == null) return;
                Group.OnNationChange += OnGroupNationChanged;
                Group.OnSelectChange += OnGroupSelectChanged;
                Group.OnMoveTargetChange += OnGroupMoveTargetChanged;
            }
        }

        private ISelectionList _selectionList;

        public ISelectionList SelectionList
        {
            get => _selectionList;
            set
            {
                OnSelectChange?.Invoke(this, value);
                SelectionList?.Remove(this, true);
                _selectionList = value;
            }
        }

        private Vector2? _currentRotationTarget;

        public Vector2? CurrentRotationTarget
        {
            get => _currentRotationTarget;
            set
            {
                OnRotateTargetChange?.Invoke(this, value);
                _currentRotationTarget = value;
            }
        }

        public SceneT Scene;

        public event OnSelectChangeAction OnSelectChange;
        public event OnGroupChangeAction OnGroupChange;
        public event OnMoveTargetChangeAction OnMoveTargetChange;
        public event OnNationChangeAction OnNationChange;
        public event OnNameChangeAction OnNameChange;
        public event OnRotateTargetChangeAction OnRotateTargetChange;

        public virtual void Initialize(GameDomain domain, SceneT scene)
        {
            World = domain.GameWorld;
            Scene = scene;
        }

        public virtual void Deinitialize()
        {
        }

        public int CompareTo(IUnit other)
        {
            if (other == null) return 1;
            if (other.Equals(this)) return 0;
            if (other is BaseUnit<SceneT> baseUnit)
                return CompareTo(baseUnit);
            return -Math.Sign(other.CompareTo(this));
        }

        public int CompareTo(BaseUnit<SceneT> other)
        {
            if (other == null) return 1;
            return other.Equals(this) ? 0 : Scene.CompareTo(other.Scene);
        }

        public int CompareTo(object obj)
        {
            switch (obj)
            {
                case SceneT comparable:
                    return Scene.CompareTo(comparable);
                case IUnit groupable:
                    return CompareTo(groupable);
                default:
                    return -1;
            }
        }

        public bool Equals(BaseUnit<SceneT> other)
            => other != null && Equals(other.Scene);

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            while (true)
            {
                switch (obj)
                {
                    case BaseUnit<SceneT> unit:
                        return Equals(unit);
                    case SceneT scene:
                        obj = scene;
                        continue;
                }

                return obj.Equals(this) || Scene.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual void AddMovementTarget(Vector2 vec)
        {
            if (!CanMove) return;
            if (_targets.Count == 0)
                OnMoveTargetChange?.Invoke(this, vec);
            IsMovingIndependently = true;
            _targets.Enqueue(vec);
        }

        public virtual void ClearMovementTargets()
        {
            OnMoveTargetChange?.Invoke(this, null);
            IsMovingIndependently = false;
            _targets.Clear();
        }

        public virtual Vector2 PopMovementTarget()
        {
            OnMoveTargetChange?.Invoke(this, _targets.Count > 1 ? (Vector2?) _targets.ElementAt(1) : null);
            IsMovingIndependently &= _targets.Count != 1;
            return _targets.Dequeue();
        }

        public void AddToGroup(IGroup group)
        {
            Group?.Remove(this);
            Group = group;
        }

        public void RemoveFromGroup()
        {
            Group = null;
        }

        public void AddToNation(INation nation)
        {
            Nation?.Remove(this);
            Nation = nation;
        }

        public void RemoveFromNation()
        {
            Nation = null;
        }

        public void Delete()
        {
            OnGroupChange = null;
            OnNationChange = null;
            OnSelectChange = null;
            OnMoveTargetChange = null;
            Group?.Remove(this);
            Nation?.Remove(this);
            SelectionList = null;
            RemoveFromGroup();
            _nation = null;
        }

        public int CompareOverlap(BaseUnit<SceneT> other)
        {
            if (other == null) return 1;
            return other.Equals(Scene) ? 0 : Scene.CompareOverlap(other.Scene);
        }

        private void OnGroupNationChanged(IUnit group, INation nation)
        {
            AddToNation(nation);
            nation.Add(this);
        }

        private void OnGroupSelectChanged(IUnit group, ISelectionList list)
        {
            SelectionList = list;
        }

        private void OnGroupMoveTargetChanged(IUnit group, Vector2? vector)
        {
            IsMovingIndependently = !IsSelected;
        }
    }
}