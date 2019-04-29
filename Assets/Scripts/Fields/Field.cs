using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fields
{
    public class Field : MonoBehaviour
    {
        [SerializeField] protected Field nextField;

        public Field GetNext() => nextField;

        private List<Player> _players = new List<Player>();

        private const float Size = 1f;
        
        protected readonly List<GameObject> Arrows = new List<GameObject>();

        private bool _hasTrap;

        private GameObject _spikes;
        private Player _spikeOwner;

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

        protected virtual void OnDrawGizmos()
        {
            if (!nextField)
            {
                return;
            }
            
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, nextField.transform.position);
        }

        public virtual void OnEnter(Player player)
        {
            // Nothing
        }

        public virtual void OnStay(Player player)
        {
            if (!_hasTrap || player == _spikeOwner)
            {
                return;
            }
            
            Game.Instance.GetSpikeAudio().Play();
            
            _spikes.GetComponent<Animator>().Play("Active");
            player.Hurt(2);

            if (_spikeOwner.GetHealth() > 0)
            {
                _spikeOwner.Heal(2);
            }
            
            Game.Instance.Wait(1);
        }

        private void OnMouseEnter()
        {
            if (!Game.Instance.IsTrapPlacementMode() || !AllowTrapPlacement())
            {
                return;
            }
            
            SpawnArrows();
        }
        
        private void OnMouseExit()
        {
            if (!Game.Instance.IsTrapPlacementMode() || !AllowTrapPlacement())
            {
                return;
            }
            
            ClearArrows();
        }

        private void OnMouseDown()
        {
            if (!Game.Instance.IsTrapPlacementMode() || !AllowTrapPlacement())
            {
                return;
            }
            
            var player = Game.Instance.GetActivePlayer();
            
            _spikes = Instantiate(Resources.Load<GameObject>("Spikes"), transform, true);
            _spikes.transform.localPosition = new Vector3(0, 0, 0);
            _spikeOwner = player;

            _hasTrap = true;
            ClearArrows();
            
            Game.Instance.SetTrapPlacementMode(false);

            player.Hurt(4);
            if (player.GetHealth() <= 0)
            {
                return;
            }
            
            Game.Instance.GetCamera().ExitZoom();
            Game.Instance.StopWaiting();

            player.RegisterMovementFinishedCallback(() => { Game.Instance.HandleFinishedMovement(player); });
            player.SetField(player.GetField().GetNext());
        }

        public void SpawnArrows()
        {
            var arrow1 = Instantiate(Resources.Load<GameObject>("Arrow"));
            arrow1.transform.position = transform.position + new Vector3(2.2358f, 0.360138f, 0);
            arrow1.transform.localScale = new Vector3(.4f, .2f, .6f);
            Destroy(arrow1.GetComponent<Arrow>());

            arrow1.transform.LookAt(transform);
            arrow1.transform.rotation = Quaternion.Euler(
                0,
                arrow1.transform.rotation.eulerAngles.y - 90,
                0
            );

            Arrows.Add(arrow1);

            var arrow2 = Instantiate(Resources.Load<GameObject>("Arrow"));
            arrow2.transform.position = transform.position + new Vector3(-2.2358f, 0.360138f, 0);
            arrow2.transform.localScale = new Vector3(.4f, .2f, .6f);
            Destroy(arrow2.GetComponent<Arrow>());

            arrow2.transform.LookAt(transform);
            arrow2.transform.rotation = Quaternion.Euler(
                0,
                arrow2.transform.rotation.eulerAngles.y - 90,
                0
            );

            Arrows.Add(arrow2);

            var arrow3 = Instantiate(Resources.Load<GameObject>("Arrow"));
            arrow3.transform.position = transform.position + new Vector3(0, 0.360138f, 2.2358f);
            arrow3.transform.localScale = new Vector3(.4f, .2f, .6f);
            Destroy(arrow3.GetComponent<Arrow>());

            arrow3.transform.LookAt(transform);
            arrow3.transform.rotation = Quaternion.Euler(
                0,
                arrow3.transform.rotation.eulerAngles.y - 90,
                0
            );

            Arrows.Add(arrow3);

            var arrow4 = Instantiate(Resources.Load<GameObject>("Arrow"));
            arrow4.transform.position = transform.position + new Vector3(0, 0.360138f, -2.2358f);
            arrow4.transform.localScale = new Vector3(.4f, .2f, .6f);
            Destroy(arrow4.GetComponent<Arrow>());

            arrow4.transform.LookAt(transform);
            arrow4.transform.rotation = Quaternion.Euler(
                0,
                arrow4.transform.rotation.eulerAngles.y - 90,
                0
            );

            Arrows.Add(arrow4);
        }

        public void ClearArrows()
        {
            Arrows.ForEach(Destroy);
            Arrows.Clear();
        }

        protected virtual bool AllowTrapPlacement() => !_hasTrap;
    }
}
