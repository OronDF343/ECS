using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace ECS.Model.Xml
{
    public static class CircuitXmlUtils
    {
        [NotNull]
        public static IEnumerable<DiagramObject> ToDiagram(this CircuitXml cx)
        {
            var nodes = cx.Nodes.Where(n => n != null).ToDictionary(n => n.Id);
            foreach (var n in nodes.Values) yield return n;
            Node ln;
            foreach (var r in cx.Resistors.Where(r => r != null))
            {
                if (r.Node1Id != null && nodes.TryGetValue(r.Node1Id.Value, out ln)) r.Node1 = ln;
                if (r.Node2Id != null && nodes.TryGetValue(r.Node2Id.Value, out ln)) r.Node2 = ln;
                yield return r;
            }
            foreach (var v in cx.VoltageSources)
            {
                if (v.Node1Id != null && nodes.TryGetValue(v.Node1Id.Value, out ln)) v.Node1 = ln;
                if (v.Node2Id != null && nodes.TryGetValue(v.Node2Id.Value, out ln)) v.Node2 = ln;
                yield return v;
            }
        }

        [NotNull]
        public static CircuitXml ToCircuitXml(IEnumerable<DiagramObject> diagramObjects)
        {
            var cx = new CircuitXml();
            cx.Nodes.AddRange(diagramObjects.OfType<Node>());
            cx.Resistors.AddRange(diagramObjects.OfType<Resistor>());
            cx.VoltageSources.AddRange(diagramObjects.OfType<VoltageSource>());
            return cx;
        }
    }
}
