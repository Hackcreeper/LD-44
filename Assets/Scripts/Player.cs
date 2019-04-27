using Fields;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class Player : MonoBehaviour
{
    private const float LerpingModifier = 3f;
    
    private Vector3? _lastPosition;
    private Field _currentField;

    private MoveStep _moveStep = MoveStep.Lerp1;
    
    private float _moveTimer;
    private int _fieldId;

    private void Start()
    {
        SetField(Game.Instance.GetStartField());
        _moveStep = MoveStep.Finish;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetField(_currentField.GetNext());
            return;
        }

        if (_moveStep == MoveStep.Finish)
        {
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
            LerpTo(q1, pB, MoveStep.Finish);
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

    private void SetField(Field field)
    {
        if (_currentField)
        {
            _lastPosition = transform.position;
        }
        
        _currentField = field;
        _moveStep = MoveStep.Lerp1;
        _moveTimer = 0;

        _fieldId = _currentField.AddPlayer(this);
    }

    public Player SetPlayerName(string name)
    {
        return this;
    }
}

internal enum MoveStep
{
    Lerp1,
    Lerp2,
    Finish
}