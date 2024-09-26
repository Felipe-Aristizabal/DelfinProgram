using System.Collections;
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
    private Vector2 dragOffset;

    private bool isOverlapping = false;
    private float overlapTime = 0f;
    private ItemComponent overlappingItemComponent = null;
    private float requiredOverlapTime = 1f; // El tiempo que los ítems deben estar en contacto para combinarse

    [SerializeField] private Sprite recipeSlot;

    public Vector2 OriginalPosition
    {
        get { return originalPosition; }
        private set { originalPosition = value; }
    }

    void Awake()
    {
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
            itemCanvas.sortingOrder = 100; // Bring to front
        }

        OriginalPosition = rectTransform.anchoredPosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint);
        dragOffset = rectTransform.anchoredPosition - localPoint;

        isOverlapping = false;
        overlapTime = 0f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint);
        rectTransform.anchoredPosition = localPoint + dragOffset;

        // Si está en contacto, incrementar el tiempo de contacto
        if (isOverlapping && overlappingItemComponent != null)
        {
            overlapTime += Time.deltaTime;
            Debug.Log("Tiempo de contacto con " + overlappingItemComponent.gameObject.name + ": " + overlapTime);

            // Si el tiempo de contacto es suficiente, combinar los ítems
            if (overlapTime >= requiredOverlapTime)
            {
                Debug.Log("Combinando ítems después de " + overlapTime + " segundos.");
                Combine(overlappingItemComponent);
                isOverlapping = false; // Evitar múltiples combinaciones
            }
        }
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
            itemCanvas.sortingOrder = originalSortingOrder;
        }

        rectTransform.anchoredPosition = OriginalPosition;

        // Reiniciar los valores cuando se deja de arrastrar
        isOverlapping = false;
        overlapTime = 0f;
        overlappingItemComponent = null;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        ItemComponent otherItemComponent = other.GetComponent<ItemComponent>();
        if (otherItemComponent == null)
        {
            Debug.LogError("El objeto con el que se intentó combinar no tiene un componente ItemComponent.");
            return;
        }

        // Iniciar corrutina que verifica la combinación después de un tiempo
        StartCoroutine(WaitAndCombine(otherItemComponent));
    }
        
    void OnTriggerExit2D(Collider2D other)
    {
        // Aquí podrías detener la corrutina si los objetos dejan de estar en contacto
        StopAllCoroutines();
        Debug.Log("Saliendo del contacto con: " + other.gameObject.name);
    }

    IEnumerator WaitAndCombine(ItemComponent otherItemComponent)
    {
        // Tiempo de espera antes de intentar combinar (por ejemplo, 1 segundo)
        float waitTime = 0.3f;

        // Esperar el tiempo definido
        yield return new WaitForSeconds(waitTime);

        // Después de la espera, intenta combinar los ítems
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
            Debug.Log("Intentando combinar: " + thisItem.itemName + " con " + otherItem.itemName);

            Item combinedItem = itemManager.CombineItems(thisItem, otherItem);

            if (combinedItem != null)
            {
                Debug.Log("Combinación exitosa: " + combinedItem.itemName);

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
                    color.a = 0f; // Hacer el sprite completamente transparente
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
