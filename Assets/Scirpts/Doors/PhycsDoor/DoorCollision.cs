using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCollision : MonoBehaviour
{
    public bool isABox;
    Door door;
    TriggerDoor triggerDoor;
    Collider2D _collider;
    // Start is called before the first frame update
    void Start()
    {
        door = GetComponentInParent<Door>();
        _collider = GetComponent<Collider2D>();
        triggerDoor = GetComponentInParent<TriggerDoor>();
    }

    private void FixedUpdate() {
        // if(isABox)
        //     if(door.GetDeltaAngle() > 0)
        //     {
        //         _collider.enabled = true;
        //     }
        //     else
        //     {
        //         _collider.enabled = false;
        //     }
        // else
        //     if(door.GetDeltaAngle() < 0)
        //     {
        //         _collider.enabled = true;
        //     }
        //     else
        //     {
        //         _collider.enabled = false;
        //     }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Enemy"))
        {
            other.GetComponent<simpleFSM>().BeatDown(Vector3.zero);
        }
        if(triggerDoor != null)
        {
            if(!isABox)
                triggerDoor.GetComponent<Rigidbody2D>().AddForce(triggerDoor.transform.up * 100f);
            else
                triggerDoor.GetComponent<Rigidbody2D>().AddForce(-triggerDoor.transform.up * 100f);
        }
    }
}
