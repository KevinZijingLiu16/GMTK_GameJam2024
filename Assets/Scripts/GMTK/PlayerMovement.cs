using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpSpeed = 7f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    public float scaleStep = 0.25f;
    public float gravityScaleStep = 0.1f;
    public float jumpSpeedStep = 0.5f;

    public Vector3 maxScale = new Vector3(3f, 3f, 3f);
    public Vector3 minScale = new Vector3(0.25f, 0.25f, 0.25f);
    public float maxGravityScale = 5f;
    public float minGravityScale = 0.2f;
    public float maxJumpSpeed = 20f;
    public float minJumpSpeed = 2f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool movingRight = true;

    public TextMeshProUGUI scaleText;
    private float scaleCounter = 1f;
    private Vector3 initialScale;
    private float initialGravityScale;
    private float initialJumpSpeed;
    private bool isAtCheckPoint = false;

    private SpriteRenderer spriteRenderer;

    public float targetMin = 0f;
    public float targetMax = 3.2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        CircleCollider2D groundCheckCollider = groundCheck.gameObject.AddComponent<CircleCollider2D>();
        groundCheckCollider.isTrigger = true;
        groundCheckCollider.radius = groundCheckRadius;

        initialScale = transform.localScale;
        initialGravityScale = rb.gravityScale;
        initialJumpSpeed = jumpSpeed;

        spriteRenderer = transform.Find("Image").GetComponent<SpriteRenderer>();

        UpdateScaleText();
    }

    void Update()
    {
        if (!isAtCheckPoint)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

            if (isGrounded)
            {
                Jump();
            }

            Move();
        }
    }

    private void UpdateScaleText()
    {
        scaleText.text = "Scale: " + scaleCounter.ToString("F2") + "x";

        if (scaleCounter >= maxScale.x)
        {
            scaleText.text += " (You have reached the maximum scale!)";
        }
        else if (scaleCounter <= minScale.x)
        {
            scaleText.text += " (You have reached the minimum scale!)";
        }
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
                playerRb.velocity = new Vector2(playerRb.velocity.x, jumpSpeed * 2);
            }
        }
        else if (collision.CompareTag("Food"))
        {
            if (scaleCounter < maxScale.x)
            {
                scaleCounter = Mathf.Min(scaleCounter + scaleStep, maxScale.x);
                UpdatePlayerScale();
            }
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("LostWeight"))
        {
            if (scaleCounter > minScale.x)
            {
                scaleCounter = Mathf.Max(scaleCounter - scaleStep, minScale.x);
                UpdatePlayerScale();
            }
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("CheckPoint"))
        {
            HandleCheckPoint();
        }
        else if (collision.CompareTag("Trap"))
        {
            StartCoroutine(HandleTrapCollision());
        }
    }

    void HandleCheckPoint()
    {
        isAtCheckPoint = true; // 停止玩家的移动和跳跃

        // 定义目标 scaleCounter 的范围，例如 2.8 到 3.2
        

        // 检查 scaleCounter 是否在目标范围内
        if (scaleCounter >= targetMin && scaleCounter <= targetMax)
        {
            scaleText.text = "Congrats, you win!";
            StartCoroutine(ProceedToNextLevel(3f)); // 3秒后进入下一关
        }
        else
        {
            scaleText.text = "Haven't reached the target, go back and try to control your weight.";
            StartCoroutine(RestoreScaleText(3f)); // 3秒后恢复 scaleText
        }
    }

    IEnumerator HandleTrapCollision()
    {
        spriteRenderer.color = Color.black; // 将玩家变为黑色
        yield return new WaitForSeconds(1f); // 等待1秒

        // 开始淡出效果
        float fadeDuration = 1f;
        float fadeStep = 0.05f;
        for (float alpha = 1f; alpha > 0; alpha -= fadeStep)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(fadeDuration * fadeStep);
        }

        // 重启当前场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator ProceedToNextLevel(float delay)
    {
        yield return new WaitForSeconds(delay);
        // 在这里加载下一关的代码
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Debug.Log("Proceeding to next level...");
    }

    IEnumerator RestoreScaleText(float delay)
    {
        yield return new WaitForSeconds(delay);
        UpdateScaleText();
        isAtCheckPoint = false;
    }

    void UpdatePlayerScale()
    {
        transform.localScale = new Vector3(
            initialScale.x * scaleCounter * (movingRight ? 1 : -1),
            initialScale.y * scaleCounter,
            initialScale.z * scaleCounter
        );

        rb.gravityScale = Mathf.Clamp(initialGravityScale * (1 / scaleCounter), minGravityScale, maxGravityScale);
        jumpSpeed = Mathf.Clamp(initialJumpSpeed * (1 / scaleCounter), minJumpSpeed, maxJumpSpeed);
        Debug.Log("Player scale adjusted. Current scale: " + scaleCounter);
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
