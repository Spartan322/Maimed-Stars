using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;
using Godot.Collections;
using MSG.Script.UI.Base;
using MSG.Settings;
using SpartansLib;
using SpartansLib.Attributes;
using SpartansLib.Common;
using SpartansLib.Extensions;
using MenuDic = System.Collections.Generic.Dictionary<string, MSG.Script.UI.Base.MenuRow>;

namespace MSG.Script.UI.Settings.Tab
{
	public class Controls : HBoxContainer
	{
		public static readonly PackedScene MenuRowScene = GD.Load<PackedScene>("res://Scenes/UI/Menu/MenuRow.tscn");

		public static SortedDictionary<string, Array<InputEvent>> NameToActions;

		public static readonly MenuDic NameToRowControl = new MenuDic();

		public RebindPopupDialog RebindDialog;

		[Node("ScrollContainer/VBox")] public VBoxContainer ControlVBox;

		public override void _Ready()
		{
			CreateEdgeScrollCheckButton();
			CreateCameraMovementSlider();
			LoadInputList();
			GenerateInputList();
		}

		public void CreateCameraMovementSlider()
		{
			var menuRow = ControlVBox.AddChildAndReturn(new MenuRow<VBoxContainer>());
			menuRow.Label.Text = "Camera Movement Multiplier";
			var slider = menuRow.InputObject.AddChildAndReturn(new HSlider());
			slider.FocusMode = FocusModeEnum.None;
			slider.MinValue = 0.1;
			slider.MaxValue = 2;
			slider.Step = 0.1;
			slider.Value = ControlSettings.CameraMovementMultiplier;
			var sliderEdit = menuRow.InputObject.AddChildAndReturn(new LineEdit());
			slider.Connect("value_changed", this, nameof(OnCameraMovementSliderValueChanged), new[] {sliderEdit});
			sliderEdit.Align = LineEdit.AlignEnum.Center;
			//Label.Autowrap = true;
			sliderEdit.SizeFlagsHorizontal = (int) SizeFlags.Fill;
			sliderEdit.MouseFilter = MouseFilterEnum.Pass;
			sliderEdit.Text = slider.Value.ToString();
			sliderEdit.Connect(Signal.TextEntered, this, nameof(OnCameraMovementLineEditEntered),
				new Node[] {sliderEdit, slider});
			NameToRowControl[menuRow.Label.Text] = menuRow;
		}

		public void CreateEdgeScrollCheckButton()
		{
			var menuRow = ControlVBox.AddChildAndReturn(new MenuRow<CenterContainer>());
			menuRow.Label.Text = "Edge Scroll";
			var checkBox = menuRow.InputObject.AddChildAndReturn(new CheckBox());
			checkBox.Pressed = ControlSettings.EdgeScroll;
			checkBox.Connect(Signal.Toggled, this, nameof(OnEdgeScrollCheckButtonToggled));
			ControlSettings.OnSettingChanged += (name, val) =>
			{
				if (name == nameof(ControlSettings.EdgeScroll))
					checkBox.Pressed = (bool) val;
			};
		}

		public void LoadInputList()
		{
			var actionArray = InputMap.GetActions().Cast<string>();
			NameToActions = new SortedDictionary<string, Array<InputEvent>>();
			foreach (var name in actionArray.SkipWhile(ControlSettings.ActionDisplayBlacklist.Contains))
				NameToActions[name] = InputMap.GetActionList(name).Cast<InputEvent>();
		}

		public void GenerateInputList()
		{
			foreach (var pair in NameToActions)
			{
				var row = ControlVBox.AddChildAndReturn(new MenuRow<Button>());
				row.Label.Text = ConvertActionName(pair.Key);
				row.InputObject.Text = ConvertButtonText(pair.Value[0]);
				if (ControlSettings.ActionDescriptions.ContainsKey(pair.Key))
					row.HintTooltip = ControlSettings.ActionDescriptions[pair.Key];
				row.InputObject.HintTooltip = row.HintTooltip;
				row.Label.HintTooltip = row.HintTooltip;
				row.InputObject.Connect(Signal.Pressed, this, nameof(HandleControlButtonPress), new[] {pair.Key});
				NameToRowControl[pair.Key] = row;
			}

			ControlVBox.AddChild(new Control());
		}

		public static string ConvertButtonText(InputEvent @event)
		{
			if (@event is InputEventMouseButton mouseButton)
			{
				switch (mouseButton.ButtonIndex)
				{
					case 4: return "Wheel Up";
					case 5: return "Wheel Down";
					case 8: return "Mouse Button 1";
					case 9: return "Mouse Button 2";
					case var index:
						return ((ButtonList) index).ToString() + " Click";
				}
			}

			var text = @event.AsText();
			switch (text)
			{
				case "QuoteLeft": return "`";
				case "Minus": return "-";
				case "Equal": return "=";
				case "Slash": return "/";
				case "Period": return ".";
				case "Comma": return ",";
				case "BracketLeft": return "[";
				case "BracketRight": return "]";
				case "BackSlash": return "\\";
			}

			if (text.StartsWith("Kp", StringComparison.Ordinal))
				text = "NumPad" + text.Substring(2);
			return text.SpaceCapitals((c, i) => text[i - 1] != '+');
		}

		public static string ConvertActionName(string name)
		{
			name = name.ToLower();
			var result = new StringBuilder(name.Length);
			var split = name.Split('_');
			for (var i = 0; i < split.Length; i++)
			{
				result.Append(char.ToUpper(split[i][0]) + split[i].Substring(1));
				if (i < split.Length - 1) result.Append(' ');
			}

			return result.ToString();
		}

		#region Callbacks

		public void HandleControlButtonPress(string actionName)
		{
			RebindDialog.ActionNameToRebind = actionName;
			RebindDialog.ActionToRebind = NameToActions[actionName][0];
			if (ControlSettings.ActionDescriptions.ContainsKey(actionName))
				RebindDialog.DescriptionLabel.Text = ControlSettings.ActionDescriptions[actionName];
			else RebindDialog.DescriptionLabel.Text = "";
			RebindDialog.Active = true;
			Logger.Info($"Trying to rebind {actionName}");
		}

		private static bool ignoreSliderChange;

		public void OnCameraMovementSliderValueChanged(float value, LineEdit edit)
		{
			if (ignoreSliderChange)
			{
				ignoreSliderChange = false;
				return;
			}

			ControlSettings.CameraMovementMultiplier = value;
			edit.Text = value.ToString();
		}

		public void OnCameraMovementLineEditEntered(string text, LineEdit edit, HSlider slider)
		{
			if (!float.TryParse(text, out var value)) return;
			if (value < 0.1) value = 0.1f;
			ignoreSliderChange = true;
			slider.Value = value > slider.MaxValue ? slider.MaxValue : value;
			ControlSettings.CameraMovementMultiplier = value;
			edit.Text = value.ToString();
		}

		public void OnEdgeScrollCheckButtonToggled(bool pressed)
			=> ControlSettings.EdgeScroll = pressed;

		#endregion
	}
}
