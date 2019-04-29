using System;
using Fields;
using UnityEngine;

public class Player : MonoBehaviour
{
    public const float LerpingModifier = 3f;
    
    private Vector3? _lastPosition;
    private Field _currentField;

    private MoveStep _moveStep = MoveStep.Lerp1;
    
    private float _moveTimer;
    public int _fieldId;

    private string _playerName;
    private int _playerId;
    private int _publicId;

    private Action _movementFinishedCallback;

    private int _health = 10;
    private int _doubleDice;

    [SerializeField]
    private AudioClip dieClip;

    [SerializeField]
    private AudioSource audioSource;

    private void Start()
    {
        SetField(Game.Instance.GetStartField());
        _moveStep = MoveStep.Finish;
    }

    private void Update()
    {
        if (_moveStep == MoveStep.Stopped)
        {
            return;
        }

        if (_moveStep == MoveStep.PlaySound)
        {
            Game.Instance.GetWalkAudio().Play();
            _moveStep = MoveStep.Finish;
        }
        
        if (_moveStep == MoveStep.Finish)
        {
            _movementFinishedCallback?.Invoke();
            
            return;
        }

        Debug.Assert(_lastPosition != null, nameof(_lastPosition) + " != null");
        
        var pA = _lastPosition.Value;
        var pB = _currentField.transform.position + _currentField.GetOffset(_fieldId);
        var d = Vector3.Distance(pA, pB);
        var m = (pA + pB) / 2;
        var h = d * .3f;
        var q1 = m + new Vector3(0, h, 0);
        
        if (_moveStep == MoveStep.Lerp1)
        {
            LerpTo(pA, q1, MoveStep.Lerp2);

            return;
        }

        if (_moveStep == MoveStep.Lerp2)
        {
            LerpTo(q1, pB, MoveStep.PlaySound);
        }

        if (_moveStep == MoveStep.RePosition)
        {
            LerpTo(pA, pB, MoveStep.Finish);
        }
    }

    private void LerpTo(Vector3 start, Vector3 end, MoveStep nextStep)
    {
        transform.position = Vector3.Lerp(start, end, _moveTimer);
        _moveTimer += LerpingModifier * Time.deltaTime;

        if (_moveTimer >= 1f)
        {
            _moveStep = nextStep;
            _moveTimer = 0f;
        }
    }

    public void SetField(Field field)
    {
        if (_currentField)
        {
            _currentField.RemovePlayer(this);
            _lastPosition = transform.position;
        }
        
        _currentField = field;
        _moveStep = MoveStep.Lerp1;
        _moveTimer = 0;

        _fieldId = _currentField.AddPlayer(this);
    }

    public void SetFieldId(int id)
    {
        _fieldId = id;

        if (_moveStep != MoveStep.Finish) return;
        
        _lastPosition = transform.position;
        _moveStep = MoveStep.RePosition;
    }

    public Player SetPlayerNameAndId(string newName, int id)
    {
        _playerName = newName;
        _playerId = id;
        _publicId = id;

        return this;
    }

    public void RefreshPlayerId(int id)
    {
        _playerId = id;
    }

    public Field GetField() => _currentField;

    public string GetName() => _playerName;
    public int GetId() => _playerId;

    public int GetPublicId() => _publicId;

    public void RegisterMovementFinishedCallback(Action callback)
    {
        _movementFinishedCallback += callback;
    }

    public void ClearCallback()
    {
        _movementFinishedCallback = null;
    }

    public void Stop()
    {
        _moveStep = MoveStep.Stopped;
    }

    public virtual bool IsBot() => false;

    public int GetHealth() => _health;

    public void Hurt(int damage)
    {
        _health -= damage;
        Game.Instance.RenderHearts();

        SpawnBubble(-damage);

        var blood = Instantiate(Resources.Load<GameObject>("BloodParticles"));
        blood.transform.position = transform.position;

        if (_health <= 0)
        {
            Stop();
            Game.Instance.Kill(this);
            audioSource.PlayOneShot(dieClip);
            gameObject.AddComponent<Rigidbody>();
        }
    }

    private void SpawnBubble(int points)
    {
        var bubble = Instantiate(Resources.Load<GameObject>("HealthPopup"));
        bubble.transform.position = transform.position + new Vector3(0, 1.18f, -0.75f);
        bubble.GetComponent<Popup>().SetPoints(points);
    }

    public void Heal(int points)
    {
        _health = Mathf.Clamp(_health + points, 0, 10);
        Game.Instance.RenderHearts();

        SpawnBubble(points);
    }

    public int GetDoubleDice() => _doubleDice;

    public void DoubleDiceUsed()
    {
        _doubleDice--;
    }

    public void BoughtDoubleDice()
    {
        _doubleDice = 2;
    }
}

internal enum MoveStep
{
    Lerp1,
    Lerp2,
    Finish,
    RePosition,
    Stopped,
    PlaySound
}