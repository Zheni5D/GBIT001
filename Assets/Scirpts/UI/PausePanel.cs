using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PausePanel : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    [SerializeField]private bool isDebug4UI = false;
    private MouseMove mouseMove;
    private PlayerWeapon playerWeapon;
    bool flag = false;
    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if(!isDebug4UI){
            mouseMove = GameObject.FindWithTag("Player").GetComponent<MouseMove>();
            playerWeapon = GameObject.FindWithTag("Player").GetComponent<PlayerWeapon>();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.P)){
            if(flag == false)
                PopOn();
            else
                PopOff();
        }
#else
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(flag == false)
                PopOn();
            else
                PopOff();
        }
#endif
    }

    void PopOn(){
        SoundManager.PlayAudio("TypeUI");
        canvasGroup.alpha = 1.0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        flag = true;
        DOTween.timeScale = 0.00001f;
        if(!isDebug4UI){
            mouseMove.enabled = false;
            playerWeapon.enabled = false;
        }
        
        MessageCenter.SendMessage(new CommonMessage()
        {
            Mid = (int)MESSAGE_TYPE.PAUSE_ON,
            content = null
        });
    }

    void PopOff(){
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        flag = false;
        DOTween.timeScale = 1.0f;
        if(!isDebug4UI){
            mouseMove.enabled = true;
            playerWeapon.enabled = true;
        }
        MessageCenter.SendMessage(new CommonMessage()
        {
            Mid = (int)MESSAGE_TYPE.PAUSE_OFF,
            content = null
        });
    }

    public void continueButton(){
        SoundManager.PlayAudio("TypeUI");
        PopOff();
    }

    public void RestartButton(){
        SoundManager.PlayAudio("TypeUI");
        DOTween.CompleteAll();
        DOTween.Clear();
        SoundManager.WhenSceneReload();
        
        DOTween.timeScale = 1.0f;
        UIManager.Instance.FadeInOut(.5f);
        MessageCenter.SendMessage(new CommonMessage()
        {
            Mid = (int)MESSAGE_TYPE.GAME_OVER,
            content = null
        });
        Invoke(nameof(reloadCurScene),.6f);
    }

    public void MenuButton(){
        SoundManager.PlayAudio("TypeUI");
        PopOff();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        LevelManager.Instance.LoadAssignedLevel(0);
    }

    public void reloadCurScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
