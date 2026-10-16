using Newtonsoft.Json;

// Original item skeletons exceed Unity JsonUtility's ten-level serialization limit.
// Keep the complete saved hierarchy, including fingers/wings, instead of truncating it.
public static class SourceModelJson {
    public static T Read<T>(string json){
        return JsonConvert.DeserializeObject<T>(json,new JsonSerializerSettings{MaxDepth=256});
    }
}
