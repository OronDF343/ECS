using System.Collections.Generic;
using ECS.Core;
using ECS.Core.Model;
using ECS.Model;
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
            var head = new Node {Name = "Head"};
            var node1 = new Node {Name = "N1"};
            var refnode = new Node { IsReferenceNode = true, Name = "Ref"};
            var nodes = new List<INode> { head, node1, refnode };

            var vs = new VoltageSource(12) { Node1 = head, Node2 = refnode, Name = "Vin1" };
            var r1 = new Resistor(100) { Node1 = head, Node2 = refnode, Name = "R1" };
            var r2 = new Resistor(100) { Node1 = node1, Node2 = refnode, Name = "R2" };
            var r3 = new Resistor(220) { Node1 = head, Node2 = node1, Name = "R3" };
            var r4 = new Resistor(1000) { Node1 = node1, Node2 = refnode, Name = "R4" };
            var components = new List<IComponent> { vs, r1, r2, r3, r4 };

            Simulator.ModifiedNodalAnalysis(new SimulationCircuit(nodes, components));

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
        ///     Test for circut with 11 resistors
        /// </summary>
        [Fact]
        public void Test2()
        {
            var head = new Node {Name = "Head"};
            var negativeHead = new Node {Name = "I'm Negative"};
            var node1 = new Node { Name = "N1" };
            var node2 = new Node { Name = "N2" };
            var node3 = new Node { Name = "N3" };
            var node4 = new Node { Name = "N4" };
            var node5 = new Node { Name = "N5" };
            var refnode = new Node {IsReferenceNode = true};
            var nodes = new List<INode> { head, negativeHead, node1, node2, node3, node4, node5, refnode };

            var vs1 = new VoltageSource(15) { Name = "Vin1", Node1 = head, Node2 = refnode };
            var vs2 = new VoltageSource(25) { Name = "Vin2", Node1 = node5, Node2 = negativeHead };
            var r1 = new Resistor(120) { Name = "R1", Node1 = head, Node2 = node1 };
            var r2 = new Resistor(170) { Name = "R2", Node1 = node1, Node2 = node2 };
            var r3 = new Resistor(150) { Name = "R3", Node1 = node2, Node2 = refnode };
            var r4 = new Resistor(70) { Name = "R4", Node1 = node1, Node2 = node3 };
            var r5 = new Resistor(100) { Name = "R5", Node1 = node3, Node2 = node4 };
            var r6 = new Resistor(1000) { Name = "R6", Node1 = node4, Node2 = node2 };
            var r7 = new Resistor(300) { Name = "R7", Node1 = node4, Node2 = node2 };
            var r8 = new Resistor(1700) { Name = "R8", Node1 = node3, Node2 = negativeHead };
            var r9 = new Resistor(1500) { Name = "R9", Node1 = negativeHead, Node2 = node5 };
            var r10 = new Resistor(100) { Name = "R10", Node1 = node5, Node2 = node4 };
            var r11 = new Resistor(400) { Name = "R11", Node1 = node5, Node2 = node4 };
            var components = new List<IComponent> { vs1, vs2, r1, r2, r3, r4, r5, r6, r7, r8, r9, r10, r11 };

            Simulator.ModifiedNodalAnalysis(new SimulationCircuit(nodes, components));

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
            Assert.Equal(0.0396, r1.Current, 4);

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
        ///     A basic test for the MNA algorithm with a switch.
        /// </summary>
        [Fact]
        public void Test3()
        {
            var head = new Node { Name = "Head?" };
            var node1 = new Node { Name = "N1" };
            var refnode = new Node { IsReferenceNode = true, Name = "Reference" };
            var node2 = new Node {Name = "N2"};
            var nodes = new List<INode> { head, node1, refnode, node2 };

            var vs = new VoltageSource(12) { Name = "Vin", Node1 = head, Node2 = refnode };
            var r1 = new Resistor(100) { Name = "R1", Node1 = head, Node2 = refnode };
            var r2 = new Resistor(100) { Name = "R2", Node1 = node1, Node2 = refnode };
            var r3 = new Resistor(220) { Name = "R3", Node1 = head, Node2 = node2 };
            var r4 = new Resistor(1000) { Name = "R4", Node1 = node1, Node2 = refnode };
            var sw = new Switch { IsClosed = true, Name = "LOL I'm a switch", Node1 = node1, Node2 = node2 };
            var components = new List<IComponent> { vs, r1, r2, r3, r4, sw };

            Simulator.ModifiedNodalAnalysis(new SimulationCircuit(nodes, components));

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
