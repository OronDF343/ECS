using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using ECS.Model;
using JetBrains.Annotations;

namespace ECS.Xml
{
    /// <summary>
    /// Describes all of the links to a specific <see cref="Node"/>. 
    /// </summary>
    public class Link
    {
        /// <summary>
        /// Gets the id of the linked <see cref="Node"/>.
        /// </summary>
        [XmlAttribute]
        public int NodeId { get; }

        /// <summary>
        /// Gets a list of links to specific <see cref="Component"/>s.
        /// </summary>
        [XmlArray, XmlArrayItem(nameof(ComponentLink)), NotNull]
        public List<ComponentLink> ComponentLinks { get; }

        /// <summary>
        /// Create a new link.
        /// </summary>
        /// <param name="n">The <see cref="Node"/> to link.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="n"/> is null.</exception>
        public Link([NotNull] Node n)
        {
            if (n == null) throw new ArgumentNullException(nameof(n));
            NodeId = n.Id;
            ComponentLinks = n.Components.Select(i => new ComponentLink(i, NodeId)).ToList();
        }
    }

    /// <summary>
    /// A link between a <see cref="Node"/> and a <see cref="Component"/>.
    /// </summary>
    public class ComponentLink
    {
        /// <summary>
        /// Gets the <see cref="Component"/> id.
        /// </summary>
        [XmlAttribute]
        public int Id { get; }

        /// <summary>
        /// Gets whether the <see cref="Component"/> is connceted on the positive terminal.
        /// </summary>
        [XmlAttribute]
        public bool IsPlus { get; }

        /// <summary>
        /// Gets whether the <see cref="Component"/> is a <see cref="VoltageSource"/>.
        /// </summary>
        [XmlAttribute]
        public bool IsVoltageSource { get;}

        /// <summary>
        /// Creates a component link between a <see cref="Component"/> and a <see cref="Node"/> id.
        /// </summary>
        /// <param name="c">A <see cref="Component"/>.</param>
        /// <param name="nodeId">A <see cref="Node"/> id.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="c"/> is null.</exception>
        public ComponentLink([NotNull] Component c, int nodeId)
        {
            if (c == null) throw new ArgumentNullException(nameof(c));
            Id = c.Id;
            IsPlus = c.Node1?.Id == nodeId;
            IsVoltageSource = c is VoltageSource;
        }
    }
}
