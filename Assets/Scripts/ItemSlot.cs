// using UnityEngine;
//
// public class ItemSlot : MonoBehaviour
// {
//     public ItemManager itemManager;
//     public Item item;
//     private RectTransform rectTransform;
//
//     void Awake()
//     {
//         rectTransform = GetComponent<RectTransform>();
//     }
//
//     public void Combine(DragAndDrop draggedItem)
//     {
//         Debug.Log($"Combine: Trying to combine {draggedItem.gameObject.name} with {gameObject.name}");
//
//         // Obtener el Item del componente DragAndDrop arrastrado
//         Item draggedItemData = draggedItem.GetComponent<ItemComponent>().item;
//         
//         if (draggedItemData != null)
//         {
//             Item combinedItem = itemManager.CombineItems(this.item, draggedItemData);
//             if (combinedItem != null)
//             {
//                 Debug.Log($"Combine: Combination successful, new item is {combinedItem.itemName}");
//
//                 // Replace items with the new combined item
//                 this.item = combinedItem;
//                 this.GetComponent<UnityEngine.UI.Image>().sprite = combinedItem.sprite;
//                 draggedItem.GetComponent<ItemComponent>().ClearItem();
//                 draggedItem.GetComponent<DragAndDrop>().OriginalPosition = rectTransform.anchoredPosition; // Update original position to the new slot
//             }
//             else
//             {
//                 Debug.Log($"Combine: No valid combination found for {draggedItem.gameObject.name} and {gameObject.name}");
//                 // Si no hay combinación, regresar el ítem a su posición original
//                 draggedItem.GetComponent<RectTransform>().anchoredPosition = draggedItem.OriginalPosition;
//             }
//         }
//         else
//         {
//             Debug.Log($"Combine: Dragged item {draggedItem.gameObject.name} has no item data");
//         }
//     }
//
//     public void ClearItem()
//     {
//         Debug.Log($"ClearItem: Clearing item in slot {gameObject.name}");
//         item = null;
//         GetComponent<UnityEngine.UI.Image>().sprite = null;
//     }
// }