using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Godot;
using MSG.Game.Region;
using MSG.Global;

namespace MSG.Game.Unit
{
    public delegate void OnSelectChangeDelegate(UnitAgent unit, bool isSelected);

    public delegate void OnOwnerChangeDelegate(UnitAgent unit, object newOwner);

    //internal interface IUnitAgent<T> : IUnitAgent where T : Node2D
    //   {
    //       new T Node { get; }
    //}

    //  internal interface IUnitAgent
    //  {
    //event OnSelectChangeDelegate OnSelectChange;
    //event OnOwnerChangeDelegate OnOwnerChange;

    //      Node2D Node { get; }
    //string Name { get; set; }
    //    float MaxSpeed { get; set; }
    //    Rect2 SelectRect { get; }
    //    Nation State { get; set; }
    //    UnitGroupAgent Parent { get; }
    //    bool CanSelect { get; }
    //    bool Selected { get; set; }
    //    int SelectionOffset { get; set; }
    //    float? RotateTo { get; set; }
    //    Vector2? AimTo { get; set; }
    //    bool CanMove { get; }
    //    Vector2 CurrentMove { get; set; }
    //    bool QueueMove(Vector2 m, bool clear = false, float speedLimit = -1);
    //    void ClearMoves();
    //}

    public abstract class UnitAgent<T> : UnitAgent //, IUnitAgent<T>
        where T : Node2D
    {
        protected UnitAgent(T node, UnitGroupAgent parent = null) : base(node, parent)
        {
            Node = node;
        }

        public new T Node { get; }
    }

    public abstract class UnitAgent : /*IUnitAgent,*/ IComparable<UnitAgent>, IComparable
    {
        internal static List<UnitAgent> allUnits = new List<UnitAgent>();

        public static ReadOnlyCollection<UnitAgent> AllUnits => new ReadOnlyCollection<UnitAgent>(allUnits);
        public static List<UnitAgent> TopUnits => AllUnits.Where(i => i.IsTopLevel).ToList();

        protected UnitAgent(Node2D node, UnitGroupAgent parent = null)
        {
            Node = node;
            Parent = parent;
        }

        internal virtual void Create()
        {
            allUnits.Add(this);
            allUnits.Sort();
        }

        internal virtual void Destroy()
        {
            Parent?.Remove(this);
            allUnits.Remove(this);
            allUnits.Sort();
        }

        public Node2D Node { get; }
        public abstract string Name { get; set; }
        public abstract float MaxSpeed { get; set; }

        public GameNation State { get; set; }

        public UnitGroupAgent Parent { get; internal set; }

        public bool IsTopLevel => Parent == null;

        public virtual Rect2 SelectRect => new Rect2(Node.Position, Vector2.Zero);

        public void PhysicsProcess(float delta)
        {
            OnPhysicsProcess(delta);
            OnMoveProcess(delta);
        }

        protected virtual void OnPhysicsProcess(float delta)
        {
        }

        public virtual void OnSelected(bool selected)
        {
            OnSelectChange?.Invoke(this, selected);
        }

        public virtual void OnOwnerChanged(object newOwner)
        {
            OnOwnerChange?.Invoke(this, newOwner);
        }

        protected virtual void OnSetSelectionOffset(int offset)
        {
        }

        public virtual bool CanSelect
            => (State?.IsActive).GetValueOrDefault() && (State?.HasPlayer).GetValueOrDefault();

        public bool Selected
        {
            get => SelectionOffset > -1;
            set => SelectionOffset = value ? 0 : -1;
        }

        private int selectionOffset = -1;

        public event OnSelectChangeDelegate OnSelectChange;
        public event OnOwnerChangeDelegate OnOwnerChange;

        public int SelectionOffset
        {
            get => selectionOffset;
            set
            {
                if (value == selectionOffset) return;
                selectionOffset = value;
                if (Selected) SelectionHandler.Singleton.Add(this);
                else SelectionHandler.Singleton.Remove(this);
                OnSelected(Selected);
                OnSetSelectionOffset(selectionOffset);
            }
        }

        public bool Select(bool deselectAll = false, int offset = 0) =>
            SelectionHandler.Select(this, deselectAll, offset);

        public bool Deselect() => SelectionHandler.Deselect(this);
        public void MarkSelect() => SelectionHandler.MarkSelect(this);

        public virtual float? RotateTo { get; set; }
        public virtual Vector2? AimTo { get; set; }

        public virtual bool CanMove => true;
        public virtual bool QueueMove(Vector2 m, bool addGoals = false, float speedLimit = -1) => false;

        protected virtual void OnMoveProcess(float delta)
        {
        }

        public virtual Vector2 DequeueMove() => new Vector2();

        public Vector2 CurrentMove
        {
            get => new Vector2();
            set => throw new NotSupportedException();
        }

        public virtual void ClearMoves()
        {
        }

        public int CompareTo(UnitAgent other)
        {
            if (other == null) return 1;
            if (other == this) return 0;
            var sign = -Math.Sign(Node.ZIndex - other.Node.ZIndex);
            if (sign == 0)
                return Math.Sign(other.Node.GetIndex() - Node.GetIndex());
            return sign;
        }

        public int CompareTo(object obj)
        {
            if (obj is UnitAgent unit)
                return CompareTo(unit);
            return -1;
        }

        public int CompareUnitPriority(UnitAgent other) => -CompareTo(other);
    }
}