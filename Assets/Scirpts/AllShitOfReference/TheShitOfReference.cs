using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TheShitOfReference
{
    private static AllShitOfReference allShitOfReference;
    public static AllShitOfReference AllShitOfReference{
        get{
            if(allShitOfReference == null){
                allShitOfReference = Resources.Load<AllShitOfReference>("AllShitOfReferenceSO");
            }
            return allShitOfReference;
        }
    }
}
