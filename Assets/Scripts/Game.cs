using Fields;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField]
    private Field startField;

    public static Game Instance { private set; get; }

    private Player[] _players;
    
    private void Awake()
    {
        Instance = this;
        
        // Spawning multiple players √
        // Each field needs to keep track of how many players are there (probably even which players) √
        // when a player enters a field, the offset is dtermined by the amount of players √
        
        // when a field changes players, the offset for each existing player needs to be recalculated
        // so basically a new movement type === reposition
        // and we are using rounds
        
        var names = NameEngine.GetNames(4);
        _players = new[]
        {
            SpawnPlayer(names[0]),
            SpawnPlayer(names[1]),
            SpawnPlayer(names[2]),
            SpawnPlayer(names[3])
        };
    }

    public Field GetStartField() => startField;

    private Player SpawnPlayer(string name)
    {
        var prefab = Resources.Load<GameObject>("Player");
        var instance = Instantiate(prefab);

        instance.transform.position = startField.transform.position + new Vector3(0, .5f, 0);
        
        return instance.GetComponent<Player>().SetPlayerName(name);
    }
}