using Godot;
using Godot.Collections;
using MSG.Game.Rts.Unit;
using MSG.Script.Game.Unit;
using MSG.Global;
using SpartansLib.Common;
using SpartansLib.Attributes;
using System.Linq;

namespace MSG.Script.Gui.Game.Select
{
    [Global]
    public class ControlGroupButtonList : HBoxContainer
    {
        [Export]
        public float CreateGroupWaitTimeSeconds = 0.3f;

        internal BaseButton[] ButtonList;

        private Timer _controlGroupTimer = new Timer { Autostart = false, OneShot = true };
        private int _controlGroupNum = -1;
        private bool _pressed;

        [RetrieveNode]
        private SelectionDisplay _selectionDisplay;

        public override void _Ready()
        {
            AddChild(_controlGroupTimer);
            _controlGroupTimer.Connect("timeout", this, nameof(OnGroupTimerComplete));

            ButtonList = new BaseButton[GameData.controlGroups.Length];
            for (var i = 0; i < ButtonList.Length; i++)
            {
                ButtonList[i] = new Button { Text = i.ToString(), Visible = false };
                AddChild(ButtonList[i]);
                var indexArr = new Array(new[] { i });
                ButtonList[i].Connect("pressed", this, nameof(ControlGroupButtonPressed), indexArr);
                ButtonList[i].Connect("gui_input", this, nameof(ControlGroupGuiInput), indexArr);
                GameData.AddControlGroupChangeEvent(i, (g, num) => ButtonList[num].Visible = g != null);
            }

            ButtonList[0].MoveInParent(-1);
            MoveChild(ButtonList[0], GetChildCount());
        }

        public void ControlGroupButtonPressed(int groupNum)
        {
            GetFocusOwner()?.ReleaseFocus();
            GameData.TrySelectControlGroup(groupNum);
        }

        public void ControlGroupGuiInput(InputEvent @event, int groupNum)
        {
            if (ButtonList[groupNum].Disabled) return;
            if (@event is InputEventMouseButton mouseButton
                && (1 << (mouseButton.ButtonIndex - 1) & (int)Godot.ButtonList.MaskRight) > 0)
                GameData.SetControlGroup(groupNum, null);
        }

        public override void _UnhandledInput(InputEvent e)
        {
            var ctrlGroupInfo = GetPressedControlGroup(e);
            if (ctrlGroupInfo == default) return;

            if (e.IsActionReleased(ctrlGroupInfo.Name))
            {
                GameData.TrySelectControlGroup(ctrlGroupInfo.Index);
                StopTimer();
                return;
            }

            _controlGroupNum = ctrlGroupInfo.Index;
            _controlGroupTimer.Start(CreateGroupWaitTimeSeconds);
        }

        private void OnGroupTimerComplete()
        {
            if (_selectionDisplay.FocusUnit is GroupUnit group
                && _selectionDisplay.SelectList.All(u => group.Contains(u)))
            {
                GameData.SetControlGroup(_controlGroupNum, new SelectList() { group });
            }
            else
                GameData.SetControlGroup(_controlGroupNum,
                    new SelectList(_selectionDisplay.SelectList));
            StopTimer();
        }

        private void StopTimer()
        {
            _controlGroupNum = -1;
            _controlGroupTimer.Stop();
        }

        public static string GetControlGroupKeyName(int index)
        {
            var name = $"game_control_group_{index}_key";
            return InputMap.HasAction(name) ? name : null;
        }

        public (string Name, int Index) GetPressedControlGroup(InputEvent e)
        {
            for (var i = 0; i < ControlGroupMaxCount; i++)
            {
                var groupName = $"game_control_group_{i}_key";
                if (e.IsAction(groupName))
                    return (groupName, i);
            }
            return default;
        }

        private int _controlGroupMaxCount = -1;

        public int ControlGroupMaxCount
        {
            get
            {
                if (_controlGroupMaxCount > -1) return _controlGroupMaxCount;
                var i = 0;
                for (; GetControlGroupKeyName(i) != null; i++) ;
                _controlGroupMaxCount = i;
                return i;
            }
        }
    }
}
