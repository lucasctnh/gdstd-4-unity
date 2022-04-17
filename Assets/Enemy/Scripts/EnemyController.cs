using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour {
	[SerializeField] private float _lookRadius = 10f;
	[SerializeField] private float _minDistanceRadius = 1f;

    private StateMachine _stateMachine;
	private NavMeshAgent _navMeshAgent;
	private Animator _animator;

	private void Awake() {
		_animator = GetComponent<Animator>();
		_navMeshAgent = GetComponent<NavMeshAgent>();

        _stateMachine = new StateMachine();

        IState roam = new Roam(this, _navMeshAgent);
        IState followPlayer = new FollowPlayer(this, _navMeshAgent, _minDistanceRadius);

		void When(Func<bool> condition, IState from, IState to) => _stateMachine.AddTransition(to, from, condition);

		When(PlayerInRange(), roam, followPlayer);
		When(PlayerNotInRange(), followPlayer, roam);

		_stateMachine.SetState(roam);

		Func<bool> PlayerInRange() => () => Vector3.Distance(Player.Instance.transform.position, transform.position) <= _lookRadius;
		Func<bool> PlayerNotInRange() => () => !(Vector3.Distance(Player.Instance.transform.position, transform.position) <= _lookRadius);
	}

	private void Update() {
		_stateMachine.Tick();
		SetBlendTreeAnimation();
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, _lookRadius);
		Gizmos.DrawWireSphere(transform.position, _minDistanceRadius);
	}

	private void SetBlendTreeAnimation() {
		_animator.SetFloat("Speed", _navMeshAgent.velocity.magnitude);
		_animator.SetFloat("MotionSpeed", 1f);
	}
}
