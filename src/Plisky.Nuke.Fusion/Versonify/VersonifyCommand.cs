namespace Plisky.Nuke.Fusion;

/// <summary>
/// The values for the -Command parameter in the Versonify tool.
/// </summary>
public enum VersonifyCommand {
    /// <summary>
    ///  This is not a valid setting.
    /// </summary>
    Unknown,
    /// <summary>
    /// Updates the code files with the version number.
    /// </summary>
    UpdateFiles,
    /// <summary>
    /// Returns the version number without updating any files.
    /// </summary>
    Passive,
    /// <summary>
    /// Creates a new version number.
    /// </summary>
    CreateVersion,
    /// <summary>
    /// Adds an override to the version number.
    /// </summary>
    Override
}