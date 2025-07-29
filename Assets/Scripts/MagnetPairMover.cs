using UnityEngine;

public class MagnetPairMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

    private bool hasScored = false;
    private Transform player;

    void OnEnable()
    {
        hasScored = false;
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        if (!hasScored && transform.position.x < player.position.x)
        {
            hasScored = true;
            ScoreManager.Instance.AddPassedObstacle();
        }

        if (transform.position.x < -12f)
        {
            gameObject.SetActive(false);
        }
    }
}
