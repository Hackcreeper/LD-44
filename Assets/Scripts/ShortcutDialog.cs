using Fields;
using UnityEngine;

public class ShortcutDialog : MonoBehaviour
{
    private IShortcutField _shortcutField;

    private Player _player;

    public void Init(IShortcutField field, Player player)
    {
        _shortcutField = field;
        _player = player;
    }

    public void Yes() => _shortcutField.Accepted(_player);
    public void No() => _shortcutField.Canceled(_player);
}