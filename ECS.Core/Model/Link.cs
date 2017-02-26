namespace ECS.Core.Model
{
    public class Link
    {
        public Component Component { get; set; }
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
