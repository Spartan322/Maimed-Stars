using Godot;

namespace MSG.Game.Unit
{
    public interface IUnitController
    {
        Node2D NodeObject { get; }

        float Mass { get; }
        float MaximumMoveSpeed { get; }
        float MaximumAcceleration { get; }
        float MaximumAngularSpeed { get; }

        void OnSelectChange(bool isToBeSelected);
    }
}