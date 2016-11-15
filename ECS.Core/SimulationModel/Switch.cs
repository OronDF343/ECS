namespace ECS.Core.SimulationModel
{
    public class Switch : Component
    {
        public bool IsClosed { get; set; }

        public Switch(int id) : base(id) { }
    }
}
