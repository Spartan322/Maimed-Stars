using System;
using System.Collections.Generic;
using System.Linq;
using MSG.Game.Unit.Control.Group;
using MSG.Game.Unit.Control.Select;
using MSG.Script.UI.Game;
using SpartansLib;

namespace MSG.Global
{
    public static class GameData
    {
        static GameData()
        {
            controlGroups = new ISelectionList[InputHandler.ControlGroupMaxCount];
        }

        #region Control Groups

        internal static readonly ISelectionList[] controlGroups; // = new Tuple<BaseButton, IReadOnlyList<UnitAgent>>[];

        public delegate void ControlGroupChange(ISelectionList agents, int groupNum);

        public static ISelectionList GetControlGroup(int index)
            => controlGroups[index];

        public static IList<ISelectionList> GetControlGroups()
            => controlGroups.ToList();

        private static Dictionary<int, ControlGroupChange> controlGroupChangeEvents =
            new Dictionary<int, ControlGroupChange>();

        public static void AddControlGroupChangeEvent(int num, ControlGroupChange func)
        {
            if (!controlGroupChangeEvents.ContainsKey(num)) controlGroupChangeEvents[num] = func;
            else controlGroupChangeEvents[num] += func;
        }

        public static void SetControlGroup(int num, ISelectionList selectionList)
        {
            if (num < 0 || num >= controlGroups.Length)
                throw new ArgumentOutOfRangeException(nameof(num));
            if (selectionList?.Count == 0) selectionList = null;
            if (controlGroups[num] == null && selectionList == null) return;
            controlGroups[num] = selectionList;
            controlGroupChangeEvents[num]?.Invoke(selectionList, num);
        }

        public static void TrySelectControlGroup(int num)
        {
            var group = GetControlGroup(num);
            if (group != null)
            {
                var addControlNotPressed = !InputHandler.AddControlKeyPressed;
                var selectionMenu = NodeRegistry.Get<SelectionMenu>();
                if (group is IGroup unitGroup)
                    selectionMenu.SelectedGroup = unitGroup;
                else
                    selectionMenu.AddRange(group);
            }
        }

        #endregion
    }
}