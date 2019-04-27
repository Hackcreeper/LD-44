using System.Collections.Generic;
using System.Linq;
using Fields;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    [SerializeField]
    private Field startField;

    [SerializeField]
    private Transform camera; 

    public static Game Instance { private set; get; }

    private readonly List<Player> _players = new List<Player>();

    private int _activePlayer;
    private int _remaining;
    private bool _diceFinished;
    private bool _moving;

    private bool _inProgress;

    private GameObject _dice;

    [SerializeField]
    private Text playerIntroduction;
    
    [SerializeField]
    private Text remainingFields;
    
    [SerializeField]
    private Image colorPreview;

    [SerializeField]
    private Text winInfo;
    
    [SerializeField]
    private Text playerIntroductionTemplate;

    [SerializeField]
    private FollowingCamera followingCamera;

    [SerializeField]
    private Transform winHouse;

    [SerializeField]
    private Transform[] fireworkSpawner;

    private bool _won;

    private readonly Color[] _colors = new[]
    {
        Color.green,
        Color.blue,
        Color.red,
        Color.yellow,
    };

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

        var position = startField.transform.position;
        _players[0].transform.position = position + startField.GetOffset(1);
        _players[1].transform.position = position + startField.GetOffset(2);
        _players[2].transform.position = position + startField.GetOffset(3);
        _players[3].transform.position = position + startField.GetOffset(4);

        StartTurn();
    }

    private void Update()
    {
        if (_won)
        {
            return;
        }
        
        if (!_inProgress)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartNewTurn();
            }

            return;
        }

        if (_inProgress && !_diceFinished)
        {
            return;
        }

        if (_moving)
        {
            return;
        }

        _moving = true;
        
        var player = _players[_activePlayer];
        player.SetField(player.GetField().GetNext());
        player.RegisterMovementFinishedCallback(() => { HandleFinishedMovement(player); });
    }

    private void HandleFinishedMovement(Player player)
    {
        player.ClearCallback();
        player.GetField().OnEnter();

        if (_won)
        {
            return;
        }

        _remaining--;
        
        remainingFields.text = _remaining.ToString();
        if (_remaining <= 0)
        {
            _moving = false;
            _inProgress = false;
            _diceFinished = false;
            
            Destroy(_dice);

            _activePlayer++;
            if (_activePlayer >= _players.Count)
            {
                _activePlayer = 0;
            }
            
            StartTurn();

            return;
        }
        
        player.RegisterMovementFinishedCallback(() => { HandleFinishedMovement(player); });
        player.SetField(player.GetField().GetNext());
    }

    private void StartNewTurn()
    {
        _inProgress = true;
        
        _dice = Instantiate(Resources.Load<GameObject>("Dice"));
        _dice.transform.position = camera.transform.position - new Vector3(2f, 3f, 4f);
        _dice.GetComponentsInChildren<MeshRenderer>().ToList().ForEach(mesh =>
            {
                mesh.material.color = _colors[_players[_activePlayer].GetId() - 1];
            });

        _dice.GetComponent<Dice.Dice>().RegisterCallback(face =>
        {
            _diceFinished = true;
            _remaining = face;
            remainingFields.text = face.ToString();
        });
    }
    
    private void StartTurn()
    {
        var player = _players[_activePlayer];
        var playerName = player.GetName();
        var id = player.GetId();
        
        followingCamera.SetTarget(player.transform);

        playerIntroduction.text = playerIntroductionTemplate.text
            .Replace("{{id}}", id.ToString())
            .Replace("{{name}}", playerName);

        remainingFields.text = "";

        colorPreview.color = _colors[player.GetId() - 1];
    }

    public Field GetStartField() => startField;

    private Player SpawnPlayer(string newName, int id)
    {
        var prefab = Resources.Load<GameObject>("Player");
        var instance = Instantiate(prefab);
        
        instance.transform.position = startField.transform.position + new Vector3(0, .5f, 0);
        instance.GetComponentInChildren<MeshRenderer>().material.color = _colors[id - 1];
        
        var playerComp = instance.GetComponent<Player>().SetPlayerNameAndId(newName, id);
        playerComp.SetField(startField);

        return playerComp;
    }

    public void Win()
    {
        _won = true;
        
        _players.ForEach(player => { player.Stop(); });
        
        playerIntroduction.gameObject.SetActive(false);
        remainingFields.gameObject.SetActive(false);
        colorPreview.gameObject.SetActive(false);

        winInfo.text = winInfo.text.Replace("{{id}}", _players[_activePlayer].GetId().ToString());
        winInfo.gameObject.SetActive(true);

        followingCamera.SetTarget(winHouse);
        followingCamera.RotateAround();

        var firework = Resources.Load<GameObject>("Firework");
        foreach (var spawner in fireworkSpawner)
        {
            var instance = Instantiate(firework);
            instance.transform.position = spawner.position;
        }
    }
}