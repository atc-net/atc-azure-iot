namespace SimulationModule.DataGenerator;

public class NumberGeneratorConfig
{
    // Function to keep value close to expected value. Should increase the further we get from the expected level of the metric.
    public Func<double, double> ValueBiasFunc { get; set; } = x => 0;

    // Hard min limit at which values will be clipped
    public double MinClipValue { get; set; } = double.MinValue;

    // Hard max limit at which values will be clipped
    public double MaxClipValue { get; set; } = double.MaxValue;

    // Volatility of data
    public double Volatility { get; set; } = 0.5;

    // Floating point precision of values generated
    public int DecimalPrecision { get; set; } = 1;

    // Function with base value over time, should return a value in range [-1, 1]
    public Func<DateTimeOffset, double> BaseValueFunc { get; set; } = x => Math.Sin((2 * Math.PI / 60) * x.Minute);

    public NumberGeneratorConfig WithIntegerPrecision()
    {
        this.DecimalPrecision = 0;
        return this;
    }

    public NumberGeneratorConfig WithSinglePointPrecision()
    {
        this.DecimalPrecision = 1;
        return this;
    }

    public NumberGeneratorConfig WithPointPrecision(int precision)
    {
        this.DecimalPrecision = precision;
        return this;
    }

    public NumberGeneratorConfig WithNoMinValue()
    {
        this.MinClipValue = double.MinValue;
        return this;
    }

    public NumberGeneratorConfig WithMinValue(double minValue)
    {
        this.MinClipValue = minValue;
        return this;
    }

    public NumberGeneratorConfig WithNoMaxValue()
    {
        this.MaxClipValue = double.MaxValue;
        return this;
    }

    public NumberGeneratorConfig WithMaxValue(double maxValue)
    {
        this.MaxClipValue = maxValue;
        return this;
    }

    public NumberGeneratorConfig WithInvertedOffsetSinDailyCycle(int offset)
    {
        BaseValueFunc = x => -Math.Sin((2 * Math.PI / (60 * 24)) * ((x.Minute + (x.Hour * 60) + offset) % 60));
        return this;
    }

    public NumberGeneratorConfig WithOffsetSinDailyCycle(int offset)
    {
        BaseValueFunc = x => Math.Sin((2 * Math.PI / (60 * 24)) * ((x.Minute + (x.Hour * 60) + offset) % 60));
        return this;
    }

    public NumberGeneratorConfig WithSinDailyCycle()
    {
        BaseValueFunc = x => Math.Sin((2 * Math.PI / (60 * 24)) * (x.Minute + (x.Hour * 60)));
        return this;
    }

    public NumberGeneratorConfig WithInvertedSinDailyCycle()
    {
        BaseValueFunc = x => -Math.Sin((2 * Math.PI / (60 * 24)) * (x.Minute + (x.Hour * 60)));
        return this;
    }

    public NumberGeneratorConfig WithOffsetSinHourCycle(int offset)
    {
        BaseValueFunc = x => Math.Sin((2 * Math.PI / 60) * ((x.Minute + offset) % 60));
        return this;
    }

    public NumberGeneratorConfig WithSinHourlyCycle()
    {
        BaseValueFunc = x => Math.Sin((2 * Math.PI / 60) * x.Minute);
        return this;
    }

    public NumberGeneratorConfig WithInvertedSinHourCycle()
    {
        BaseValueFunc = x => -Math.Sin((2 * Math.PI / 60) * x.Minute);
        return this;
    }

    public NumberGeneratorConfig WithBaseFunction(Func<DateTimeOffset, double> baseFunction)
    {
        BaseValueFunc = baseFunction;
        return this;
    }

    public NumberGeneratorConfig WithLowVolatility()
    {
        Volatility = 0.1;
        return this;
    }

    public NumberGeneratorConfig WithNormalVolatility()
    {
        Volatility = 0.5;
        return this;
    }

    public NumberGeneratorConfig WithHighVolatility()
    {
        Volatility = 1;
        return this;
    }

    public NumberGeneratorConfig WithParabolicBias(double biasValue, double width)
    {
        ValueBiasFunc = x =>
            GetNormalizedParabolicEquationFromPointAndWidth(biasValue, width)(x) * (x > biasValue ? -1 : 1);
        return this;
    }

    public NumberGeneratorConfig WithMinMaxPolynomialBias(double biasValue)
    {
        var leftSide = GetNormalizedParabolicEquationFromPointAndWidth(biasValue, biasValue - MinClipValue);
        var rightSide = GetNormalizedParabolicEquationFromPointAndWidth(biasValue, MaxClipValue - biasValue);

        ValueBiasFunc = x => x > biasValue ? -rightSide(x) * 10 : leftSide(x) * 10;
        return this;
    }

    private static Func<double, double> GetNormalizedParabolicEquationFromPointAndWidth(double x, double width)
    {
        var x1 = x - width;
        var y1 = 1.0;
        var x2 = x;
        var y2 = 0.0;
        var x3 = x + width;
        var y3 = 1.0;

        var (a, b, c) = GetParabolicCoefficients(x1, y1, x2, y2, x3, y3);
        return p => (a * p * p) + (b * p) + c;
    }

    private static (double A, double B, double C) GetParabolicCoefficients(double x1, double y1, double x2, double y2, double x3, double y3)
    {
        var denom = (x1 - x2) * (x1 - x3) * (x2 - x3);
        var a = ((x3 * (y2 - y1)) + (x2 * (y1 - y3)) + (x1 * (y3 - y2))) / denom;
        var b = ((x3 * x3 * (y1 - y2)) + (x2 * x2 * (y3 - y1)) + (x1 * x1 * (y2 - y3))) / denom;
        var c = ((x2 * x3 * (x2 - x3) * y1) + (x3 * x1 * (x3 - x1) * y2) + (x1 * x2 * (x1 - x2) * y3)) / denom;
        return (a, b, c);
    }
}