using Godot;
using Godot.Collections;
using MSG.Global;
using SpartansLib.Common;

namespace MSG.Script.UI.Game
{
    public class ControlGroupButtonList : HBoxContainer
    {
        internal BaseButton[] ButtonList;

        public override void _Ready()
        {
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
    }
}
