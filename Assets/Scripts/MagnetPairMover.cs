using UnityEngine;

public class MagnetPairMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;                 // 초기 속도 (미사용)


    private Transform player;


    void OnEnable()
    {

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // GameManager의 전역 속도 사용
        transform.position += Vector3.left * GameManager.Instance.GlobalMoveSpeed * Time.deltaTime;

        if (transform.position.x < -12f)
        {
            gameObject.SetActive(false);
        }
    }
}
