using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ECS.Model;
using Serilog;

namespace ECS.Xml
{
    public class Serialization
    {
        private readonly XmlSerializer _ser = new XmlSerializer(typeof(CircuitXml));
        public void Serialize(CircuitXml cx, Stream s)
        {
            foreach (var node in cx.Nodes)
                cx.Links.Add(new Link(node));
            _ser.Serialize(s, cx);
        }

        public CircuitXml Deserialize(Stream s)
        {
            var cx = (CircuitXml)_ser.Deserialize(s);
            var dn = cx.Nodes.ToDictionary(i => i.Id);
            var dc = cx.Resistors.Cast<Component>().ToDictionary(i => i.Id);
            var dv = cx.VoltageSources.Cast<Component>().ToDictionary(i => i.Id);
            foreach (var link in cx.Links)
            {
                Node n;
                var success = dn.TryGetValue(link.NodeId, out n);
                if (!success)
                {
                    // TODO decide if log or exp
                    Log.Error("Invalid link: Node #{0} not found", link.NodeId);
                    continue;
                }
                foreach (var cl in link.ComponentLinks)
                {
                    Component c;
                    success = (cl.IsVoltageSource ? dv : dc).TryGetValue(cl.Id, out c);
                    if (!success)
                    {
                        // TODO decide if log or exp
                        Log.Error("Invalid component link: {1} #{0} not found", cl.Id, cl.IsVoltageSource ? "Voltage source" : "Component");
                        continue;
                    }
                    if (cl.IsPlus) c.Node1 = n;
                    else c.Node2 = n;
                }
            }

            return cx;
        }
    }
}
