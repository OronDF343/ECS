namespace ECS.Core.Model
{
    /// <summary>
    ///     A voltage source.
    /// </summary>
    public interface IVoltageSource : IComponent
    {
        /// <summary>
        ///     Gets the voltage of the voltage source, in volts.
        /// </summary>
        double Voltage { get; }

        /// <summary>
        ///     Gets or sets the total current drawn from this voltage source, in amperes.
        /// </summary>
        double Current { get; set; }
    }
}
