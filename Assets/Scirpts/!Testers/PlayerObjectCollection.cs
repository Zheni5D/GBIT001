using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjectCollection : MonoBehaviour
{

    private void Awake()
    {
        if(transform.childCount > 0)
        {
            for(int j = 0; j < this.transform.childCount;)
            {
                transform.GetChild(j).SetParent(null);
            }
        }
    }
}
