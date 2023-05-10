using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacterController : MonoBehaviour
{
    public static PlayerCharacterController Instance { get; private set; }

    private Animator _animator;

    private float _health = 100f;

    [SerializeField] private Scrollbar _healthbar;

    [SerializeField] private GameObject _looseMenu;

    [SerializeField] private Button[] _weaponButtons;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    public void Attack(GameObject weapon)
    {
        _animator.SetBool("IsAttacking", true);

        weapon.GetComponent<IWeapon>().Attack();
    }

    public void TakingDamage()
    {
        _animator.SetBool("IsTakingDamage", true);

        _health -= 5;
        _healthbar.size = _health / 100;

        if (_health <= 0)
            Death();
    }

    private void Death()
    {
        _animator.SetBool("IsDead", true);

        _looseMenu.SetActive(true);

        for (int i = 0; i < _weaponButtons.Length; i++)
            _weaponButtons[i].interactable = false;

        EnemyGenerator.Instance.IsGenerating = false;

        BackgroundBlur.Instance.Blur();

        this.enabled = false;
    }
}
