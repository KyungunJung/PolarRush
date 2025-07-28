using UnityEngine;

public class Controller : MonoBehaviour
{
    public enum Polarity { North, South }

    [Header("�ؼ� ����")]
    [SerializeField] private Polarity currentPolarity = Polarity.North;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("�ڷ� ����")]
    [SerializeField] private float attractionForce = 5f;
    [SerializeField] private float repulsionForce = 3f;
    [SerializeField] private float detectRadius = 5f;
    [SerializeField] private LayerMask magnetLayer;

    [Header("�̵� ����")]
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
        // X���� ���� �ӵ��� ����, Y���� �ڷ¿� ���� ��ȭ
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

            // ��/�Ʒ� �ڼ��� ����: Y�� ���̰� ����� ū ��츸
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
            // ���� �� �� �ݹ�
            rb.AddForce(-direction * repulsionForce, ForceMode2D.Force);
        }
        else
        {
            // �ٸ� �� �� �η�
            rb.AddForce(direction * attractionForce, ForceMode2D.Force);
        }
    }
}
