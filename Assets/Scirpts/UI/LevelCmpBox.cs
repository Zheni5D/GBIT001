using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class LevelCmpBox : MonoBehaviour
{
    [SerializeField]private CanvasGroup stageClearTextGroup;
    private CanvasGroup group;
    private SimpleTimer timer;
    // Start is called before the first frame update
    void Start()
    {
        group = GetComponent<CanvasGroup>();
        timer = GetComponentInChildren<SimpleTimer>();
    }

    public void PlayLevelCmp()
    {
        // text.text = levelCmp;
        // text.color = Color.red;
        group.DOFade(1.0f,1.0f);
        group.interactable = true;
        group.blocksRaycasts = true;
        timer.CaclusTotalTime();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void PlayStageCmp()
    {
        stageClearTextGroup.DOFade(1.0f,.5f).SetLoops(2,LoopType.Yoyo);   
    }

    public void OnContinueButtonClick(){
        group.DOFade(0,.5f);
        group.interactable = false;
        group.blocksRaycasts = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
