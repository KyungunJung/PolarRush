using UnityEngine;
using System.Collections.Generic;

public class MagnetSpawner : MonoBehaviour
{
    [SerializeField] private GameObject magnetPairPrefab;
    [SerializeField] private int poolSize = 5;
    [SerializeField] private float spawnInterval = 2f;

    [Header("Y ��ġ ���� ����")]
    [SerializeField] private float minY = -1f;
    [SerializeField] private float maxY = 1f;

    [Header("���Ʒ� �ڼ� �� �Ÿ�")]
    [SerializeField] private float minGap = 2.5f;
    [SerializeField] private float maxGap = 3.5f;
  
    [Header("x�� �ּ� / �ִ� ũ��")]
    [SerializeField] private float minScaleX = 0.5f;
    [SerializeField] private float maxScaleX = 2.0f;

    [SerializeField] private float screenTopY = 5f;
    [SerializeField] private float screenBottomY = -5f;
    [SerializeField] private float gapHeightMin = 2f;
    [SerializeField] private float gapHeightMax = 3.5f;

    private List<GameObject> pool = new List<GameObject>();
    private float timer;

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(magnetPairPrefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
        for (int i = 0; i < 3; i++)
        {
            float x = i * 4;
            SpawnMagnetPairAtPosition(x);
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
    }

    void SpawnMagnetPair()
    {
        GameObject obj = GetPooledObject();
        if (obj == null) return;

        // �θ�(Pair) ��ġ�� ȭ�� �����ʿ� ��ġ
        obj.transform.position = new Vector3(10f, 0f, 0f);

        Transform top = obj.transform.Find("TopMagnet");
        Transform bottom = obj.transform.Find("BottomMagnet");

        float gapHeight = Random.Range(gapHeightMin, gapHeightMax);
        float totalHeight = screenTopY - screenBottomY;
        float usableHeight = totalHeight - gapHeight;
        float ratio = Random.Range(0.3f, 0.7f);
        float topHeight = usableHeight * ratio;
        float bottomHeight = usableHeight * (1 - ratio);

        //  �ڽ��� ���� ��ġ�� ��ġ
        top.localPosition = new Vector3(0f, screenTopY, 0f);
        bottom.localPosition = new Vector3(0f, screenBottomY, 0f);

        top.localScale = new Vector3(top.localScale.x, topHeight, 1f);
        bottom.localScale = new Vector3(bottom.localScale.x, bottomHeight, 1f);

        //  �ؼ� ���� ����
        Magnet topMagnet = top.GetComponent<Magnet>();
        Magnet bottomMagnet = bottom.GetComponent<Magnet>();
        topMagnet.Polarity = Random.value > 0.5f ? Controller.Polarity.North : Controller.Polarity.South;
        bottomMagnet.Polarity = Random.value > 0.5f ? Controller.Polarity.North : Controller.Polarity.South;

        obj.SetActive(true);
    }
    void SpawnMagnetPairAtPosition(float xPos)
    {
        GameObject obj = GetPooledObject();
        if (obj == null) return;

        obj.transform.position = new Vector3(xPos, 0f, 0f);

        Transform top = obj.transform.Find("TopMagnet");
        Transform bottom = obj.transform.Find("BottomMagnet");

        float gapHeight = Random.Range(gapHeightMin, gapHeightMax);
        float totalHeight = screenTopY - screenBottomY;
        float usableHeight = totalHeight - gapHeight;
        float ratio = Random.Range(0.3f, 0.7f);
        float topHeight = usableHeight * ratio;
        float bottomHeight = usableHeight * (1 - ratio);

        top.localPosition = new Vector3(0f, screenTopY, 0f);
        bottom.localPosition = new Vector3(0f, screenBottomY, 0f);
        top.localScale = new Vector3(top.localScale.x, topHeight, 1f);
        bottom.localScale = new Vector3(bottom.localScale.x, bottomHeight, 1f);

        Magnet topMagnet = top.GetComponent<Magnet>();
        Magnet bottomMagnet = bottom.GetComponent<Magnet>();
        topMagnet.Polarity = Random.value > 0.5f ? Controller.Polarity.North : Controller.Polarity.South;
        bottomMagnet.Polarity = Random.value > 0.5f ? Controller.Polarity.North : Controller.Polarity.South;

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
}
