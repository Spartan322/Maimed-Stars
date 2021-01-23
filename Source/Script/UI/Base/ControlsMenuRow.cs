using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;
using SpartansLib.Common;
using SpartansLib.Extensions;

namespace MSG.Source.Script.UI.Base
{
	public class ControlsMenuRow<T> : Control
		where T : Control, new()
	{
		[Export]
		public NodePath LabelPath;
		public Label Label;

		public T InputObject;

		public override void _Ready()
		{
			Label = GetNode<Label>(LabelPath);
			InputObject = GetNode<HBoxContainer>("HBoxContainer").AddChildAndReturn(new T());
		}
	}
}
