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
        /// <param name="s">
        ///     A <see cref="Stream" /> which the XML will be written to.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="cx" /> or <paramref name="s" /> is null.
        /// </exception>
        public void Serialize([NotNull] CircuitXml cx, [NotNull] Stream s)
        {
            if (cx == null) throw new ArgumentNullException(nameof(cx));
            if (s == null) throw new ArgumentNullException(nameof(s));
            _ser.Serialize(s, cx);
        }

        /// <summary>
        ///     Serializes a circuit.
        /// </summary>
        /// <param name="cx">A <see cref="CircuitXml" /> object.</param>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="cx" /> is null.
        /// </exception>
        public string Serialize([NotNull] CircuitXml cx)
        {
            if (cx == null) throw new ArgumentNullException(nameof(cx));
            string r;
            using (var sw = new StringWriter())
            {
                _ser.Serialize(sw, cx);
                r = sw.ToString();
            }
            return r;
        }

        /// <summary>
        ///     Deserializes a circuit.
        /// </summary>
        /// <param name="s">A <see cref="Stream" /> of XML.</param>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="s" /> is null.
        /// </exception>
        /// <returns>
        ///     A <see cref="CircuitXml" /> object.
        /// </returns>
        [CanBeNull]
        public CircuitXml Deserialize([NotNull] Stream s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            return (CircuitXml)_ser.Deserialize(s);
        }

        /// <summary>
        ///     Deserializes a circuit.
        /// </summary>
        /// <param name="s">A <see cref="string" /> of XML.</param>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="s" /> is null.
        /// </exception>
        /// <returns>
        ///     A <see cref="CircuitXml" /> object.
        /// </returns>
        [CanBeNull]
        public CircuitXml Deserialize([NotNull] string s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            CircuitXml cx;
            using (var sr = new StringReader(s))
            {
                cx = (CircuitXml)_ser.Deserialize(sr);
            }
            return cx;
        }
    }
}
