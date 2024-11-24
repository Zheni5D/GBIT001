using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WeaponInfo : MonoBehaviour
{
    private Sprite[] numbers{
        get{
            return UIManager.Instance.numbers;
        }
    }
    [SerializeField]private Image one;
    [SerializeField]private Image ten;
    [SerializeField]private Image hdr;
    [SerializeField]private int bulletCount;
    // Start is called before the first frame update
    void Start()
    {
        setBulletCount(0);
    }

    // Update is called once per frame
    void Update()
    {
        // if(Input.GetKeyDown(KeyCode.Return)){
        //     setBulletCount(bulletCount);
        // }
    }

    public void setBulletCount(int count){
        int h = count / 100;
        count %= 100;
        int t = count / 10;
        count %= 10;
        hdr.sprite = numbers[h];
        ten.sprite = numbers[t];
        one.sprite = numbers[count];
    }
}
