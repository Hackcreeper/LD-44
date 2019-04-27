using Fields;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Field _currentField;

    private void Start()
    {
        _currentField = Game.Instance.GetStartField();
        MoveToCurrentField();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
        {
            return;
        }

        _currentField = _currentField.GetNext();
        MoveToCurrentField();
        _currentField.OnEnter();
    }

    private void MoveToCurrentField()
    {
        transform.position = _currentField.transform.position;
    }
}