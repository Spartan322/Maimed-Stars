using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Godot;
using MSG.Global;
using MSG.Script.UI.Game;
using SpartansLib.Common;
using SpartansLib.Structure;

namespace MSG.Game.Unit
{
    public class UnitGroupAgent<T> : UnitGroupAgent //, IUnitAgent<T>
        where T : Node2D
    {
        public UnitGroupAgent(T node, UnitGroupAgent parent = null) : base(node, parent)
        {
            Node = node;
        }

        public new T Node { get; }
    }

    public abstract class UnitGroupAgent : UnitAgent, IList<UnitAgent>, IReadOnlyList<UnitAgent>
    {
        const int MIN_UNITS_FOR_GROUPING = 2;

        public delegate void GroupEvent(UnitGroupAgent agent);

        public delegate void GroupEvent<T>(UnitGroupAgent agent, T item1);

        public delegate void GroupEvent<T, T2>(UnitGroupAgent agent, T item1, T2 item2);

        public event GroupEvent<UnitAgent> OnAdd;
        public event GroupEvent<UnitAgent> OnRemove;
        public event GroupEvent<SelectionMenu, bool> OnSetAsTopSelection;
        public event GroupEvent OnUpdate;

        private static readonly List<UnitGroupAgent> allGroupUnits = new List<UnitGroupAgent>();

        public static IReadOnlyList<UnitGroupAgent> AllGroupUnits
            => new ReadOnlyCollection<UnitGroupAgent>(allGroupUnits);

        protected UnitGroupAgent(Node2D node, UnitGroupAgent parent = null) : base(node, parent)
        {
        }

        internal override void Create()
        {
            base.Create();
            allGroupUnits.Add(this);
            Formation = new FormationBasicDynamic();
        }

        internal override void Destroy()
        {
            base.Destroy();
            allGroupUnits.Remove(this);
        }

        private string name;

        public override string Name
        {
            get => name;
            set
            {
                name = value;
                OnUpdate?.Invoke(this);
            }
        }

        private float maxSpeed;

        public override float MaxSpeed
        {
            get => maxSpeed;
            set
            {
                if (value >= 0)
                    maxSpeed = value;
                else
                    foreach (var s in this)
                        if (s.MaxSpeed < MaxSpeed)
                            maxSpeed = s.MaxSpeed;
                OnUpdate?.Invoke(this);
            }
        }

        private FormationBase formation;

        public FormationBase Formation
        {
            get => formation;
            set
            {
                if (value != null) value.Applicator = this;
                else formation.Applicator = null;
                formation = value;
                OnUpdate?.Invoke(this);
            }
        }

        private readonly List<UnitAgent> unitGrouping = new List<UnitAgent>();
        protected bool NoReparent;
        protected bool NoSort;

        public UnitAgent this[int index]
        {
            get => unitGrouping[index];
            set
            {
                unitGrouping[index] = value;
                Sort();
            }
        }

        UnitAgent IReadOnlyList<UnitAgent>.this[int index] => this[index];

        public int Count => unitGrouping.Count;
        public bool IsReadOnly => false;
        public UnitAgent Commander => Count > 0 ? this[0] : null;
        public override Rect2 SelectRect => new Rect2(Node.Position, Vector2.One);

        public bool Add(UnitAgent add)
        {
            var oldCmd = Commander;
            HandleAdd(add);
            unitGrouping.Add(add);
            var success = this[Count - 1] == add;
            Sort(false);
            if (!NoReparent && oldCmd != Commander)
                ReparentToCommanderNode();
            OnUpdate?.Invoke(this);
            return success;
        }

        public void Clear()
        {
            for (int i = Count - 1; i >= 0; i--)
                HandleRemove(this[i]);
            unitGrouping.Clear();
            if (!NoReparent) ReparentToCommanderNode();
            OnUpdate?.Invoke(this);
        }

        public bool Contains(UnitAgent item)
            => unitGrouping.Contains(item);

        public void CopyTo(UnitAgent[] array, int arrayIndex)
            => unitGrouping.CopyTo(array, arrayIndex);

        public IEnumerator<UnitAgent> GetEnumerator()
            => unitGrouping.GetEnumerator();

        public int IndexOf(UnitAgent item)
            => unitGrouping.IndexOf(item);

        public void Insert(int index, UnitAgent item)
        {
            var oldCmd = Commander;
            HandleAdd(item);
            unitGrouping.Insert(index, item);
            Sort(false);
            if (!NoReparent && oldCmd != Commander)
                ReparentToCommanderNode();
            OnUpdate?.Invoke(this);
        }

