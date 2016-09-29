using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MathNet.Numerics.LinearAlgebra;

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
        /// <param name="head">The starting node. Must be different than the reference node(s).</param>
        /// <param name="numVars">The number of nodes with unknown voltage (excluding the reference node(s)).</param>
        /// <param name="numSrc">The number of voltage sources (with unknown current).</param>
        /// <exception cref="ArgumentException">If <paramref name="head"/> is a reference node.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="numVars"/> or <paramref name="numSrc"/> are less than 1.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="head"/> is equal to <code>null</code>.</exception>
        /// <exception cref="SimulationException">If a critical error occured during the analysis.</exception>
        public static void ModifiedNodalAnalysis([NotNull] Node head, int numVars, int numSrc)
        {
            if (head == null) throw new ArgumentNullException(nameof(head));
            if (head.Id < 0) throw new ArgumentException("Starting node must not be a reference node", nameof(head));
            if (numVars < 1) throw new ArgumentOutOfRangeException(nameof(numVars), numVars, "Number of nodes must be greater than zero");
            if (numSrc < 1) throw new ArgumentOutOfRangeException(nameof(numSrc), numSrc, "Number of voltage sources must be greater than zero");
            /* init structures - non-modified only:
            var a = Matrix<double>.Build.Dense(numVars, numVars);
            var b = Vector<double>.Build.Dense(numVars); */
            // init structures - modified:
            var a = Matrix<double>.Build.Dense(numVars + numSrc, numVars + numSrc);
            var b = Vector<double>.Build.Dense(numVars + numSrc);

            // keep track of visited nodes and voltage sources for later
            var ln = new List<Node>();
            var lv = new List<VoltageSource>();

            // do BFS
            var q = new Queue<Node>();
            q.Enqueue(head);
            while (q.Count > 0)
            {
                var n = q.Dequeue();
                ln.Add(n);
                n.Mark = true;
                // Check for issues
                if (n.Id >= numVars) throw new SimulationException("Invalid node id: " + n.Id, n);
                foreach (var c in n.Components)
                {
                    // For C#7: use switch expression patterns
                    if (c is Resistor && !c.Mark) // A resistor which we haven't visited yet
                    {
                        var r = c as Resistor;
                        a[n.Id, n.Id] += r.Conductance; // Conductance = 1/Resistance
                        var o = r.OtherNode(n); // OtherNode returns the connected node which is != n

                        // Check for issues
                        if (o == null)
                        {
                            // TODO: Print warning to log instead of CMD
                            Console.WriteLine("[MNA] Found detatched resistor with id " + r.Id);
                            continue;
                        }
                        if (o.Id >= numVars) throw new SimulationException("Invalid node id: " + o.Id, o);

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
                        lv.Add(v);
                        // Check for issues
                        if (v.Id >= numSrc) throw new SimulationException("Invalid voltage source id: " + v.Id, v);
                        a[numVars + v.Id, n.Id] = a[n.Id, numVars + v.Id] = Equals(v.Node1, n) ? 1 : -1; // Node1 is the node connected to the plus terminal
                        if (!v.Mark) b[numVars + v.Id] = v.Voltage;
                    }
                    c.Mark = true;
                }
            }
            // solve the problem:
            var x = a.Solve(b);
            // Input voltages at nodes
            foreach (var n in ln)
                n.Voltage = x[n.Id];
            // Input current at voltage sources
            foreach (var v in lv)
                v.Current = -x[numVars + v.Id]; // Result is in opposite direction, fix it
        }
    }

    public class SimulationException : Exception
    {
        public SimulationException(object item = null)
        {
            Item = item;
        }

        public SimulationException(string message, object item = null)
            : base(message)
        {
            Item = item;
        }

        public SimulationException(string message, Exception innerException, object item = null)
            : base(message, innerException)
        {
            Item = item;
        }

        public object Item { get; }
    }
}
