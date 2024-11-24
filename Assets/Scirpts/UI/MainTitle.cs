using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MainTitle : MonoBehaviour
{
    [SerializeField]private Button startBtn;
    [SerializeField]private Button staffBtn;
    [SerializeField]private Button exitBtn;
    [SerializeField]private CanvasGroup MatinTitilePanel;
    [SerializeField]private GameObject choosePanel;
    [SerializeField]private GameObject staffPanel;
    // Start is called before the first frame update
    void Start()
    {
        startBtn.onClick.AddListener(StartBtn);
        staffBtn.onClick.AddListener(StaffBtn);
        exitBtn.onClick.AddListener(ExitBtn);
        //transform.DORotate(new Vector3(0,0,5f),3f,RotateMode.Fast).SetLoops(-1,LoopType.Yoyo).SetEase(Ease.Linear); 
    }

    private void OnDisable() {
        startBtn.onClick.RemoveListener(StartBtn);
        staffBtn.onClick.RemoveListener(StaffBtn);
        exitBtn.onClick.RemoveListener(ExitBtn);
    }

    public void StartBtn(){
        SoundManager.PlayAudio("TypeUI");
        MatinTitilePanel.DOFade(0f,1.0f);
        // levelGrids.SetActive(false);
        MatinTitilePanel.interactable = false;
        MatinTitilePanel.blocksRaycasts = false;
        choosePanel.GetComponent<CanvasGroup>().interactable = true;
        choosePanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void StaffBtn(){
        SoundManager.PlayAudio("TypeUI");
        staffPanel.SetActive(true);
    }

    public void StaffReturnButton()
    {
        SoundManager.PlayAudio("TypeUI");
        staffPanel.SetActive(false);
    }

    public void ExitBtn(){
        SoundManager.PlayAudio("TypeUI");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    #region 关卡选择界面
    public void ReturnButton(){
        SoundManager.PlayAudio("TypeUI");
        MatinTitilePanel.DOFade(1.0f,1.0f);
        // levelGrids.SetActive(true);
        MatinTitilePanel.interactable = true;
        MatinTitilePanel.blocksRaycasts = true;
        choosePanel.GetComponent<CanvasGroup>().interactable = false;
        choosePanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void ChooseLevel(int index){
        SoundManager.PlayAudio("TypeUI");
        BGMManager.Instance.StartGame();
        LevelManager.Instance.LoadAssignedLevel(index);
    }
    #endregion
}
