using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public enum Polarity { North, South } 
    [SerializeField] private Polarity currentPolarity;           // �ؼ� ����
    [SerializeField] private float attractionForce = 5f;
    [SerializeField] private float repulsionForce = 3f;

    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

  
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            currentPolarity = (currentPolarity == Polarity.North) ? Polarity.South : Polarity.North;
            
          
        }
        DetectClosestMagnet();
    }

    [SerializeField] private float detectRadius = 5f;
    [SerializeField] private LayerMask magnetLayer;

    void DetectClosestMagnet()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRadius, magnetLayer);

        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            float dist = Vector2.Distance(transform.position, hit.transform.position);
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
        // �ڼ� �ؼ� ��� (�ڼ��� MagnetComponent ��ũ��Ʈ�� �ִٰ� ����)
        Polarity magnetPolarity = magnetTransform.GetComponent<Magnet>().Polarity;

        // ���� ���� ���
        Vector2 direction = (magnetTransform.position - transform.position).normalized;

        // ���� ���̸� �о��, �ٸ� ���̸� ������
        if (currentPolarity == magnetPolarity)
        {
            // �ݹ߷�
            rb.AddForce(-direction * repulsionForce, ForceMode2D.Force);
        }
        else
        {
            // �η�
            rb.AddForce(direction * attractionForce, ForceMode2D.Force);
        }
    }
}
