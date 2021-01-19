namespace MSG.Game.Unit.Control.Move
{
    public delegate void OnMoveTargetChangeAction<VecObj>(IMoveObject<VecObj> obj, VecObj? target)
        where VecObj : struct;

    public interface IMoveObject<VecObj>
        where VecObj : struct
    {
        event OnMoveTargetChangeAction<VecObj> OnMoveTargetChange;
        float MaxSpeed { get; set; }
        bool CanMove { get; set; }
        bool IsMoving { get; }
        VecObj? CurrentMovementTarget { get; set; }
        void AddMovementTarget(VecObj vec);
        void ClearMovementTargets();
        VecObj PopMovementTarget();
    }
}