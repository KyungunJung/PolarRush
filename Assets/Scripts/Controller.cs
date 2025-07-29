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

    // 초기화용 
    private Vector3 initialPosition;
    private Polarity initialPolarity;


    public bool IsControllable { get; set; } = false; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 초기 상태 저장
        initialPosition = transform.position;
        initialPolarity = currentPolarity;

        UpdateColor();
    }

    void FixedUpdate()
    {
        if (!IsControllable) return;

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

        Vector2 offset = magnetTransform.position - transform.position;
        float distance = offset.magnitude;
        if (distance < 0.01f) distance = 0.01f;

        Vector2 direction = offset.normalized;

        if (magnet.Polarity == currentPolarity)
        {
            // 같은 극성 → 반발력 (거리 기반)
            float forceMagnitude = repulsionForce * (1f + (1f / distance)) * 0.3f;

            // 💡 반발은 impulse 느낌 유지, 방향 제한 없음
            rb.AddForce(-direction * forceMagnitude, ForceMode2D.Impulse);
        }
        else
        {
            // 다른 극성 → 인력 (거리에 따라 줄여도 됨)
            float forceMagnitude = attractionForce;
            rb.AddForce(direction * forceMagnitude, ForceMode2D.Force);
        }
    }




    public void ResetController()
    {
        // 위치 및 속도 초기화
        transform.position = initialPosition;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // 극성 초기화
        currentPolarity = initialPolarity;
        UpdateColor();
    }
    public void SetRadius()
    {

        detectRadius = 5f;
    }

}
