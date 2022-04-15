using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour {

	[SerializeField] private float _lookRadius = 10f;
	[SerializeField] private float _minDistanceRadius = 1f;
	[SerializeField] private Transform _target;

	private NavMeshAgent _agent;
	private Animator _animator;

	private void Start() {
		_agent = GetComponent<NavMeshAgent>();
		_animator = GetComponent<Animator>();
	}

	private void Update() {
		float distance = Vector3.Distance(_target.position, transform.position);
		if (distance <= _lookRadius) {
			MoveToTarget();
			FaceTarget();

			// if (distance <= _agent.stoppingDistance)
				// Attack target
		}

		SetBlendTreeAnimation();
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, _lookRadius);
		Gizmos.DrawWireSphere(transform.position, _minDistanceRadius);
	}

	private void MoveToTarget() {
		Vector3 direction = (_target.position - transform.position).normalized;
		_agent.SetDestination(_target.position - (direction * _minDistanceRadius));
	}

	private void SetBlendTreeAnimation() {
		_animator.SetFloat("Speed", _agent.velocity.magnitude);
		_animator.SetFloat("MotionSpeed", 1f);
	}

	private void FaceTarget() {
		Vector3 direction = (_target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
	}
}
