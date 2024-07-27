using UnityEngine;

public class ItemComponent : MonoBehaviour
{
    public Item item;

    public void ClearItem()
    {
        item = null;
        GetComponent<UnityEngine.UI.Image>().sprite = null;
    }
}