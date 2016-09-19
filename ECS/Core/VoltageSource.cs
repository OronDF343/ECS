namespace ECS.Core
{
    /// <summary>
    /// A voltage source.
    /// </summary>
    public class VoltageSource : Component
    {
        /// <summary>
        /// Creates a new <see cref="VoltageSource"/> with a given voltage.
        /// </summary>
        /// <param name="id">The unique identifier of the voltage source.</param>
        /// <param name="v">The voltage of the voltage source, in volts.</param>
        public VoltageSource(int id, double v)
            : base(id)
        {
            Voltage = v;
        }

        /// <summary>
        /// Gets the voltage of the voltage source, in volts.
        /// </summary>
        public double Voltage { get; }
    }
}
