using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        // Verificar si estamos en el nivel 3
        bool isLevel3 = itemManager.GetCurrentLevel() == 3;
        bool magicNecklacePlaced = false;

        // Recorrer todos los ItemComponents en el grid
        foreach (var itemComponent in itemComponents)
        {
            // Verificar si el sprite tiene transparencia (alpha = 0)
            Image itemImage = itemComponent.GetComponent<UnityEngine.UI.Image>();
            if (itemImage != null && itemImage.color.a == 0f)
            {
                // Restaurar la transparencia original
                Color color = itemImage.color;
                color.a = 1f;  // Devolver la transparencia al valor completo
                itemImage.color = color;
            }

            // Si el itemComponent está vacío, añadir un nuevo ítem
            if (itemComponent.item == null || string.IsNullOrEmpty(itemComponent.item.itemName) || itemComponent.item.sprite == null)
            {
                // Para el nivel 3, colocar solo 1 "Magic Necklace" aleatoriamente
                if (isLevel3 && !magicNecklacePlaced)
                {
                    Item magicNecklace = itemsForCurrentLevel.Find(item => item.itemName == "Magic_Necklace");
                    if (magicNecklace != null)
                    {
                        // Asignar el "Magic Necklace" al slot actual
                        itemComponent.item = magicNecklace;
                        itemComponent.GetComponent<UnityEngine.UI.Image>().sprite = magicNecklace.sprite;

                        // Remover el "Magic Necklace" de la lista
                        itemsForCurrentLevel.Remove(magicNecklace);
                        
                        // Marcar que ya hemos colocado un "Magic Necklace"
                        magicNecklacePlaced = true;
                        continue; // Saltar al siguiente slot sin hacer más relleno
                    }
                }

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
