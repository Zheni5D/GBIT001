using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName = "TileBaseDictionarySO", menuName = "Project-Blood/TileBaseDictionarySO", order = 3)]
public class TileBaseDictionarySO : ScriptableObject {
    [Serializable]
    public struct DictionaryElement{
        public TileBase key;
        public TileBase value;
    }
    [SerializeField]private List<DictionaryElement> TileBaseDictionary;
    public bool Contains(TileBase key){
        foreach (var element in TileBaseDictionary)
        {
            if(element.key == key){
                return true;
            }
        }
        return false;
    }

    public TileBase GetValue(TileBase key){
        foreach (var element in TileBaseDictionary)
        {
            if(element.key == key){
                return element.value;
            }
        }
        return null;
    }
}