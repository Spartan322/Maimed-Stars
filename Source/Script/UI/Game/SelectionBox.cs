using System.Collections.Generic;
using System.Linq;
using Godot;
using MSG.Game.Unit;
using MSG.Global;
using MSG.Script.Agent;
using SpartansLib;
using SpartansLib.Attributes;
using SpartansLib.Extensions;

namespace MSG.Script.UI.Game
{
	// TODO: convert SelectionBox to use its Area2D child
	[Tool]
	public class SelectionBox : Control
	{
		[Node] public Area2D Area2D;

		private Color _color = new Color(1, 1, 1);

		[Export]
		public Color Color
		{
			get => _color;
			set
			{
				_color = value;
				Update();
			}
		}

		private float _width = 1;

		[Export]
		public float Width
		{
			get => _width;
			set
			{
				_width = value;
				Update();
			}
		}

		private bool _antialiased;

		[Export]
		public bool Antialiased
		{
			get => _antialiased;
			set
			{
				_antialiased = value;
				Update();
			}
		}

		[Export] public float SelectionStartThreshold = 20;

		/*public Vector2 StartVector
			=> RectPosition.Sub(Width).Add(Width < 1+Mathf.Epsilon ? Width : Width/2) * RectScale;

		public Vector2 EndingRectSize
			=> RectSize.Add(Width >= 1 + Mathf.Epsilon ? Width : 0);

		public Vector2 EndVector
			=> StartVector + EndingRectSize * RectScale;

		public Vector2 EndXVector
			=> StartVector + EndingRectSize.Y(0) * RectScale;

		public Vector2 EndYVector
			=> StartVector + EndingRectSize.X(0) * RectScale;*/

		internal Rect2 selectGlobal;
		public Rect2 SelectGlobal => selectGlobal.Abs();

		private bool _isActivelySelecting;

		public bool IsActivelySelecting
		{
			get => _isActivelySelecting;
			set
			{
				GD.Print($"IsActivelySelecting {value}");
				SetPhysicsProcess(value);
				GD.Print($"SetPhysicsProcess {value}");
				//Area2D.Monitoring = value;
				// GD.Print($"Area2D.Monitoring {value}");
				_isActivelySelecting = value;
			}
		}

		public bool HasValidSelectionArea =>
			Visible || SelectGlobal.Size.LengthSquared() >= Mathf.Pow(SelectionStartThreshold, 2);

		private void UpdateRect()
		{
			RectSize = SelectGlobal.Size;
			RectPosition = SelectGlobal.Position;
			var areaValue = RectSize / 2;
			Area2D.Position = areaValue;
			Area2D.Scale = areaValue;
		}

		public void Reset()
		{
			Visible = false;
			IsActivelySelecting = false;
			selectGlobal = new Rect2();
			UpdateRect();
		}

		private SelectionMenu _selectionMenu;
		public override void _Ready()
		{
			_selectionMenu = NodeRegistry.Get<SelectionMenu>();
			IsActivelySelecting = IsActivelySelecting;
		}

		public override void _PhysicsProcess(float delta)
		{
			GD.Print("process");
			if (Engine.EditorHint || GetTree().IsInputHandled()) return;
			GD.Print("start");
			selectGlobal.End = MouseWatcher.MouseOriginGlobal;
			Visible = InputHandler.SelectionHandled = HasValidSelectionArea;
			UpdateRect();
			GD.Print("rect updated");
			// if (!InputHandler.LeftMouseJustReleased) return;
			//
			// Visible = IsActivelySelecting = InputHandler.SelectionHandled = false;
			// if (!HasValidSelectionArea) return;
			// GD.Print("has valid selection area");
			//
			// var addControlNotPressed = !InputHandler.AddControlKeyPressed;
			// if (_areaSelected.Count > 0)
			// 	_selectionMenu.AddRange(_areaSelected, addControlNotPressed);
			// else if (addControlNotPressed) _selectionMenu.Clear();
			// GD.Print("finished modifying selection menu");
		}

		public override void _UnhandledInput(InputEvent @event)
		{
			if (@event.LeftMouseIsJustPressed())
			{
				GD.Print("left mouse just pressed");
				selectGlobal.Position = MouseWatcher.MouseOriginGlobal;
				IsActivelySelecting = true;
			} else if (IsActivelySelecting && @event.LeftMouseIsJustReleased())
			{
				GD.Print("left mouse just released");
				Visible = false;
				GD.Print("visible false");
				IsActivelySelecting = false;
				GD.Print("isactivelyselecting false");
				InputHandler.SelectionHandled = false;
				GD.Print("Selection Handled false");
				if (!HasValidSelectionArea)
				{
					GD.Print("invalid selection area");
					return;
				}
				GD.Print("has valid selection area");

				var addControlNotPressed = !InputHandler.AddControlKeyPressed;
				if (_areaSelected.Count > 0)
					_selectionMenu.AddRange(_areaSelected, addControlNotPressed);
				else if (addControlNotPressed) _selectionMenu.Clear();
				GD.Print("finished modifying selection menu");
			}
		}

		private readonly List<GameUnit> _areaSelected = new List<GameUnit>();

		[Connect("area_entered", "Area2D")]
		public void OnAreaEntered(Area2D area)
		{
			if (area.GetParent() is GameUnit unit)
			{
				_areaSelected.Add(unit);
			}
		}

		[Connect("area_exited", "Area2D")]
		public void OnAreaExited(Area2D area)
		{
			if (area.GetParent() is GameUnit unit)
			{
				_areaSelected.Remove(unit);
			}
		}


		public override void _Draw()
		{
			var topLeft = new Vector2() * RectScale;
			var bottomLeft = topLeft + Vector2.Down * RectSize * RectScale;
			var topRight = topLeft + Vector2.Right * RectSize * RectScale;
			var bottomRight = topRight + Vector2.Down * RectSize * RectScale;
			var modulateForWidth = Width > 1 ? Width / 2 : 0;
			/*
			//Left
			DrawLine(StartVector.SubY(Width/2), endSizeX.AddY(Width/2), Color, Width, Antialiased);
			//Top
			DrawLine(StartVector.SubX(Width/2), endSizeY.AddX(Width/2), Color, Width, Antialiased);
			//Bottom
			DrawLine(endSizeX, endSize.AddX(Width < 1+Mathf.Epsilon ? 0 : Width/2), Color, Width, Antialiased);
			//Right
			DrawLine(endSizeY, endSize.SubY(Width < 1 + Mathf.Epsilon ? 0 : Width / 2), Color, Width, Antialiased);
			*/
			//DrawPolylineColors(new[] { /*topLeft,*/ topRight, bottomRight, bottomLeft/*, topLeft.SubY(Width/2)*/ },
			//	new[] { /*Colors.Blue,*/ Colors.Yellow, Colors.Black, Colors.Orange/*, Colors.Green*/ }, Width, Antialiased);
			DrawLine(topLeft, topRight.AddX(modulateForWidth), Color, Width, Antialiased);
			DrawLine(topRight, bottomRight.AddY(modulateForWidth), Color, Width, Antialiased);
			DrawLine(bottomRight, bottomLeft.SubX(modulateForWidth), Color, Width, Antialiased);
			DrawLine(bottomLeft, topLeft.SubY(modulateForWidth), Color, Width, Antialiased);
		}
	}
}
