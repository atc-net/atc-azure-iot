namespace Atc.Azure.IoT.Services.IoTHub;

public sealed class IoTHubRequestOptions
{
    /// <summary>
    /// Gets or sets the maximum number of retries to use, in addition to the original call.
    /// </summary>
    /// <value>
    /// The default value is 3 retries.
    /// </value>
    /// <remarks>
    /// To retry indefinitely use <see cref="int.MaxValue"/>. Note that the reported attempt number is capped at <see cref="int.MaxValue"/>.
    /// </remarks>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Gets or sets the type of the back-off.
    /// </summary>
    /// <value>
    /// The default value is <see cref="DelayBackoffType.Constant"/>.
    /// </value>
    public DelayBackoffType BackoffType { get; set; } = DelayBackoffType.Constant;

    /// <summary>
    /// Gets or sets the base delay between retries.
    /// </summary>
    /// <remarks>
    /// This value is used with the combination of <see cref="BackoffType"/> to generate the final delay for each individual retry attempt:
    /// <list type="bullet">
    /// <item>
    /// <see cref="DelayBackoffType.Exponential"/>: Represents the median delay to target before the first retry.
    /// </item>
    /// <item>
    /// <see cref="DelayBackoffType.Linear"/>: Represents the initial delay, the following delays increasing linearly with this value.
    /// </item>
    /// <item>
    /// <see cref="DelayBackoffType.Constant"/> Represents the constant delay between retries.
    /// </item>
    /// </list>
    /// </remarks>
    /// <value>
    /// The default value is 2 seconds.
    /// </value>
    public TimeSpan Delay { get; set; } = TimeSpan.FromSeconds(2);

    /// <summary>
    /// Gets or sets the maximum delay between retries.
    /// </summary>
    /// <remarks>
    /// This property is used to cap the maximum delay between retries. It is useful when you want to limit the maximum delay after a certain
    /// number of retries when it could reach an unreasonably high values, especially if <see cref="DelayBackoffType.Exponential"/> backoff is used.
    /// If not specified, the delay is not capped.
    /// </remarks>
    /// <value>
    /// The default value is <see langword="null"/>.
    /// </value>
    public TimeSpan? MaxDelay { get; set; }

    /// <summary>
    /// Gets or sets the maximum duration to wait for the request to complete before timing out.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.Zero;
}