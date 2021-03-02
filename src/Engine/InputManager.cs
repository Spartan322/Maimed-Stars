using Godot;

namespace MSG.Engine
{
    public static class InputManager
    {
        #region Key Names
        public const string UiNextFocus = "ui_focus_next";
        public const string PauseKeyName = "ui_cancel";
        public const string LeftMouseName = "game_left_click";
        public const string MiddleMouseName = "game_center_click";
        public const string RightMouseName = "game_right_click";
        public const string AddControlKeyName = "game_add_select_control";
        public const string SelectAllShipsName = "game_select_all_ships";
        public const string SelectionDeleteName = "game_selection_delete";
        public const string SpeedUpName = "game_speed_up";
        public const string SpeedDownName = "game_speed_down";
        public const string SpeedPauseName = "game_speed_pause";
        #endregion

        #region Key Input Calls
        public static bool UiNextFocusKeyIsJustPressed(this InputEvent e) => e.IsActionPressed(UiNextFocus);
        public static bool PauseKeyIsJustPressed(this InputEvent e) => e.IsActionPressed(PauseKeyName);
        public static bool AddControlKeyPressed => Input.IsActionPressed(AddControlKeyName);
        public static bool LeftMouseJustPressed => Input.IsActionJustPressed(LeftMouseName);
        public static bool LeftMouseIsJustPressed(this InputEvent e) => e.IsActionPressed(LeftMouseName);
        public static bool LeftMouseJustReleased => Input.IsActionJustReleased(LeftMouseName);
        public static bool LeftMouseIsJustReleased(this InputEvent e) => e.IsActionReleased(LeftMouseName);
        public static bool MiddleMousePressed => Input.IsActionPressed(MiddleMouseName);
        public static bool RightMousePressed => Input.IsActionPressed(RightMouseName);
        public static bool RightMouseIsPressed(this InputEvent e) => e.IsActionPressed(RightMouseName);
        public static bool IsMouseAction(this InputEvent e) =>
            e.IsAction(LeftMouseName) || e.IsAction(MiddleMouseName) || e.IsAction(RightMouseName);

        public static bool MouseActionPressed => Input.IsActionPressed(LeftMouseName) ||
                                                 Input.IsActionPressed(MiddleMouseName) ||
                                                 Input.IsActionPressed(RightMouseName);

        public static bool SelectAllShipsKeyIsJustPressed(this InputEvent e) => e.IsActionPressed(SelectAllShipsName);
        public static bool SelectionDeleteKeyIsJustPressed(this InputEvent e) => e.IsActionPressed(SelectionDeleteName);
        public static bool SpeedUpPressed => Input.IsActionPressed(SpeedUpName);
        public static bool SpeedDownPressed => Input.IsActionPressed(SpeedDownName);
        public static bool SpeedPausePressed => Input.IsActionPressed(SpeedPauseName);
        #endregion
    }
}