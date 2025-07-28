using UnityEngine;

public class MagnetPairMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

    void Update()
    {
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        if (transform.position.x < -12f)
        {
            gameObject.SetActive(false); 
        }
    }
}
