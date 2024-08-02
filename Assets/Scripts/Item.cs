using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;
    public Sprite sprite;
    public bool isUsedInRecipe;
    public bool isLastTier;
}

[System.Serializable]
public class Recipe
{
    public Item item1;
    public Item item2;
    public Item result;
}

