using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPickUp : MonoBehaviour {
	
	private bool triggered = false;

	void OnTriggerEnter(Collider other){
		if(other.CompareTag("Player") && !triggered){
			triggered = true;
			foreach (Transform t in gameObject.transform){
				t.gameObject.SetActive(false);
			}

			other.gameObject.GetComponent<Movement>().DoSpeedIncrease();
			AudioSource audioSource = GetComponent<AudioSource>();
			audioSource.Play();
			Destroy(gameObject, audioSource.clip.length);
		}
	}
}
