using System;

namespace ECS.Core
{
    /// <summary>
    /// A resistor.
    /// </summary>
    public class Resistor : Component
    {
        /// <summary>
        /// Creates a new <see cref="Resistor"/> with a given resistance.
        /// </summary>
        /// <param name="id">The unique identifier of the resistor.</param>
        /// <param name="r">The resistance of the resistor, in ohms. Must be greater than 0.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="r"/> is 0 or less.</exception>
        public Resistor(int id, double r)
            : base(id)
        {
            if (r <= 0) throw new ArgumentOutOfRangeException(nameof(r), r, nameof(r) + " must be greater than 0");
            Resistance = r;
        }

        /// <summary>
        /// Gets the resistance of the resistor, in ohms.
        /// </summary>
        public double Resistance { get; }
        /// <summary>
        /// Gets the conductance of the resistor, in 1/ohms.
        /// </summary>
        public double Conductance => 1 / Resistance;
        /// <summary>
        /// Gets the voltage at the resistor, in volts.
        /// </summary>
        public double Voltage => Math.Abs(Node1.Voltage - Node2.Voltage);
        /// <summary>
        /// Gets the current on the resistor, in amperes.
        /// </summary>
        public double Current => Voltage / Resistance;
    }
}
