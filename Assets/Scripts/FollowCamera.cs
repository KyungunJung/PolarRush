using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FollowCamera : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    void LateUpdate()
    {
        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition + offset, smoothSpeed);
    }
}
