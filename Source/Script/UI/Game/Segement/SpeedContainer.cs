using System.Globalization;
using Godot;
using SpartansLib;
using SpartansLib.Attributes;

namespace MSG.Script.UI.Game.Segement
{
    public class SpeedContainer : HBoxContainer
    {
        [Node("SpeedLabelButton/SpeedHBox/SpeedLabel")]
        public Label SpeedLabel;

        public override void _Ready()
        {
            var domain = NodeRegistry.Get<Script.Game>().Domain;
            SpeedLabel.Text = domain.ActionSpeed.ToString(CultureInfo.InvariantCulture);
            domain.OnGameSpeedChange += (root, speed) =>
            {
                SpeedLabel.Text = Mathf.IsZeroApprox(speed) ? "Paused" : speed.ToString(CultureInfo.InvariantCulture);
            };
            //this.Connect(Signal.GuiInput, GetParent(), nameof(_GuiInput));
        }

        public void SpeedUpButtonPressed()
        {
            NodeRegistry.Get<Script.Game>().Domain.AddActionSpeed();
        }

        public void SpeedDownButtonPressed()
        {
            NodeRegistry.Get<Script.Game>().Domain.SubActionSpeed();
        }

        public void SpeedLabelButtonPressed()
        {
            NodeRegistry.Get<Script.Game>().Domain.ToggleActionSpeed();
        }
    }
}
