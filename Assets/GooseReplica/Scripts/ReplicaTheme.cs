using UnityEngine;
[CreateAssetMenu(menuName="Goose Replica/Theme Library")]
public class ReplicaTheme : ScriptableObject {
    public ThemeEntry[] themes;
    // Serialized dependency keeps the source level catalog in WebGL/WeChat
    // builds even though gameplay also supports Resources.Load lookup.
    public ReplicaSourceLevelPlan sourceLevelCatalog;
    public string pvpEndpoint="https://goose-3d-pvp-197372-5-1312757046.sh.run.tcloudbase.com";
    public AudioClip homeMusic,gameMusic,pvpMusic,clickSound,selectSound,matchSound,winSound,loseSound;
    public Font font;
}
[System.Serializable] public class ThemeEntry {
    public string id,title,subtitle,gooseSprite;
    public string sourceEnvironment;
    public GameObject[] prefabs;
    public ChallengeLayer[] layers;
    public int seconds=600;
}
public class ReplicaTile : MonoBehaviour { public int id; }
