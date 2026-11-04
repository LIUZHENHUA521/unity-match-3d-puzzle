using UnityEngine;
[CreateAssetMenu(menuName="Goose Match3D/Theme Library")]
public class Match3DTheme : ScriptableObject {
    public ThemeEntry[] themes;
    // Serialized dependency keeps the source level catalog in WebGL/WeChat
    // builds even though gameplay also supports Resources.Load lookup.
    public LevelLayoutConfig sourceLevelCatalog;
    public string pvpEndpoint="https://9nanimal.cn";
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
public class Match3DTile : MonoBehaviour { public int id; }
