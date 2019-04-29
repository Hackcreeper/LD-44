using UnityEngine;

public class LifeInsuranceDialog : MonoBehaviour
{
    private float _botTimer;
    private bool _isBot;

    public void Init()
    {
        if (!Game.Instance.GetActivePlayer().IsBot()) return;

        _isBot = true;
        _botTimer = Random.Range(1f, 2.5f);
    }

    public void Yes()
    {
        var player = Game.Instance.GetActivePlayer();
        player.Hurt(2);
        player.GainLifeInsurance();
        
        Game.Instance.HideLifeInsuranceDialog();
        Game.Instance.GetCamera().ExitZoom();
        Game.Instance.Wait(.1f);
    }
    public void No()
    {
        Game.Instance.HideLifeInsuranceDialog();
        Game.Instance.GetCamera().ExitZoom();
        Game.Instance.Wait(.1f);
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

        if (Game.Instance.GetActivePlayer().GetHealth() <= 2)
        {
            No();
        }
        else
        {
            if (Random.Range(1, 101) <= 50)
            {
                Yes();
            }
            else
            {
                No();
            }
        }

        _isBot = false;
        _botTimer = 0f;
    }
}