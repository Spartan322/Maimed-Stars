using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MSG.Script.Unit;

namespace MSG.Game.Unit
{
    public sealed class MainSelectList : GameUnit.InternalSelectList
    {
        public delegate void FocusUnitChangedAction(MainSelectList list, GameUnit unit);
        public event FocusUnitChangedAction OnFocusUnitChanged;

        /// <summary>
        /// Fires before the list of selected units is to be changed in some manner,
        /// all parameters are null if the list is being cleared.
        /// </summary>
        /// <param name="list">The SelectList being changed</param>
        /// <param name="addSingle">Non-null if a singular unit is being added, null otherwise</param>
        /// <param name="addMulti">Non-null if a range of units are being added, null otherwise</param>
        /// <param name="subSingle">Non-null if a singular unit is being removed, null otherwise</param>
        public delegate void SelectListChangedAction(MainSelectList list, GameUnit addSingle, GameUnit[] addMulti, GameUnit subSingle);
        public event SelectListChangedAction OnSelectListChanged;

        /// <summary>
        /// Fires after the list of selected units has been changed in some manner,
        /// all parameters are null if the list is being cleared.
        /// </summary>
        /// <param name="list">The SelectList being changed</param>
        /// <param name="addSingle">Non-null if a singular unit is being added, null otherwise</param>
        /// <param name="addMulti">Non-null if a range of units are being added, null otherwise</param>
        /// <param name="subSingle">Non-null if a singular unit is being removed, null otherwise</param>
        public delegate void SelectListPostChangedAction(MainSelectList list, GameUnit addSingle, GameUnit[] addMulti, GameUnit subSingle);
        public event SelectListChangedAction OnSelectListPostChanged;

        private List<GameUnit.NameChangeAction> _focusUnitNameChangeCache = new List<GameUnit.NameChangeAction>();
        public event GameUnit.NameChangeAction OnFocusUnitNameChange
        {
            add
            {
                _focusUnitNameChangeCache.Add(value);
                if (FocusUnit != null)
                    FocusUnit.OnNameChange += value;
            }
            remove
            {
                _focusUnitNameChangeCache.Remove(value);
                if (FocusUnit != null)
                    FocusUnit.OnNameChange -= value;
            }
        }

        private GameUnit _focusUnit;
        public GameUnit FocusUnit
        {
            get => _focusUnit;
            set
            {
                if (object.ReferenceEquals(FocusUnit, value)) return;
                if (FocusUnit != null && _focusUnitNameChangeCache.Count > 0)
                {
                    foreach (var cachedAction in _focusUnitNameChangeCache)
                        FocusUnit.OnNameChange -= cachedAction;
                }
                OnFocusUnitChanged?.Invoke(this, value);
                _focusUnit = value;
                if (FocusUnit is IEnumerable<GameUnit> range)
                    AddRange(range);
                if (FocusUnit == null) return;
                if (_focusUnitNameChangeCache.Count > 0)
                {
                    foreach (var cachedAction in _focusUnitNameChangeCache)
                        FocusUnit.OnNameChange += cachedAction;
                }
            }
        }

        public MainSelectList() : base() { }
        public MainSelectList(int capacity) : base(capacity) { }

        protected override bool IsMainSelector => true;

        private bool _ignoreFocusUnitInAdd;
        public override void Add(GameUnit item)
        {
            if (FocusUnit == item || Contains(item)) return;
            if (Count == 0)
            {
                if (!_ignoreFocusUnitInAdd)
                {
                    FocusUnit = item;
                    if (FocusUnit is IEnumerable<GameUnit> group)
                        return;
                }
            }
            else if (FocusUnit != null && Contains(FocusUnit))
            {
                FocusUnit = null;
            }
            OnSelectListChanged?.Invoke(this, item, null, null);
            base.Add(item);
            OnSelectListPostChanged?.Invoke(this, item, null, null);
        }

        public override void AddRange(IEnumerable<GameUnit> items)
        {
            if (FocusUnit is IEnumerable<GameUnit>)
                _ignoreFocusUnitInAdd = true;
            else if (Count == 1)
            {
                FocusUnit = null;
            }
            GameUnit[] diff;
            if (Count > 0) diff = items.Except(this).ToArray();
            else diff = items.ToArray();
            if (diff.Length == 0)
            {
                _ignoreFocusUnitInAdd = false;
                return;
            }
            OnSelectListChanged?.Invoke(this, null, diff, null);
            _ignoreFocusUnitInAdd = _ignoreFocusUnitInAdd || Count + diff.Length > 1;
            base._AddRange(diff, diff.Length);
            _ignoreFocusUnitInAdd = false;
            OnSelectListPostChanged?.Invoke(this, null, diff, null);
        }

        protected override bool _Remove(GameUnit item, GameUnit.InternalSelectList nextSelector)
        {
            if (!Contains(item)) return false;
            OnSelectListChanged?.Invoke(this, null, null, item);
            var rem = base._Remove(item, nextSelector);
            OnSelectListPostChanged?.Invoke(this, null, null, item);
            return rem;
        }

        public override void RemoveAt(int index)
        {
            if (index < 0 || index > Count) throw new IndexOutOfRangeException();
            GameUnit removed = this[index];
            OnSelectListChanged?.Invoke(this, null, null, removed);
            base.RemoveAt(index);
            OnSelectListPostChanged?.Invoke(this, null, null, removed);
        }

        public override void Clear()
        {
            OnSelectListChanged?.Invoke(this, null, null, null);
            base.Clear();
            FocusUnit = null;
            OnSelectListPostChanged?.Invoke(this, null, null, null);
        }
    }
}