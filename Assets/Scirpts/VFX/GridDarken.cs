using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridDarken : MonoBehaviour
{
    [SerializeField]private Color targetColor;
    private Color oriColor;
    private Tilemap tilemap;
    private Tween tween;
    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        oriColor = tilemap.color;
        MessageCenter.AddListener(OnFocusOn,MESSAGE_TYPE.FOCUS_ON);
        MessageCenter.AddListener(OnFocusOff,MESSAGE_TYPE.FOCUS_OFF);
    }


    private void OnDestroy() {
        MessageCenter.RemoveListener(OnFocusOff,MESSAGE_TYPE.FOCUS_OFF);
        MessageCenter.RemoveListener(OnFocusOn,MESSAGE_TYPE.FOCUS_ON);
    }

    public void OnTimeStopOn(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.TIME_STOP_ON) return;
        if(tween!=null)
            tween.Kill();
        tween = DOTween.To(()=>tilemap.color,x=>tilemap.color = x,targetColor,1.0f);
    }

    public void OnTimeStopOff(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.TIME_STOP_OFF) return;
        if(tween!=null)
            tween.Kill();
        tween = DOTween.To(()=>tilemap.color,x=>tilemap.color = x,oriColor,1.0f);
    }
    public void OnFocusOn(CommonMessage msg)
    {
        if(tween!=null)
            tween.Kill();
        tween = DOTween.To(()=>tilemap.color,x=>tilemap.color = x,targetColor,1.0f);
    }

    public void OnFocusOff(CommonMessage msg)
    {
        if(tween!=null)
            tween.Kill();
        tween = DOTween.To(()=>tilemap.color,x=>tilemap.color = x,oriColor,1.0f);
    }
}
