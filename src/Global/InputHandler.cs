using Godot;
using MSG.Global.Attribute;
using SpartansLib;
using System.Linq;
using MSG.Game.Rts.Unit;
using MSG.Script.Game.Unit;
using MSG.Script.Game.World;
using MSG.Script.Gui.Game.Select;
using MSG.Engine;

namespace MSG.Global
{
    public static class InputHandler
    {
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
            var addControlPressed = InputManager.AddControlKeyPressed;

            if (e.SelectAllShipsKeyIsJustPressed())
            {
                // TODO: replace SelectionHandler with SelectionMenu's UnitSelectList
            }

            if (e.RightMouseIsPressed() || !addControlPressed && InputManager.RightMousePressed)
                _selectionDisplay.SelectList.QueueMoveForSelection(MouseWatcher.MouseOriginGlobal, addControlPressed);

            if (!e.LeftMouseIsJustReleased()) return;

            global.GetFocusOwner()?.ReleaseFocus();
            var ship = SingleUnit.MouseOver;
            if (!addControlPressed) _selectionDisplay.Clear();
            if (ship != null)
                _selectionDisplay.Add(ship);
        }

        public const float CONTROL_GROUP_SET_WAIT_TIME = 0.3f;
        public const float SPEED_CHANGE_ECHO_DELAY = 0.4f;

        private static Timer controlGroupTimer = new Timer { Autostart = false, OneShot = true };
        private static int controlGroupNum = -1;

        private static Timer speedChangeTimer = new Timer { Autostart = false, OneShot = true };

        private static SelectionDisplay _selectionDisplay;

        [Ready]
        public static void OnReady(GlobalScript script)
        {
            script.AddChild(controlGroupTimer);
            script.AddChild(speedChangeTimer);
            _selectionDisplay = NodeRegistry.Get<SelectionDisplay>();
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
                    if (_selectionDisplay.FocusUnit is GroupUnit group
                        && _selectionDisplay.SelectList.All(u => group.Contains(u)))
                    {
                        GameData.SetControlGroup(controlGroupNum,
                            new SelectList() { group });
                    }
                    else
                        GameData.SetControlGroup(controlGroupNum,
                            new SelectList(_selectionDisplay.SelectList));
                    StopTimer();
                }
                else if (!pressed)
                {
                    GameData.TrySelectControlGroup(controlGroupNum);
                    StopTimer();
                }
            }

            if (InputManager.SpeedPausePressed)
                HandleGameSpeedChange(GameSpeedInteraction.Pause);
            else if (InputManager.SpeedUpPressed)
                HandleGameSpeedChange(GameSpeedInteraction.Up);
            else if (InputManager.SpeedDownPressed)
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
            var domain = NodeRegistry.Get<GameDomain>();
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