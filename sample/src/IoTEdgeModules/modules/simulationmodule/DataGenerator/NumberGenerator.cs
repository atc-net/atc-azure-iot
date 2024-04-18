namespace SimulationModule.DataGenerator;

public class NumberGenerator
{
    private readonly NumberGeneratorConfig config;
    private readonly Random random = Random.Shared;
    private double current;

    public NumberGenerator(NumberGeneratorConfig config)
    {
        this.config = config;
        current = this.config.MaxClipValue - this.config.MinClipValue;
    }

    public NumberGenerator(double initialValue, NumberGeneratorConfig config)
        : this(config) => current = initialValue;

    [SuppressMessage("Security", "SCS0005:Weak random number generator.", Justification = "Not used for crypto")]
    [SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "Not used for crypto")]
    public double GetNextValue()
    {
        var timestamp = DateTimeOffset.Now;
        var magnitude = Convert.ToInt32(current).ToString(new NumberFormatInfo()).Length;
        var timeBias = config.Volatility * magnitude * config.BaseValueFunc(timestamp) * 0.5;
        var valueBias = config.Volatility * config.ValueBiasFunc(current) * 2;
        var randomnessBias = config.Volatility * magnitude * ((random.NextDouble() * 2) - 1) * 0.5;
        var unclipped = current + timeBias + randomnessBias + valueBias;
        var clipped = Math.Max(Math.Min(unclipped, config.MaxClipValue), config.MinClipValue);

        current = Math.Round(clipped, config.DecimalPrecision);
        return current;
    }
}