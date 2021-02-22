using System.Collections.Generic;
using MSG.Script.UI.Base;

namespace MSG.Script.UI.Base
{
    public interface IMenuParent
    {
        bool IsPanelActive { get; set; }
        BaseMenuPanel ActiveMenu { get; }

        IList<BaseMenuPanel> MenuPanels { get; }

        void RequestSetActiveMenu(BaseMenuPanel panel);

        void HandleCancelPress();
    }
}