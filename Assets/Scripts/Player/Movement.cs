using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

	[Header("---| Settings |---")]
	[SerializeField] private AudioSource audioSource;
    [SerializeField] private ParticleSystem[] speedObjects;     // The thruster gameobjects to be activated
	[SerializeField] private int increaseAmount = 5;
	[SerializeField] private int increaseDuration = 5;
	[SerializeField] private Transform planeModel;

	[Header("---| Game Settings |---")]
	[SerializeField] private float speed = 40f;
	[SerializeField] private float rotationSpeed = 300f;
	[SerializeField] private int maxTiltDegrees = 60;
	[SerializeField] private int tiltSpeed = 60;
	[SerializeField] private float currentSpeed;

	private Camera mainCamera;

	private bool canMove = false;
	
    public bool CanMove {
        get {
            return canMove;
        }

        set {
            canMove = value;
        }
    }

	void OnEnable(){
		CancelInvoke("ReduceSpeed");
		ReduceSpeed();
	}

	void Awake() {
		mainCamera = Camera.main;
		currentSpeed = speed;
	}

	void Update () {
		if(CanMove){
			transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
			LookAtPoint();
         	transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
		} else {
			transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
		}
	}

	private void LookAtPoint() {
		Vector3 targetPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
		targetPosition.y = 0;

		var dir = targetPosition - transform.position;
		dir.y = 0;

		Quaternion rot = Quaternion.LookRotation(dir);
		
		transform.rotation = Quaternion.Lerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);

		//Tilt();
	}

	/* private void Tilt(){
		//planeModel.rotation = Quaternion.Euler(50,0,0);
	}*/

	public void DoSpeedIncrease() {
		foreach (ParticleSystem a in speedObjects) {
			a.Play();
		}

        currentSpeed = speed + increaseAmount;
		audioSource.pitch = 1.50f;

		CancelInvoke("ReduceSpeed");
		Invoke("ReduceSpeed", increaseDuration);
	}

	public void ReduceSpeed() {
		foreach (ParticleSystem a in speedObjects) {
			a.Stop();
		}
		audioSource.pitch = 1.0f;
		currentSpeed = speed;
	}
}
