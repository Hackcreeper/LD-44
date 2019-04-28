using UnityEngine;

namespace Fields
{
    public class JumpToField : Field
    {
        public override void OnStay(Player player)
        {
            // If the number is negative we will chose the correct field form an array (4 items big)
            // And set the new target field. In this case, we'll need to restart the game cycle to wait for the movement
            // And then handle the regular game (for exmaple, if the player then lands on a spike field)

            var random = Random.Range(1, 5);

            Game.Instance.IncreaseRemaining(random);
            Game.Instance.Wait(.1f, false);
            
            Debug.Log("Jump on field!!");
        }
    }
}