using System.Linq;
using Godot;
using MSG.Script.UI.Game;

namespace MSG.Script.UI.Base
{
	public class BasePauseMenuPanel : PanelContainer
	{
		public PauseMenu MenuParent { get; internal set; }

		public bool Active
		{
			get => Visible;
			set
			{
				Visible = value;
				if (!value) return;
				MenuParent.PausePanelActive = false;
				foreach (var panel in MenuParent.MenuPanels.Where(panel => panel != this))
				{
					panel.Active = false;
				}

				MenuParent.ActiveMenu = this;
			}
		}

		public virtual void HandleCancelPress()
		{
			MenuParent.PausePanelActive = true;
		}
	}
}
