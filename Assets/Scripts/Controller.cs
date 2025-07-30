using UnityEngine;

public class Controller : MonoBehaviour
{
    public enum Polarity { North, South }

    [Header("극성 설정")]
    [SerializeField] private Polarity currentPolarity = Polarity.North;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("자력 설정")]
    [SerializeField] private float attractionForce = 300f;
    [SerializeField] private float repulsionForce = 180f;
    [SerializeField] private float detectRadius = 5f;
    [SerializeField] private LayerMask magnetLayer;

    [Header("사망 조건")]
    [SerializeField] private float screenTopY = 6f;
    [SerializeField] private float screenBottomY = -6f;
    [SerializeField] private float killDistance = 0.5f;

    private Rigidbody2D rb;

    private Vector3 initialPosition;
    private Polarity initialPolarity;

    public bool IsControllable { get; set; } = false;

    private bool isDead = false;

    private Vector2 accumulatedImpulse = Vector2.zero;
    private Vector2 accumulatedForce = Vector2.zero;

    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        initialPosition = transform.position;
        initialPolarity = currentPolarity;

        UpdateColor();
    }

    void FixedUpdate()
    {
        if (!IsControllable || isDead) return;

        // 반발력 처리 (Impulse)
        if (accumulatedImpulse != Vector2.zero)
        {
            rb.AddForce(accumulatedImpulse, ForceMode2D.Impulse);
            accumulatedImpulse = Vector2.zero;
        }

        // 인력 처리 (Force)
        if (accumulatedForce != Vector2.zero)
        {
            rb.AddForce(accumulatedForce * Time.fixedDeltaTime, ForceMode2D.Force);
            accumulatedForce = Vector2.zero;
        }
    }

    void Update()
    {
        if (!IsControllable || isDead) return;

        if (Input.GetMouseButtonDown(0))
        {
            TogglePolarity();
        }

        DetectClosestMagnet();
        CheckDeathByPosition();
        CheckDeathByPolarity();
    }

    void TogglePolarity()
    {
        currentPolarity = (currentPolarity == Polarity.North) ? Polarity.South : Polarity.North;
        UpdateColor();
    }

    void UpdateColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = (currentPolarity == Polarity.North) ? Color.blue : Color.red;
        }
    }

    void DetectClosestMagnet()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRadius, magnetLayer);

        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            Vector2 offset = hit.transform.position - transform.position;
            if (Mathf.Abs(offset.y) < 0.5f) continue;

            float dist = offset.sqrMagnitude;
            if (dist < minDist)
            {
                minDist = dist;
                closest = hit.transform;
            }
        }

        if (closest != null)
        {
            ApplyMagnetForce(closest);
        }
    }

    void ApplyMagnetForce(Transform magnetTransform)
    {
        Magnet magnet = magnetTransform.GetComponent<Magnet>();
        if (magnet == null) return;

        Vector2 offset = magnetTransform.position - transform.position;
        float distance = Mathf.Max(offset.magnitude, 0.01f);
        Vector2 direction = offset.normalized;

        if (magnet.Polarity == currentPolarity)
        {
            // 반발력 (위로 밀어냄)
            float forceMagnitude = repulsionForce * (1f + (1f / distance)) * 0.3f;
            accumulatedImpulse += -direction * forceMagnitude;
        }
        else
        {
            // 인력 (아래로 당김)
            float forceMagnitude = attractionForce;
            accumulatedForce += direction * forceMagnitude;
        }
    }

    void CheckDeathByPosition()
    {
        float y = transform.position.y;
        if (y > screenTopY || y < screenBottomY)
        {
            Die("OutOfBounds");
        }
    }

    void CheckDeathByPolarity()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRadius, magnetLayer);

        foreach (var hit in hits)
        {
            Magnet magnet = hit.GetComponent<Magnet>();
            if (magnet == null) continue;

            if (magnet.Polarity != currentPolarity)
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < killDistance)
                {
                    Die("OppositePolarity");
                    break;
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        Magnet magnet = collision.gameObject.GetComponent<Magnet>();
        if (magnet == null) return;

        if (magnet.Polarity != currentPolarity)
        {
            Die("CollidedWithOppositePolarity");
        }
    }

    void Die(string reason)
    {
        if (isDead) return;
        isDead = true;

        IsControllable = false;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        ScoreManager.Instance?.StopScoring();
        UIManager.Instance?.GameOver();

        Debug.Log($"플레이어 사망 - 이유: {reason}");
    }

    public void ResetController()
    {
        transform.position = initialPosition;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        currentPolarity = initialPolarity;
        isDead = false;
        IsControllable = false;
        rb.isKinematic = false;

        UpdateColor();
    }

    public void SetRadius()
    {
        detectRadius = 3f;
    }
}
