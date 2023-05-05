using Realms;
using Realms.Exceptions;

namespace LazerMusicExporter.OsuRealm;

public static class RealmHelper
{
    /// <summary>
    /// Terrible implementation to recursively retry whilst incrementing the schema version until correct value is found.
    /// </summary>
    /// <param name="config">The config function</param>
    /// <param name="schemaVersion">The schema version to use</param>
    /// <returns>The realm instance</returns>
    public static Realm? GetInstanceByRecursiveSchemaVersion(Func<ulong, RealmConfiguration> config, ulong schemaVersion = 0)
    {
        try
        {
            return Realm.GetInstance(config(schemaVersion));
        }
        catch(RealmException)
        {
            return GetInstanceByRecursiveSchemaVersion(config, schemaVersion + 1);
        }
    }
}