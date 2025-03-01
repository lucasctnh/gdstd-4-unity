﻿using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Roam : IState {
	private readonly EnemyController _enemy;
	private NavMeshAgent _navMeshAgent;

	private float _initialSpeed;
	private bool _canRoam = true;

	private readonly float[] ROAM_SPEED_MIN_MAX = { 2f, 6f };
	private readonly float[] ROAM_DISTANCE_MIN_MAX = { 10f, 20f };
	private readonly int[] ROAM_WAIT_TIME_MIN_MAX = { 2, 5 };

	public Roam(EnemyController enemy, NavMeshAgent navMeshAgent) {
		_enemy = enemy;
		_navMeshAgent = navMeshAgent;
	}

	public void OnEnter() {
		_navMeshAgent.enabled = true;
		_initialSpeed = _navMeshAgent.speed;
		_navMeshAgent.speed = Random.Range(ROAM_SPEED_MIN_MAX[0], ROAM_SPEED_MIN_MAX[1]);
	}

	public void OnExit() {
		_navMeshAgent.speed = _initialSpeed;
		_navMeshAgent.enabled = false;
	}

	public void Tick() {
		if (_navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete && _navMeshAgent.remainingDistance < 1 && _canRoam) {
			Vector3 away = GetRandomPoint();
			_navMeshAgent.SetDestination(away);

			WaitToRoamAgain();
		}
	}

	private Vector3 GetRandomPoint() {
		Vector3 direction = Random.insideUnitSphere;
		direction.y = 0;

		Vector3 endPoint = _enemy.transform.position + (direction * Random.Range(ROAM_DISTANCE_MIN_MAX[0], ROAM_DISTANCE_MIN_MAX[1]));
		if (NavMesh.SamplePosition(endPoint, out NavMeshHit hit, 10f, NavMesh.AllAreas)) {
			return hit.position;
		}

		return _enemy.transform.position;
	}

	private async void WaitToRoamAgain() {
		_canRoam = false;

		for (int i = 0; i < Random.Range(ROAM_WAIT_TIME_MIN_MAX[0], ROAM_WAIT_TIME_MIN_MAX[1]) * 100; i++)
			await Task.Yield();

		_canRoam = true;
	}
}