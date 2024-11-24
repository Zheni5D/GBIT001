using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



[CreateAssetMenu(menuName="Project-Blood/DeadBody")]
public class DeadBodySO : ScriptableObject
{
    [Serializable]
    public struct DictionaryElement{
       public AttackArea.AttackType type;
        public List<Sprite> spriteList;
    }
    public List<DictionaryElement> deadBodyDictionary;
}
