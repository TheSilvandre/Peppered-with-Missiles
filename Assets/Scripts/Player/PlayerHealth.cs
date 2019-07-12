using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour {

	[Header("Explosions")]
	[SerializeField] private GameObject explosionEffect;
	[SerializeField] private GameObject explosionParent;

	[Header("Invulnerability")]
	[SerializeField] private float invulnerabilityDuration = 5f;
	[SerializeField] private GameObject shield;
	
	// Events
    public GameEvent damageEvent;

	private bool invulnerable = false;

	private void OnTriggerEnter(Collider other){
		if(other.CompareTag("Missile") && !invulnerable){
			//audioSource.PlayOneShot(missileOnMissileSounds[Random.Range(0, missileOnMissileSounds.Length)]);
			var explosion = Instantiate(explosionEffect, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
			explosion.transform.parent = explosionParent.transform;
			damageEvent.Raise();
			gameObject.SetActive(false);
		}
    }

	public void InvulnerabilityPickedUp(){
		shield.SetActive(true);
        invulnerable = true;
        CancelInvoke("RemoveInvulnerability");
        Invoke("RemoveInvulnerability", invulnerabilityDuration);
    }

    private void RemoveInvulnerability(){
		// TODO play sound
        shield.SetActive(false);
        invulnerable = false;
    }

}