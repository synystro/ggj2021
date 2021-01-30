using System.Collections.Generic;
using UnityEngine;

public static class Inventory {    
    public static List<Item> items = new List<Item>();

    public static void Init() {
        //items.Add(Resources.Load<Item>("Items/key_frontdoor"));
        //Debug.Log("frontdoor key added to player's inventory");
    }
}
