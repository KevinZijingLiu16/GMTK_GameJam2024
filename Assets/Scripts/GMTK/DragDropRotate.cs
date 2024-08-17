using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DragDropRotate : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Vector3 worldPlatformScale = new Vector3(1f, 1f, 1f);
    [SerializeField] private float rotationSpeed = 100f; 
    [SerializeField] private bool clockwise = true;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 initialPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        initialPosition = rectTransform.anchoredPosition; 
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin drag on: " + gameObject.name);
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End drag on: " + gameObject.name);

        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CreateWorldPlatform(worldPosition);

        rectTransform.anchoredPosition = initialPosition;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pointer down on: " + gameObject.name);
    }

    private void CreateWorldPlatform(Vector2 worldPosition)
    {
        GameObject worldPlatform = new GameObject("RotatingPlatform");

        worldPlatform.tag = "Obstacle";
        worldPlatform.layer = LayerMask.NameToLayer("Ground");

        BoxCollider2D boxCollider = worldPlatform.AddComponent<BoxCollider2D>();
        SpriteRenderer spriteRenderer = worldPlatform.AddComponent<SpriteRenderer>();

        spriteRenderer.sprite = GetComponent<Image>().sprite;

        worldPlatform.transform.position = worldPosition;
        worldPlatform.transform.localScale = worldPlatformScale;

        boxCollider.size = spriteRenderer.bounds.size / worldPlatform.transform.localScale.x;

        Rigidbody2D rb = worldPlatform.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;

       
        RotatingPlatform rotatingPlatform = worldPlatform.AddComponent<RotatingPlatform>();
        rotatingPlatform.rotationSpeed = rotationSpeed;
        rotatingPlatform.clockwise = clockwise;
    }
}

public class RotatingPlatform : MonoBehaviour
{
    public float rotationSpeed = 100f; 
    public bool clockwise = true;

    private void Update()
    {
        
        float direction = clockwise ? -1f : 1f;
        transform.Rotate(0, 0, rotationSpeed * direction * Time.deltaTime);
    }
}
