using UnityEngine.UI;
using UnityEngine;

public class ReplayToggle : MonoBehaviour
{
    private Toggle toggle;
    // Start is called before the first frame update
    void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnEnableReplayChanged);
        toggle.isOn = LevelManager.Instance.enableReplay;
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveAllListeners();
    }

    public void OnEnableReplayChanged(bool value)
    {
        LevelManager.Instance.enableReplay = value;
    }
}
