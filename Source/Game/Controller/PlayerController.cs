namespace MSG.Game.Controller
{
    public class PlayerController : BaseController
    {
        public PlayerController(int id, string name, bool? isClientPlayer = null) : base(id, true, isClientPlayer)
        {
            Name = name;
        }
    }
}