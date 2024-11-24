using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordCamera : MonoBehaviour
{
    private Transform player;
    public Transform Player{
        get{
            if(player==null){
                player = GameObject.FindWithTag("Player").transform;
            }
            return player;
        }
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(Player.position.x, Player.position.y, -10);
    }
}
