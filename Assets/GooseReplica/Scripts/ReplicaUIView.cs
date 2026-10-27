using System;
using UnityEngine;
using UnityEngine.UI;

// Serialized references survive renaming and reparenting visual elements in Prefab Mode.
public sealed class ReplicaUIView : MonoBehaviour {
    [Serializable] public struct Binding { public string key; public GameObject target; }
    public Binding[] bindings;
    public T Get<T>(string key) where T:Component {
        foreach(var binding in bindings) if(binding.key==key && binding.target) {
            var component=binding.target.GetComponent<T>();
            if(component)return component;
        }
        throw new InvalidOperationException(name+": missing UI binding "+key+" ("+typeof(T).Name+")");
    }
    public void Text(string key,string value){Get<Text>(key).text=value;}
    public void Show(string key,bool visible){Get<Transform>(key).gameObject.SetActive(visible);}
    public Button Bind(string key,Action action){
        var button=Get<Button>(key);button.onClick.RemoveAllListeners();
        button.onClick.AddListener(()=>{ReplicaFlow.Instance.HomeClick();action();});return button;
    }
    public static ReplicaUIView Load(string name,Transform parent){
        var prefab=Resources.Load<ReplicaUIView>("Replica/UI/"+name);
        if(!prefab)throw new InvalidOperationException("Missing UI prefab: Replica/UI/"+name);
        var view=Instantiate(prefab,parent,false);
        if(parent && name!="CollectionTray"){
            var rect=(RectTransform)view.transform;rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;
            rect.offsetMin=rect.offsetMax=Vector2.zero;rect.localScale=Vector3.one;rect.localRotation=Quaternion.identity;
        }
        return view;
    }
}
