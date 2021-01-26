using Godot;

namespace MSG.Script
{
    public class UnitHolder : Node2D
    {
        private bool isBelow;

        [Export]
        public bool IsBelow
        {
            get => isBelow;
            set
            {
                isBelow = value;
                if (IsBelow)
                    ZIndex = -GetChildCount() * 2;
                else
                    ZIndex = 0;
                Update();
            }
        }
    }
}