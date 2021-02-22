using System.Linq;
using Godot;
using MSG.Script.UI.Game;

namespace MSG.Script.UI.Base
{
    public class BaseMenuPanel : PanelContainer
    {
        public IMenuParent MenuParent { get; internal set; }

        public bool Active
        {
            get => Visible;
            set
            {
                Visible = value;
                if (!value) return;
                MenuParent.IsPanelActive = false;
                foreach (var panel in MenuParent.MenuPanels.Where(panel => panel != this))
                {
                    panel.Active = false;
                }
                MenuParent.RequestSetActiveMenu(this);
            }
        }

        public virtual void HandleCancelPress()
        {
            MenuParent.IsPanelActive = true;
        }
    }
}
