using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public sealed class PotMasterReplayAvatar : MonoBehaviour {
    UnityWebRequest request;Texture2D owned;
    public void Load(string url){if(Match3DUserProfileView.AllowedAvatar(url))StartCoroutine(Fetch(url));}
    IEnumerator Fetch(string url){
        using(var r=UnityWebRequestTexture.GetTexture(url,true)){
            request=r;r.timeout=12;r.redirectLimit=0;yield return r.SendWebRequest();request=null;
            if(r.result!=UnityWebRequest.Result.Success)yield break;
            owned=DownloadHandlerTexture.GetContent(r);var image=GetComponent<PotAvatarGraphic>();image.texture=owned;image.color=Color.white;
        }
    }
    void OnDestroy(){if(request!=null)request.Abort();if(owned)Destroy(owned);}
}
