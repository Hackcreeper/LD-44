using UnityEngine;

public class PlayerHolder : MonoBehaviour
{
    private static PlayerHolder _instance;

    private int _playerAmount;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _instance = this;
    }

    public int GetPlayerAmount() => _playerAmount;

    public void SetPlayerAmount(int playerAmount)
    {
        _playerAmount = playerAmount;
    }

    public static PlayerHolder GetInstance()
    {
        if (!_instance)
        {
            var instance = new GameObject("Player Holder").AddComponent<PlayerHolder>();
            instance.SetPlayerAmount(1);

            _instance = instance;
            return _instance;
        }

        return _instance;
    }
}