        public bool Remove(UnitAgent item)
        {
            var oldCmd = Commander;
            var success = unitGrouping.Remove(item);
            HandleRemove(item);
            Sort(false);
            if (!NoReparent && oldCmd != Commander)
                ReparentToCommanderNode();
            OnUpdate?.Invoke(this);
            return success;
        }

        public void RemoveAt(int index)
        {
            var oldCmd = Commander;
            var oldUnit = this[index];
            unitGrouping.RemoveAt(index);
            HandleRemove(oldUnit);
            Sort(false);
            if (!NoReparent && oldCmd != Commander)
                ReparentToCommanderNode();
            OnUpdate?.Invoke(this);
        }

        public bool ReplaceWith(UnitAgent original, UnitAgent @new)
        {
            var oldCmd = Commander;
            var index = IndexOf(original);
            if (index == -1) return false;
            HandleAdd(@new);
            unitGrouping[index] = @new;
            original.Parent = null;
            Sort(false);
            if (oldCmd != Commander)
                ReparentToCommanderNode();
            OnUpdate?.Invoke(this);
            return unitGrouping[index] == @new;
        }

        public void Set(IEnumerable<UnitAgent> source)
        {
            NoReparent = true;
            Clear();
            NoReparent = false;
            NoSort = true;
            if (source != null)
                foreach (var ua in source)
                    Add(ua);
            NoSort = false;
            Sort();
        }

        public void ExpandSelection()
        {
            // Remove group from list without alerting SelectionHandler
            SelectionHandler.Singleton.selectedObjects.Remove(this);
            // Select group as an enumerable
            SelectionHandler.SelectMultiple(this);
        }

        public bool IsTopSelection { get; private set; }

        public void SetTopSelection(SelectionMenu menu, bool isTop)
        {
            IsTopSelection = isTop;
            if (isTop) ExpandSelection();
            else Selected = false;
            OnSetAsTopSelection?.Invoke(this, menu, isTop);
        }

        public override void OnSelected(bool selected)
        {
            GD.Print($"Group {Name} {selected}");
            // TODO: fix highlighting
            // either doubles OnSelected call and sets selected as false after deselecting group (current)
            // or removes ability to see selected agents of contained groups
            foreach (var agnt in this)
                agnt.OnSelected(selected);
            base.OnSelected(selected);
        }

        public UnitGroupAgent Sort(bool updateData = true)
        {
            if (!NoSort) unitGrouping.Sort();
            if (updateData) OnUpdate?.Invoke(this);
            return this;
        }

        public bool TryFree()
        {
            if (Count < MIN_UNITS_FOR_GROUPING)
            {
                for (int i = Count - 1; i >= 0; i--)
                    this[i].Parent = null;
                Node.GetParent().RemoveChild(Node);
                Node.QueueFree();
                return Node.IsQueuedForDeletion();
            }

            return false;
        }

        public override bool QueueMove(Vector2 m, bool addGoals = false, float speedLimit = -1)
        {
            return Formation.QueueFormationMove(new Offset(m, Node.Position),
                this,
                addGoals,
                MaxSpeed = speedLimit
            );
        }

        public override Vector2 DequeueMove()
        {
            var index = 0;
            Vector2 result = new Vector2();
            foreach (var agnt in this)
            {
                result = Commander == agnt ? agnt.DequeueMove() : result;
                index++;
            }

            return result;
        }

        public override void ClearMoves()
        {
            foreach (var agnt in this)
                agnt.ClearMoves();
        }

        void ICollection<UnitAgent>.Add(UnitAgent item) => Add(item);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void HandleAdd(UnitAgent unit)
        {
            if (unit.Parent == this) return;
            if (unit.Parent != null) unit.Parent.Remove(unit);
            OnAdd?.Invoke(this, unit);
            unit.Parent = this;
            if (unit.MaxSpeed < MaxSpeed)
                MaxSpeed = unit.MaxSpeed;
        }

        private void HandleRemove(UnitAgent unit)
        {
            if (unit.Parent != this) return;
            OnRemove?.Invoke(this, unit);
            unit.Parent = null;
            if (Mathf.IsEqualApprox(unit.MaxSpeed, MaxSpeed))
                MaxSpeed = -1;
        }

        private void ReparentToCommanderNode()
        {
            if (Commander != null)
                Node.ReparentTo(Commander.Node);
        }

        public class FormationBasicDynamic : FormationDynamic
        {
            public FormationBasicDynamic()
            {
                Callback = (f, index) => new Offset();
            }
        }
    }
}