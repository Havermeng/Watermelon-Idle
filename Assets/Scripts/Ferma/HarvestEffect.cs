using UnityEngine;

public class HarvestEffect : MonoBehaviour
{
    public static HarvestEffect Instance;
    private ParticleSystem ps;

    void Awake()
    {
        Instance = this;
        ps = GetComponent<ParticleSystem>();
    }

    public void Play(Vector3 position)
    {
        transform.position = position;
        ps.Play();
    }
}