using System;
using Fields;
using UnityEngine;

public class BoatAnimation : MonoBehaviour
{
    private Player _player;

    private Vector3 _playerPosition;

    [SerializeField]
    private Transform playerInBoat;

    [SerializeField]
    private Animator boatAnimator;
    
    private Field _targetField;

    private float _moveTimer;
    
    public static BoatAnimation Instance { private set; get; }
    
    private AnimationStep _animationStep = AnimationStep.Idle;

    private bool _waitingForAnimation;

    private void Awake()
    {
        Instance = this;
    }

    public void StartAnimation(Player player, Field targetField)
    {
        _player = player;
        _targetField = targetField;
        _playerPosition = _player.transform.position;
        _animationStep = AnimationStep.JumpOnBoat1;
        _waitingForAnimation = false;
        _moveTimer = 0f;
    }

    private void Update()
    {
        if (_animationStep == AnimationStep.Idle)
        {
            return;
        }
        
        if (_animationStep == AnimationStep.JumpOnBoat1)
        {
            var pA = _playerPosition;
            var pB = playerInBoat.position;
            var d = Vector3.Distance(pA, pB);
            var m = (pA + pB) / 2;
            var h = d * .3f;
            var q1 = m + new Vector3(0, h, 0);
            
            LerpTo(pA, q1, AnimationStep.JumpOnBoat2);
            return;
        }

        if (_animationStep == AnimationStep.JumpOnBoat2)
        {
            var pA = _playerPosition;
            var pB = playerInBoat.position;
            var d = Vector3.Distance(pA, pB);
            var m = (pA + pB) / 2;
            var h = d * .3f;
            var q1 = m + new Vector3(0, h, 0);

            LerpTo(q1, pB, AnimationStep.DriveBoat);
            return;
        }


        if (_waitingForAnimation)
        {
            _player.transform.position = playerInBoat.transform.position;

            if (boatAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                boatAnimator.Play("BoatEnd");
                
                _animationStep = AnimationStep.Idle;
                
                Game.Instance.StopWaiting();
            
                _player.RegisterMovementFinishedCallback(() => { Game.Instance.HandleFinishedMovement(_player); });
                _player.SetField(_targetField);
            }
                
            return;
        }

        if (_animationStep == AnimationStep.DriveBoat)
        {
            _waitingForAnimation = true;
            Game.Instance.GetWalkAudio().Play();
            boatAnimator.Play("BoatStart");
        }
    }
    
    private void LerpTo(Vector3 start, Vector3 end, AnimationStep nextStep)
    {
        _player.transform.position = Vector3.Lerp(start, end, _moveTimer);
        _moveTimer += Player.LerpingModifier * Time.deltaTime;

        if (_moveTimer >= 1f)
        {
            _animationStep = nextStep;
            _moveTimer = 0f;
        }
    }
}

internal enum AnimationStep
{
    Idle,
    JumpOnBoat1,
    JumpOnBoat2,
    DriveBoat,
    JumpOffBoat1,
    JumpOffBoat2,
}