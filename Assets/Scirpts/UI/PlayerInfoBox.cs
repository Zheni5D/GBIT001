using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class PlayerInfoBox : MonoBehaviour
{
    private CanvasGroup group;
    public GameObject TimeEnergyBox;
    private WeaponInfo weaponInfo;
    private Image timeEnergyBar;
    private Image timeEnergyIcon;

     private void OnEnable() {
        Shooting.OnAmmoChangedStatic += SetAmmoText;
    }
    // Start is called before the first frame update
    void Start()
    {
        group = GetComponent<CanvasGroup>();
        weaponInfo = transform.Find("WeaponInfo").GetComponent<WeaponInfo>();
        timeEnergyBar = TimeEnergyBox.transform.Find("TEBar").GetComponent<Image>();
        timeEnergyIcon = TimeEnergyBox.transform.Find("TEIcon").GetComponent<Image>();
    }

    private void OnDisable() {
        Shooting.OnAmmoChangedStatic -= SetAmmoText;
    }

    public void SetAmmoText(int amount)
    {
        weaponInfo.setBulletCount(amount);
    }

    public void SetTimeEnergyBar(int amount)
    {
        timeEnergyBar.fillAmount = amount;
    }

    public void SetTimeEnergyIcon(bool _isOn)
    {
        if(_isOn)
            timeEnergyIcon.DOColor(Color.yellow,0.5f);
        else
            timeEnergyIcon.DOColor(Color.white,0.5f);
    }

    // public void ToggleBox(bool b)
    // {
    //     if(b)
    //         group.alpha = 1;
    //     else
    //         group.alpha = 0;
    // }
}
