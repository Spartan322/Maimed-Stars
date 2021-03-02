using System;
using System.Collections.Generic;
using System.Linq;
using SpartansLib;
using MSG.Game.Rts.Unit;
using MSG.Script.Game.Unit;
using MSG.Script.Gui.Game.Select;
using MSG.Engine;

namespace MSG.Global
{
    public static class GameData
    {
        static GameData()
        {
            controlGroups = new BaseUnit.InternalSelectList[InputHandler.ControlGroupMaxCount];
        }

        #region Control Groups
        internal static readonly BaseUnit.InternalSelectList[] controlGroups; // = new Tuple<BaseButton, IReadOnlyList<UnitAgent>>[];

        public delegate void ControlGroupChange(BaseUnit.InternalSelectList agents, int groupNum);

        public static BaseUnit.InternalSelectList GetControlGroup(int index)
            => controlGroups[index];

        public static IList<BaseUnit.InternalSelectList> GetControlGroups()
            => controlGroups.ToList();

        private static Dictionary<int, ControlGroupChange> controlGroupChangeEvents =
            new Dictionary<int, ControlGroupChange>();

        public static void AddControlGroupChangeEvent(int num, ControlGroupChange func)
        {
            if (!controlGroupChangeEvents.ContainsKey(num)) controlGroupChangeEvents[num] = func;
            else controlGroupChangeEvents[num] += func;
        }

        public static void SetControlGroup(int num, BaseUnit.InternalSelectList selectionList)
        {
            if (num < 0 || num >= controlGroups.Length)
                throw new ArgumentOutOfRangeException(nameof(num));
            if (selectionList?.Count == 0) selectionList = null;
            if (controlGroups[num] == null && selectionList == null) return;
            controlGroups[num] = selectionList;
            controlGroupChangeEvents[num]?.Invoke(controlGroups[num], num);
        }

        private static SelectionDisplay _selectionDisplay = NodeRegistry.Get<SelectionDisplay>();
        public static void TrySelectControlGroup(int num)
        {
            var group = GetControlGroup(num);
            if (group != null)
            {
                if (!InputManager.AddControlKeyPressed)
                    _selectionDisplay.Clear();
                if (group.Count == 1 && group[0] is GroupUnit selectedGroupUnit)
                {
                    _selectionDisplay.Add(selectedGroupUnit);
                }
                else
                {
                    _selectionDisplay.AddRange(group);
                }
            }
        }
        #endregion
    }
}