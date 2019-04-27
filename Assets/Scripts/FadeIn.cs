using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FadeIn : MonoBehaviour
{
    private const float SmoothFactor = .3f;

    [SerializeField]
    private Text text;

    private FadeState _state = FadeState.FadeOut;

    private float _progress;
    private float _wait = 1.3f;
    
    public void StartFadeIn()
    {
        _state = FadeState.FadeIn;
        _progress = 0f;
        _wait = 1.3f;
    }

    public void StartFadeOut()
    {
        _state = FadeState.FadeOut;
        _progress = 0f;
        _wait = 1.3f;
    }

    private void Update()
    {
        _progress += Time.deltaTime * SmoothFactor;
        
        if (_state == FadeState.FadeIn)
        {
            if (_progress >= .3f)
            {
                _wait -= Time.deltaTime;
                if (_wait <= 0f)
                {
                    StartFadeOut();
                }
                
                return;
            }
            
            text.color = Color.Lerp(text.color, new Color(0, 0, 0, 1), _progress);
            return;
        }
        
        text.color = Color.Lerp(text.color, new Color(0, 0, 0, 0), _progress);
    }
}

internal enum FadeState
{
    FadeIn,
    FadeOut
}