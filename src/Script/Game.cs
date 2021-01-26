using System;
using Godot;
using MSG.Game;
using SpartansLib.Attributes;

namespace MSG.Script
{
	[Global]
	public class Game : Node
	{
		public GameDomain Domain { get; } = new GameDomain();

		public bool IsReady { get; private set; }

		// ReSharper disable once InconsistentNaming
		private event Action<Game> _OnReady;
		public event Action<Game> OnReady
		{
			add
			{
				if (IsReady) value(this);
				else _OnReady += value;
			}
			remove => _OnReady -= value;
		}

		public override void _Ready()
		{
			Domain.Initialize(this);
			Domain.GameWorld.Initialize(this);
			IsReady = true;
			_OnReady?.Invoke(this);
			_OnReady = null;
		}
	}
}
