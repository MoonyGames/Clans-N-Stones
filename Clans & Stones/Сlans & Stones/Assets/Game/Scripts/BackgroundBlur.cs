using UnityEngine;

public class BackgroundBlur : MonoBehaviour
{
    public static BackgroundBlur Instance { get; private set; }

    [SerializeField] PostProcessor _postProcessor;

    [SerializeField] private float _speed;

    private bool _canStart;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    private void Update()
    {
        if (_canStart)
        {
            _postProcessor.BlurEnabled = true;

            _postProcessor.BlurRadius = Mathf.Lerp(_postProcessor.BlurRadius, 10f, Time.deltaTime * _speed);
        }
    }

    public void Blur() => _canStart = true;
}
