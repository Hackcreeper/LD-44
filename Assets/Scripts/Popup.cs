using TMPro;
using UnityEngine;

public class Popup : MonoBehaviour
{
    private const float SmoothModifier = .15f;
    
    private float _progress;

    private Vector3 _targetPosition;
    
    [SerializeField]
    private TextMeshPro textComponent;
    
    private Color _targetColor;

    [SerializeField]
    private Color _negativeColor;
    
    [SerializeField]
    private Color _positiveColor;

    private void Start()
    {
        _targetPosition = transform.position + new Vector3(0, 0.5f, -0.2f);
    }

    public void SetPoints(int points)
    {
        var color = points >= 0 ? _positiveColor : _negativeColor;
        _targetColor = new Color(color.r, color.g, color.b, 0f);

        textComponent.color = color;

        var prefix = points >= 0 ? "+" : "-";
        textComponent.text = $"{prefix}{points} health";
    }
    
    private void Update()
    {
        _progress += SmoothModifier * Time.deltaTime;

        transform.position = Vector3.Lerp(transform.position, _targetPosition, _progress);
        textComponent.color = Color.Lerp(textComponent.color, _targetColor, _progress);

        if (_progress >= 1f)
        {
            Destroy(gameObject);
        }
    }
}