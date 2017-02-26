namespace ECS.Core.Model
{
    /// <summary>
    ///     A voltage source.
    /// </summary>
    public class VoltageSource : Component
    {
        /// <summary>
        ///     Creates a new <see cref="VoltageSource" /> with a given voltage.
        /// </summary>
        /// <param name="id">The unique identifier of the voltage source.</param>
        /// <param name="v">The voltage of the voltage source, in volts.</param>
        public VoltageSource(double v)
        {
            Voltage = v;
        }

        /// <summary>
        ///     Gets the voltage of the voltage source, in volts.
        /// </summary>
        public double Voltage { get; }

        /// <summary>
        ///     Gets or sets the total current drawn from this voltage source, in amperes.
        /// </summary>
        public double Current { get; set; }
    }
}
