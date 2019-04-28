using Fields;
using UnityEngine;

public class ShortcutDialog : MonoBehaviour
{
    private IShortcutField _shortcutField;

    private Player _player;

    private float _botTimer;
    private bool _isBot;

    public void Init(IShortcutField field, Player player)
    {
        _shortcutField = field;
        _player = player;

        if (_player.IsBot())
        {
            _isBot = true;
            _botTimer = Random.Range(1f, 2.5f);
        }
    }

    public void Yes() => _shortcutField.Accepted(_player);
    public void No() => _shortcutField.Canceled(_player);

    private void Update()
    {
        if (!_isBot)
        {
            return;
        }

        _botTimer -= Time.deltaTime;

        if (_botTimer > 0f)
        {
            return;
        }
        
        // Intelligenz benutzen brudi
        if (_shortcutField.GetPrice() >= _player.GetHealth())
        {
            No();
        }
        else
        {
            if (Random.Range(1, 101) <= 50)
            {
                Yes();
            }
            else
            {
                No();
            }
        }

        _isBot = false;
        _botTimer = 0f;
    }
}