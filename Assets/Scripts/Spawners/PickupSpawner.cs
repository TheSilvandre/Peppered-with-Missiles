using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour {

	public List<GameObject> powerupList;
	
	public float spawnDelay = 15f;
	[SerializeField] private Transform plane;
	[SerializeField] private int minimumSpawnRadius = 50;
	[SerializeField] private int maximumSpawnRadius = 50;

	private bool spawning = false;

	void OnDrawGizmosSelected()
    {
        // Display the spawn radius when selected
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(plane.position, minimumSpawnRadius);
        Gizmos.DrawWireSphere(plane.position, maximumSpawnRadius);
    }
	
	public void StartSpawning() {
		if(!spawning){
			spawning = true;
			InvokeRepeating("SpawnPickup", spawnDelay, spawnDelay);
		} else {
			Debug.LogError("Attempting to spawn pickups twice!");
		}
	}

	private void SpawnPickup(){
		int x = Random.Range(minimumSpawnRadius, maximumSpawnRadius);
		int z = Random.Range(minimumSpawnRadius, maximumSpawnRadius);
		float signX = Mathf.Sign(Random.Range(-1f, 1f));
		float signZ = Mathf.Sign(Random.Range(-1f, 1f));

		Vector3 randomPosition = new Vector3(x * signX, 0, z * signZ);
        Vector3 relativePos = plane.position - randomPosition;
		int index = Random.Range(0, powerupList.Count);
		Instantiate(powerupList[index], relativePos, Quaternion.identity);
	}

    public void StopSpawning() {
        CancelInvoke("SpawnPickup");
        spawning = false;
    }
}
