using System;
using System.Collections.Generic;
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
        public static void ModifiedNodalAnalysis(Node head, int numVars, int numSrc)
        {
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
                foreach (var c in n.Components)
                {
                    // For C#7: use switch expression patterns
                    if (c is Resistor && !c.Mark) // A resistor which we haven't visited yet
                    {
                        var r = c as Resistor;
                        a[n.Id, n.Id] += r.Conductance; // Conductance = 1/Resistance
                        var o = r.OtherNode(n); // OtherNode returns the connected node which is != n

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
                v.Current = Math.Abs(x[numVars + v.Id]); // Use absolute value since result is negative
        }
    }
}
