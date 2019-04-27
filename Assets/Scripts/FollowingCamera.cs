using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    private const float RotateSpeed = 2f;
    
    private Vector3 _offset = new Vector3(-1.2f, 11.3f, 2f);
    
    private Vector3 _rotateOffset = new Vector3(0, 5, 8);
    
    private Transform _target;
    
    private bool _rotating;

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public void RotateAround()
    {
        _rotating = true;

        var position = _target.position;
        transform.position  = new Vector3(
                position.x + _rotateOffset.x,
                13f,
                position.z + _rotateOffset.z
        );
        
        transform.rotation = Quaternion.Euler(45, 180, 0);
    }

    private void Update()
    {
        if (!_rotating)
        {
            var position = _target.position;
            transform.position = Vector3.Lerp(transform.position, new Vector3(
                    position.x + _offset.x,
                    13f,
                    position.z + _offset.z
                ), 4 * Time.deltaTime);

            return;
        }

        transform.LookAt(_target);
        transform.Translate(Vector3.right * RotateSpeed * Time.deltaTime);
    }
}