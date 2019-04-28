using System.Collections.Generic;
using UnityEngine;

namespace Fields
{
    public class JumpShortcutField : Field, IShortcutField
    {
        [SerializeField]
        private Field target;

        private List<GameObject> _arrows = new List<GameObject>();
        
        public override void OnStay(Player player)
        {
            Game.Instance.GetCamera().Zoom();
            Game.Instance.ShowShortcutDialog(3, player, this);
            
//            target.GetComponentInChildren<MeshRenderer>().material.color = new Color(1, 0.5f, 1f);
            var arrow1 = Instantiate(Resources.Load<GameObject>("Arrow"));
            arrow1.transform.position = target.transform.position + new Vector3(2.2358f, 0.360138f, 0);
            arrow1.transform.localScale = new Vector3(.4f, .2f, .6f);
            Destroy(arrow1.GetComponent<Arrow>());
            
            arrow1.transform.LookAt(target.transform);
            arrow1.transform.rotation = Quaternion.Euler(
                0,
                arrow1.transform.rotation.eulerAngles.y - 90,
                0
            );
            
            _arrows.Add(arrow1);
            
            var arrow2 = Instantiate(Resources.Load<GameObject>("Arrow"));
            arrow2.transform.position = target.transform.position + new Vector3(-2.2358f, 0.360138f, 0);
            arrow2.transform.localScale = new Vector3(.4f, .2f, .6f);
            Destroy(arrow2.GetComponent<Arrow>());
            
            arrow2.transform.LookAt(target.transform);
            arrow2.transform.rotation = Quaternion.Euler(
                0,
                arrow2.transform.rotation.eulerAngles.y - 90,
                0
            );
            
            _arrows.Add(arrow2);
            
            var arrow3 = Instantiate(Resources.Load<GameObject>("Arrow"));
            arrow3.transform.position = target.transform.position + new Vector3(0, 0.360138f,2.2358f);
            arrow3.transform.localScale = new Vector3(.4f, .2f, .6f);
            Destroy(arrow3.GetComponent<Arrow>());
            
            arrow3.transform.LookAt(target.transform);
            arrow3.transform.rotation = Quaternion.Euler(
                0,
                arrow3.transform.rotation.eulerAngles.y - 90,
                0
            );
            
            _arrows.Add(arrow3);
            
            var arrow4 = Instantiate(Resources.Load<GameObject>("Arrow"));
            arrow4.transform.position = target.transform.position + new Vector3(0, 0.360138f,-2.2358f);
            arrow4.transform.localScale = new Vector3(.4f, .2f, .6f);
            Destroy(arrow4.GetComponent<Arrow>());
            
            arrow4.transform.LookAt(target.transform);
            arrow4.transform.rotation = Quaternion.Euler(
                0,
                arrow4.transform.rotation.eulerAngles.y - 90,
                0
            );
            
            _arrows.Add(arrow4);

            Game.Instance.Wait(float.MaxValue);
        }

        public void Accepted(Player player)
        {
            _arrows.ForEach(Destroy);
            _arrows.Clear();
            
            Game.Instance.HideShortcutDialog();
            Game.Instance.GetCamera().ExitZoom();
            Game.Instance.StopWaiting();
            
            player.Hurt(6);
            
            player.RegisterMovementFinishedCallback(() => { Game.Instance.HandleFinishedMovement(player); });
            player.SetField(target);            
        }

        public void Canceled(Player player)
        {
            _arrows.ForEach(Destroy);
            _arrows.Clear();
            
            Game.Instance.HideShortcutDialog();
            Game.Instance.GetCamera().ExitZoom();
            Game.Instance.StopWaiting();
            Game.Instance.Wait(.1f);
        }
    }
} 