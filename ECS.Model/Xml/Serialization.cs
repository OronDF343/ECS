using System;
using System.IO;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace ECS.Model.Xml
{
    /// <summary>
    ///     Manages serialization of circuits.
    /// </summary>
    public class Serialization
    {
        [NotNull]
        private readonly XmlSerializer _ser = new XmlSerializer(typeof(CircuitXml));

        /// <summary>
        ///     Serializes a circuit.
        /// </summary>
        /// <param name="cx">A <see cref="CircuitXml" /> object.</param>
        /// <param name="s">A <see cref="Stream" /> which the XML will be written to.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="cx" /> or <paramref name="s" /> is null.</exception>
        public void Serialize([NotNull] CircuitXml cx, [NotNull] Stream s)
        {
            if (cx == null) throw new ArgumentNullException(nameof(cx));
            if (s == null) throw new ArgumentNullException(nameof(s));
            _ser.Serialize(s, cx);
        }

        /// <summary>
        ///     Deserializes a circuit.
        /// </summary>
        /// <param name="s">A <see cref="Stream" /> of XML.</param>
        /// <returns>A <see cref="CircuitXml" /> object.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="s" /> is null.</exception>
        [CanBeNull]
        public CircuitXml Deserialize([NotNull] Stream s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            return (CircuitXml)_ser.Deserialize(s);
        }
    }
}
