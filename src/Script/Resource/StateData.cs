using Godot;
using Godot.Collections;

namespace MSG.Script.Resource
{
    public class StateData : Godot.Resource
    {
        [Export] public string Name = "InvalidState";

        [Export] public Array<int> ControllerIds = new Array<int>();
    }
}