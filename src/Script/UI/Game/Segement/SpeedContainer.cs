using System.Globalization;
using Godot;
using MSG.Script.World;
using SpartansLib;
using SpartansLib.Attributes;

namespace MSG.Script.UI.Game.Segement
{
    public class SpeedContainer : HBoxContainer
    {
        private GameDomain _domain;

        [Node("SpeedLabelButton/SpeedHBox/SpeedLabel")]
        public Label SpeedLabel;

        public override void _Ready()
        {
            _domain = NodeRegistry.Get<GameDomain>();
            SpeedLabel.Text = _domain.ActionSpeed.ToString(CultureInfo.InvariantCulture);
            _domain.OnGameSpeedChange += (root, speed) =>
            {
                SpeedLabel.Text = Mathf.IsZeroApprox(speed) ? "Paused" : speed.ToString(CultureInfo.InvariantCulture);
            };
            //this.Connect(Signal.GuiInput, GetParent(), nameof(_GuiInput));
        }

        public void SpeedUpButtonPressed()
        {
            _domain.AddActionSpeed();
        }

        public void SpeedDownButtonPressed()
        {
            _domain.SubActionSpeed();
        }

        public void SpeedLabelButtonPressed()
        {
            _domain.ToggleActionSpeed();
        }
    }
}
