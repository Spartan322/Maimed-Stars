using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MSG.Game.Unit;

namespace MSG.Global
{
    public sealed class SelectionHandler : IReadOnlyList<UnitAgent>
    {
        public static SelectionHandler Singleton { get; } = new SelectionHandler();

        #region Selection Events

        /* bool = deselectAll */
        public static event Action<SelectionHandler, bool> MultiSelected;

        public static event Action<SelectionHandler> MultiDeselected;

        /* bool = deselectAll */
        public static event Action<SelectionHandler, UnitAgent, int, bool> ObjectSelected;
        public static event Action<SelectionHandler, UnitAgent, int> ObjectDeselected;
        public static event Action<SelectionHandler> SelectionEmptied;

        #endregion

        #region QoL Static Selection Methods

        public static int SelectionCount => Singleton.Count;

        /// <summary>
        /// Selects the specified unit.
        /// </summary>
        /// <returns>Whether the unit is or was successfully selected.</returns>
        /// <param name="unit">The unit to select.</param>
        /// <param name="deselectAll">If set to <c>true</c> deselects all other units.</param>
        /// <param name="offset">The negative offset to manage the unit as.</param>
        public static bool Select(UnitAgent unit, bool deselectAll = false, int offset = 0)
            => Singleton.DoSelect(unit, deselectAll, offset);

        /// <summary>
        /// Selects the units in the list.
        /// </summary>
        /// <remarks>Fires <see cref="ObjectSelected"/> and <see cref="ObjectDeselected"/>
        /// on units that are either selected or deselected only,
        /// units already selected and included in <paramref name="units"/> are ignored</remarks>
        /// <returns><c>true</c>, if even one of the units was selected, <c>false</c> otherwise.</returns>
        /// <param name="units">The units to select.</param>
        /// <param name="deselectAll">If set to <c>true</c> deselects all other units.</param>
        /// <param name="offset">The negative offset to manage the units as.</param>
        public static bool SelectMultiple(IEnumerable<UnitAgent> units, bool deselectAll = false, int offset = 0)
            => Singleton.DoSelectMultiple(units, deselectAll, offset);

        /// <summary>
        /// Deselect the specified unit.
        /// </summary>
        /// <returns>Whether the unit is or was successfully deselected.</returns>
        /// <param name="unit">The unit to deselect.</param>
        public static bool Deselect(UnitAgent unit)
            => Singleton.DoDeselect(unit);

        /// <summary>
        /// Deselects the units in the list.
        /// </summary>
        /// <returns><c>true</c>, if all listed units were deselected, <c>false</c> otherwise.</returns>
        /// <param name="units">The units to deselect.</param>
        public static bool DeselectMultiple(IEnumerable<UnitAgent> units)
            => Singleton.DoDeselectMultiple(units);

        /// <summary>
        /// Deselects all selected units excluding <paramref name="except"/>.
        /// </summary>
        /// <returns><c>true</c>, if all units aside from <paramref name="except"/> were deselected, <c>false</c> otherwise.</returns>
        /// <param name="except">The units to exclude from deselection.</param>
        public static bool DeselectAll(IEnumerable<UnitAgent> except = null)
            => Singleton.DoDeselectAll(except);

        /// <summary>
        /// Deselects all selected units excluding the on <paramref name="except"/>.
        /// </summary>
        /// <returns><c>true</c>, if all units aside from <paramref name="except"/> was deselected, <c>false</c> otherwise.</returns>
        /// <param name="except">The one unit to exclude from deselection.</param>
        public static bool DeselectAll(UnitAgent except)
            => Singleton.DoDeselectAll(except);

        public static void MarkSelect(UnitAgent s)
            => Singleton.DoMarkSelect(s);

        public static void SelectMarked(bool deselectAll = false, int offset = 0)
            => Singleton.DoSelectMarked(deselectAll, offset);

        public static void MoveSelectionTo(Vector2 point, bool addGoals = false, float speedLimit = -1)
            => Singleton.DoMoveSelectionTo(point, addGoals, speedLimit);

        public static bool DeselectingAll { get; private set; }

