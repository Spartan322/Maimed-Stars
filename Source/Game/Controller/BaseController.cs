using Godot;
using MSG.Game.Region;

namespace MSG.Game.Controller
{
    public delegate void OnControllerNationChangeAction(BaseController controller, INation nation);

    public abstract class BaseController : Reference
    {
        public event OnControllerNationChangeAction OnControllerNationChange;

        public string Name { get; set; }
        public bool IsPlayer { get; }
        public bool IsClientPlayer { get; internal set; }
        public int Identifier { get; }

        private INation _nation;

        public INation Nation
        {
            get => _nation;
            private set
            {
                OnControllerNationChange?.Invoke(this, value);
                _nation = value;
            }
        }

        public void AddToNation(INation nation)
        {
            Nation?.Remove(this);
            Nation = nation;
        }

        public void RemoveFromNation()
        {
            Nation = null;
        }

        public bool IsActive => true;

        private static bool foundClientPlayer;

        protected BaseController(int id, bool isPlayer, bool? isClientPlayer = null)
        {
            IsPlayer = isPlayer;
            if (isPlayer && (isClientPlayer ?? !foundClientPlayer))
            {
                IsClientPlayer = true;
                foundClientPlayer = true;
            }

            Identifier = id;
        }

        public virtual void Process(float delta)
        {
        }

        public override bool Equals(object obj)
            => obj is BaseController controller && controller.Identifier == Identifier;

        public override int GetHashCode()
            => Identifier.GetHashCode();
    }
}