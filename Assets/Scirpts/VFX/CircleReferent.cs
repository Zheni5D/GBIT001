using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CircleReferent : BaseBehavior
{
    [SerializeField]private GameObject filler;
    [SerializeField]private float triggerDelay = 3f;
    public GameObject bombPrefab;
    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update

    private void Awake()
    {
        MessageCenter.AddListener(OnGameRestart);
    }

    private void OnDestroy()
    {
        tween.Kill();
        MessageCenter.RemoveListner(OnGameRestart);
    }

    void Start()
    {
        spriteRenderer = filler.GetComponent<SpriteRenderer>();
    }
    private Tween tween;
    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Space)){
        if(tween==null){
            //To(DOGetter<T> getter, DOSetter<T> setter, T endValue, float duration)
            tween = DOTween.To
            (
                ()=>filler.transform.localScale,
                (x)=>filler.transform.localScale = x,
                Vector3.one,
                triggerDelay
            ).OnUpdate(applyTimeStop)
            .OnComplete(() =>
            {
                Instantiate(bombPrefab, transform.position, Quaternion.identity);
                Destroy(gameObject);
                //Debug.Log(1);
            });
            tween.SetAutoKill(true);
        }
        //}
        //if(Input.GetKeyDown(KeyCode.Backspace)){
        //    if(tween!=null){
        //        if(tween.IsComplete()){
        //            tween.PlayBackwards();
        //        }
        //    }
        //}
    }

    void applyTimeStop(){
        tween.timeScale = time.timeScale;
    }

    void OnGameRestart(CommonMessage msg)
    {
        if (msg.Mid != (int)MESSAGE_TYPE.GAME_RESTART) return;
        Destroy(gameObject);
    }

    public void SetDelay(float delay)
    {
        triggerDelay = delay;
    }
}
