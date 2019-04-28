using TMPro;
using UnityEngine;

public class Popup : MonoBehaviour
{
    private const float SmoothModifier = .25f;
    
    private float _progress;

    private Vector3 _targetPosition;
    
    [SerializeField]
    private TextMeshPro textComponent;
    
    private Color _targetColor;

    private void Start()
    {
        _targetPosition = transform.position + new Vector3(0, 0.5f, -0.2f);

        var color = textComponent.color;
        _targetColor = new Color(color.r, color.g, color.b, 0f);
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