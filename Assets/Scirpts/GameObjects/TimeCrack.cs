using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class TimeCrack : MonoBehaviour
{
    [Tooltip("时间值损失因数,正数时间值减少")]
    public int ConsumeFactor = 1;
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            other.GetComponent<PlayerControl>().ChangeTimeEnergyConsumeFactor(ConsumeFactor);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Player")){
            other.GetComponent<PlayerControl>().ChangeTimeEnergyConsumeFactor(-ConsumeFactor);
        }
    }
}
