using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Roam : IState {
	private readonly EnemyController _enemy;
	private NavMeshAgent _navMeshAgent;

	private float _initialSpeed;
	private const float ROAM_SPEED = 2F;
	private const float ROAM_DISTANCE = 5F;

	public Roam(EnemyController enemy, NavMeshAgent navMeshAgent) {
		_enemy = enemy;
		_navMeshAgent = navMeshAgent;
	}

	public void OnEnter() {
		_navMeshAgent.enabled = true;
		_initialSpeed = _navMeshAgent.speed;
		_navMeshAgent.speed = ROAM_SPEED;
	}

	public void Tick() {
		if (_navMeshAgent.remainingDistance < 1f) {
			Vector3 away = GetRandomPoint();
			_navMeshAgent.SetDestination(away);
		}
	}

	private Vector3 GetRandomPoint() {
		Vector3 direction = Random.insideUnitSphere;
		direction.y = 0;

		Vector3 endPoint = _enemy.transform.position + (direction * ROAM_DISTANCE);
		if (NavMesh.SamplePosition(endPoint, out NavMeshHit hit, 10f, NavMesh.AllAreas)) {
			return hit.position;
		}

		return _enemy.transform.position;
	}

	public void OnExit() {
		_navMeshAgent.speed = _initialSpeed;
		_navMeshAgent.enabled = false;
	}
}