using Fields;
using UnityEngine;

public class TowerAnimation : MonoBehaviour
{
    private Player _player;

    private Vector3 _playerPosition;

    [SerializeField]
    private Transform towerPosition;

    private Field _targetField;

    private float _moveTimer;
    
    public static TowerAnimation Instance { private set; get; }
    
    private TowerAnimationStep _animationStep = TowerAnimationStep.Idle;

    private void Awake()
    {
        Instance = this;
    }

    public void StartAnimation(Player player, Field targetField)
    {
        _player = player;
        _targetField = targetField;
        _playerPosition = _player.transform.position;
        _animationStep = TowerAnimationStep.JumpOnTower1;
        _moveTimer = 0f;
    }

    private void Update()
    {
        if (_animationStep == TowerAnimationStep.Idle)
        {
            return;
        }
        
        if (_animationStep == TowerAnimationStep.JumpOnTower1)
        {
            var pA = _playerPosition;
            var pB = towerPosition.position;
            var d = Vector3.Distance(pA, pB);
            var m = (pA + pB) / 2;
            var h = d * .3f;
            var q1 = m + new Vector3(0, h, 0);
            
            LerpTo(pA, q1, TowerAnimationStep.JumpOnTower2);
            return;
        }

        if (_animationStep == TowerAnimationStep.JumpOnTower2)
        {
            var pA = _playerPosition;
            var pB = towerPosition.position;
            var d = Vector3.Distance(pA, pB);
            var m = (pA + pB) / 2;
            var h = d * .3f;
            var q1 = m + new Vector3(0, h, 0);

            LerpTo(q1, pB, TowerAnimationStep.MoveToField);
            return;
        }

        Debug.Log("Finish!");
        Game.Instance.StopWaiting();
            
        _player.RegisterMovementFinishedCallback(() => { Game.Instance.HandleFinishedMovement(_player); });
        _player.SetField(_targetField);

        _animationStep = TowerAnimationStep.Idle;
    }
    
    private void LerpTo(Vector3 start, Vector3 end, TowerAnimationStep nextStep)
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

internal enum TowerAnimationStep
{
    Idle,
    JumpOnTower1,
    JumpOnTower2,
    MoveToField
}