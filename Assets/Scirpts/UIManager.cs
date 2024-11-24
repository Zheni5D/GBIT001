using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : SingleTon<UIManager>
{
    public Sprite[] numbers = new Sprite[10];
    [SerializeField]private Image FadeInOutImage;//之后可能会换成GameObj或脚本
    private LevelCmpBox levelCmpBox;

    protected override void Awake()
    {
        base.Awake();
        // DontDestroyOnLoad(gameObject);
        levelCmpBox = GetComponentInChildren<LevelCmpBox>();
    }

    public void FadeIn(float time)
    {
        FadeInOutImage.DOFade(1.0f,time).SetUpdate(true);
    }

    public void FadeOut(float time)
    {
        FadeInOutImage.DOFade(0,time).SetUpdate(true);
    }

    public void FadeInOut(float time)
    {
        FadeInOutImage.DOFade(1.0f,time).SetLoops(2,LoopType.Yoyo).SetUpdate(true);
    }

    public void LevelCmpAnim()
    {
        levelCmpBox.PlayLevelCmp();
    }

    public void StageCmpAnim()
    {
        levelCmpBox.PlayStageCmp();
    }
}
