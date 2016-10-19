using System.Collections.Generic;
using System.Xml.Serialization;
using ECS.Model;

namespace ECS.Xml
{
    public class CircuitXml
    {
        [XmlArray, XmlArrayItem(nameof(Node))]
        public List<Node> Nodes { get; set; }
        [XmlArray, XmlArrayItem(nameof(Resistor))]
        public List<Resistor> Resistors { get; set; }
        [XmlArray, XmlArrayItem(nameof(VoltageSource))]
        public List<VoltageSource> VoltageSources { get; set; }
        [XmlArray, XmlArrayItem(nameof(Link))]
        public List<Link> Links { get; set; }

        

    }
}
