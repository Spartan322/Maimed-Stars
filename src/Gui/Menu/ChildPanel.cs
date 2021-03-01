using System.Linq;
using Godot;

namespace MSG.Gui.Menu
{
    public class ChildPanel : PanelContainer
    {
        public ParentPanel MenuParent { get; internal set; }

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
