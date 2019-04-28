using System.Collections.Generic;
using UnityEngine;

namespace Fields
{
    public class Field : MonoBehaviour
    {
        [SerializeField]
        private Field nextField;

        public Field GetNext() => nextField;

        private List<Player> _players = new List<Player>();

        private const float Size = 1f;

        public int AddPlayer(Player player)
        {
            _players.Add(player);

            RecalculatePositions();
            
            return _players.Count;
        }

        public void RemovePlayer(Player player)
        {
            _players.Remove(player);
            
            RecalculatePositions();
        }

        public void RecalculatePositions()
        {
            for (int i = 1; i <= _players.Count; i++)
            {
                _players[i - 1].SetFieldId(i);
            }
        }

        public Vector3 GetOffset(int id)
        {
            if (_players.Count == 1)
            {
                return new Vector3(0, .62f, 0);
            }

            if (_players.Count == 2)
            {
                return new[]
                {
                    new Vector3(Size / 4f - Size / 2f, .62f, Size / 4f - Size / 2f), 
                    new Vector3(Size - Size / 4f - Size / 2f, .62f, Size - Size / 4f - Size / 2f), 
                }[id-1];
            }
            
            if (_players.Count == 3)
            {
                return new[]
                {
                    new Vector3(0f, 0.62f, Size / 4 - Size / 2),
                    new Vector3(Size / 4f - Size / 2f, 0.62f, Size - Size / 4f - Size / 2f),
                    new Vector3(Size - Size / 4f - Size / 2f, .62f, Size - Size / 4f - Size / 2f),
                }[id-1];
            }
            
            if (_players.Count == 4)
            {
                return new[]
                {
                    new Vector3(Size / 4f - Size / 2f, .62f, Size / 4f - Size / 2f), 
                    new Vector3(Size - Size / 4f - Size / 2f, .62f, Size / 4f - Size / 2f), 
                    new Vector3(Size / 4f - Size / 2f, 0.62f, Size - Size / 4f - Size / 2f),
                    new Vector3(Size - Size / 4f - Size / 2f, .62f, Size - Size / 4f - Size / 2f), 
                }[id-1];
            }

            return Vector3.zero;
        }

        private void OnDrawGizmos()
        {
            if (!nextField)
            {
                return;
            }
            
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, nextField.transform.position);
        }

        public virtual void OnEnter()
        {
            // Nothing
        }

        public virtual void OnStay(Player player)
        {
            // Nothing
        }
    }
}
