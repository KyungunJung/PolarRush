using UnityEngine;

public class MagnetPairMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;                 // �ʱ� �ӵ� (�̻��)


    private Transform player;


    void OnEnable()
    {

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // GameManager�� ���� �ӵ� ���
        transform.position += Vector3.left * GameManager.Instance.GlobalMoveSpeed * Time.deltaTime;

        if (transform.position.x < -12f)
        {
            gameObject.SetActive(false);
        }
    }
}
