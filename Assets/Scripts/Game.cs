using Fields;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField]
    private Field startField;

    public static Game Instance { private set; get; }

    private void Awake()
    {
        Instance = this;
    }

    public Field GetStartField() => startField;
}