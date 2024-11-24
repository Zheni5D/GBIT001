using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlitchAllocation : MonoBehaviour
{
    [SerializeField]private Material[] materials;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0;i<transform.childCount;i++){
            Transform t = transform.GetChild(i);
            t.GetComponent<Image>().material = materials[i%materials.Length];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
