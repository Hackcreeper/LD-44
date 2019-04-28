using System;
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

    public void Init(Player player, Field target)
    {
        _player = player;
        _target = target;
    }
}