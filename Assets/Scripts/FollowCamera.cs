using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public GameObject target;
    public Vector3 offset = Vector3.zero;

    void LateUpdate()
    {
        if (target != null && offset != Vector3.zero)
        {
            transform.position = target.transform.position + offset;
        }
    }
}
