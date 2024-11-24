using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class simpleUIManager : MonoBehaviour
{
    public static simpleUIManager instance;

    public GameObject dialogBox;
    public GameObject optionBox;
    public TMP_Text speakerName;
    public TMP_Text speakText;
    public GameObject spaceBar;//OPTIONAL
    public CanvasGroup charcBox;
    private void Awake() {
        if(instance == null)
            instance = this;
        else
            if(instance != this)
                Destroy(this.gameObject);//防止加载场景时有两个UIManager
    }
    void Start()
    {
        ToggleDialogBox(false);
    }

    public void ToggleDialogBox(bool state)
    {
        if(state)
            {dialogBox.GetComponent<CanvasGroup>().alpha = 1;charcBox.alpha=1;}
        else
            {dialogBox.GetComponent<CanvasGroup>().alpha = 0;charcBox.alpha=0;}
    }

    public void ToggleOptionBox(bool state)
    {
        if(state)
        {
            optionBox.GetComponent<CanvasGroup>().alpha = 1;
            optionBox.GetComponent<CanvasGroup>().interactable = true;
        }
        else
        {
            optionBox.GetComponent<CanvasGroup>().alpha = 0;
            optionBox.GetComponent<CanvasGroup>().interactable = false;
        }
    }

    public void ToggleSpaceBar(bool state)
    {
        spaceBar.gameObject.SetActive(state);
    }
    
    //                                                      字体大小
    public void SetTextContent(string _name, string _line, int _size)
    {
        speakerName.text = _name;
        speakText.text = _line;
        speakText.fontSize = _size;

        ToggleDialogBox(true);
    }
}
