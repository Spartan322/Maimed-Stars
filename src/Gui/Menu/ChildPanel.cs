using System.Linq;
using Godot;

namespace MSG.Gui.Menu
{
    public abstract class ChildPanel : Control
    {
        public ParentPanel MenuParent { get; internal set; }

        public abstract Control PanelControl { get; }

        public bool Active
        {
            get => PanelControl.Visible;
            set
            {
                PanelControl.Visible = value;
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
