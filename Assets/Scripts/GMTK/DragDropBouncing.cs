using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DragDropBouncing : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Vector3 worldPlatformScale = new Vector3(1f, 1f, 1f);
    [SerializeField] private float bounceMultiplier = 3f; 
    [SerializeField] private float effectDuration = 1f;  

    [Header("ScaleChangeEffect")]
    [SerializeField] private float scaleEffectDuration = 0.5f;   
    [SerializeField] private float pressedMulX = 1.5f;           
    [SerializeField] private float pressedMulY = 0.5f;           
    [SerializeField] private float releasedMulX = 0.5f;          
    [SerializeField] private float releasedMulY = 1.5f;          

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
        GameObject worldPlatform = new GameObject("BouncingPlatform");

        worldPlatform.tag = "BouncingPlatform";
        worldPlatform.layer = LayerMask.NameToLayer("Ground");

        BoxCollider2D boxCollider = worldPlatform.AddComponent<BoxCollider2D>();
        SpriteRenderer spriteRenderer = worldPlatform.AddComponent<SpriteRenderer>();

        spriteRenderer.sprite = GetComponent<Image>().sprite;

        worldPlatform.transform.position = worldPosition;
        worldPlatform.transform.localScale = worldPlatformScale;

        boxCollider.size = spriteRenderer.bounds.size / worldPlatform.transform.localScale.x;

        Rigidbody2D rb = worldPlatform.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;

        
        worldPlatform.AddComponent<BouncingPlatformCollisionHandler>().Initialize(bounceMultiplier, effectDuration, scaleEffectDuration, pressedMulX, pressedMulY, releasedMulX, releasedMulY);

        worldPlatform.AddComponent<FadeAndDestroy>();
    }

    
    private class BouncingPlatformCollisionHandler : MonoBehaviour
    {
        private float bounceMultiplier;
        private float effectDuration;
        private float scaleEffectDuration;
        private float pressedMulX;
        private float pressedMulY;
        private float releasedMulX;
        private float releasedMulY;
        private Vector3 originalScale;

        public void Initialize(float bounceMultiplier, float effectDuration, float scaleEffectDuration, float pressedMulX, float pressedMulY, float releasedMulX, float releasedMulY)
        {
            this.bounceMultiplier = bounceMultiplier;
            this.effectDuration = effectDuration;
            this.scaleEffectDuration = scaleEffectDuration;
            this.pressedMulX = pressedMulX;
            this.pressedMulY = pressedMulY;
            this.releasedMulX = releasedMulX;
            this.releasedMulY = releasedMulY;
            originalScale = transform.localScale;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("BouncingPlatform: Player collided with platform!");

                Rigidbody2D rb = collision.collider.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    float initialVerticalSpeed = rb.velocity.y;

                    rb.velocity = new Vector2(0, initialVerticalSpeed * bounceMultiplier);

                    Debug.Log("BouncingPlatform: Applied bounce force to player.");

                    StartCoroutine(RestoreInitialSpeed(rb, initialVerticalSpeed));
                    StartCoroutine(ApplyScaleEffect());
                }
            }
        }

        private IEnumerator RestoreInitialSpeed(Rigidbody2D rb, float initialVerticalSpeed)
        {
            yield return new WaitForSeconds(effectDuration);

            rb.velocity = new Vector2(rb.velocity.x, initialVerticalSpeed);
            Debug.Log("BouncingPlatform: Restored initial speed to player.");
        }

        private IEnumerator ApplyScaleEffect()
        {
            
            transform.localScale = new Vector3(originalScale.x * pressedMulX, originalScale.y * pressedMulY, originalScale.z);
            yield return new WaitForSeconds(scaleEffectDuration);

            
            transform.localScale = new Vector3(originalScale.x * releasedMulX, originalScale.y * releasedMulY, originalScale.z);
            yield return new WaitForSeconds(scaleEffectDuration);

           
            transform.localScale = originalScale;
            Debug.Log("BouncingPlatform: Restored original scale.");
        }
    }
}
