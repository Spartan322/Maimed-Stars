using Godot;
using MSG.Game.Unit;
using MSG.Global.Attribute;
using MSG.Script.Agent;
using MSG.Script.UI.Game;
using SpartansLib;

namespace MSG.Global
{
    public static class InputHandler
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

        public static string GetControlGroupKeyName(int index)
        {
            var name = $"game_control_group_{index}_key";
            return InputMap.HasAction(name) ? name : null;
        }

        private static int controlGroupMaxCount = -1;

        public static int ControlGroupMaxCount
        {
            get
            {
                if (controlGroupMaxCount > -1) return controlGroupMaxCount;
                var i = 0;
                for (; GetControlGroupKeyName(i) != null; i++) ;
                controlGroupMaxCount = i;
                return i;
            }
        }

        public static bool SelectionHandled { get; internal set; }

        [UnhandledInput]
        public static void OnUnhandledInput(GlobalScript global, InputEvent e)
        {
            var addControlPressed = AddControlKeyPressed;

            if (e.SelectAllShipsKeyIsJustPressed())
            {
                // TODO: replace SelectionHandler with SelectionMenu's UnitSelectList
                //SelectionHandler.SelectMultiple(UnitShipAgent.AllShipUnits, !addControlPressed);
            }

            if (e.RightMouseIsPressed() || !addControlPressed && RightMousePressed)
                _selectionMenu.SelectionList.QueueMoveForSelection(MouseWatcher.MouseOriginGlobal, addControlPressed);
            //SelectionHandler.MoveSelectionTo(MouseWatcher.MouseOriginGlobal, addControlPressed);

            if (!e.LeftMouseIsJustReleased()) return;

            global.GetFocusOwner()?.ReleaseFocus();
            if (SelectionHandled) return;
            var ship = Ship.MouseOverShip;
            if (ship != null)
                _selectionMenu.Add(ship);
            // ship.Select(!addControlPressed);
            else if (!addControlPressed) _selectionMenu.Clear();
        }

        public const float CONTROL_GROUP_SET_WAIT_TIME = 0.3f;
        public const float SPEED_CHANGE_ECHO_DELAY = 0.4f;

        private static Timer controlGroupTimer = new Timer {Autostart = false, OneShot = true};
        private static int controlGroupNum = -1;

        private static Timer speedChangeTimer = new Timer {Autostart = false, OneShot = true};

        private static SelectionMenu _selectionMenu;

        [Ready]
        public static void OnReady(GlobalScript script)
        {
            script.AddChild(controlGroupTimer);
            script.AddChild(speedChangeTimer);
            _selectionMenu = NodeRegistry.Get<SelectionMenu>();
        }

        [Process]
        public static void OnProcess(GlobalScript script, float delta)
        {
            if (script.GetFocusOwner() != null) return;
            var pressed = false;
            var ctrlGroupStr = GetControlGroupKeyName(0);
            for (var i = 0; (ctrlGroupStr = GetControlGroupKeyName(i)) != null; i++)
            {
                pressed |= Input.IsActionPressed(ctrlGroupStr);
                if (!Input.IsActionJustPressed(ctrlGroupStr)) continue;
                controlGroupNum = i;
                controlGroupTimer.Start(CONTROL_GROUP_SET_WAIT_TIME);
                break;
            }

            if (controlGroupNum > -1)
            {
                if (Mathf.IsZeroApprox(controlGroupTimer.TimeLeft))
                {
                    var selectionMenu = NodeRegistry.Get<SelectionMenu>();
                    GameData.SetControlGroup(controlGroupNum,
                        /* (ISelectionList) selectionMenu.SelectedGroup
                        ??  */selectionMenu.SelectionList);
                    StopTimer();
                }
                else if (!pressed)
                {
                    GameData.TrySelectControlGroup(controlGroupNum);
                    StopTimer();
                }
            }

            if (SpeedPausePressed)
                HandleGameSpeedChange(GameSpeedInteraction.Pause);
            else if (SpeedUpPressed)
                HandleGameSpeedChange(GameSpeedInteraction.Up);
            else if (SpeedDownPressed)
                HandleGameSpeedChange(GameSpeedInteraction.Down);
            else waitForNextPress = false;
        }

        private enum GameSpeedInteraction
        {
            Up,
            Down,
            Pause
        }

        private static bool waitForNextPress;

        private static void HandleGameSpeedChange(GameSpeedInteraction speed)
        {
            if (speed == GameSpeedInteraction.Pause && waitForNextPress) return;
            if (!waitForNextPress)
            {
                ChangeSpeed(speed);
                speedChangeTimer.Start(SPEED_CHANGE_ECHO_DELAY);
                waitForNextPress = true;
            }
            else if (Mathf.IsZeroApprox(speedChangeTimer.TimeLeft))
                ChangeSpeed(speed);
        }

        private static void ChangeSpeed(GameSpeedInteraction speed)
        {
            var domain = NodeRegistry.Get<Script.Game>().Domain;
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (speed)
            {
                case GameSpeedInteraction.Up:
                    domain.AddActionSpeed();
                    break;
                case GameSpeedInteraction.Down:
                    domain.SubActionSpeed();
                    break;
                case GameSpeedInteraction.Pause:
                    domain.ToggleActionSpeed();
                    break;
            }
        }

        private static void StopTimer()
        {
            controlGroupNum = -1;
            controlGroupTimer.Stop();
        }
        //private static Ship GetSelectedShip(Viewport view)
        //{
        //    var intersects = view.World2d.DirectSpaceState
        //            .IntersectPoint(
        //                MouseWatcher.MouseOriginGlobal,
        //                collisionLayer: 2,
        //                collideWithBodies: false,
        //                collideWithAreas: true
        //            );
        //    if (intersects.Count == 0) return null;
        //    Ship target = null, ship;
        //    foreach(var intersectInfo in intersects)
        //    {
        //        ship = GetFromIntersect(intersectInfo);
        //        if (target != null && target.CompareTo(ship) < 0)
        //            target = ship;
        //        else if (target == null) target = ship;
        //    }
        //    return target;
        //}

        //private static Ship GetFromIntersect(object input)
        //{
        //    return (Ship)((CollisionObject2D)((Dictionary)input)["collider"]).GetParent();
        //}
    }
}