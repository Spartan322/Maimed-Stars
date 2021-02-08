using System;
using System.Collections.Generic;
using System.Linq;
using MSG.Game.Unit;
using MSG.Script.Unit;
using MSG.Script.UI.Game;
using SpartansLib;
using Godot;

namespace MSG.Global
{
    public static class GameData
    {
        static GameData()
        {
            controlGroups = new GameUnit.InternalSelectList[InputHandler.ControlGroupMaxCount];
        }

        #region Control Groups
        internal static readonly GameUnit.InternalSelectList[] controlGroups; // = new Tuple<BaseButton, IReadOnlyList<UnitAgent>>[];

        public delegate void ControlGroupChange(GameUnit.InternalSelectList agents, int groupNum);

        public static GameUnit.InternalSelectList GetControlGroup(int index)
            => controlGroups[index];

        public static IList<GameUnit.InternalSelectList> GetControlGroups()
            => controlGroups.ToList();

        private static Dictionary<int, ControlGroupChange> controlGroupChangeEvents =
            new Dictionary<int, ControlGroupChange>();

        public static void AddControlGroupChangeEvent(int num, ControlGroupChange func)
        {
            if (!controlGroupChangeEvents.ContainsKey(num)) controlGroupChangeEvents[num] = func;
            else controlGroupChangeEvents[num] += func;
        }

        public static void SetControlGroup(int num, GameUnit.InternalSelectList selectionList)
        {
            if (num < 0 || num >= controlGroups.Length)
                throw new ArgumentOutOfRangeException(nameof(num));
            if (selectionList?.Count == 0) selectionList = null;
            if (controlGroups[num] == null && selectionList == null) return;
            controlGroups[num] = selectionList;
            controlGroupChangeEvents[num]?.Invoke(controlGroups[num], num);
        }

        private static SelectionMenu _selectionMenu = NodeRegistry.Get<SelectionMenu>();
        public static void TrySelectControlGroup(int num)
        {
            var group = GetControlGroup(num);
            if (group != null)
            {
                if (!InputHandler.AddControlKeyPressed)
                    _selectionMenu.Clear();
                if (group.Count == 1 && group[0] is SelectableGroup selectedGroupUnit)
                {
                    _selectionMenu.Add(selectedGroupUnit);
                }
                else
                {
                    _selectionMenu.AddRange(group);
                }
            }
        }
        #endregion
    }
}