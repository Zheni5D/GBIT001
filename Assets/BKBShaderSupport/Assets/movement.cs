using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    public Vector3 move;
    // Start is called before the first frame update
    void Start()
    {
            
    }

    private void FixedUpdate() {
        transform.Translate(move);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
