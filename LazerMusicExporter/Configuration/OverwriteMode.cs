namespace LazerMusicExporter.Configuration;

public enum OverwriteMode
{
    /// <summary>
    /// Files will be indexed if names are the same
    /// </summary>
    None = 0,

    /// <summary>
    /// Files will be overwritten if same
    /// </summary>
    IgnoreIfSame = 1
}