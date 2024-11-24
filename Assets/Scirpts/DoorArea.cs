using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DoorArea : MonoBehaviour
{
    private NewDoor newDoor;
    private bool playerInRange;


    private void Awake()
    {
        newDoor = GetComponentInParent<NewDoor>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInRange)
        {
            SoundManager.PlayAudio("openDoor");
            newDoor.open = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && newDoor.open)
        {
            if (newDoor.rotated)
            {
                newDoor.targetRotation = newDoor.originalRotation;
                newDoor.gameObject.layer = LayerMask.NameToLayer("Door");
            }
            else if (transform.InverseTransformPoint(collision.transform.position).x < 0)
            {
                newDoor.targetRotation = Quaternion.Euler(0f, 0f, -90f) * newDoor.originalRotation;
            }
            else
            {
                newDoor.targetRotation = Quaternion.Euler(0f, 0f, 90f) * newDoor.originalRotation;
            }

            newDoor.open = false;
            newDoor.rotated = !newDoor.rotated;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
