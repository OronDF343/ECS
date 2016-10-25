using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ECS.Model;
using JetBrains.Annotations;
using Serilog;

namespace ECS.Xml
{
    /// <summary>
    /// Manages serialization of circuits.
    /// </summary>
    public class Serialization
    {
        [NotNull]
        private readonly XmlSerializer _ser = new XmlSerializer(typeof(CircuitXml));

        /// <summary>
        /// Serializes a circuit.
        /// </summary>
        /// <param name="cx">A <see cref="CircuitXml"/> object.</param>
        /// <param name="s">A <see cref="Stream"/> which the XML will be written to.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="cx"/> or <paramref name="s"/> is null.</exception>
        public void Serialize([NotNull] CircuitXml cx, [NotNull] Stream s)
        {
            if (cx == null) throw new ArgumentNullException(nameof(cx));
            if (s == null) throw new ArgumentNullException(nameof(s));
            foreach (var node in cx.Nodes)
                cx.Links.Add(new Link(node));
            _ser.Serialize(s, cx);
        }

        /// <summary>
        /// Deserializes a circuit. 
        /// </summary>
        /// <param name="s">A <see cref="Stream"/> of XML.</param>
        /// <returns>A <see cref="CircuitXml"/> object.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="s"/> is null.</exception>
        [CanBeNull] // TODO: check null cases
        public CircuitXml Deserialize([NotNull] Stream s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
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
