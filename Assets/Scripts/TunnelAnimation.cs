using Fields;
using UnityEngine;

public class TunnelAnimation : MonoBehaviour
{
    private Player _player;

    private Vector3 _playerPosition;

    [SerializeField] private Transform tunnelEntryPosition;
    
    [SerializeField] private Transform tunnelExitPosition;

    private Field _targetField;

    private float _moveTimer;

    public static TunnelAnimation Instance { private set; get; }

    private TunnelAnimationStep _animationStep = TunnelAnimationStep.Idle;

    private void Awake()
    {
        Instance = this;
    }

    public void StartAnimation(Player player, Field targetField)
    {
        _player = player;
        _targetField = targetField;
        _playerPosition = _player.transform.position;
        _animationStep = TunnelAnimationStep.GoInTunnel;
        _moveTimer = 0f;
    }

    private void Update()
    {
        if (_animationStep == TunnelAnimationStep.Idle)
        {
            return;
        }

        if (_animationStep == TunnelAnimationStep.GoInTunnel)
        {
            var pA = _playerPosition;
            var pB = tunnelEntryPosition.position;
            var d = Vector3.Distance(pA, pB);
            var m = (pA + pB) / 2;
            var h = d * .3f;
            var q1 = m + new Vector3(0, h, 0);

            LerpTo(pA, q1, TunnelAnimationStep.MoveToExit);
            return;
        }
        
        if (_animationStep == TunnelAnimationStep.MoveToExit)
        {
            var pA = tunnelEntryPosition.position;
            var pB = tunnelExitPosition.position;
            var d = Vector3.Distance(pA, pB);
            var m = (pA + pB) / 2;
            var h = d * .3f;
            var q1 = m + new Vector3(0, h, 0);

            LerpTo(pA, q1, TunnelAnimationStep.MoveToField);
            return;
        }

        Game.Instance.GetWalkAudio().Play();
        Game.Instance.StopWaiting();

        _player.RegisterMovementFinishedCallback(() => { Game.Instance.HandleFinishedMovement(_player); });
        _player.SetField(_targetField);

        _animationStep = TunnelAnimationStep.Idle;
    }

    private void LerpTo(Vector3 start, Vector3 end, TunnelAnimationStep nextStep)
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

internal enum TunnelAnimationStep
{
    Idle,
    GoInTunnel,
    MoveToExit,
    MoveToField
}