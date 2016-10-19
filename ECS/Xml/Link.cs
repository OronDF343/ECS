using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using ECS.Model;

namespace ECS.Xml
{
    public class Link
    {
        [XmlAttribute]
        public int NodeId { get; set; }
        [XmlArray, XmlArrayItem(nameof(ComponentLink))]
        public List<ComponentLink> ComponentLinks { get; set; }

        public Link(Node n)
        {
            NodeId = n.Id;
            ComponentLinks = n.Components.Select(i => new ComponentLink(i, NodeId)).ToList();
        }
    }

    public class ComponentLink
    {
        [XmlAttribute]
        public int Id { get; set; }
        [XmlAttribute]
        public bool IsPlus { get; set; }
        [XmlAttribute]
        public bool IsVoltageSource { get; set; }

        public ComponentLink(Component c, int nodeId)
        {
            Id = c.Id;
            IsPlus = c.Node1?.Id == nodeId;
            IsVoltageSource = c is VoltageSource;
        }
    }
}
