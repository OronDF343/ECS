using ECS.Model.Xml;

namespace ECS.Core
{
    public static class SimUpdate
    {
        // TODO: BAD CODE!
        public static void Simulate(CircuitXml cx)
        {
            /*var c = CircuitUtils.FromXml(cx);
            Simulator.ModifiedNodalAnalysis(c);

            var nd = cx.Nodes.ToDictionary(n => n.Id);
            var rd = cx.Resistors.ToDictionary(r => r.Id);
            var vsd = cx.VoltageSources.ToDictionary(vs => vs.Id);

            var q = new Queue<Node>();
            q.Enqueue(c.Head);
            while (q.Count > 0)
            {
                var n = q.Dequeue();
                Log.Information("Visiting node #{0}", n.Id);

                var x = nd[n.Id];
                x.Voltage = n.Voltage;
                n.Mark = true;
                foreach (var tmpc in n.Links)
                {
                    // For C#7: use switch expression patterns
                    if (tmpc.Component is Resistor && !tmpc.Component.Mark) // A resistor which we haven't visited yet
                    {
                        var r = tmpc.Component as Resistor;
                        Log.Information("Visiting resistor #{0} connected to node #{1}", r.Id, n.Id);
                        var x2 = rd[r.SimulationIndex];

                        x2.Voltage = r.Voltage;
                        x2.Current = r.Current;
                        var o = r.OtherNode(n); // OtherNode returns the connected node which is != n

                        q.Enqueue(o);
                    }
                    else if (tmpc.Component is VoltageSource) // A power source with known voltage (V)
                    {
                        var v = tmpc.Component as VoltageSource;
                        Log.Information("Visiting voltage source #{0} connected to node #{1}", v.Id, n.Id);
                        var x2 = vsd[v.SimulationIndex];

                        x2.Current = v.Current;
                    }
                    tmpc.Component.Mark = true;
                }
            }*/
        }
    }
}
