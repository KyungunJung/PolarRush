using UnityEngine;

public class Controller : MonoBehaviour
{
    public enum Polarity { North, South }

    [Header("극성 설정")]
    [SerializeField] private Polarity currentPolarity = Polarity.North;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("자력 설정")]
    [SerializeField] private float attractionForce = 5f;
    [SerializeField] private float repulsionForce = 3f;
    [SerializeField] private float detectRadius = 5f;
    [SerializeField] private LayerMask magnetLayer;

    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 2f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateColor();
    }

    void FixedUpdate()
    {
        // X축은 일정 속도로 진행, Y축은 자력에 의해 변화
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TogglePolarity();
        }

        DetectClosestMagnet();
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

            // 위/아래 자석만 감지: Y축 차이가 충분히 큰 경우만
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

        Vector2 direction = (magnetTransform.position - transform.position).normalized;

        if (magnet.Polarity == currentPolarity)
        {
            // 같은 극 → 반발
            rb.AddForce(-direction * repulsionForce, ForceMode2D.Force);
        }
        else
        {
            // 다른 극 → 인력
            rb.AddForce(direction * attractionForce, ForceMode2D.Force);
        }
    }
}
