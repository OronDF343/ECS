namespace ECS.Core.Model
{
    /// <summary>
    ///     A resistor.
    /// </summary>
    public interface IResistor : IComponent
    {
        /// <summary>
        ///     Gets the resistance of the resistor, in ohms.
        /// </summary>
        double Resistance { get; set; }

        /// <summary>
        ///     Gets the conductance of the resistor, in 1/ohms.
        /// </summary>
        double Conductance { get; }

        /// <summary>
        ///     Gets the voltage at the resistor, in volts.
        /// </summary>
        double Voltage { get; set; }

        /// <summary>
        ///     Gets the current on the resistor, in amperes.
        /// </summary>
        double Current { get; set; }
    }
}
