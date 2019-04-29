using System.Collections.Generic;
using UnityEngine;

namespace Fields
{
    public class CrossingField : Field
    {
        [SerializeField]
        private Field[] additionalFields;

        private static readonly List<GameObject> Arrows = new List<GameObject>();

        private bool _waitingForBot = false;
        private float _waitTimer;
        private Player _bot;
        
        public override void OnEnter(Player player)
        {
            var fields = new List<Field> {nextField};
            fields.AddRange(additionalFields);
            
            fields.ForEach(field =>
            {
                var arrow = Instantiate(Resources.Load<GameObject>("Arrow"));
                
                var pA = transform.position;
                var pB = field.transform.position;
                var d = Vector3.Distance(pA, pB);
                var m = (pA + pB) / 2;
                var h = d * .3f;
                var q1 = m + new Vector3(0, h, 0);

                arrow.transform.position = q1;
                arrow.transform.LookAt(field.transform);
                arrow.transform.rotation = Quaternion.Euler(
                    0,
                    arrow.transform.rotation.eulerAngles.y - 90,
                    0
                );

                arrow.GetComponent<Arrow>().Init(player, field);
                if (player.IsBot())
                {
                    _bot = player;
                    Destroy(arrow.GetComponent<Arrow>());
                }
                else
                {
                    Game.Instance.SetArrowInfoVisiblity(true);
                }

                Arrows.Add(arrow);
            });

            if (player.IsBot())
            {
                _waitingForBot = true;
                _waitTimer = Random.Range(1f, 2f);
            }
            
            Game.Instance.Wait(float.MaxValue);
        }

        public static void PathSelected(Field field, Player player)
        {
            Arrows.ForEach(arrow =>
            {
                Destroy(arrow.gameObject);
            });
            
            Arrows.Clear();
            Game.Instance.SetArrowInfoVisiblity(false);
            
            player.RegisterMovementFinishedCallback(() => { Game.Instance.HandleFinishedMovement(player); });
            player.SetField(field);

            Game.Instance.StopWaiting();
        }

        private void Update()
        {
            if (!_waitingForBot)
            {
                return;
            }

            _waitTimer -= Time.deltaTime;

            if (_waitTimer > 0f)
            {
                return;
            }
            
            var fields = new List<Field> {nextField};
            fields.AddRange(additionalFields);

            PathSelected(fields[Random.Range(0, fields.Count)], _bot);
            
            _waitTimer = 0f;
            _waitingForBot = false;
            _bot = null;
        }

        protected override void OnDrawGizmos()
        {
            if (!nextField)
            {
                return;
            }
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, nextField.transform.position);
            foreach (var field in additionalFields)
            {
                Gizmos.DrawLine(transform.position, field.transform.position);
            }
        }
        
        public override bool AllowTrapPlacement() => false;
    }
}