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
        MessageCenter.AddListener(OnTimeStopOn);
        MessageCenter.AddListener(OnTimeStopOff);
    }

    private void Update() {

    }

    private void OnDestroy() {
        MessageCenter.RemoveListner(OnTimeStopOff);
        MessageCenter.RemoveListner(OnTimeStopOn);
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
}
