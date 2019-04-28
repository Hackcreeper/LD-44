using System.Collections.Generic;
using System.Linq;
using Fields;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    private Text winInfo;
    
    [SerializeField]
    private Text playerIntroductionTemplate;

    [SerializeField]
    private FollowingCamera followingCamera;

    [SerializeField]
    private Transform winHouse;

    [SerializeField]
    private Transform[] fireworkSpawner;

    [SerializeField]
    private GameObject pressSpace;

    [SerializeField]
    private Image[] heartImages;

    [SerializeField]
    private Sprite emptyHeart;
    
    [SerializeField]
    private Sprite halfHeart;
    
    [SerializeField]
    private Sprite fullHeart;

    [SerializeField]
    private GameObject infoRestart;

    [SerializeField]
    private GameObject infoTemplate;
    
    [SerializeField]
    private GameObject playerList;
    
    private float _botTimer = 1f;

    private bool _won;

    private readonly Color[] _colors = new[]
    {
        Color.green,
        Color.cyan,
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
            SpawnPlayer(names[1], 2, true),
            SpawnPlayer(names[2], 3),
            SpawnPlayer(names[3], 4, true)
        });

        var position = startField.transform.position;
        _players[0].transform.position = position + startField.GetOffset(1);
        _players[1].transform.position = position + startField.GetOffset(2);
        _players[2].transform.position = position + startField.GetOffset(3);
        _players[3].transform.position = position + startField.GetOffset(4);
        
        RenderInfoPanel();

        StartTurn();
    }

    private void RenderInfoPanel()
    {
        for (int i = 1; i < playerList.transform.childCount; i++)
        {
            Destroy(playerList.transform.GetChild(i).gameObject);
        }
        
        foreach (var player in _players)
        {
            AddPlayerToInfoPanel(player);
        }
    }

    private void AddPlayerToInfoPanel(Player player)
    {
        var instance = Instantiate(infoTemplate, playerList.transform);
        instance.transform.Find("Color").GetComponent<Image>().color = _colors[player.GetId() - 1];
        instance.transform.Find("Name").GetComponent<Text>().text = 
            instance.transform.Find("Name").GetComponent<Text>().text
            .Replace("{{prefix}}", player.IsBot() ? "Bot" : "Player")
            .Replace("{{id}}", player.GetPublicId().ToString());

        if (_players[_activePlayer] == player)
        {
            instance.GetComponent<Image>().enabled = true;
        }
        
        // TODO: Health
        instance.SetActive(true);

        var images = instance.transform.Find("HealthBar").GetComponentsInChildren<Image>();
        var health = player.GetHealth();
        
        images[0].sprite =
            health <= 0
                ? emptyHeart
                : health == 1
                    ? halfHeart
                    : fullHeart;

        images[1].sprite =
            health <= 2
                ? emptyHeart
                : health == 3
                    ? halfHeart
                    : fullHeart;

        images[2].sprite =
            health <= 4
                ? emptyHeart
                : health == 5
                    ? halfHeart
                    : fullHeart;

        images[3].sprite =
            health <= 6
                ? emptyHeart
                : health == 7
                    ? halfHeart
                    : fullHeart;

        images[4].sprite =
            health <= 8
                ? emptyHeart
                : health == 9
                    ? halfHeart
                    : fullHeart;
    }

    private void Update()
    {
        if (infoRestart.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("Game");
        }
        
        if (_won)
        {
            return;
        }

        var player = _players[_activePlayer];
        if (Input.GetKeyDown(KeyCode.K))
        {
            player.Hurt(1);
        }
        
        
        if (!_inProgress)
        {
            if (player.IsBot())
            {
                _botTimer -= Time.deltaTime;
            }
            
            if (Input.GetKeyDown(KeyCode.Space) || (player.IsBot() && _botTimer <= 0f))
            {
                pressSpace.SetActive(false);
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
            player.GetField().OnStay(player);
            
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
                mesh.material.color = _colors[_players[_activePlayer].GetPublicId() - 1];
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
        _botTimer = Random.Range(1f, 2f);
        
        var player = _players[_activePlayer];
        var playerName = player.GetName();
        var id = player.GetPublicId();
        
        followingCamera.SetTarget(player.transform);

        playerIntroduction.text = playerIntroductionTemplate.text
            .Replace("{{prefix}}", player.IsBot() ? "Bot" : "Player")
            .Replace("{{id}}", id.ToString())
            .Replace("{{name}}", playerName);

        remainingFields.text = "";

        RenderHearts();

        if (player.IsBot())
        {
            return;
        }
        
        pressSpace.SetActive(true);
    }

    public void RenderHearts()
    {
        var health = _players[_activePlayer].GetHealth();
        heartImages[0].sprite =
            health <= 0
                ? emptyHeart
                : health == 1
                    ? halfHeart
                    : fullHeart;

        heartImages[1].sprite =
            health <= 2
                ? emptyHeart
                : health == 3
                    ? halfHeart
                    : fullHeart;

        heartImages[2].sprite =
            health <= 4
                ? emptyHeart
                : health == 5
                    ? halfHeart
                    : fullHeart;

        heartImages[3].sprite =
            health <= 6
                ? emptyHeart
                : health == 7
                    ? halfHeart
                    : fullHeart;

        heartImages[4].sprite =
            health <= 8
                ? emptyHeart
                : health == 9
                    ? halfHeart
                    : fullHeart;
        
        RenderInfoPanel();
    }

    public Field GetStartField() => startField;

    private Player SpawnPlayer(string newName, int id, bool bot = false)
    {
        var prefab = Resources.Load<GameObject>("Player");
        var instance = Instantiate(prefab);
        
        instance.transform.position = startField.transform.position + new Vector3(0, .5f, 0);
        instance.GetComponentInChildren<MeshRenderer>().material.color = _colors[id - 1];


        var playerComp = instance.GetComponent<Player>();
        if (bot)
        {
            Destroy(playerComp);
            playerComp = instance.AddComponent<Bot>();
        }
        
        playerComp.SetPlayerNameAndId(newName, id);
        playerComp.SetField(startField);

        return playerComp;
    }

    public void Win()
    {
        _won = true;
        
        _players.ForEach(p => { p.Stop(); });
        
        playerIntroduction.gameObject.SetActive(false);
        remainingFields.gameObject.SetActive(false);
        heartImages.ToList().ForEach(image => image.gameObject.SetActive(false));

        var player = _players[_activePlayer];
        winInfo.text = winInfo.text
            .Replace("{{prefix}}", player.IsBot() ? "Bot" : "Player")
            .Replace("{{id}}", player.GetPublicId().ToString());
        
        winInfo.gameObject.SetActive(true);

        followingCamera.SetTarget(player.transform);
        followingCamera.RotateAround();

        var firework = Resources.Load<GameObject>("Firework");
        foreach (var spawner in fireworkSpawner)
        {
            var instance = Instantiate(firework);
            instance.transform.position = spawner.position;
        }
        
        pressSpace.SetActive(false);
        infoRestart.SetActive(true);
    }

    public void Kill(Player player)
    {
        _players.Remove(player);
        pressSpace.SetActive(false);

        for (var i = 1; i < _players.Count; i++)
        {
            _players[i-1].RefreshPlayerId(i);
        }

        _remaining = 0;
        _activePlayer--;
        HandleFinishedMovement(player);

        if (_players.Count == 1)
        {
            Win();
            return;
        }

        if (_players.All(p => p.IsBot()))
        {
            infoRestart.SetActive(true);
        }
    }
}