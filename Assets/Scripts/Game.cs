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
        
        var names = NameEngine.GetNames(3);
        Debug.Log(names[0]);
        Debug.Log(names[1]);
        Debug.Log(names[2]);
    }

    public Field GetStartField() => startField;
}