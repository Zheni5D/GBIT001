using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeEnergyCubeBar : MonoBehaviour
{
    private List<Image> _timeEnergyCubes = new List<Image>();

    private float _energy = 100f;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            _timeEnergyCubes.Add(transform.GetChild(i).GetComponent<Image>());
        }
    }

    public void EnergySet(float energy)
    {
        _energy = energy;
        _energy = Mathf.Clamp(_energy, 0f, TimeScaleChange.MAX_TIME_ENERGY);
        int frac = Mathf.FloorToInt(TimeScaleChange.MAX_TIME_ENERGY / 10);
        int cnt = Mathf.FloorToInt(_energy / frac);
        //cnt 0     1    2   3   4   5   6   7   8   9   10
        //index     0            0-3         0-6          0-9                         
        //cnt = active
        for (int i = 0; i < _timeEnergyCubes.Count; i++)
        {
            //能量块消失/补充
            if (i < cnt)
            {
                _timeEnergyCubes[i].color = Color.white;
            }
            else
            {
                _timeEnergyCubes[i].color = Color.clear;
            }
            
        }
    }
}
