using System;
using MSG.Game.Region;
using MSG.Game.Unit.Control.Group;
using MSG.Game.Unit.Control.Select;
using VectorObj = Godot.Vector2;

namespace MSG.Game.Unit
{
    public interface IInside<in DerpT>
    {
        void Derp(DerpT d);
    }

    public interface IDerp
    {
        IInside<IDerp> Derp { get; }
    }
    public interface IDerp<in DerpT> : IDerp where DerpT : IDerp
    {
        new IInside<DerpT> Derp { get; }
    }

    public delegate void OnNationChangeAction(IUnit unit, INation nation);

    public delegate void OnNameChangeAction(IUnit unit, string name);

    public delegate void OnSelectChangeAction(IUnit obj, ISelectionList selection);

    public delegate void OnGroupChangeAction(IUnit obj, IGroup group);

    public delegate void OnMoveTargetChangeAction(IUnit obj, VectorObj? target);

    public delegate void OnRotateTargetChangeAction(IUnit obj, VectorObj? target);

    public interface IUnit : IComparable, IComparable<IUnit>
    {
        #region Selection Members

        event OnSelectChangeAction OnSelectChange;
        ISelectionList SelectionList { get; set; }
        bool CanSelect { get; }
        bool IsSelected { get; }

        #endregion

        #region Groupable Members

        event OnGroupChangeAction OnGroupChange;
        IGroup Group { get; }
        void AddToGroup(IGroup group);
        void RemoveFromGroup();

        #endregion

        #region Moveable Members

        event OnMoveTargetChangeAction OnMoveTargetChange;
        float TargetSpeed { get; set; }
        float MaxSpeed { get; set; }
        bool CanMove { get; set; }
        bool IsMoving { get; }
        VectorObj? CurrentMovementTarget { get; set; }
        bool HasSecondaryTarget { get; }
        void AddMovementTarget(VectorObj vec);
        void ClearMovementTargets();
        VectorObj PopMovementTarget();

        #endregion

        #region Rotatable Members

        event OnRotateTargetChangeAction OnRotateTargetChange;
        VectorObj? CurrentRotationTarget { get; set; }

        #endregion

        event OnNationChangeAction OnNationChange;
        event OnNameChangeAction OnNameChange;
        string Name { get; set; }
        GameWorld World { get; set; }
        INation Nation { get; }
        void AddToNation(INation nation);
        void RemoveFromNation();
        void Delete();
    }
}