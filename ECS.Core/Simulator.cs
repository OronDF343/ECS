using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ECS.Core.Model;
using JetBrains.Annotations;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Serilog;

namespace ECS.Core
{
    /// <summary>
    ///     Provides circuit simulation functionality.
    /// </summary>
    public static class Simulator
    {
        public static void AnalyzeAndUpdate([NotNull] IEnumerable<INode> nodes,
                                            [NotNull] IEnumerable<IComponent> components)
        {
            // Some middleware to make the simulation code more flexible
            var nodesList = new List<INode>(nodes);
            var componentsList = new List<IComponent>(components);

            // *** Build circuit for simulation ***
            int rId = 0, vsId = 0, nId = 0, rnId = -1;
            INode h = null, refNode = null;
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
                else if (n.IsReferenceNode && refNode == null) refNode = n;
            });
            if (h == null) throw new InvalidOperationException("Missing non-reference node!");
            if (refNode == null) throw new InvalidOperationException("Missing reference node!");

            // Link all components in default direction and assign them indexes
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
                    foreach (var link in sw.Node2.Links) sw.Node1.Links.Add(link);
                }
                componentsList.Remove(sw);
            }

            // Assign indexes to nodes
            nodesList.ForEach(n =>
            {
                if (n.EquivalentNode == null) n.SimulationIndex = n.IsReferenceNode ? rnId++ : nId++;
            });

            // Circuit is ready
            // Ninja-fix: Use h.OrEquivalent just in case it's rght on the right of a switch
            var circuit = new SimulationCircuit(h.OrEquivalent, nId, vsId);

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
                r.Voltage = (r.Node1?.Voltage ?? 0) - (r.Node2?.Voltage ?? 0);
                if (r.Resistance > 0) r.Current = r.Voltage / r.Resistance;
                else r.Resistance = r.Voltage / r.Current;
            }
        }

        /// <summary>
        ///     Performs Modified Nodal Analysis (MNA) on a given circuit, and
        ///     computes all the values in the circuit.
        /// </summary>
        /// <remarks>
        ///     How it works:
        ///     There are N nodes.
        ///     The equations in the first N rows of A are of the form Sigma(Vn-Vm / Rn) + Sigma(+/- Iv) = Sigma(+/- Ir)
        ///     * For a node numbered "n",
        ///     Rn are resistors connected between this node and others,
        ///     Vn is the voltage at this node,
        ///     Vm is the voltage at the node on the other side of the resistor,
        ///     * So Sigma(Vn-Vm / Rn) is all the current flowing through *resistors* via this node!
        ///     * In each specific case (Vn-Vm1 / Rn1) the result will be negative if the current flows *out* of this node!
        ///     Iv is the current flowing through this node from/to a connected voltage source
        ///     * If Iv flows out of this node we need to subtract it! (If we are connected to the positive side of the voltage
        ///     source it is flowing in)
        ///     Ir is the current applied by a current source (or in out case via a resistor with unknown R)
        ///     * The same rule from Iv applies to Ir (but reversed since we are on the other side of the equation)
        ///     The equations are based on Kirchoff's law: the sum of currents flowing in is equal to the sum of currents flowing
        ///     out,
        ///     So SIi = SIo is the same as SIi - SIo = 0
        ///     As long as we move stuff between sides of the function correctly, the equation remains true.
        ///     MNA takes advantage of this and puts it into a matrix by grouping the variables in a helpful way.
        ///     How can we treat resistors with known I but unknown R as current sources?
        ///     The equation uses Ohm's law: I = (1/R) * V (b=Ax)
        ///     What we did is reversed that. The matrix requires 1/R to be known but we don't have that.
        ///     So instead we said (1/R) * V = I and used I instead on the other side of the equation (vector B)
        ///     And that is why a current source and a resistor are the same thing here: Both follow Ohm's law.
        ///     The rest of the rows in A:
        ///     There are S voltage sources.
        ///     For the last S rows in A: Vp-Vn = Vs
        ///     * For a node numbered "s",
        ///     Vp is the voltage at the node connected to the *positive* side of the voltage source
        ///     Vn is the voltage at the node connected to the *negative* side of the voltage source
        ///     Vs is the voltage of the source "s"
        ///     We need more than N equations since the current through each voltage source is also an unknown
        ///     This equation is much simpler - it is trivial
        ///     The voltage of "s" is the voltage across "s". So the difference between Vp and Vn is ALWAYS Vs.
        ///     Vp and Vn are both unknowns, therefore we can use this equation to figure them out.
        ///     "Wait a minute. Can't we just set Vp to Vs and Vn to 0 ahead of time?"
        ///     - NO. Voltages are relative to the reference point (node) therefore Vp-Vn=Vs is possible even with other values and
        ///     that is perfectly legitimate.
        ///     It is also common IRL: Two AA batteries in series. For one, Vp1=1.5V and Vn1=0V, for the other Vp2=3V and Vn2=1.5V.
        ///     It really just depends where the reference point is.
        ///     "Do we need a reference node?"
        ///     YES. If we have two voltage sources then as with the example above we can't always set Vn=0V unless we pick a
        ///     reference node.
        ///     Ideally the user should choose it, but we can do it for them as well.
        /// </remarks>
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

            var ndict = new Dictionary<int, INode>();
            // do BFS
            var q = new Queue<INode>();
            q.Enqueue(circuit.Head);
            while (q.Count > 0)
            {
                var n = q.Dequeue();
                ndict.Add(n.SimulationIndex, n);
                Log.Information("Visiting node {0}", n.ToString());
                // Check for issues
                if (n.SimulationIndex >= circuit.NodeCount)
                    throw new SimulationException("Invalid index for node {0}" + n, n);
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
                            var o = c.OtherNode(n).OrEquivalent;

                            // Check for issues
                            if (o == null)
                            {
                                Log.Warning("Resistor {0} is detached!", r.ToString());
                                continue;
                            }
                            if (o.SimulationIndex >= circuit.NodeCount)
                                throw new SimulationException("Invalid index for node " + o, o);

                            // we don't want to visit reference node(s)
                            if (!o.IsReferenceNode)
                            {
                                // Add the node - if it hasn't been added yet!
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
                        // Handle a case when the current is known but not the resistance.
                        else if (r.Current != 0 && r.Resistance <= 0)
                        {
                            Log.Information("Resistor {0} has known current, adding to vector B", r.ToString());

                            // Get node connected to OTHER side of this component!
                            var o = c.OtherNode(n).OrEquivalent;

                            // Check for issues
                            if (o == null)
                            {
                                Log.Warning("Resistor {0} is detached!", r.ToString());
                                continue;
                            }
                            if (o.SimulationIndex >= circuit.NodeCount)
                                throw new SimulationException("Invalid index for node " + o, o);
                            // we don't want to visit reference node(s)
                            // Add the node - if it hasn't been added yet!
                            if (!o.Mark && !o.IsReferenceNode)
                            {
                                q.Enqueue(o);
                                o.Mark = true;
                            }

                            // Treat the resistor as if it is a current source
                            b[n.SimulationIndex] += c.IsPositive ? -r.Current : r.Current;
                        }
                    }
                    else if (c.Component is IVoltageSource) // A power source with known voltage (V)
                    {
                        var v = (IVoltageSource)c.Component;
                        Log.Information("Visiting voltage source {0} connected to node {1}", v.ToString(),
                                        n.ToString());
                        // Check for issues
                        if (v.SimulationIndex >= circuit.SourceCount)
                            throw new SimulationException("Invalid index for voltage source " + v, v);
                        a[circuit.NodeCount + v.SimulationIndex, n.SimulationIndex] =
                            a[n.SimulationIndex, circuit.NodeCount + v.SimulationIndex] =
                                Equals(v.Node1?.OrEquivalent, n) ? 1 : -1;
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
            // Solve the linear equation system
            // Compute A+
            var ap = a.PseudoInverse();
            // Check if there actually is a solution (A*A+*b==b)
            // BUG: MathNet issue causes this check to fail sometimes, so the fix is to be imprecise
            if (!b.AlmostEqual(a * ap * b, 1e-13)) throw new SimulationException("No solution found!");
            // Compute A+*b
            var apb = ap * b;
            // Compute I-A+*A
            var identity = Matrix<double>.Build.DenseIdentity(a.RowCount, a.ColumnCount);
            var f = identity - ap * a;
            Vector<double> x;
            // If I-A+*A is zero, then there is a single solution
            // BUG: Same issue as above causes this check to fail sometimes, same fix
            if (!f.Exists(v => Math.Abs(v) > 1e-13)) { x = apb; }
            else
            {
                // Find "optimal" solution for indeterminate linear equation system
                // Vector w can contain any values, it is multiplied by f then added to apb to get x
                // In our case, we want:
                // 1. No resistors with negative resistance values
                // 2. "Average" resistor values (multiples of the same value, use same values where possible, etc)
                // How do we do this?
                // Put identical voltage drops over the unknown resistors, by manipulating the node values.
                // Some node values can't be changed by w (has a corresponding zero row in f).
                // There is (TODO: at least or exactly?) one of these, let's call it fn.
                // Some rows of f are identical which means that the two nodes have a constant
                // voltage drop between them, which mens the resistance is known.
                // We will take that voltage drop into account and divide the rest of the drop
                // from fn to 0 (reference) between the other nodes.
                // This algorithm is difficult to understand, but *should* work.

                // Clean up values of f
                for (var i = 0; i < f.RowCount; ++i)
                    for (var j = 0; j < f.ColumnCount; ++j)
                        f[i, j] = Math.Round(f[i, j], 13);

                // Get linearly-dependent rows of f and the (constant) voltage drop/rise
                var dep = (from col in f.Kernel()
                           select col.EnumerateIndexed(Zeros.AllowSkip).ToList()
                           into ld
                           where ld.Count >= 2
                           select new Tuple<int, int, double>(ld[0].Item1, ld[1].Item1, apb[ld[0].Item1] - apb[ld[1].Item1])).ToList();
                
                // Should be for each path
                //var r = apb[2] - dep.Sum(t => t.Item3);
                // Why 3? Number of Nodes on path
                //r /= 3;

                // We need to find a desired vector w which will give us the values we want
                var desiredw = Vector<double>.Build.Dense(apb.Count);

                // This list stores the path traversed in the search
                var update = new List<Tuple<int, int>>();
                // This is the number of nodes that have a non-zero row in f
                var div = 0;

                // Start at Node fn which has a zero row in f
                // Usually on the positive side of a voltage source
                // desiredw is zero, so this works to find a zero row
                var si = f.EnumerateRowsIndexed().FirstOrDefault(r => r.Item2.AlmostEqual(desiredw, 1e-13)).Item1;
                var fn = ndict[si];
                // Save the (constant) voltage of fn
                var diff = apb[fn.SimulationIndex];
                // Save voltage of fn. No effect on result, just used in update code.
                desiredw[fn.SimulationIndex] = apb[fn.SimulationIndex];
                // All nodes were marked previously, so treat marked as unmarked and vice versa
                fn.Mark = false;
                // Reuse the old queue
                q.Enqueue(fn);
                while (q.Count > 0)
                {
                    var n = q.Dequeue();
                    // No reference nodes will be here - if any was visited the simulation would have crashed a long time ago!
                    
                    // Traverse through the circuit
                    foreach (var l1 in n.Links)
                    {
                        // Do not traverse voltage sources (TODO: ???)
                        if (!(l1.Component is IResistor)) continue;
                        // Get node on other side
                        var o = l1.OtherNode(n).OrEquivalent;
                        // Visit each node only once!
                        // Also, we can reach reference nodes. Avoid them as usual.
                        if (o.IsReferenceNode || !o.Mark) continue;
                        
                        var depInf =
                            dep.FirstOrDefault(d => d.Item1 == n.SimulationIndex && d.Item2 == o.SimulationIndex);
                        var depInf2 =
                            dep.FirstOrDefault(d => d.Item2 == n.SimulationIndex && d.Item1 == o.SimulationIndex);
                        if (depInf != null)
                        {
                            diff -= depInf.Item3;
                            desiredw[o.SimulationIndex] = -depInf.Item3;
                            update.Add(new Tuple<int, int>(o.SimulationIndex, n.SimulationIndex));
                        }
                        else if (depInf2 != null)
                        {
                            diff += depInf2.Item3;
                            desiredw[o.SimulationIndex] = depInf2.Item3;
                            update.Add(new Tuple<int, int>(o.SimulationIndex, n.SimulationIndex));
                        }

                        // Did we just pass over a 
                        /*var r = l1.Component as IResistor;
                        if (r != null && r.Resistance > 0)
                        {
                            var i = apb[o.SimulationIndex] - apb[n.SimulationIndex];
                            diff += i;
                            desiredw[o.SimulationIndex] = i;
                            update.Add(new Tuple<int, int>(o.SimulationIndex, n.SimulationIndex));
                        }
                        /*else if (l1.Component is IVoltageSource)
                            {
                                var v = (IVoltageSource)l1.Component;
                                diff += v.Voltage;
                                desiredw[o.SimulationIndex] = v.Voltage;
                                update.Add(new Tuple<int, int>(o.SimulationIndex, n.SimulationIndex));
                            }*/
                        else
                        {
                            update.Add(new Tuple<int, int>(o.SimulationIndex, n.SimulationIndex));
                            update.Add(new Tuple<int, int>(o.SimulationIndex, -1));
                        }
                        q.Enqueue(o);
                        // Count nodes with non-zero row in f
                        // This line is down here to avoid counting fn
                        ++div;
                        o.Mark = false;
                    }
                }
                diff = diff / div;

                foreach (var i in update)
                    if (i.Item2 > -1) desiredw[i.Item1] += desiredw[i.Item2];
                    else desiredw[i.Item1] -= diff;

                /*diff = 0.0; // prev
                    foreach (var t in path)
                    {
                        if (t.Item2)
                        {
                            var depInf = dep.FirstOrDefault(d => d.Item2 == t.Item1);
                            desiredw[t.Item1] = diff - (depInf?.Item3 ?? div);
                        }
                        else desiredw[t.Item1] = apb[t.Item1];
                        diff = desiredw[t.Item1];
                    }*/

                for (var i = 0; i < circuit.SourceCount; ++i)
                    desiredw[circuit.NodeCount + i] = apb[circuit.NodeCount + i];

                // Why 4? Not Node.
                // desiredw[4] = apb[4]; // =0
                // Start at src[0].Node1
                // Why copy from apb? Zero row in f.
                // Add connected to queue (sidx:0, below)
                // desiredw[2] = apb[2]; // =0
                // Why 2? Beginning of path.
                // Search from sidx:2
                // Add connected to queue (nothing)
                // Detect as dependent, add other side to queue with offset dep.Item3
                // var prev = desiredw[dep[0].Item1] = desiredw[2] - div; // -apb[dep[0].Item1]
                // Continue on path
                // Reached here, applied offset since we have one
                // Add connected to queue (sidx:1, below)
                // prev = desiredw[dep[0].Item2] = prev - dep[0].Item3; // -apb[dep[0].Item2]
                // Continue on path
                // Add connected to queue (nothing)
                // prev = desiredw[1] = prev - div; // -apb[dep[0].Item2]

                desiredw -= apb;

                x = apb + f * desiredw;
            }
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
