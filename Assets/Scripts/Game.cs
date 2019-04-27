using System.Collections.Generic;
using Fields;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    [SerializeField]
    private Field startField;

    public static Game Instance { private set; get; }

    private readonly List<Player> _players = new List<Player>();

    private int _activePlayer;

    [SerializeField]
    private Text playerIntroduction;
    
    [SerializeField]
    private Text playerIntroductionTemplate;
    
    private void Awake()
    {
        Instance = this;
        
        var names = NameEngine.GetNames(4);
        _players.AddRange(new []
        {
            SpawnPlayer(names[0], 1),
            SpawnPlayer(names[1], 2),
            SpawnPlayer(names[2], 3),
            SpawnPlayer(names[3], 4)
        });


        StartTurn();
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
        
        StartTurn();
    }

    private void StartTurn()
    {
        var player = _players[_activePlayer];
        var playerName = player.GetName();
        var id = player.GetId();

        playerIntroduction.text = playerIntroductionTemplate.text
            .Replace("{{id}}", id.ToString())
            .Replace("{{name}}", playerName);
        
        playerIntroduction.gameObject.SetActive(true);
    }

    public Field GetStartField() => startField;

    private Player SpawnPlayer(string newName, int id)
    {
        var prefab = Resources.Load<GameObject>("Player");
        var instance = Instantiate(prefab);

        instance.transform.position = startField.transform.position + new Vector3(0, .5f, 0);
        
        return instance.GetComponent<Player>().SetPlayerNameAndId(newName, id);
    }
}