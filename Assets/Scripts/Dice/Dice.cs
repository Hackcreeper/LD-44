using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dice
{
    public class Dice : MonoBehaviour
    {
        private Quaternion _targetRotation;
        private Rigidbody _rigidbody;

        private const float SmoothModifier = 0.9f;

        private float _progress;
        private bool _grounded;
        private bool _finished;

        [SerializeField]
        private DiceFace[] _faces;

        private Action<int> _whenFinished;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _targetRotation = Random.rotationUniform;
        }

        private void Update()
        {
            if (_finished)
            {
                return;
            }
            
            if (_grounded)
            {
                if (_rigidbody.angularVelocity.magnitude > 0f)
                {
                    return;
                }

                _finished = true;
                _whenFinished?.Invoke(3);
                
                return;
            }
            
            _progress += SmoothModifier * Time.deltaTime;
            _rigidbody.rotation = Quaternion.Slerp(Quaternion.identity, _targetRotation, _progress);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit))
            {
                if (hit.distance < 0.8f)
                {
                    _grounded = true;
                }
            }
        }

        public int GetHighestFace()
        {
            var highest = float.MinValue;
            
            
            return 1;
        }

        public void RegisterCallback(Action<int> callback)
        {
            _whenFinished += callback;
        }
    }
}