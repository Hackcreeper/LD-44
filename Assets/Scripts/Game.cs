using System.Collections.Generic;
using Fields;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField]
    private Field startField;

    public static Game Instance { private set; get; }

    private List<Player> _players = new List<Player>();

    private int _activePlayer = 0;
    
    private void Awake()
    {
        Instance = this;
        
        // Spawning multiple players √
        // Each field needs to keep track of how many players are there (probably even which players) √
        // when a player enters a field, the offset is dtermined by the amount of players √
        // and we are using rounds √
        
        // when a field changes players, the offset for each existing player needs to be recalculated
        // so basically a new movement type === reposition
        
        var names = NameEngine.GetNames(4);
        _players.AddRange(new []
        {
            SpawnPlayer(names[0]),
            SpawnPlayer(names[1]),
            SpawnPlayer(names[2]),
            SpawnPlayer(names[3])
        });
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) return;
        
        var player = _players[_activePlayer];
        player.SetField(player.GetField().GetNext());

        _activePlayer++;
        if (_activePlayer >= _players.Count)
        {
            _activePlayer = 0;
        }
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