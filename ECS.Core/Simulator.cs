using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ECS.Core.Model;
using JetBrains.Annotations;
using MathNet.Numerics.LinearAlgebra;
using Serilog;

namespace ECS.Core
{
    /// <summary>
    ///     Provides circuit simulation functionality.
    /// </summary>
    public static class Simulator
    {
        public static void AnalyzeAndUpdate([NotNull] IEnumerable<INode> nodes, [NotNull] IEnumerable<IComponent> components)
        {
            // Some middleware to make the simulation code more flexible
            var nodesList = new List<INode>(nodes);
            var componentsList = new List<IComponent>(components);

            // *** Build circuit for simulation ***
            int rId = 0, vsId = 0, nId = 0, rnId = -1;
            INode h = null;
            // Clean the nodes
            nodesList.ForEach(n =>
            {
                // Clear previous links
                n.Links.Clear();
                // Set default values
                n.EquivalentNode = null;
                n.SimulationIndex = int.MinValue;
                n.Mark = false;
                // Get first non-reference node:
                if (!n.IsReferenceNode && h == null) h = n;
            });
            if (h == null) throw new InvalidOperationException("Missing non-reference node!");

            // Link all components and assign them indexes
            foreach (var c in componentsList.Where(i => !(i is ISwitch)))
            {
                c.Mark = false;
                // Create relevant links
                c.Node1?.Links.Add(new Link(c, true));
                c.Node2?.Links.Add(new Link(c, false));
                // Assign an index
                if (c is IResistor) c.SimulationIndex = rId++;
                else if (c is IVoltageSource) c.SimulationIndex = vsId++;
            }

            // Switch handling: Closed switch will merge the Nodes it's connected to. Open ones are ignored.
            foreach (var sw in componentsList.OfType<ISwitch>().ToList())
            {
                if (sw.IsClosed)
                {
                    // Merge the nodes
                    sw.Node2.EquivalentNode = sw.Node1;
                    foreach (var link in sw.Node2.Links)
                        sw.Node1.Links.Add(link);
                }
                componentsList.Remove(sw);
            }

            // Assign indexes to nodes
            nodesList.ForEach(n =>
            {
                if (n.EquivalentNode == null) n.SimulationIndex = n.IsReferenceNode ? rnId++ : nId++;
            });

            // Circuit is ready
            var circuit = new SimulationCircuit(h, nId, vsId);

            // Do simultaion
            var result = ModifiedNodalAnalysis(circuit);

            Log.Information("Updating circuit elements");
            // Input voltages at nodes
            foreach (var n in nodesList.Where(n => n.SimulationIndex > -1))
            {
                n.Voltage = result[n.SimulationIndex];
                Log.Information("Voltage at node {0}: {1}", n.ToString(), n.Voltage);
            }
            // Update merged nodes
            foreach (var n in nodesList.Where(n => n.EquivalentNode != null))
            {
                n.Voltage = n.EquivalentNode.Voltage;
                Log.Information("Voltage at node {0} is the same as at node {1}", n.EquivalentNode.ToString(),
                                n.ToString());
            }
            // Input current at voltage sources
            foreach (var v in componentsList.OfType<IVoltageSource>())
            {
                v.Current = -result[circuit.NodeCount + v.SimulationIndex]; // Result is in opposite direction, fix it
                Log.Information("Current at voltage source {0}: {1}", v.ToString(), v.Current);
            }

            // Update resistor information
            Log.Information("Updating resistor information");
            foreach (var r in componentsList.OfType<IResistor>())
            {
                r.Voltage = Math.Abs((r.Node1?.Voltage ?? 0) - (r.Node2?.Voltage ?? 0));
                if (r.Resistance > 0) r.Current = r.Voltage / r.Resistance;
                else r.Resistance = r.Voltage / r.Current;
            }
        }

