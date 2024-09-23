using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private int originalSortingOrder;
    private Canvas itemCanvas;
    [SerializeField] private Sprite recipeSlot; 
    
    
    public Vector2 OriginalPosition
    {
        get { return originalPosition; }
        private set { originalPosition = value; }
    }

    void Awake()
    {
        // Debug.Log("Este script está adjunto al objeto: " + gameObject.name);
        
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        itemCanvas = GetComponent<Canvas>();
        if (itemCanvas != null)
        {
            originalSortingOrder = itemCanvas.sortingOrder;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;
        }

        if (itemCanvas != null)
        {
            itemCanvas.sortingOrder = 100; // Un valor alto para asegurarse de que está al frente
        }

        OriginalPosition = rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        if (itemCanvas != null)
        {
            itemCanvas.sortingOrder = originalSortingOrder; // Restaurar el orden original
        }

        // Restaurar la posición original si no hay combinación
        rectTransform.anchoredPosition = OriginalPosition;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        ItemComponent otherItemComponent = other.GetComponent<ItemComponent>();
        if (otherItemComponent == null)
        {
            Debug.LogError("El objeto con el que se intentó combinar no tiene un componente ItemComponent");
            return;
        }

        Combine(otherItemComponent);
    }

    private void Combine(ItemComponent otherItemComponent)
    {
        Item thisItem = GetComponent<ItemComponent>().item;
        Item otherItem = otherItemComponent.item;
        ItemManager itemManager = FindObjectOfType<ItemManager>();

        if (itemManager == null)
        {
            Debug.LogError("No se encontró el ItemManager.");
            return;
        }

        if (thisItem != null && otherItem != null)
        {
            Item combinedItem = itemManager.CombineItems(thisItem, otherItem);

            if (combinedItem != null)
            {
                // Actualizar el ítem del objeto sobre el que se arrastró
                otherItemComponent.item = combinedItem;
                otherItemComponent.GetComponent<UnityEngine.UI.Image>().sprite = combinedItem.sprite;

                // Borrar los datos del ítem arrastrado
                GetComponent<ItemComponent>().ClearItem();
                
                // Actualizar el sprite del ítem arrastrado a transparente
                Image draggedItemImage = GetComponent<UnityEngine.UI.Image>();
                if (draggedItemImage != null)
                {
                    Color color = draggedItemImage.color;
                    color.a = 0f;  // Hacer el sprite completamente transparente
                    draggedItemImage.color = color;
                }
            }
            else
            {
                Debug.Log("No se pudo combinar los ítems.");
            }
        }
    }
}
