using Godot;

namespace MSG.Script.Resource
{
    public class ShipInstanceData : Godot.Resource
    {
        [Export] public string Name = "";
        [Export] public int[] SectionIntegrity;
        [Export] public int[] HullDamage;
        [Export] public int[] ArmorDamage;
        [Export] public int Shields;

        public ShipInstanceData()
        {
        }
    }
}