using System.Collections.Generic;
using UnityEngine;

public class GridFiller : MonoBehaviour
{
    [SerializeField] private ItemManager itemManager;

    private List<ItemComponent> itemComponents = new List<ItemComponent>();

    void Start()
    {
        // Obtener todos los ItemComponents en el grid y almacenarlos en una lista
        foreach (Transform slot in transform)
        {
            foreach (Transform child in slot)
            {
                ItemComponent itemComponent = child.GetComponent<ItemComponent>();
                if (itemComponent != null)
                {
                    itemComponents.Add(itemComponent);
                }
            }
        }

        // Suscribirse al evento de combinación de ítems
        if (itemManager != null)
        {
            itemManager.OnItemsCombined += FillGrid;
        }

        FillGrid();
    }

    public void FillGrid()
    {
        if (itemManager == null) 
        {
            Debug.LogError("ItemManager is not assigned.");
            return;
        }

        // Obtener la lista de ítems disponibles para el nivel actual
        List<Item> itemsForCurrentLevel = itemManager.GetItemsForCurrentLevel();

        // Recorrer todos los ItemComponents en el grid
        foreach (var itemComponent in itemComponents)
        {
            // Si el itemComponent está vacío, añadir un nuevo ítem
            if (itemComponent.item == null || string.IsNullOrEmpty(itemComponent.item.itemName) || itemComponent.item.sprite == null)
            {
                // Asignar un ítem aleatorio de la lista al nuevo ítem
                if (itemsForCurrentLevel.Count > 0)
                {
                    int randomIndex = Random.Range(0, itemsForCurrentLevel.Count);
                    Item randomItem = itemsForCurrentLevel[randomIndex];

                    // Asignar el nuevo ítem
                    itemComponent.item = randomItem;
                    itemComponent.GetComponent<UnityEngine.UI.Image>().sprite = randomItem.sprite;
                }
            }
        }
    }
   
    private void OnDestroy()
    {
        if (itemManager != null)
        {
            itemManager.OnItemsCombined -= FillGrid;
        }
    }
}
