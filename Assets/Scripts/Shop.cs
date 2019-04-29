using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private bool _isBot;
    private float _botTimer;
    
    public void Init()
    {
        var player = Game.Instance.GetActivePlayer();

        if (player.IsBot())
        {
            // TODO: Disable buttons
            _isBot = true;
            _botTimer = Random.Range(1f, 2.5f);
        }
    }
    
    public void BuyDoubleDice()
    {
        var player = Game.Instance.GetActivePlayer();
        Game.Instance.HideShopDialog();
        
        player.Hurt(3);
        if (player.GetHealth() <= 0)
        {
            return;
        }
        
        player.BoughtDoubleDice();
        
        Game.Instance.GetCamera().ExitZoom();
        Game.Instance.StopWaiting();

        player.RegisterMovementFinishedCallback(() => { Game.Instance.HandleFinishedMovement(player); });
        player.SetField(player.GetField().GetNext());
    }

    public void BuyTrap()
    {
        Game.Instance.HideShopDialog();
        Game.Instance.SetTrapPlacementMode(true);
    }

    public void Cancel()
    {
        var player = Game.Instance.GetActivePlayer();
        
        Game.Instance.HideShopDialog();
        Game.Instance.GetCamera().ExitZoom();
        Game.Instance.StopWaiting();

        player.RegisterMovementFinishedCallback(() => { Game.Instance.HandleFinishedMovement(player); });
        player.SetField(player.GetField().GetNext());
    }

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

        _isBot = false;
        _botTimer = 0f;
        
        var player = Game.Instance.GetActivePlayer();
        
        var possible = new List<Upgrade>();
        if (player.GetHealth() > 3)
        {
            possible.Add(Upgrade.DoubleDice);
        }
        if (player.GetHealth() > 4)
        {
//            possible.Add(Upgrade.Trap);
        }

        if (possible.Count == 0)
        {
            Cancel();
            return;
        }

        if (Random.Range(1, 101) <= 50)
        {
            Cancel();
            return;
        }

        var item = possible[Random.Range(0, possible.Count)];
        switch (item)
        {
            case Upgrade.Trap:
                Cancel();
                return;
            
            case Upgrade.DoubleDice:
                BuyDoubleDice();
                return;
            
            default:
                Cancel();
                return;
        }
    }
}

internal enum Upgrade
{
    DoubleDice,
    Trap
}