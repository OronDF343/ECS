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
        /// Performs Modified Nodal Analysis (MNA) on a given circuit.
        /// </summary>
        /// <param name="head">The starting node. Must be different than <paramref name="refNode"/>.</param>
        /// <param name="refNode">The reference node (connected to ground).</param>
        /// <param name="numVars">The number of nodes excluding <paramref name="refNode"/> (with unknown voltage).</param>
        /// <param name="numSrc">The number of voltage sources (with unknown current).</param>
        /// <returns>A vector containing the voltage of each node followed by the total current of each voltage source, ordered by ID.</returns>
        public static Vector<double> ModifiedNodalAnalysis(Node head, Node refNode, int numVars, int numSrc)
        {
            /* init structures - non-modified only:
            var a = Matrix<double>.Build.Dense(numVars, numVars);
            var b = Vector<double>.Build.Dense(numVars); */
            // init structures - modified:
            var a = Matrix<double>.Build.Dense(numVars + numSrc, numVars + numSrc);
            var b = Vector<double>.Build.Dense(numVars + numSrc);

            // do BFS
            var q = new Queue<Node>();
            q.Enqueue(head);
            while (q.Count > 0)
            {
                var n = q.Dequeue();
                n.Mark = true;
                foreach (var c in n.Components)
                {
                    c.Mark = true;
                    // For C#7: use switch expression patterns
                    if (c is Resistor && !c.Mark) // A resistor which we haven't visited yet
                    {
                        var r = c as Resistor;
                        a[n.Id, n.Id] += r.Conductance; // Conductance = 1/Resistance
                        var o = r.OtherNode(n); // OtherNode returns the connected node which is != n
                        if (o != refNode) // we don't want to visit refNode
                        {
                            q.Enqueue(o);
                            a[o.Id, o.Id] += r.Conductance;
                            a[o.Id, n.Id] -= r.Conductance;
                            a[n.Id, o.Id] -= r.Conductance;
                        }
                    } /* non-modified (and optionally modified as well):
                    else if (c is CurrentSource) // A power source with known current (I)
                    {
                        var s = c as CurrentSource;
                        b[n.Id] += s.Current;
                    } */ // modified:
                    else if (c is VoltageSource) // A power source with known voltage (V)
                    {
                        var v = c as VoltageSource;
                        a[numVars + v.Id, n.Id] = a[n.Id, numVars + v.Id] = v.Node1 == n ? 1 : -1; // Node1 is the node connected to the plus terminal
                        if (!v.Mark) b[numVars + v.Id] = v.Voltage;
                    }
                }
            }
            // solve the problem:
            return a.Solve(b);
        }
    }
}
