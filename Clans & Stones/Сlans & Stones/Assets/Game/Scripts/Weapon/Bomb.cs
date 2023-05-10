using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Bomb : MonoBehaviour, IWeapon
{
    [SerializeField] private float _attackSpeedInSec = 0.5f, _timeToReload = 5f;

    private Vector3 _finalScale = Vector3.one * 5;

    [SerializeField] private SphereCollider[] _sphereCollider;

    [SerializeField] private Button _thisWeaponButton;

    private Transform _bombParticlesTransform;

    private Rigidbody _bombRigidbody;

    private AudioSource _audioSource;

    private void Awake()
    {
        for(int i = 0; i < _sphereCollider.Length; i++)
            _sphereCollider[i].enabled = false;

        _bombParticlesTransform = transform.GetChild(0);

        _bombRigidbody = GetComponent<Rigidbody>();

        _audioSource = GetComponent<AudioSource>();
    }

    public void Attack()
    {
        for (int i = 0; i < _sphereCollider.Length; i++)
            _sphereCollider[i].enabled = true;

        transform.localScale = Vector3.zero;
        _bombParticlesTransform.localScale = Vector3.zero;

        _thisWeaponButton.interactable = false;

        CameraShaker.Instance.ShakeCamera();

        transform.DOScale(_finalScale, _attackSpeedInSec).SetEase(Ease.InOutFlash).OnComplete(() => {
            transform.DOScale(0, 0.25f).SetEase(Ease.InOutSine).OnComplete(() => {
                for (int i = 0; i < _sphereCollider.Length; i++)
                    _sphereCollider[i].enabled = false;

                Invoke(nameof(ActivateBombButton), _timeToReload);
            });            
        });

        _bombParticlesTransform.gameObject.SetActive(true);

        _bombParticlesTransform.DOScale(1f, _attackSpeedInSec).SetEase(Ease.InOutFlash).OnComplete(() => {
            _bombParticlesTransform.DOScale(0, 0.25f).SetEase(Ease.InOutSine);
        });

        _audioSource.Play();
    }

    private void ActivateBombButton() => _thisWeaponButton.interactable = true;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Enemy")
        {
            collider.gameObject.GetComponent<Enemy>().Death();
        }
    }
}
