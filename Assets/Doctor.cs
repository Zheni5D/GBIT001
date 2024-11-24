using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doctor : MonoBehaviour
{
    public GameObject dogPrefab;
    public Transform makeTransform;

    public void MakeDog()
    {
        SoundManager.PlayAudio("makedog");
        Instantiate(dogPrefab, makeTransform.position, transform.rotation, null).GetComponent<simpleFSM>().paramater.isSummoned = true; // 如果生成到Stage里，Restart后还会存在
    }
}
