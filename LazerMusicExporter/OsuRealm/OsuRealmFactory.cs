using LazerMusicExporter.Configuration;
using Realms;

namespace LazerMusicExporter.OsuRealm;

public class OsuRealmFactory : IOsuRealmFactory
{
    private readonly string _realmName = "client.realm";

    private readonly IExportSettings _exportSettings;

    public OsuRealmFactory(IExportSettings exportSettings)
    {
        _exportSettings = exportSettings;
    }

    public Realm GetInstance()
    {
        return RealmHelper.GetInstanceByRecursiveSchemaVersion(schemaVersion => new RealmConfiguration(Path.Combine(_exportSettings.OsuDirectory, _realmName))
        {
            IsReadOnly = true,
            SchemaVersion = 26
        }) ?? throw new InvalidOperationException("Failed to load client.realm");
    }
}