namespace Fermion.Extensions.HttpContexts;

/// <summary>
/// Represents information about the device and operating system of a user.
/// </summary>
public class DeviceInfo
{
    /// <summary>
    /// Gets or sets the family of the device.
    /// </summary>
    public string? DeviceFamily { get; set; }

    /// <summary>
    /// Gets or sets the model of the device.
    /// </summary>
    public string? DeviceModel { get; set; }

    /// <summary>
    /// Gets or sets the family of the operating system.
    /// </summary>
    public string? OsFamily { get; set; }

    /// <summary>
    /// Gets or sets the version of the operating system.
    /// </summary>
    public string? OsVersion { get; set; }

    /// <summary>
    /// Gets or sets the family of the browser.
    /// </summary>
    public string? BrowserFamily { get; set; }

    /// <summary>
    /// Gets or sets the version of the browser.
    /// </summary>
    public string? BrowserVersion { get; set; }
}