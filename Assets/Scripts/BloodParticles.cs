using UnityEngine;

public class BloodParticles : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem particleSystem;
    
    public void Start()
    {
        var main = particleSystem.main;
        var totalDuration = main.duration + main.startLifetime.constantMax + 1f;
        
        Destroy(gameObject, totalDuration);
    }
}