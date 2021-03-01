namespace MSG.Script.UI.Base
{
    //TODO:delete
    public enum StandardButton
    {
        None = -1,
        Minimize,
        Maximize,
        Exit,
        Max
    }

    public static class StandardButtonExtensions
    {
        public static bool IsMinimize(this StandardButton button)
            => button == StandardButton.Minimize;

        public static bool IsMaximize(this StandardButton button)
            => button == StandardButton.Maximize;

        public static bool IsExit(this StandardButton button)
            => button == StandardButton.Exit;
    }
}