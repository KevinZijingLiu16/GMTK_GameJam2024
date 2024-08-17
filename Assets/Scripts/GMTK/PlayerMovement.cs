using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpSpeed = 7f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    public float scaleIncreaseFactor = 1.1f; 
    public float gravityScaleIncrease = 0.9f; 
    public float jumpSpeedDecreaseFactor = 0.9f; 

    public float scaleDecreaseFactor = 0.9f; 
    public float gravityScaleDecrease = 1.1f; 
    public float jumpSpeedIncreaseFactor = 1.1f; 

    public Vector3 maxScale = new Vector3(5f, 5f, 5f); 
    public Vector3 minScale = new Vector3(0.2f, 0.2f, 0.2f); 
    public float maxGravityScale = 5f; 
    public float minGravityScale = 0.2f; 
    public float maxJumpSpeed = 20f; 
    public float minJumpSpeed = 2f; 

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool movingRight = true;


    public TextMeshProUGUI scaleText;
    private Vector3 initialScale;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        CircleCollider2D groundCheckCollider = groundCheck.gameObject.AddComponent<CircleCollider2D>();
        groundCheckCollider.isTrigger = true;
        groundCheckCollider.radius = groundCheckRadius;

        initialScale = transform.localScale;
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            Jump();
        }

        Move();

        UpdateScaleText();
    }

    private void UpdateScaleText()
    {
        float scaleMultiplier = transform.localScale.magnitude / initialScale.magnitude;
        scaleText.text = "Scale: " + scaleMultiplier.ToString("F2") + "x";
    }

    void Move()
    {
        float moveDirection = movingRight ? 1f : -1f;
        rb.velocity = new Vector2(moveSpeed * moveDirection, rb.velocity.y);
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
    }

    void Flip()
    {
        movingRight = !movingRight;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (movingRight ? 1 : -1); 
        transform.localScale = scale;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BouncingPlatform"))
        {
            Rigidbody2D playerRb = GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.velocity = new Vector2(playerRb.velocity.x, jumpSpeed * 3);
            }
        }
        else if (collision.CompareTag("Food")) 
        {
            EatFood();
            Destroy(collision.gameObject); 
        }
        else if (collision.CompareTag("LostWeight")) 
        {
            Shrink();
            Destroy(collision.gameObject); 
        }
    }

    void EatFood()
    {
        
        transform.localScale = new Vector3(
            Mathf.Clamp(Mathf.Abs(transform.localScale.x) * scaleIncreaseFactor, minScale.x, maxScale.x) * (movingRight ? 1 : -1),
            Mathf.Clamp(transform.localScale.y * scaleIncreaseFactor, minScale.y, maxScale.y),
            Mathf.Clamp(transform.localScale.z * scaleIncreaseFactor, minScale.z, maxScale.z)
        );

        
        rb.gravityScale = Mathf.Clamp(rb.gravityScale * gravityScaleIncrease, minGravityScale, maxGravityScale);

        
        jumpSpeed = Mathf.Clamp(jumpSpeed * jumpSpeedDecreaseFactor, minJumpSpeed, maxJumpSpeed);

        Debug.Log("Player ate food: Scale, gravity, and jump force adjusted.");
    }

    void Shrink()
    {
        
        transform.localScale = new Vector3(
            Mathf.Clamp(Mathf.Abs(transform.localScale.x) * scaleDecreaseFactor, minScale.x, maxScale.x) * (movingRight ? 1 : -1),
            Mathf.Clamp(transform.localScale.y * scaleDecreaseFactor, minScale.y, maxScale.y),
            Mathf.Clamp(transform.localScale.z * scaleDecreaseFactor, minScale.z, maxScale.z)
        );

        
        rb.gravityScale = Mathf.Clamp(rb.gravityScale * gravityScaleDecrease, minGravityScale, maxGravityScale);

        
        jumpSpeed = Mathf.Clamp(jumpSpeed * jumpSpeedIncreaseFactor, minJumpSpeed, maxJumpSpeed);

        Debug.Log("Player shrunk: Scale, gravity, and jump force adjusted.");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    Jump();
                    break;
                }
            }
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            Flip();
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    Jump();
                    break;
                }
            }
        }
    }
}
