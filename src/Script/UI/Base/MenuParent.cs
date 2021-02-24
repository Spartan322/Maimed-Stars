using System.Collections;
using System.Collections.Generic;
using Godot;
using MSG.Script.UI.Base;
using MSG.Script.UI.Settings;

namespace MSG.Script.UI.Base
{
    public abstract class MenuParent : Control, IReadOnlyList<BaseMenuPanel>, ICollection<BaseMenuPanel>
    {

        protected abstract PanelContainer Panel { get; set; }
        protected abstract SettingsMenu SettingsMenu { get; set; }

        public override void _Ready()
        {
            //GD.Print(GetNode(PausePanelPath)?.ToString() ?? "Null"); // for some reason fails
            // PausePanel = GetNode<PanelContainer>("PausePanel"); // for some reason works fine
            foreach (var child in GetChild(0).GetChildren())
            {
                if (!(child is BaseMenuPanel panel)) continue;
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

        private BaseMenuPanel _activeMenu;
        public BaseMenuPanel ActiveMenu
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

        public BaseMenuPanel this[int index] => _menuPanels[index];

        private readonly List<BaseMenuPanel> _menuPanels = new List<BaseMenuPanel>();

        public virtual bool HandleCancelPress()
        {
            if (ActiveMenu != null)
            {
                ActiveMenu.HandleCancelPress();
                return true;
            }
            return false;
        }

        public void Add(BaseMenuPanel item) => _menuPanels.Add(item);
        public void Clear() => _menuPanels.Clear();
        public bool Contains(BaseMenuPanel item) => _menuPanels.Contains(item);
        public void CopyTo(BaseMenuPanel[] array, int arrayIndex) => _menuPanels.CopyTo(array, arrayIndex);
        public bool Remove(BaseMenuPanel item) => _menuPanels.Remove(item);
        public IEnumerator<BaseMenuPanel> GetEnumerator() => _menuPanels.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}