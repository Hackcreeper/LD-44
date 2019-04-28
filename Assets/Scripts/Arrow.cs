using Fields;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Field _target;

    private Player _player;

    private void OnMouseDown()
    {
        CrossingField.PathSelected(_target, _player);
    }

    private void OnMouseEnter()
    {
        GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;
    }
    
    private void OnMouseExit()
    {
        GetComponentInChildren<MeshRenderer>().material.color = Color.red;
    }

    public void Init(Player player, Field target)
    {
        _player = player;
        _target = target;
    }
}