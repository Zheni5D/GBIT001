using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Serialization;

public class PlayerInfoBox : MonoBehaviour
{
    private CanvasGroup group;
    [FormerlySerializedAs("TimeEnergyBox")] public GameObject timeEnergyBox;
    private WeaponInfo _weaponInfo;
    private Image _timeEnergyIcon;
    
     private void OnEnable() {
        MagazineBaseShooting.OnAmmoChangedStatic += SetAmmoText;
    }
    // Start is called before the first frame update
    void Start()
    {
        group = GetComponent<CanvasGroup>();
        _weaponInfo = transform.Find("WeaponInfo").GetComponent<WeaponInfo>();
        _timeEnergyIcon = timeEnergyBox.transform.Find("TEIcon").GetComponent<Image>();
    }

    private void OnDisable() {
        Shooting.OnAmmoChangedStatic -= SetAmmoText;
    }

    public void SetAmmoText(int amount)
    {
        _weaponInfo.setBulletCount(amount);
    }

    public void SetTimeEnergyBar(int amount)
    {
    }

    public void SetTimeEnergyIcon(bool _isOn)
    {
        if(_isOn)
            _timeEnergyIcon.DOColor(Color.yellow,0.5f);
        else
            _timeEnergyIcon.DOColor(Color.white,0.5f);
    }

    //10个块=100，每减10掉1个块
}
