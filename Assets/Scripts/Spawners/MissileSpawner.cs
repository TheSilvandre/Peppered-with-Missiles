using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSpawner : MonoBehaviour {

	[SerializeField] private List<GameObject> missileList;
	
	[SerializeField] private float firstSpawnDelay = 1.0f;
	[SerializeField] private float spawnDelay = 10.0f;
	[SerializeField] private int minimumSpawnRadius = 100;
	[SerializeField] private int maximumSpawnRadius = 400;
	[SerializeField] private Transform plane;
	[SerializeField] private GameObject missileParent;

	private float level = 0;

	void OnDrawGizmosSelected()
    {
        // Display the spawn radius when selected
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(plane.position, minimumSpawnRadius);
        Gizmos.DrawWireSphere(plane.position, maximumSpawnRadius);
    }

	public void StartSpawning(float level = 0){
		if(this.level == 0){
			if(level == 0){
				this.level++;
			} else {
				this.level = level;
			}
			InvokeRepeating("SpawnMissile", firstSpawnDelay, spawnDelay);
		} else {
			Debug.LogError("Attempting to spawn missiles twice!");
		}
	}

	private void SpawnMissile(){

		int numberOfMissiles = getNumberOfMissiles(level-1, level, level+2);

		for(int i = 0; i < numberOfMissiles; i++){
			int index = Random.Range(0, missileList.Count);

			int x = Random.Range(minimumSpawnRadius, maximumSpawnRadius);
			int z = Random.Range(minimumSpawnRadius, maximumSpawnRadius);
			float signX = Mathf.Sign(Random.Range(-1f, 1f));
			float signZ = Mathf.Sign(Random.Range(-1f, 1f));

			Vector3 randomPosition = new Vector3(x * signX, 0, z * signZ);
        	Vector3 relativePos = plane.position - randomPosition;
        	Quaternion q = Quaternion.LookRotation(relativePos);
        	GameObject missile = Instantiate(missileList[index], randomPosition, q);
        	missile.transform.position =  plane.position + randomPosition;
			missile.transform.parent = missileParent.transform;
		}

		level += 0.5f;
	}

    public float StopSpawning() {
        CancelInvoke("SpawnMissile");
		float oldLevel = level;
        level = 0;
		return oldLevel;
    }

	private int getNumberOfMissiles(float c, float xmin, float xmax) {
		float randomNumber = Random.Range(0f, 1f);
        if (randomNumber <= (c - xmin) / (xmax - xmin)){
            return Mathf.RoundToInt( xmin + Mathf.Sqrt((xmax - xmin) * (c - xmin) * randomNumber) );
		} else {
			return Mathf.RoundToInt( xmax - Mathf.Sqrt((xmax - xmin) * (xmax - c) * (1 - randomNumber)) );
		}
    }

}
