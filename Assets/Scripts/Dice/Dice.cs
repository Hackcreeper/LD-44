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

        private float _timer = 1.5f;

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
                _timer -= Time.deltaTime;
                
                if (_timer > 0f || _rigidbody.angularVelocity.magnitude > 0f)
                {
                    return;
                }

                _finished = true;
                _whenFinished?.Invoke(GetHighestFace());
                
                return;
            }
            
            _progress += SmoothModifier * Time.deltaTime;
            _rigidbody.rotation = Quaternion.Slerp(Quaternion.identity, _targetRotation, _progress);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit))
            {
                if (hit.distance < 0.9f)
                {
                    _grounded = true;
                }
            }
        }

        private int GetHighestFace()
        {
            return 1;
            
            var highest = float.MinValue;
            int highestFace = 1;
            
            foreach (var face in _faces)
            {
                var y = face.transform.position.y;
                if (y > highest)
                {
                    highest = y;
                    highestFace = face.GetEyes();
                }
            }
            
            return highestFace;
        }

        public void RegisterCallback(Action<int> callback)
        {
            _whenFinished += callback;
        }
    }
}