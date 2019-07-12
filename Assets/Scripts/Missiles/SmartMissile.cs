using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class SmartMissile : MonoBehaviour {
	
	[Header("---| Game Settings |---")]
	public float speed = 50f;
	public float rotationSpeed = 70f;
	public int rotationDeadzone = 4;

	[Header("---| Attributes |---")]
	public Transform plane;
	public GameObject missileModel;
	public GameObject missileLight;
	public GameObject thruster;
	public Transform explosionParent;

	[Header("---| Particle Effects |---")]
	public GameObject explosionEffect;

	private bool exploded = false;

	void Start () {	// Missiles should maybe be pooled as a way to optimize the game
		plane = GameManager.Instance.planeObject.transform;
		explosionParent = GameManager.Instance.explosionParent.transform;
	}
	
	void Update () {
		if(!exploded){
			if(plane != null) {
				transform.Translate(Vector3.forward * speed * Time.deltaTime);
				LookAtPoint();
			}
		}
	}

	void LookAtPoint(){
		Vector3 targetPosition = plane.transform.position;
		targetPosition.y = 0;

		var dir = targetPosition - transform.position;
        var ang = Vector3.SignedAngle(transform.forward, dir, Vector3.up);
        var sign = Mathf.Sign(ang);
		
        if(Mathf.Abs(ang) > rotationDeadzone){
			transform.Rotate(Vector3.up, rotationSpeed * sign * Time.deltaTime);
		} else {
			sign = 0;
		}
	}

    void OnTriggerEnter(Collider other) {
		missileLight.SetActive(false);
		if(!other.CompareTag("Player")) {
			var explosion = Instantiate(explosionEffect, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
			explosion.transform.parent = explosionParent;
			if(!other.CompareTag("Shield")){
				if(other.CompareTag("Missile")){
					ScoreManager.Instance.CurrentScore++;
				} else {
					Destroy(other.gameObject);
				}
			} else {
				ScoreManager.Instance.CurrentScore++;
			}
		}
		CameraShaker.Instance.ShakeOnce(10f, 10f, 0.1f, 1f);
		SelfDestruct();
    }

	public void SelfDestruct(){
		exploded = true;
		missileModel.SetActive(false);
		thruster.SetActive(false);
		gameObject.GetComponent<Collider>().enabled = false;	// So player cant hit the collider after the missile is destroyed

		GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity) as GameObject;
		explosion.transform.parent = explosionParent;
		
		AudioSource audioSource = gameObject.GetComponent<AudioSource>();
		audioSource.Play();

		Destroy(gameObject, audioSource.clip.length);
	}
}
