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

    [Header("카메라 기준 화면 높이")]
    [SerializeField] private float screenTopY = 6f;
    [SerializeField] private float screenBottomY = -6f;

    [Header("Easy Mode")]
    [SerializeField] private float easymodesec = 10f;

    [Header("플레이어 참조")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float distanceAhead = 16f; // 플레이어 앞에 유지할 거리

    private List<GameObject> pool = new List<GameObject>();
    private float timer;
    private float gameStartTime;
    private float lastSpawnX = 0f;

    private bool easyMode => Time.time - gameStartTime < easymodesec;

    void Start()
    {
        gameStartTime = Time.time;

        // 화면 위아래 계산
        screenTopY = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, 0f)).y;
        screenBottomY = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).y;

        // 오브젝트 풀 생성
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(magnetPairPrefab);
            obj.SetActive(false);
            pool.Add(obj);
        }

        // 초기 배치
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
        if (timer >= spawnInterval)
        {
            SpawnMagnetPair();
            timer = 0f;
        }

        // 항상 플레이어 앞쪽에 자석 유지
        float targetX = playerTransform.position.x + distanceAhead;
        while (lastSpawnX < targetX)
        {
            SpawnMagnetPairAtPosition(lastSpawnX);
            lastSpawnX += 4f;
        }
    }

    void SpawnMagnetPair()
    {
        SpawnMagnetPairAtPosition(playerTransform.position.x + distanceAhead);
    }

    void SpawnMagnetPairAtPosition(float xPos)
    {
        GameObject obj = GetPooledObject();
        if (obj == null) return;

        obj.transform.position = new Vector3(xPos, 0f, 0f);

        Transform top = obj.transform.Find("TopMagnet");
        Transform bottom = obj.transform.Find("BottomMagnet");

        float gap = Random.Range(gapHeightMin, gapHeightMax);
        float topHeight = Random.Range(minScaleY, maxScaleY);
        float bottomHeight = Random.Range(minScaleY, maxScaleY);
        float topWidth = Random.Range(minScaleX, maxScaleX);
        float bottomWidth = Random.Range(minScaleX, maxScaleX);

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
            top.localScale = new Vector3(topWidth, topHeight, 1f);
            ApplyRandomPolarity(top);
        }

        bottom.localPosition = new Vector3(0f, bottomY, 0f);
        bottom.localScale = new Vector3(bottomWidth, bottomHeight, 1f);
        ApplyRandomPolarity(bottom);

        obj.SetActive(true);
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

        lastSpawnX = 0f;
        for (int i = 0; i < 4; i++)
        {
            float x = i * 4;
            SpawnMagnetPairAtPosition(x);
            lastSpawnX = x + 4f;
        }

        timer = 0f;
        gameStartTime = Time.time; // Easy 모드 재시작
    }
}
