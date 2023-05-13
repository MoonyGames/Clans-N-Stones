using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacterController : MonoBehaviour
{
    public static PlayerCharacterController Instance { get; private set; }

    private Animator _animator;

    private float _health = 100f;

    [SerializeField] private Scrollbar _healthbar;

    [SerializeField] private GameObject _looseMenu, _winMenu;

    [SerializeField] private Button[] _weaponButtons;

    private int _killCounter;
    public int KillCounter
    {
        get { return _killCounter; }

        set
        {
            _killCounter = value;

            if (_killCounter >= ChangeLevels.Instance.WavesCountForLevels[PlayerPrefs.GetInt("Level", 0)] * 2)
                Win();
        }
    }

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

    private void Win()
    {
        _winMenu.SetActive(true);

        for (int i = 0; i < _weaponButtons.Length; i++)
            _weaponButtons[i].interactable = false;

        EnemyGenerator.Instance.IsGenerating = false;

        BackgroundBlur.Instance.Blur();

        this.enabled = false;
    }
}
