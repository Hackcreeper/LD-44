using System.Collections.Generic;
using System.Linq;
using Fields;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour
{
    [SerializeField] private Field startField;

    [SerializeField] private Transform camera;

    public static Game Instance { private set; get; }

    private readonly List<Player> _players = new List<Player>();

    private int _activePlayer;
    private int _remaining;
    private bool _diceFinished;
    private bool _moving;

    private bool _inProgress;

    private GameObject _dice;
    private GameObject _dice2;

    private int _diceValue1;
    private int _diceValue2;

    [SerializeField] private GameObject mainPanel;

    [SerializeField] private Text playerIntroduction;

    [SerializeField] private Text remainingFields;

    [SerializeField] private Text winInfo;

    [SerializeField] private Text playerIntroductionTemplate;

    [SerializeField] private FollowingCamera followingCamera;

    [SerializeField] private Transform[] fireworkSpawner;

    [SerializeField] private GameObject pressSpace;

    [SerializeField] private Image[] heartImages;

    [SerializeField] private Sprite emptyHeart;

    [SerializeField] private Sprite halfHeart;

    [SerializeField] private Sprite fullHeart;

    [SerializeField] private GameObject infoRestart;

    [SerializeField] private GameObject infoTemplate;

    [SerializeField] private GameObject playerList;

    [FormerlySerializedAs("_zoomPanel")] [SerializeField]
    private GameObject zoomPanel;

    [SerializeField] private GameObject arrowInfo;

    [SerializeField] private GameObject walkInfo;

    [SerializeField] private Text walkInfoText;

    [SerializeField] private Text walkInfoTemplate;

    [SerializeField] private GameObject shortcutDialog;
    
    [SerializeField] private GameObject shopDialog;

    [SerializeField] private GameObject shortcutButtons;

    [SerializeField] private Text shortcutInfo;

    [SerializeField] private Text shortcutTemplate;

    [SerializeField] private AudioSource clicker;

    [SerializeField] private AudioSource shortcutAudio;

    [SerializeField] private AudioSource spikeAudio;

    [SerializeField] private AudioSource jumpAudio;

    [SerializeField] private AudioSource walkAudio;

    [SerializeField] private GameObject pause;

    [SerializeField] private AudioSource hospitalAudio;

    private bool _paused;
    private bool _trapPlacementMode;

    private float _botTimer = 1f;
    private float _waitTimer = 0f;

    private bool _startTurnAfterWait;
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
        var amount = PlayerHolder.GetInstance().GetPlayerAmount();
        _players.AddRange(new[]
        {
            SpawnPlayer(names[0], 1),
            SpawnPlayer(names[1], 2, amount < 2),
            SpawnPlayer(names[2], 3, amount < 3),
            SpawnPlayer(names[3], 4, amount < 4)
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
        if (Input.GetKeyDown(KeyCode.P))
        {
            _paused = !_paused;

            pause.SetActive(_paused);
            Time.timeScale = _paused ? 0f : 1f;
        }

        if (infoRestart.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("Game");
        }

        if (_won)
        {
            return;
        }

        if (_waitTimer > 0f)
        {
            _waitTimer -= Time.deltaTime;
            return;
        }

        if (_startTurnAfterWait)
        {
            _moving = false;
            _inProgress = false;
            _diceFinished = false;

            Destroy(_dice);
            if (_dice2) Destroy(_dice2);

            _activePlayer++;
            if (_activePlayer >= _players.Count)
            {
                _activePlayer = 0;
            }

            StartTurn();

            _startTurnAfterWait = false;

            return;
        }

        if (_won)
        {
            return;
        }

        var player = _players[_activePlayer];
        if (!_inProgress)
        {
            if (player.IsBot())
            {
                _botTimer -= Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.Space) || (player.IsBot() && _botTimer <= 0f))
            {
                clicker.Play();
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

    public void HandleFinishedMovement(Player player)
    {
        player.ClearCallback();
        player.GetField().OnEnter(player);

        if (_won || _waitTimer > 0f)
        {
            return;
        }

        _remaining--;

        remainingFields.text = _remaining.ToString();
        if (_remaining <= 0)
        {
            player.GetField().OnStay(player);

            if (_waitTimer > 0f)
            {
                return;
            }

            _moving = false;
            _inProgress = false;
            _diceFinished = false;

            Destroy(_dice);
            if (_dice2) Destroy(_dice2);

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
        _diceValue1 = 0;
        _diceValue2 = 0;

        var player = _players[_activePlayer];

        _dice = Instantiate(Resources.Load<GameObject>("Dice"));
        _dice.transform.position = camera.transform.position - new Vector3(2f, 3f, 4f);
        _dice.GetComponentsInChildren<MeshRenderer>().ToList().ForEach(mesh =>
        {
            mesh.material.color = _colors[_players[_activePlayer].GetPublicId() - 1];
        });

        if (player.GetDoubleDice() > 0)
        {
            player.DoubleDiceUsed();
            
            _dice2 = Instantiate(Resources.Load<GameObject>("Dice"));
            _dice2.transform.position = camera.transform.position - new Vector3(3f, 3f, 4f);
            _dice2.GetComponentsInChildren<MeshRenderer>().ToList().ForEach(mesh =>
            {
                mesh.material.color = _colors[_players[_activePlayer].GetPublicId() - 1];
            });

            _dice.GetComponent<Dice.Dice>().RegisterCallback(face =>
            {
                _diceValue1 = face;

                if (_diceValue1 == 0 || _diceValue2 == 0)
                {
                    return;
                }
                
                _diceFinished = true;
                _remaining = _diceValue1 + _diceValue2;
                remainingFields.text = _remaining.ToString();
            });

            _dice2.GetComponent<Dice.Dice>().RegisterCallback(face =>
            {
                _diceValue2 = face;
                
                if (_diceValue1 == 0 || _diceValue2 == 0)
                {
                    return;
                }
                
                _diceFinished = true;
                _remaining = _diceValue1 + _diceValue2;
                remainingFields.text = _remaining.ToString();
            });
        }
        else
        {
            _dice.GetComponent<Dice.Dice>().RegisterCallback(face =>
            {
                _diceFinished = true;
                _remaining = face;
                remainingFields.text = face.ToString();
            });
        }
    }

    private void StartTurn()
    {
        _botTimer = Random.Range(1f, 2f);

        var player = _players[_activePlayer];
        var playerName = player.GetName();
        var id = player.GetPublicId();

        followingCamera.SetTarget(player.transform);

        walkInfo.gameObject.SetActive(false);

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

    public void Win(Player player)
    {
        _won = true;

        _players.ForEach(p => { p.Stop(); });

        playerIntroduction.gameObject.SetActive(false);
        remainingFields.gameObject.SetActive(false);
        zoomPanel.SetActive(false);
        walkInfo.SetActive(false);
        heartImages.ToList().ForEach(image => image.gameObject.SetActive(false));

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
            _players[i - 1].RefreshPlayerId(i);
        }

        if (_players.Count == 1)
        {
            Win(_players.First());
            return;
        }

        _remaining = 0;
        _activePlayer--;

        Wait(.1f, true);

        if (_players.All(p => p.IsBot()))
        {
            infoRestart.SetActive(true);
        }
    }

    public void Wait(float seconds, bool startNewTurnAfterWait = true)
    {
        _startTurnAfterWait = startNewTurnAfterWait;
        _waitTimer = seconds;
    }

    public void IncreaseRemaining(int remaining)
    {
        _moving = false;
        _remaining += remaining;

        remainingFields.text = _remaining.ToString();
    }

    public void StopWaiting()
    {
        _waitTimer = 0f;
        _startTurnAfterWait = false;
    }

    public void SetArrowInfoVisiblity(bool visible)
    {
        arrowInfo.SetActive(visible);
    }

    public void ShowWalkInfo(int fields, string direction)
    {
        walkInfoText.text = walkInfoTemplate.text
            .Replace("{{num}}", fields.ToString())
            .Replace("{{direction}}", direction);

        walkInfo.gameObject.SetActive(true);
    }

    public void ShowShortcutDialog(int price, Player player, IShortcutField field)
    {
        shortcutInfo.text = shortcutTemplate.text.Replace("{{price}}", price.ToString());

        mainPanel.SetActive(false);
        shortcutDialog.SetActive(true);

        shortcutDialog.GetComponent<ShortcutDialog>().Init(field, player);
        shortcutButtons.SetActive(!player.IsBot());
    }

    public void ShowShopDialog()
    {
        mainPanel.SetActive(false);
        shopDialog.SetActive(true);

        shopDialog.GetComponent<Shop>().Init();
    }
    
    public FollowingCamera GetCamera() => followingCamera;

    public void HideShortcutDialog()
    {
        shortcutDialog.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void HideShopDialog()
    {
        shopDialog.SetActive(false);
        mainPanel.SetActive(true);
    }

    public AudioSource GetClicker() => clicker;

    public AudioSource GetShortcutAudio() => shortcutAudio;

    public AudioSource GetSpikeAudio() => spikeAudio;

    public AudioSource GetJumpAudio() => jumpAudio;

    public AudioSource GetWalkAudio() => walkAudio;

    public AudioSource GetHospitalAudio() => hospitalAudio;
    
    public Player GetActivePlayer() => _players[_activePlayer];

    public bool IsTrapPlacementMode() => _trapPlacementMode;

    public void SetTrapPlacementMode(bool trap)
    {
        _trapPlacementMode = trap;
    }
}