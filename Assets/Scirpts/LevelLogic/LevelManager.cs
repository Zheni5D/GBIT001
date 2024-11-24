using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelManager : SingleTon<LevelManager>
{
    public bool enableReplay = true;
    private int curLevelIndex;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);

        curLevelIndex = SceneManager.GetActiveScene().buildIndex;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif

        }
    }

    public void LoadNextLevel()
    {
        curLevelIndex++;
        SoundManager.WhenSceneReload();
        if(curLevelIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError("关卡下标越界");
            return;
        }
        SceneManager.LoadScene(curLevelIndex);
    }

    public void Async_LoadNextLevel()
    {
        curLevelIndex++;
        if(curLevelIndex >= SceneManager.sceneCountInBuildSettings)
        {
            //Debug.LogError("关卡下标越界or恭喜通关");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            curLevelIndex=0;
            if (BGMManager.isInitialize)
            {
                BGMManager.Instance.RenturnToMainTitle();
            }
            StartCoroutine(Async_load());
            return;
        }
        StartCoroutine(Async_load());
    }

    public void LoadAssignedLevel(int index)//CAUTION: 从1开始数，因为初始界面占用了0这个位置
    {
        if(index < 0)
        {
            Debug.LogError("加载关卡下标不合法");
            return;
        }
        if(index >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError("关卡下标越界");
            return;
        }
        curLevelIndex = index;
        if (index == 0)
        {
            if (BGMManager.isInitialize)
            {
                BGMManager.Instance.RenturnToMainTitle();
            }
        }
        StartCoroutine(Async_load());
        

    }

    IEnumerator Async_load()
    {
        SoundManager.WhenSceneReload();
        //FadeInOutImage.DOFade(1.0f,1.0f);
        UIManager.Instance.FadeIn(1.0f);
        yield return new WaitForSeconds(1.5f);
        AsyncOperation async = SceneManager.LoadSceneAsync(curLevelIndex);
        async.completed += Async_complete;
    }

    private void Async_complete(AsyncOperation obj)
    {
        //FadeInOutImage.DOFade(0,1.0f);
        UIManager.Instance.FadeOut(1.0f);
    }
}
