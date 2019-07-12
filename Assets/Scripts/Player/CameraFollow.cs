using UnityEngine;

public class CameraFollow : MonoBehaviour {

	[SerializeField] private Transform plane;
	public bool follow = false; 
	public bool start = true;	// True when the camera should zoom out

	private Camera mainCamera;

	void Start(){
		plane = GameManager.Instance.planeObject.transform;
		mainCamera = Camera.main;
	}

	void LateUpdate () {
		if(plane != null){
			if(start && follow) {
				transform.position = new Vector3(plane.position.x, 10f, plane.position.z);
				mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, 40, Time.deltaTime * 3);
			} else if(!start){
				transform.position = new Vector3(plane.position.x, 10f, plane.position.z);
			} else {
				transform.position = new Vector3(plane.position.x - 10, 10f, plane.position.z);
			}
		}
	}
}
