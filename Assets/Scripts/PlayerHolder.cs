using UnityEngine;

public class PlayerHolder : MonoBehaviour
{
    private static PlayerHolder _instance;

    private int _playerAmount;
    
    [SerializeField] private AudioClip defaultMusic;
    [SerializeField] private AudioClip winMusic;

    [SerializeField] private AudioSource audioSource;
    
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

    public void ToggleToWin()
    {
        if (!audioSource || !winMusic)
        {
            return;
        }
        
        audioSource.clip = winMusic;
        audioSource.volume = 0.025f;
        audioSource.Play();
    }

    public void ToggleToDefault()
    {
        if (!audioSource || !defaultMusic)
        {
            return;
        }
        
        audioSource.clip = defaultMusic;
        audioSource.volume = 1f;
        audioSource.Play();
    }
}