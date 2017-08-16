namespace ECS.Core.Model
{
    public interface ISwitch : IComponent
    {
        bool IsClosed { get; set; }
    }
}
