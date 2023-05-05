namespace LazerMusicExporter.Configuration;

public enum OverwriteMode
{
    /// <summary>
    /// All Files will be kept and numbered if names are the same
    /// </summary>
    None = 0,

    /// <summary>
    /// Files will be overwritten if deemed the same by other configured parameters
    /// </summary>
    IgnoreIfSame = 1
}