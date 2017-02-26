using ECS.Core;
using ECS.Core.Model;
using Xunit;

namespace ECS.Tests.Core
{
    /// <summary>
    ///     Tests for the simulation core class.
    /// </summary>
    public class SimulatorTests
    {
        /// <summary>
        ///     A basic test for the MNA algorithm.
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
        
        /// <summary>
        /// Test for circut with 11 resistors
        /// </summary>
        [Fact]
        public void Test2()
        {
            var vs1 = new VoltageSource(0, 15);
            var vs2 = new VoltageSource(1, 25);
            var head = new Node(0);
            var negativeHead = new Node(1);
            var node1 = new Node(2);
            var node2 = new Node(3);
            var node3 = new Node (4);
            var node4 = new Node(5);
            var node5 = new Node(6);
            var refnode = new Node(-1);

            var r1 = new Resistor(0, 120);
            var r2 = new Resistor(1, 170);
            var r3 = new Resistor(2, 150);
            var r4 = new Resistor(3, 70);
            var r5 = new Resistor(4, 100);
            var r6 = new Resistor(5, 1000);
            var r7 = new Resistor(6, 300);
            var r8 = new Resistor(7, 1700);
            var r9 = new Resistor(8, 1500);
            var r10 = new Resistor(9, 100);
            var r11 = new Resistor(10, 400);

            CircuitUtils.Link2(vs2, negativeHead);
            CircuitUtils.Link1(vs1, head);
            CircuitUtils.Link2(vs1, refnode);
            CircuitUtils.Link1(r1, head);
            CircuitUtils.Link2(r1, node1);
            CircuitUtils.Link1(r2, node1);
            CircuitUtils.Link2(r2, node2);
            CircuitUtils.Link1(r3, node2);
            CircuitUtils.Link2(r3, refnode);
            CircuitUtils.Link1(r4, node1);
            CircuitUtils.Link2(r4, node3);
            CircuitUtils.Link1(r5, node3);
            CircuitUtils.Link2(r5, node4);
            CircuitUtils.Link1(r6, node4);
            CircuitUtils.Link2(r6, node2);
            CircuitUtils.Link1(r7, node4);
            CircuitUtils.Link2(r7, node2);
            CircuitUtils.Link1(r8, node3);
            CircuitUtils.Link2(r8, negativeHead);
            CircuitUtils.Link2(r9, negativeHead);
            CircuitUtils.Link1(r9, node5);
            CircuitUtils.Link2(r10, node5);
            CircuitUtils.Link1(r10, node4);
            CircuitUtils.Link1(vs2, node5);
            CircuitUtils.Link1(r11, node5);
            CircuitUtils.Link2(r11, node4);
            
            Simulator.ModifiedNodalAnalysis(new Circuit(head, 7, 2));

            Assert.Equal(15, head.Voltage);
            Assert.Equal(10.25, node1.Voltage, 2);
            Assert.Equal(5.94, node2.Voltage, 2);
            Assert.Equal(9.25, node3.Voltage, 2);
            Assert.Equal(9.23, node4.Voltage, 2);
            Assert.Equal(10.35, node5.Voltage, 2);
            Assert.Equal(-14.65, negativeHead.Voltage, 2);

            Assert.Equal(0.0396, vs1.Current, 4);
            Assert.Equal(0.03072, vs2.Current, 5);

            Assert.Equal(120, r1.Resistance, 0);
            Assert.Equal(4.75, r1.Voltage, 2);
            Assert.Equal(0.0396 , r1.Current, 4);

            Assert.Equal(170, r2.Resistance, 0);
            Assert.Equal(4.31, r2.Voltage, 2);
            Assert.Equal(0.02534, r2.Current, 5);

            Assert.Equal(150, r3.Resistance, 0);
            Assert.Equal(5.94, r3.Voltage, 2);
            Assert.Equal(0.0396, r3.Current, 4);

            Assert.Equal(70, r4.Resistance, 0);
            Assert.Equal(0.998, r4.Voltage, 3);
            Assert.Equal(0.01426, r4.Current, 5);

            Assert.Equal(100, r5.Resistance, 0);
            Assert.Equal(0.02009, r5.Voltage, 5);
            Assert.Equal(0.00020086, r5.Current, 8);

            Assert.Equal(1000, r6.Resistance, 0);
            Assert.Equal(3.29, r6.Voltage, 2);
            Assert.Equal(0.00329, r6.Current, 5);

            Assert.Equal(300, r7.Resistance, 0);
            Assert.Equal(3.29, r7.Voltage, 2);
            Assert.Equal(0.01097, r7.Current, 5);

            Assert.Equal(1700, r8.Resistance, 0);
            Assert.Equal(23.9, r8.Voltage, 1);
            Assert.Equal(0.01406, r8.Current, 5);

            Assert.Equal(1500, r9.Resistance, 0);
            Assert.Equal(25, r9.Voltage, 0);
            Assert.Equal(0.01667, r9.Current, 5);

            Assert.Equal(100, r10.Resistance, 0);
            Assert.Equal(1.12, r10.Voltage, 2);
            Assert.Equal(0.01124, r10.Current, 5);

            Assert.Equal(400, r11.Resistance, 0);
            Assert.Equal(1.12, r11.Voltage, 2);
            Assert.Equal(0.00281, r11.Current, 5);

        }

        /// <summary>
        ///     A basic test for the MNA algorithm with switch.
        /// </summary>
        [Fact]
        public void Test3()
        {
            var vs = new VoltageSource(0, 12);
            var head = new Node(0);
            var node1 = new Node(1);
            var refnode = new Node(-1);
            var sw = new Switch(4) {IsClosed = true};
            var node2 = new Node(2);

            var r1 = new Resistor(0, 100);
            var r2 = new Resistor(1, 100);
            var r3 = new Resistor(2, 220);
            var r4 = new Resistor(3, 1000);

            CircuitUtils.Link1(sw, node1);
            CircuitUtils.Link2(sw, node2);

            CircuitUtils.Link1(vs, head);
            CircuitUtils.Link1(r1, head);
            CircuitUtils.Link1(r3, head);

            CircuitUtils.Link2(r3, node2);
            CircuitUtils.Link1(r4, node1);
            CircuitUtils.Link1(r2, node1);

            CircuitUtils.Link2(r1, refnode);
            CircuitUtils.Link2(r2, refnode);
            CircuitUtils.Link2(r4, refnode);
            CircuitUtils.Link2(vs, refnode);

            Simulator.ModifiedNodalAnalysis(new Circuit(head, 3, 1));

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
