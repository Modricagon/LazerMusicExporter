using Realms;

namespace LazerMusicExporter.OsuRealm;

public interface IOsuRealmFactory
{
    public Realm GetInstance();
}