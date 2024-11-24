using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class NewDoor : MonoBehaviour
{
    public Quaternion originalRotation; 
    public Quaternion targetRotation;
    public bool rotated = false; 
    public float rotationSpeed = 45f;
    public bool open;

    private void Awake()
    {
        MessageCenter.AddListener(OnGameRestart);
    }

    private void OnDestroy()
    {
        MessageCenter.RemoveListner(OnGameRestart);
    }

    void Start()
    {
        targetRotation = originalRotation = transform.rotation;
    }

    private void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.transform.CompareTag("Enemy") && !rotated)
        {
            targetRotation = Quaternion.Euler(0f, 0f, -90f) * originalRotation;
            // transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("TransparentObstacle");
            rotated = !rotated;
            SoundManager.PlayAudio("openDoor");
        }

        if (collider.transform.CompareTag("Monster"))
        {
            gameObject.SetActive(false);
        }
    }

    public void OnGameRestart(CommonMessage msg)
    {
        if (msg.Mid != (int)MESSAGE_TYPE.GAME_RESTART) return;

        transform.rotation = originalRotation;
        targetRotation = originalRotation;
        rotated = false;
        gameObject.SetActive(true);
    }
}