        public static IReadOnlyList<UnitAgent> SelectedObjects => Singleton.selectedObjects.AsReadOnly();
        public static IReadOnlyList<UnitAgent> MarkedObjects => Singleton.markedForSelect.AsReadOnly();

        #endregion

        internal readonly List<UnitAgent> selectedObjects = new List<UnitAgent>();
        internal readonly List<UnitAgent> markedForSelect = new List<UnitAgent>();

        #region IReadOnlyList<UnitAgent> Implementations

        public UnitAgent this[int index] => selectedObjects[index];
        public int Count => selectedObjects.Count;
        public IEnumerator<UnitAgent> GetEnumerator() => selectedObjects.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region Selection List Internal Handler Methods

        internal void Add(UnitAgent selectable)
        {
            if (selectable.SelectionOffset < 0) selectable.SelectionOffset = Count;
            ObjectSelected?.Invoke(this, selectable, selectable.SelectionOffset, DeselectingAll);
            selectedObjects.Add(selectable);
        }

        internal void Remove(UnitAgent selectable)
        {
            ObjectDeselected?.Invoke(this, selectable, selectable.SelectionOffset);
            selectedObjects.Remove(selectable);
            if (Count == 0) SelectionEmptied?.Invoke(this);
        }

        #endregion

        #region Singleton Selection Methods

        public bool DoSelect(UnitAgent unit, bool deselectAll = false, int offset = 0)
        {
            if (unit.CanSelect)
            {
                DeselectingAll = deselectAll;
                if (deselectAll) DoDeselectAll(unit);
                if (unit.Selected) return true;
                unit.SelectionOffset = offset + Count;
                return unit.Selected;
            }

            return false;
        }

        public bool DoSelectMultiple(IEnumerable<UnitAgent> units, bool deselectAll = false, int offset = 0)
        {
            var anyAreSelectable = false;
            foreach (var unit in units)
            {
                // Skip if unit isn't selectable
                if (!unit.CanSelect) continue;
                // If any unit can be selected and hasn't yet been selected
                if (!anyAreSelectable)
                {
                    DeselectingAll = deselectAll;
                    // If deselecting all functionality is requested, exclude requested units already selected
                    if (deselectAll) DoDeselectAll(units);
                    // Set that any unit in the list could be selected
                    anyAreSelectable = true;
                }

                if (unit.Selected) continue;
                unit.SelectionOffset = offset + Count;
            }

            if (anyAreSelectable) MultiSelected?.Invoke(this, deselectAll);
            // Return that at least a single unit submitted was selected
            return anyAreSelectable;
        }

        public bool DoDeselect(UnitAgent unit)
        {
            if (Count < 1) return false;
            unit.Selected = false;
            return !unit.Selected;
        }

        public bool DoDeselectMultiple(IEnumerable<UnitAgent> units)
        {
            if (Count < 1) return false;
            var r = true;
            foreach (var unit in units)
                r &= DoDeselect(unit);
            return r;
        }

        public bool DoDeselectAll(IEnumerable<UnitAgent> except = null)
        {
            if (Count < 1) return false;
            IEnumerable<UnitAgent> deselected = this.ToArray();
            if (except != null)
                deselected = deselected.Except(except);
            var r = DoDeselectMultiple(deselected);
            return r;
        }

        public bool DoDeselectAll(UnitAgent except) => DoDeselectAll(new UnitAgent[] {except});

        public void DoMarkSelect(UnitAgent unit) => markedForSelect.Add((UnitAgent) unit);
        public void ClearMarkSelect() => markedForSelect.Clear();

        public void DoSelectMarked(bool deselectAll = false, int offset = 0)
        {
            DoSelectMultiple(markedForSelect, deselectAll, offset);
            ClearMarkSelect();
        }

        public void DoMoveSelectionTo(Vector2 point, bool addGoals = false, float speedLimit = -1)
        {
            foreach (var unit in this)
                unit.QueueMove(point, addGoals, speedLimit);
        }

        #endregion

        internal SelectionHandler Sort()
        {
            selectedObjects.Sort();
            return this;
        }
    }
}