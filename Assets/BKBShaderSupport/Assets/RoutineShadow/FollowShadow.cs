using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// internal class ShadowData
// {
//     public string SortingLayerName;
//     public int SortingOrder;
//     public SpriteRenderer Sprite;
//     public Color Color;
//     public bool Ready;
//     public ShadowData()
//     {
//         SortingLayerName = "Default";
//         SortingOrder = 0;
//         Sprite = null;
//         Color = new Color(1, 1, 1, 1);
//         Ready = false;
//     }
//     public ShadowData(string sortingLayerName, int sortingOrder, SpriteRenderer spriteRenderer, Color color, bool ready)
//     {
//         this.SortingLayerName = sortingLayerName;
//         this.SortingOrder = sortingOrder;
//         this.Sprite = spriteRenderer;
//         this.Color = color;
//         this.Ready = ready;
//     }
// }
public class FollowShadow : MonoBehaviour
{
    [SerializeField] private bool isGenerating;
    [SerializeField] private int maxShadowCacheCount = 10;
    [SerializeField,ColorUsage(true,true)]private Color shadowColor;
    [SerializeField] private Material shadowMaterial;
    [SerializeField] private float record_interval;
    private bool isGamePause = false;
    private List<SpriteRenderer> shadows = new List<SpriteRenderer>();
    private SpriteRenderer spr;
    private float _timer;
    private void OnEnable() {
        MessageCenter.RemoveListner(OnGamePauseOn);
        MessageCenter.AddListener(OnGamePauseOff);
    }
    private void OnDisable() {
        MessageCenter.RemoveListner(OnGamePauseOn);
        MessageCenter.RemoveListner(OnGamePauseOff);
    }

    private void Start()
    {
        GameObject parent = new GameObject(gameObject.name+ " FollowShadowParent");
        spr = GetComponent<SpriteRenderer>();
        for (int i = 0; i < maxShadowCacheCount; i++)
        {
            var g = new GameObject("Shadow" + i);
            SpriteRenderer s = g.AddComponent<SpriteRenderer>();
            s.transform.SetParent(parent.transform);
            s.sortingLayerName = spr.sortingLayerName;
            s.sortingOrder = spr.sortingOrder - 1;
            s.sprite = spr.sprite;
            s.color = new Color(0,0,0,.5f);
            s.material = shadowMaterial;
            s.gameObject.SetActive(false);
            shadows.Add(s);
        }
    }

    private void FixedUpdate()
    {
        if (!isGamePause && isGenerating)
        {
            _timer += Time.fixedDeltaTime;
            if (_timer >= record_interval)
            {
                _timer = 0;
                ShowShadow();
            }
        }
    }

    SpriteRenderer GetOneShadow()
    {
        foreach (var shadow in shadows)
        {
            if (shadow.gameObject.activeSelf == false)
            {
                return shadow;
            }
        }
        return null;
    }

    void ShowShadow()
    {
        SpriteRenderer shadow = GetOneShadow();
        if (shadow is not null)
        {
            shadow.gameObject.SetActive(true);
            shadow.sprite = spr.sprite;
            shadow.transform.position = transform.position;
            shadow.color = shadowColor;
            shadow.transform.rotation = transform.rotation;
            shadow.DOFade(0, 0.5f).SetEase(Ease.InExpo).OnComplete(() =>
            {
                shadow.gameObject.SetActive(false);
            });
        }
    }

    private void OnGamePauseOn(CommonMessage msg){
        if(msg.Mid != (int)MESSAGE_TYPE.PAUSE_ON) return;
        isGamePause = true;
    }

    private void OnGamePauseOff(CommonMessage msg){
        if(msg.Mid != (int)MESSAGE_TYPE.PAUSE_OFF) return;
        isGamePause = false;
    }

    public void SetGenerating(bool value)
    {
        isGenerating = value;
    }
}
