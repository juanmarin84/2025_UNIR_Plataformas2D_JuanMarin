using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;
    public CinemachineImpulseSource impulse;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Shake(float intensity = 0.05f)
    {
        impulse.GenerateImpulse(intensity);
    }
}

