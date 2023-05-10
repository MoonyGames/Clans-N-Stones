using UnityEngine;
using MilkShake;

public class CameraShaker : MonoBehaviour
{
    public static CameraShaker Instance { get; private set; }
    private Camera _camera;

    private Shaker _shaker;

    [SerializeField] private ShakePreset _shakePreset;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);

        _camera = Camera.main;

        _shaker = _camera.GetComponent<Shaker>();
    }

    public void ShakeCamera() => _shaker.Shake(_shakePreset);
}
