namespace ECS.Core.SimulationModel
{
    public class Switch : Component
    {
        public Switch(Model.Switch s)
            : base(s.Id)
        {
            IsClosed = s.IsClosed;
        }

        public Switch(int id)
            : base(id) { }

        public bool IsClosed { get; set; }
    }
}
