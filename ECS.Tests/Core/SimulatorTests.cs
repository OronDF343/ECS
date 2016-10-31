using ECS.Core;
using ECS.Core.Model;
using Xunit;

namespace ECS.Tests.Core
{
    /// <summary>
    /// Tests for the simulation core class.
    /// </summary>
    public class SimulatorTests
    {
        /// <summary>
        /// A basic test for the MNA algorithm.
        /// </summary>
        [Fact]
        public void Test1()
        {
            var vs = new VoltageSource(0, 12);
            var head = new Node(0);
            var node1 = new Node(1);
            var refnode = new Node(-1);
            var r1 = new Resistor(0, 100);
            var r2 = new Resistor(1, 100);
            var r3 = new Resistor(2, 220);
            var r4 = new Resistor(3, 1000);

            CircuitUtils.Link1(vs, head);
            CircuitUtils.Link1(r1, head);
            CircuitUtils.Link1(r3, head);

            CircuitUtils.Link2(r3, node1);
            CircuitUtils.Link1(r4, node1);
            CircuitUtils.Link1(r2, node1);

            CircuitUtils.Link2(r1, refnode);
            CircuitUtils.Link2(r2, refnode);
            CircuitUtils.Link2(r4, refnode);
            CircuitUtils.Link2(vs, refnode);

            Simulator.ModifiedNodalAnalysis(new Circuit(head, 2, 1));

            Assert.Equal(12, head.Voltage);
            Assert.Equal(3.509, node1.Voltage, 3);
            Assert.Equal(0.159, vs.Current, 3);
            Assert.Equal(12, r1.Voltage);
            Assert.Equal(0.12, r1.Current);
            Assert.Equal(3.509, r2.Voltage, 3);
            Assert.Equal(0.035, r2.Current, 3);
            Assert.Equal(8.491, r3.Voltage, 3);
            Assert.Equal(0.039, r3.Current, 3);
            Assert.Equal(3.509, r4.Voltage, 3);
            Assert.Equal(0.004, r4.Current, 3);
        }
    }
}
