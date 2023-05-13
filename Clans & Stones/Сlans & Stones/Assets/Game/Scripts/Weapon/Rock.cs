using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Rock : MonoBehaviour, IWeapon
{
    private const float _bottomPositionY = 1.5f;

    [SerializeField] private float _attackSpeedInSec = 0.5f;

    [SerializeField] private GameObject _particles;

    [SerializeField] private Button _thisWeaponButton, _anotherRockButton;

    private float _starterPositionY;

    private AudioSource _audioSource;

    private void Awake() => _audioSource = GetComponent<AudioSource>();

    private void Start() => _starterPositionY = transform.position.y;

    public void Attack()
    {
        _thisWeaponButton.interactable = false;
        _anotherRockButton.interactable = false;

        _particles.SetActive(true);

        CameraShaker.Instance.ShakeCamera();

        _audioSource.Play();

        transform.DOMoveY(_bottomPositionY, _attackSpeedInSec).SetEase(Ease.OutBounce).OnComplete(() => {
            transform.DOMoveY(_starterPositionY, 0.25f).SetEase(Ease.InOutSine).OnComplete(() => {
                _thisWeaponButton.interactable = true;
                _anotherRockButton.interactable = true;
                });
        });
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Enemy" && !collider.gameObject.GetComponent<Enemy>().IsDead)
        {
            collider.gameObject.GetComponent<Enemy>().Death();

            PlayerCharacterController.Instance.KillCounter++;
        }
    }
}
