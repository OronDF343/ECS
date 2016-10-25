using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ECS.Model;
using JetBrains.Annotations;
using MathNet.Numerics.LinearAlgebra;
using Serilog;

namespace ECS.Core
{
    /// <summary>
    /// Provides circuit simulation functionality.
    /// </summary>
    public static class Simulator
    {
        /// <summary>
        /// Performs Modified Nodal Analysis (MNA) on a given circuit, and comptes all the values in the circuit.
        /// </summary>
        /// <param name="circuit">The circuit which will be analyzed.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="circuit"/> is equal to <code>null</code>.</exception>
        /// <exception cref="SimulationException">If a critical error occured during the analysis.</exception>
        public static void ModifiedNodalAnalysis([NotNull] Circuit circuit)
        {
            if (circuit == null) throw new ArgumentNullException(nameof(circuit));
            /* init structures - non-modified only:
            var a = Matrix<double>.Build.Dense(numVars, numVars);
            var b = Vector<double>.Build.Dense(numVars); */
            // init structures - modified:
            var a = Matrix<double>.Build.Dense(circuit.NodeCount + circuit.SourceCount, circuit.NodeCount + circuit.SourceCount);
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
                Log.Information("Visiting node #{0}", n.Id);
                ln.Add(n);
                n.Mark = true;
                // Check for issues
                if (n.Id >= circuit.NodeCount) throw new SimulationException("Invalid node id: " + n.Id, n);
                foreach (var c in n.Components)
                {
                    // For C#7: use switch expression patterns
                    if (c is Resistor && !c.Mark) // A resistor which we haven't visited yet
                    {
                        var r = c as Resistor;
                        Log.Information("Visiting resistor #{0} connected to node #{1}", r.Id, n.Id);
                        a[n.Id, n.Id] += r.Conductance; // Conductance = 1/Resistance
                        var o = r.OtherNode(n); // OtherNode returns the connected node which is != n

                        // Check for issues
                        if (o == null)
                        {
                            Log.Warning("Resistor #{Id} is detatched!", r.Id);
                            continue;
                        }
                        if (o.Id >= circuit.NodeCount) throw new SimulationException("Invalid node id: " + o.Id, o);

                        if (o.Id < 0) continue; // we don't want to visit reference node(s)

                        q.Enqueue(o);
                        a[o.Id, o.Id] += r.Conductance;
                        a[o.Id, n.Id] -= r.Conductance;
                        a[n.Id, o.Id] -= r.Conductance;
                    } /* non-modified (and optionally modified as well):
                    else if (c is CurrentSource) // A power source with known current (I)
                    {
                        var s = c as CurrentSource;
                        b[n.Id] += s.Current;
                    } */ // modified:
                    else if (c is VoltageSource) // A power source with known voltage (V)
                    {
                        var v = c as VoltageSource;
                        Log.Information("Visiting voltage source #{0} connected to node #{1}", v.Id, n.Id);
                        lv.Add(v);
                        // Check for issues
                        if (v.Id >= circuit.SourceCount) throw new SimulationException("Invalid voltage source id: " + v.Id, v);
                        a[circuit.NodeCount + v.Id, n.Id] = a[n.Id, circuit.NodeCount + v.Id] = Equals(v.Node1, n) ? 1 : -1; // Node1 is the node connected to the plus terminal
                        if (!v.Mark) b[circuit.NodeCount + v.Id] = v.Voltage;
                    }
                    c.Mark = true;
                }
            }

            Log.Information("The matrix:");

            for (int i = 0; i < a.RowCount; i++)
                Log.Information("{0}", a.Row(i));
            Log.Information("The vector: {0}", b);
            // solve the problem:
            var x = a.Solve(b);
            Log.Information("The result vector: {0}", x);
            // Input voltages at nodes
            foreach (var n in ln)
            {
                n.Voltage = x[n.Id];
                Log.Information("Voltage at node #{0}: {1}", n.Id, n.Voltage);
            }
            // Input current at voltage sources
            foreach (var v in lv)
            {
                v.Current = -x[circuit.NodeCount + v.Id]; // Result is in opposite direction, fix it
                Log.Information("Current at voltage source #{0}: {1}", v.Id, v.Current);
            }
        }
    }

    /// <summary>
    /// An exception that occurred during a simulation.
    /// </summary>
    [Serializable]
    public class SimulationException : Exception
    {
        /// <summary>
        /// Create a new instance of <see cref="SimulationException"/>.
        /// </summary>
        /// <param name="item">The item which caused the error.</param>
        public SimulationException([CanBeNull] object item = null)
        {
            Item = item;
        }

        /// <summary>
        /// Create a new instance of <see cref="SimulationException"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="item">The item which caused the error.</param>
        public SimulationException([CanBeNull] string message, [CanBeNull] object item = null)
            : base(message)
        {
            Item = item;
        }

        /// <summary>
        /// Create a new instance of <see cref="SimulationException"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        /// <param name="item">The item which caused the error.</param>
        public SimulationException([CanBeNull] string message, [CanBeNull] Exception innerException, [CanBeNull] object item = null)
            : base(message, innerException)
        {
            Item = item;
        }

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected SimulationException([NotNull] SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        /// <summary>
        /// Gets the item which caused the error.
        /// </summary>
        [CanBeNull]
        public object Item { get; }
    }
}
