using System;
using System.Collections.Generic;
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
        /// <summary>
        ///     Performs Modified Nodal Analysis (MNA) on a given circuit, and computes all the values in the circuit.
        /// </summary>
        /// <param name="circuit">The circuit which will be analyzed.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="circuit" /> is equal to <code>null</code>.</exception>
        /// <exception cref="SimulationException">If a critical error occured during the analysis.</exception>
        public static void ModifiedNodalAnalysis([NotNull] SimulationCircuit circuit)
        {
            if (circuit == null) throw new ArgumentNullException(nameof(circuit));
            /* init structures - non-modified only:
            var a = Matrix<double>.Build.Dense(numVars, numVars);
            var b = Vector<double>.Build.Dense(numVars); */
            // init structures - modified:
            var a = Matrix<double>.Build.Dense(circuit.NodeCount + circuit.SourceCount,
                                               circuit.NodeCount + circuit.SourceCount);
            var b = Vector<double>.Build.Dense(circuit.NodeCount + circuit.SourceCount);

            // keep track of visited nodes and voltage sources for later
            var ln = new List<Node>();
            var lv = new List<VoltageSource>();

            Log.Information("Starting simulation");

            // do BFS
            var q = new Queue<Node>();
            q.Enqueue(circuit.Head);
            while (q.Count > 0)
            {
                var n = q.Dequeue();
                Log.Information("Visiting node {0}", n.ToString());
                ln.Add(n);
                n.Mark = true;
                // Check for issues
                if (n.SimulationIndex >= circuit.NodeCount) throw new SimulationException("Invalid index for node {0}" + n, n);
                var components = new Queue<Link>(n.Links);
                while (components.Count > 0)
                {
                    var c = components.Dequeue();
                    // For C#7: use switch expression patterns
                    // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
                    if (c.Component is Resistor && !c.Component.Mark) // A resistor which we haven't visited yet
                    {
                        var r = (Resistor)c.Component;
                        Log.Information("Visiting resistor {0} connected to node {1}", r.ToString(), n.ToString());
                        a[n.SimulationIndex, n.SimulationIndex] += r.Conductance; // Conductance = 1/Resistance
                        // Get node connected to OTHER side of this component!
                        var o = c.IsPositive ? r.Node2 : r.Node1;

                        // Check for issues
                        if (o == null)
                        {
                            Log.Warning("Resistor {0} is detached!", r.ToString());
                            continue;
                        }
                        if (o.SimulationIndex >= circuit.NodeCount) throw new SimulationException("Invalid index for node " + o, o);

                        if (o.SimulationIndex < 0) continue; // we don't want to visit reference node(s)

                        q.Enqueue(o);
                        a[o.SimulationIndex, o.SimulationIndex] += r.Conductance;
                        a[o.SimulationIndex, n.SimulationIndex] -= r.Conductance;
                        a[n.SimulationIndex, o.SimulationIndex] -= r.Conductance;
                    } /* non-modified (and optionally modified as well):
                    else if (c is CurrentSource) // A power source with known current (I)
                    {
                        var s = c as CurrentSource;
                        b[n.Id] += s.Current;
                    } */ // modified:
                    else if (c.Component is VoltageSource) // A power source with known voltage (V)
                    {
                        var v = (VoltageSource)c.Component;
                        Log.Information("Visiting voltage source {0} connected to node {1}", v.ToString(), n.ToString());
                        lv.Add(v);
                        // Check for issues
                        if (v.SimulationIndex >= circuit.SourceCount) throw new SimulationException("Invalid index for voltage source " + v, v);
                        a[circuit.NodeCount + v.SimulationIndex, n.SimulationIndex] =
                            a[n.SimulationIndex, circuit.NodeCount + v.SimulationIndex] = Equals(v.Node1, n) ? 1 : -1;
                        // Node1 is the node connected to the plus terminal
                        if (!v.Mark) b[circuit.NodeCount + v.SimulationIndex] = v.Voltage;
                    }
                    else if (c.Component is Switch && !c.Component.Mark) // TODO: Make the switch handling actually work...
                    {
                        // *** Handle a switch like a resistor with a resistance of 0 ohms ***
                        var s = (Switch)c.Component;
                        Log.Information("Visiting switch {0} connected to node {1}", s.ToString(), n.ToString());
                        if (s.IsClosed)
                        {
                            Log.Information("Switch {0} is closed, proceeding", s.ToString());
                            a[n.SimulationIndex, n.SimulationIndex] = double.PositiveInfinity;
                            // Get node connected to OTHER side of this component!
                            var o = c.IsPositive ? s.Node2 : s.Node1;

                            // Check for issues
                            if (o == null)
                            {
                                Log.Warning("Switch {0} is detached!", s.ToString());
                                continue;
                            }
                            if (o.SimulationIndex >= circuit.NodeCount) throw new SimulationException("Invalid index for node " + o, o);

                            if (o.SimulationIndex < 0) continue; // we don't want to visit reference node(s)

                            q.Enqueue(o);
                            a[o.SimulationIndex, o.SimulationIndex] = double.PositiveInfinity;
                            a[o.SimulationIndex, n.SimulationIndex] = double.NegativeInfinity;
                            a[n.SimulationIndex, o.SimulationIndex] = double.NegativeInfinity;
                        }
                    }
                    c.Component.Mark = true;
                }
            }

            Log.Information("The matrix:");

            for (var i = 0; i < a.RowCount; i++) Log.Information("{0}", a.Row(i));
            Log.Information("The vector: {0}", b);
            // Solve the linear equation system:
            var x = a.Solve(b);
            Log.Information("The result vector: {0}", x);
            // Input voltages at nodes
            foreach (var n in ln)
            {
                n.Voltage = x[n.SimulationIndex];
                Log.Information("Voltage at node {0}: {1}", n.ToString(), n.Voltage);
            }
            // Input current at voltage sources
            foreach (var v in lv)
            {
                v.Current = -x[circuit.NodeCount + v.SimulationIndex]; // Result is in opposite direction, fix it
                Log.Information("Current at voltage source {0}: {1}", v.ToString(), v.Current);
            }
        }
    }

    /// <summary>
    ///     An exception that occurred during a simulation.
    /// </summary>
    [Serializable]
    public class SimulationException : Exception
    {
        /// <summary>
        ///     Create a new instance of <see cref="SimulationException" />.
        /// </summary>
        /// <param name="item">The item which caused the error.</param>
        public SimulationException([CanBeNull] object item = null)
        {
            Item = item;
        }

        /// <summary>
        ///     Create a new instance of <see cref="SimulationException" />.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="item">The item which caused the error.</param>
        public SimulationException([CanBeNull] string message, [CanBeNull] object item = null)
            : base(message)
        {
            Item = item;
        }

        /// <summary>
        ///     Create a new instance of <see cref="SimulationException" />.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or a null reference (Nothing in
        ///     Visual Basic) if no inner exception is specified.
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
        /// <exception cref="SerializationException">The class name is null or <see cref="P:System.Exception.HResult" /> is zero (0). </exception>
        /// <exception cref="ArgumentNullException">The <paramref name="info" /> parameter is null. </exception>
        protected SimulationException([NotNull] SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        /// <summary>
        ///     Gets the item which caused the error.
        /// </summary>
        [CanBeNull]
        public object Item { get; }
    }
}
