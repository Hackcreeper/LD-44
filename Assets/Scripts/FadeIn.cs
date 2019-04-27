using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FadeIn : MonoBehaviour
{
    private const float SmoothFactor = .5f;

    [SerializeField]
    private Text text;

    private FadeState _state = FadeState.FadeOut;

    private float _progress;
    
    public void StartFadeIn()
    {
        _state = FadeState.FadeIn;
        _progress = 0f;
    }

    public void StartFadeOut()
    {
        _state = FadeState.FadeOut;
        _progress = 0f;
    }

    private void Update()
    {
        _progress += Time.deltaTime * SmoothFactor;
        
        if (_state == FadeState.FadeIn)
        {
            text.color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), _progress);
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