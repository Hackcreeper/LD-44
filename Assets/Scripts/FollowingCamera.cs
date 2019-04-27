using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    private Vector3 _offset = new Vector3(-1.2f, 11.3f, 2f);

    private Transform _target;

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    private void Update()
    {
        var position = _target.position;
        transform.position  = Vector3.Lerp(transform.position, new Vector3(
            position.x + _offset.x,
            13f,
            position.z + _offset.z
        ), 4 * Time.deltaTime);
    }
}