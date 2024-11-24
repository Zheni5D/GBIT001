using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadomMaterial : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Image image = GetComponent<Image>();
	    image.material.SetFloat("_Frequency", 120f);
        image.SetMaterialDirty();
	    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
