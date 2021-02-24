using System.Linq;
using Godot;
using MSG.Script.UI.Game;

namespace MSG.Script.UI.Base
{
    public class BaseMenuPanel : PanelContainer
    {
        public MenuParent MenuParent { get; internal set; }

        public bool Active
        {
            get => Visible;
            set
            {
                Visible = value;
                if (!value) return;
                MenuParent.IsPanelActive = false;
                foreach (var panel in MenuParent.Where(panel => panel != this))
                {
                    panel.Active = false;
                }
                MenuParent.ActiveMenu = this;
            }
        }

        public virtual void HandleCancelPress()
        {
            MenuParent.IsPanelActive = true;
        }
    }
}