        /// <summary>
        ///     Performs Modified Nodal Analysis (MNA) on a given circuit, and
        ///     computes all the values in the circuit.
        /// </summary>
        /// <param name="circuit">The circuit which will be analyzed.</param>
        /// <exception cref="ArgumentNullException">
        ///     <para>If <paramref name="circuit" /> is equal to</para>
        ///     <code>null</code>
        ///     <para>.</para>
        /// </exception>
        /// <exception cref="SimulationException">
        ///     If a critical error occured during the analysis.
        /// </exception>
        private static Vector<double> ModifiedNodalAnalysis([NotNull] SimulationCircuit circuit)
        {
            if (circuit == null) throw new ArgumentNullException(nameof(circuit));
            /* init structures - non-modified only:
            var a = Matrix<double>.Build.Dense(numVars, numVars);
            var b = Vector<double>.Build.Dense(numVars); */
            // init structures - modified:
            var a = Matrix<double>.Build.Dense(circuit.NodeCount + circuit.SourceCount,
                                               circuit.NodeCount + circuit.SourceCount);
            var b = Vector<double>.Build.Dense(circuit.NodeCount + circuit.SourceCount);
            
            Log.Information("Starting simulation");

            // do BFS
            var q = new Queue<INode>();
            q.Enqueue(circuit.Head);
            while (q.Count > 0)
            {
                var n = q.Dequeue();
                Log.Information("Visiting node {0}", n.ToString());
                // Check for issues
                if (n.SimulationIndex >= circuit.NodeCount) throw new SimulationException("Invalid index for node {0}" + n, n);
                var components = new Queue<Link>(n.Links);
                while (components.Count > 0)
                {
                    var c = components.Dequeue();
                    // For C#7: Should use switch expression patterns here
                    // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
                    if (c.Component is IResistor)
                    {
                        var r = (IResistor)c.Component;
                        Log.Information("Visiting resistor {0} connected to node {1}", r.ToString(), n.ToString());

                        if (r.Resistance > 0 && !c.Component.Mark)
                        {
                            Log.Information("Resistor {0} has known resistance, adding to matrix A", r.ToString());
                            a[n.SimulationIndex, n.SimulationIndex] += r.Conductance; // Conductance = 1/Resistance

                            // Get node connected to OTHER side of this component!
                            var o = c.IsPositive ? r.Node2 : r.Node1;
                            // Use alternate node if available
                            if (o.EquivalentNode != null) o = o.EquivalentNode;

                            // Check for issues
                            if (o == null)
                            {
                                Log.Warning("Resistor {0} is detached!", r.ToString());
                                continue;
                            }
                            if (o.SimulationIndex >= circuit.NodeCount) throw new SimulationException("Invalid index for node " + o, o);

                            // we don't want to visit reference node(s)
                            if (o.SimulationIndex >= 0)
                            {
                                if (!o.Mark)
                                {
                                    q.Enqueue(o);
                                    o.Mark = true;
                                }
                                a[o.SimulationIndex, o.SimulationIndex] += r.Conductance;
                                a[o.SimulationIndex, n.SimulationIndex] -= r.Conductance;
                                a[n.SimulationIndex, o.SimulationIndex] -= r.Conductance;
                            }
                        }
                        else if (r.Current > 0 && r.Resistance <= 0)
                        {
                            Log.Information("Resistor {0} has known current, adding to vector B", r.ToString());

                            // Get node connected to OTHER side of this component!
                            var o = c.IsPositive ? r.Node2 : r.Node1;
                            // Use alternate node if available
                            if (o.EquivalentNode != null) o = o.EquivalentNode;
                            /*
                            // Check for issues
                            if (o == null)
                            {
                                Log.Warning("Resistor {0} is detached!", r.ToString());
                                continue;
                            }
                            if (o.SimulationIndex >= circuit.NodeCount)
                                throw new SimulationException("Invalid index for node " + o, o);
                                */
                            if (!o.Mark && o.SimulationIndex >= 0)
                            {
                                q.Enqueue(o);
                                o.Mark = true;
                            }

                            b[n.SimulationIndex] += c.IsPositive ? -r.Current : r.Current;
                        }
                    }
                    else if (c.Component is IVoltageSource) // A power source with known voltage (V)
                    {
                        var v = (IVoltageSource)c.Component;
                        Log.Information("Visiting voltage source {0} connected to node {1}", v.ToString(), n.ToString());
                        // Check for issues
                        if (v.SimulationIndex >= circuit.SourceCount) throw new SimulationException("Invalid index for voltage source " + v, v);
                        a[circuit.NodeCount + v.SimulationIndex, n.SimulationIndex] =
                            a[n.SimulationIndex, circuit.NodeCount + v.SimulationIndex] = Equals(v.Node1, n) || Equals(v.Node1.EquivalentNode, n) ? 1 : -1;
                        // Node1 is the node connected to the plus terminal
                        if (!v.Mark) b[circuit.NodeCount + v.SimulationIndex] = v.Voltage;
                    }
                    else if (c.Component is ISwitch && !c.Component.Mark)
                    {
                        Log.Warning("Switch not removed: {0}" + c.Component);
                    }
                    c.Component.Mark = true;
                }
                n.Mark = true;
            }

            Log.Information("The matrix:");

            for (var i = 0; i < a.RowCount; i++) Log.Information("{0}", a.Row(i));
            Log.Information("The vector: {0}", b);
            // Solve the linear equation system:
            var x = a.Solve(b);
            Log.Information("The result vector: {0}", x);
            return x;
        }
    }

    /// <summary>
    ///     An exception that occurred during a simulation.
    /// </summary>
    [Serializable]
    public class SimulationException : Exception
    {
        /// <summary>
        ///     Create a new instance of <see cref="SimulationException" /> .
        /// </summary>
        /// <param name="item">The item which caused the error.</param>
        public SimulationException([CanBeNull] object item = null)
        {
            Item = item;
        }

        /// <summary>
        ///     Create a new instance of <see cref="SimulationException" /> .
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="item">The item which caused the error.</param>
        public SimulationException([CanBeNull] string message, [CanBeNull] object item = null)
            : base(message)
        {
            Item = item;
        }

        /// <summary>
        ///     Create a new instance of <see cref="SimulationException" /> .
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or a
        ///     <see langword="null" /> reference (Nothing in Visual Basic) if no
        ///     inner exception is specified.
        /// </param>
        /// <param name="item">The item which caused the error.</param>
        public SimulationException([CanBeNull] string message, [CanBeNull] Exception innerException,
                                   [CanBeNull] object item = null)
            : base(message, innerException)
        {
            Item = item;
        }

        /// <summary>
        ///     Serialization constructor.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// <exception cref="SerializationException">
        ///     The class name is <see langword="null" /> or
        ///     <see cref="System.Exception.HResult" /> is zero (0).
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="info" /> parameter is null.
        /// </exception>
        protected SimulationException([NotNull] SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        /// <summary>
        ///     Gets the item which caused the error.
        /// </summary>
        [CanBeNull]
        public object Item { get; }
    }
}
