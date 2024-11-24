using DG.Tweening;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public bool hasTriggerCondition;
    public StageID triggerCondition;
    private bool isChecked;
    private Tween cache;
    private void Start() {
        cache =  GetComponent<SpriteRenderer>().DOColor(Color.yellow,3.0f).SetLoops(-1,LoopType.Yoyo);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if(hasTriggerCondition)
            {
                if((int)triggerCondition != LevelObject.curStageIndex)
                    return;
            }
            if(!isChecked)
            {
                isChecked = true;
                cache.Kill();
                myEventManager.instance.EventProceed();
            }
            
        }
    }
}
