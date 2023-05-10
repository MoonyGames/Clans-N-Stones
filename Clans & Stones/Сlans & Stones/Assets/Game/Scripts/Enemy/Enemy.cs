using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;

    private static Transform _playerTransform;

    private const float _timeToDestroy = 10f;

    [SerializeField] private Rigidbody[] _ragdollComponents;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();

        if(_playerTransform == null)
            _playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    private void Start()
    {
        for (int i = 0; i < _ragdollComponents.Length; i++)
            _ragdollComponents[i].isKinematic = true;

        _navMeshAgent.SetDestination(_playerTransform.position);

        _animator.SetBool("RunToTarget", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            _navMeshAgent.Stop();

            _animator.SetBool("RunToTarget", false);
        }
    }

    public void AttackPlayerOnAnimationEvent() => PlayerCharacterController.Instance.TakingDamage();

    public void Death()
    {
        Destroy(_navMeshAgent);
        Destroy(_animator);

        for (int i = 0; i < _ragdollComponents.Length; i++)
            _ragdollComponents[i].isKinematic = false;

        StartCoroutine(DestroyOnTimer(_timeToDestroy));
    }

    private IEnumerator DestroyOnTimer(float time)
    {
        yield return new WaitForSeconds(time);

        Destroy(gameObject);
    }
}
