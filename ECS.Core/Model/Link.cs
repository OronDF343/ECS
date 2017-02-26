namespace ECS.Core.Model
{
    public class Link
    {
        public Link(IComponent c, bool isPositive)
        {
            Component = c;
            IsPositive = isPositive;
        }

        public IComponent Component { get; set; }
        public bool IsPositive { get; set; }

        public override int GetHashCode()
        {
            return Component.GetHashCode() + (IsPositive ? 0 : 1);
        }

        public override bool Equals(object obj)
        {
            return obj is Link && obj.GetHashCode() == GetHashCode();
        }
    }
}
