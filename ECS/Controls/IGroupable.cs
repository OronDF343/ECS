using System;

namespace ECS.Controls
{
    public interface IGroupable
    {
        Guid Id { get; }
        Guid ParentId { get; set; }
        bool IsGroup { get; set; }
    }
}
