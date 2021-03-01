using System.Collections;
using System.Collections.Generic;
using Godot;
using MSG.Script.Gui.Menu.Settings;

namespace MSG.Gui.Menu
{
    public abstract class ParentPanel : Control, IReadOnlyList<ChildPanel>, ICollection<ChildPanel>
    {

        protected abstract PanelContainer Panel { get; set; }
        protected abstract SettingsMenu SettingsMenu { get; set; }

        public override void _Ready()
        {
            //GD.Print(GetNode(PausePanelPath)?.ToString() ?? "Null"); // for some reason fails
            // PausePanel = GetNode<PanelContainer>("PausePanel"); // for some reason works fine
            foreach (var child in GetChild(0).GetChildren())
            {
                if (!(child is ChildPanel panel)) continue;
                Add(panel);
                panel.MenuParent = this;
            }
        }

        public bool IsPanelActive
        {
            get => Panel.Visible;
            set
            {
                Panel.Visible = value;
                if (!value) return;
                foreach (var panel in this)
                    panel.Active = false;
                ActiveMenu = null;
            }
        }

        private ChildPanel _activeMenu;
        public ChildPanel ActiveMenu
        {
            get => _activeMenu;
            internal set
            {
                if (ActiveMenu == null && Contains(value))
                    _activeMenu = value;
            }
        }

        public int Count => _menuPanels.Count;

        public bool IsReadOnly => false;

        public ChildPanel this[int index] => _menuPanels[index];

        private readonly List<ChildPanel> _menuPanels = new List<ChildPanel>();

        public virtual bool HandleCancelPress()
        {
            if (ActiveMenu != null)
            {
                ActiveMenu.HandleCancelPress();
                return true;
            }
            return false;
        }

        public void Add(ChildPanel item) => _menuPanels.Add(item);
        public void Clear() => _menuPanels.Clear();
        public bool Contains(ChildPanel item) => _menuPanels.Contains(item);
        public void CopyTo(ChildPanel[] array, int arrayIndex) => _menuPanels.CopyTo(array, arrayIndex);
        public bool Remove(ChildPanel item) => _menuPanels.Remove(item);
        public IEnumerator<ChildPanel> GetEnumerator() => _menuPanels.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}