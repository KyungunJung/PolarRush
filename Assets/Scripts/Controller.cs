using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public enum Polarity { North, South } 
    [SerializeField] private Polarity currentPolarity;           // 극성 설정
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
        // 자석 극성 얻기 (자석에 MagnetComponent 스크립트가 있다고 가정)
        Polarity magnetPolarity = magnetTransform.GetComponent<Magnet>().Polarity;

        // 방향 벡터 계산
        Vector2 direction = (magnetTransform.position - transform.position).normalized;

        // 같은 극이면 밀어내기, 다른 극이면 끌어당김
        if (currentPolarity == magnetPolarity)
        {
            // 반발력
            rb.AddForce(-direction * repulsionForce, ForceMode2D.Force);
        }
        else
        {
            // 인력
            rb.AddForce(direction * attractionForce, ForceMode2D.Force);
        }
    }
}
