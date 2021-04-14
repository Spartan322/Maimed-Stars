using System.Globalization;
using Godot;
using MSG.Script.Game.World;
using SpartansLib;
using SpartansLib.Attributes;
using MSG.Engine;

namespace MSG.Script.Gui.Game.Frontend
{
    public class SpeedController : HBoxContainer
    {
        [Export]
        public float SpeedChangeEchoDelaySeconds = 0.4f;


        [RetrieveNode]
        private GameDomain _gameDomain;
        private Timer _speedChangeTimer = new Timer { Autostart = false, OneShot = true };

        [Node("SpeedLabelButton/SpeedHBox/SpeedLabel")]
        public Label SpeedLabel;

        public override void _Ready()
        {
            AddChild(_speedChangeTimer);
            SpeedLabel.Text = _gameDomain.ActionSpeed.ToString(CultureInfo.InvariantCulture);
            _gameDomain.OnGameSpeedChange += (root, speed) =>
            {
                SpeedLabel.Text = Mathf.IsZeroApprox(speed) ? "Paused" : speed.ToString(CultureInfo.InvariantCulture);
            };
            //this.Connect(Signal.GuiInput, GetParent(), nameof(_GuiInput));
        }

        public override void _Process(float delta)
        {
            if (GetFocusOwner() != null) return;
            if (InputManager.SpeedPausePressed)
                HandleGameSpeedChange(GameSpeedInteraction.Pause);
            else if (InputManager.SpeedUpPressed)
                HandleGameSpeedChange(GameSpeedInteraction.Up);
            else if (InputManager.SpeedDownPressed)
                HandleGameSpeedChange(GameSpeedInteraction.Down);
            else _waitForNextPress = false;
        }

        public void SpeedUpButtonPressed()
        {
            _gameDomain.AddActionSpeed();
        }

        public void SpeedDownButtonPressed()
        {
            _gameDomain.SubActionSpeed();
        }

        public void SpeedLabelButtonPressed()
        {
            _gameDomain.ToggleActionSpeed();
        }

        private enum GameSpeedInteraction
        {
            Up,
            Down,
            Pause
        }

        private bool _waitForNextPress;

        private void HandleGameSpeedChange(GameSpeedInteraction speed)
        {
            if (speed == GameSpeedInteraction.Pause && _waitForNextPress) return;
            if (!_waitForNextPress)
            {
                ChangeSpeed(speed);
                _speedChangeTimer.Start(SpeedChangeEchoDelaySeconds);
                _waitForNextPress = true;
            }
            else if (Mathf.IsZeroApprox(_speedChangeTimer.TimeLeft))
                ChangeSpeed(speed);
        }

        private void ChangeSpeed(GameSpeedInteraction speed)
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (speed)
            {
                case GameSpeedInteraction.Up:
                    SpeedUpButtonPressed();
                    break;
                case GameSpeedInteraction.Down:
                    SpeedDownButtonPressed();
                    break;
                case GameSpeedInteraction.Pause:
                    SpeedLabelButtonPressed();
                    break;
            }
        }
    }
}
