using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class DumbMissile : MonoBehaviour {

	[Header("---| Attributes |---")]
	public float speed = 80f;
	[SerializeField] private GameObject missileModel;
	public GameObject thruster;
	public Transform explosionParent;

	[Header("---| Particle Effects |---")]
	public GameObject explosionEffect;

	[Header("---| Sound Effects |---")]
	public AudioClip explosionSound;

	private bool exploded = false;

	void Start () {	//Missiles should maybe be pooled later on as a way to optimize the game
		transform.LookAt(GameManager.Instance.planeObject.transform);
		Invoke("SelfDestruct", 20f);
		explosionParent = GameManager.Instance.explosionParent.transform;
	}
	
	void Update () {
		if(!exploded){
			transform.Translate(Vector3.forward * speed * Time.deltaTime);
		}
	}

    void OnTriggerEnter(Collider other) {
		thruster.SetActive(false);
		if(!other.CompareTag("Player")) {
			if(!other.CompareTag("Shield")){
				if(other.CompareTag("Missile")){
					// If it hits a missile, increase score
					ScoreManager.Instance.CurrentScore++;
				} else {
					Destroy(other.gameObject);
				}
			} else {
				// If it hits a shield, increase score
				ScoreManager.Instance.CurrentScore++;
			}
		}
		CameraShaker.Instance.ShakeOnce(10f, 10f, 0.1f, 1f);
		exploded = true;
		SelfDestruct();
    }

	public void SelfDestruct(){
		exploded = true;
		ParticleSystem particleSystem = gameObject.GetComponent<ParticleSystem>();
		particleSystem.Stop();
		missileModel.SetActive(false);
		thruster.SetActive(false);
		gameObject.GetComponent<Collider>().enabled = false;																			// So player cant hit the collider after the missile is destroyed

		GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity) as GameObject;
		explosion.transform.parent = explosionParent;

		AudioSource audioSource = GetComponent<AudioSource>();
		audioSource.Play();

		float audioClipDuration = audioSource.clip.length;
		//float particleDuration = particleSystem.main.duration + particleSystem.main.startLifetimeMultiplier;
		//if(audioClipDuration > particleDuration){
			Destroy(gameObject, audioClipDuration);
		//} else {
		//	Destroy(gameObject, particleDuration);
		//}
		
	}
}
