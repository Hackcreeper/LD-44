using Fields;
using UnityEngine;

public class Player : MonoBehaviour
{
    private const float LerpingModifier = 3f;
    
    private Field _lastField;
    
    private Field _currentField;

    private MoveStep _moveStep = MoveStep.Lerp1;
    
    private float _moveTimer;

    private void Start()
    {
        _currentField = Game.Instance.GetStartField();
        MoveToCurrentField();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _lastField = _currentField;
            _currentField = _currentField.GetNext();
            MoveToCurrentField();
            _currentField.OnEnter();
        }

        if (_moveStep == MoveStep.Finish)
        {
            return;
        }
        
        if (null == _lastField)
        {
            transform.position = _currentField.transform.position + new Vector3(0, .5f, 0);
            _moveStep = MoveStep.Finish;
            return;
        }
        
        var pA = _lastField.transform.position + new Vector3(0, .5f, 0);
        var pB = _currentField.transform.position + new Vector3(0, .5f, 0);
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

    private void MoveToCurrentField()
    {
        _moveStep = MoveStep.Lerp1;
    }
}

internal enum MoveStep
{
    Lerp1,
    Lerp2,
    Finish
}