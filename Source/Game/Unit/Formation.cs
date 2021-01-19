using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using SpartansLib.Extensions;
using SpartansLib.Structure;

namespace MSG.Game.Unit
{
    public abstract class FormationBase
    {
        public enum FormState
        {
            Broken,
            Forming,
            Formed
        }

        public FormState State { get; internal set; }
        public UnitGroupAgent Applicator { get; internal set; }

        /// <summary>
        /// Gets the Size of the Formation or -1 if Size is dynamic.
        /// </summary>
        /// <value>The Size of the Formation. Returns -1 if Size is dynamic</value>
        public abstract int Size { get; }

        public abstract Offset GetLocalFormationFor(int index);

        public virtual bool QueueFormationMove(Offset goal, IReadOnlyList<UnitAgent> selectables, bool addGoals,
            float speedLimit)
        {
            float cos = goal.Rotation.Cos(), sin = goal.Rotation.Sin();
            Offset loc;
            State = FormState.Forming;
            UnitAgent element;
            bool result = false;
            for (var i = 0; i < selectables.Count; i++)
            {
                loc = GetLocalFormationFor(i);
                element = selectables[i];
                result |= element.QueueMove(
                    new Vector2(
                        cos * loc.Position.x - sin * loc.Position.y,
                        sin * loc.Position.x + cos * loc.Position.y
                    ) + goal.Position, addGoals, speedLimit);
                element.RotateTo = loc.Rotation - goal.Rotation;
            }

            return result;
        }
    }

    public class FormationStatic : FormationBase, IList<Offset>
    {
        public virtual IList<Offset> FormationOffsets { get; set; } = new List<Offset>();

        public void Add(Offset transform)
            => FormationOffsets.Add(transform);

        public bool Remove(Offset transform)
            => FormationOffsets.Remove(transform);

        public void RemoveAt(int index)
            => FormationOffsets.RemoveAt(index);

        public override int Size => Count;
        public int Count => FormationOffsets.Count;
        public bool IsReadOnly => false;

        public Offset this[int index]
        {
            get => FormationOffsets[index];
            set => FormationOffsets[index] = value;
        }

        public override Offset GetLocalFormationFor(int index)
            => FormationOffsets.ElementAtOrDefault(index);

        public int IndexOf(Offset item) => FormationOffsets.IndexOf(item);
        public void Insert(int index, Offset item) => FormationOffsets.Insert(index, item);
        public void Clear() => FormationOffsets.Clear();
        public bool Contains(Offset item) => FormationOffsets.Contains(item);
        public void CopyTo(Offset[] array, int arrayIndex) => FormationOffsets.CopyTo(array, arrayIndex);
        public IEnumerator<Offset> GetEnumerator() => FormationOffsets.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class FormationResource : Resource
    {
        [Export] public Array<Offset> Offsets;

        public Offset this[int index] => Offsets[index];

        public FormationResource(ICollection<Offset> transformations = null)
        {
            if (transformations != null)
                Offsets = transformations.ToGodotArray();
            else Offsets = new Array<Offset>();
        }
    }

    public class FormationFromResource : FormationStatic
    {
        public FormationResource Resource { get; set; }

        public override IList<Offset> FormationOffsets
        {
            get => Resource.Offsets;
            set => Resource.Offsets = value.ToGodotArray();
        }
    }

    public class FormationDynamic : FormationBase
    {
        public delegate Offset FormationLocationCallback(FormationDynamic formation, int index);

        public FormationLocationCallback Callback { get; set; }

        public override int Size => -1;

        public override Offset GetLocalFormationFor(int index)
            => Callback(this, index);
    }
}