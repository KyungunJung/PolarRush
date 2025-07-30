using UnityEngine;
using System.Collections.Generic;

public class MagnetSpawner : MonoBehaviour
{
    [Header("프리팹 및 풀")]
    [SerializeField] private GameObject magnetPairPrefab;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private float spawnInterval = 2f;

    [Header("Y 위치 설정")]
    [SerializeField] private float gapHeightMin = 2.5f;
    [SerializeField] private float gapHeightMax = 3.5f;

    [Header("폭 랜덤 범위")]
    [SerializeField] private float minScaleX = 0.5f;
    [SerializeField] private float maxScaleX = 2f;

    [Header("세로 길이 랜덤 범위")]
    [SerializeField] private float minScaleY = 1.5f;
    [SerializeField] private float maxScaleY = 3.5f;

    [Header("Easy Mode")]
    [SerializeField] private float easymodesec = 10f;

    [Header("플레이어 참조")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float distanceAhead = 16f;

    private List<GameObject> pool = new List<GameObject>();
    private float timer;
    private float gameStartTime;
    private float lastSpawnX = 0f;

    private bool easyMode => Time.time - gameStartTime < easymodesec;

    void Start()
    {
        gameStartTime = Time.time;

        float screenTopY = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, 0f)).y;
        float screenBottomY = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).y;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(magnetPairPrefab);
            obj.SetActive(false);
            pool.Add(obj);
        }

        lastSpawnX = 0f;
        for (int i = 0; i < 4; i++)
        {
            float x = i * 4;
            SpawnMagnetPairAtPosition(x);
            lastSpawnX = x + 4f;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        float currentInterval = GetCurrentSpawnInterval();
        if (timer >= currentInterval)
        {
            SpawnMagnetPair();
            timer = 0f;
        }

        float targetX = playerTransform.position.x + distanceAhead;
        while (lastSpawnX < targetX)
        {
            SpawnMagnetPairAtPosition(lastSpawnX); // lastSpawnX는 내부에서 갱신됨
        }
    }

    float GetCurrentSpawnInterval()
    {
        float baseSpeed = 2f;
        float baseInterval = spawnInterval;

        float speed = GameManager.Instance != null ? GameManager.Instance.GlobalMoveSpeed : baseSpeed;

        float scaled = baseInterval * (baseSpeed / speed);
        return Mathf.Clamp(scaled, 0.5f, baseInterval); // 최소 0.5초 보장
    }

    void SpawnMagnetPair()
    {
        SpawnMagnetPairAtPosition(playerTransform.position.x + distanceAhead);
    }

    float SpawnMagnetPairAtPosition(float startX)
    {
        GameObject obj = GetPooledObject();
        if (obj == null) return 0f;

        obj.transform.position = new Vector3(startX, 0f, 0f);

        Transform top = obj.transform.Find("TopMagnet");
        Transform bottom = obj.transform.Find("BottomMagnet");

        float topScaleX = Random.Range(minScaleX, maxScaleX);
        float bottomScaleX = Random.Range(minScaleX, maxScaleX);
        float topScaleY = Random.Range(minScaleY, maxScaleY);
        float bottomScaleY = Random.Range(minScaleY, maxScaleY);

        top.localScale = new Vector3(topScaleX, topScaleY, 1f);
        bottom.localScale = new Vector3(bottomScaleX, bottomScaleY, 1f);

        float topHeight = GetHeightWithCollider(top);
        float bottomHeight = GetHeightWithCollider(bottom);

        float minGap = (topHeight / 2f) + (bottomHeight / 2f) + 0.6f;
        float gap = Mathf.Max(Random.Range(gapHeightMin, gapHeightMax), minGap);

        float centerY = 0f;
        float topY = centerY + (gap / 2f) + (topHeight / 2f);
        float bottomY = centerY - (gap / 2f) - (bottomHeight / 2f);

        if (easyMode)
        {
            top.gameObject.SetActive(false);
        }
        else
        {
            top.gameObject.SetActive(true);
            top.localPosition = new Vector3(0f, topY, 0f);
            ApplyRandomPolarity(top);
        }

        bottom.localPosition = new Vector3(0f, bottomY, 0f);
        ApplyRandomPolarity(bottom);

        obj.SetActive(true);

        // 정확한 bounds 측정
        Bounds totalBounds = new Bounds(obj.transform.position, Vector3.zero);
        foreach (var col in obj.GetComponentsInChildren<Collider2D>())
        {
            if (col.gameObject.activeInHierarchy)
                totalBounds.Encapsulate(col.bounds);
        }

        float spacing = totalBounds.size.x;
        float rightEnd = totalBounds.max.x;

        float minGapX = 1.0f;
        float maxGapX = 1.5f;

        float randomOffset = Random.Range(minGapX, maxGapX);
        lastSpawnX = rightEnd + randomOffset;

        return spacing;
    }

    float GetHeightWithCollider(Transform magnet)
    {
        BoxCollider2D col = magnet.GetComponent<BoxCollider2D>();
        if (col == null)
        {
            Debug.LogWarning("Collider 없음: " + magnet.name);
            return magnet.localScale.y;
        }

        return col.size.y * magnet.localScale.y;
    }

    GameObject GetPooledObject()
    {
        foreach (var obj in pool)
        {
            if (!obj.activeInHierarchy)
                return obj;
        }
        return null;
    }

    void ApplyRandomPolarity(Transform magnet)
    {
        var m = magnet.GetComponent<Magnet>();
        if (m != null)
        {
            m.Polarity = (Random.value > 0.5f) ? Controller.Polarity.North : Controller.Polarity.South;
        }
    }

    public void ResetAllMagnetPairs()
    {
        foreach (var obj in pool)
        {
            obj.SetActive(false);
        }

        timer = 0f;
        gameStartTime = Time.time;

        // 기준 위치를 플레이어 기준으로 변경
        float startX = playerTransform.position.x + 2f;
        lastSpawnX = startX;

        for (int i = 0; i < 4; i++)
        {
            float x = startX + i * 4;
            SpawnMagnetPairAtPosition(x);
            lastSpawnX = x + 4f;
        }
    }


    public void SetGameStartTimeNow()
    {
        gameStartTime = Time.time;
    }
}
