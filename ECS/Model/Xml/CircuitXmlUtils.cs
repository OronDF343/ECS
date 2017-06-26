using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ECS.Core.Model;
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
            foreach (var s in cx.Switches)
            {
                if (s.Node1Id != null && nodes.TryGetValue(s.Node1Id.Value, out ln)) s.Node1 = ln;
                if (s.Node2Id != null && nodes.TryGetValue(s.Node2Id.Value, out ln)) s.Node2 = ln;
                yield return s;
            }
        }

        [NotNull]
        public static CircuitXml ToCircuitXml(IEnumerable<DiagramObject> diagramObjects)
        {
            var cx = new CircuitXml();
            cx.Nodes.AddRange(diagramObjects.OfType<Node>());
            cx.Resistors.AddRange(diagramObjects.OfType<Resistor>());
            cx.VoltageSources.AddRange(diagramObjects.OfType<VoltageSource>());
            cx.Switches.AddRange(diagramObjects.OfType<Switch>());
            return cx;
        }
        
        public static int MaxDefaultId<T>([NotNull] this IEnumerable<T> collection, [NotNull] string prefix = "")
            where T : ICircuitObject
        {
            try
            {
                return collection.Max(n =>
                {
                    var m = Regex.Match(n.Name, @"$" + Regex.Escape(prefix) + @"([1-9][0-9]*)");
                    return m.Success ? int.Parse(m.Groups[0].Value) : 0;
                });
            }
            // If the collection has no elements:
            catch (InvalidOperationException) { return 0; }
        }
    }
}
