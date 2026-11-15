using UnityEngine.EventSystems;
using UnityEngine.UI;

// Pointer-down also covers tapping the track, not only dragging the thumb.
public sealed class PotReplayProgress : Slider {
    PotReplayPlayer player;
    bool dragging;
    public void Connect(PotReplayPlayer owner){player=owner;}
    public override void OnPointerDown(PointerEventData e){
        if(e.button!=PointerEventData.InputButton.Left||!IsActive()||!IsInteractable()||!player||!player.ControlsEnabled)return;
        dragging=true;player.BeginScrub();base.OnPointerDown(e);
    }
    public override void OnPointerUp(PointerEventData e){
        base.OnPointerUp(e);
        if(e.button==PointerEventData.InputButton.Left)Release();
    }
    void Release(){if(!dragging)return;dragging=false;if(player)player.EndScrub();}
    protected override void OnDisable(){Release();base.OnDisable();}
    void OnApplicationFocus(bool focus){if(!focus)Release();}
}
