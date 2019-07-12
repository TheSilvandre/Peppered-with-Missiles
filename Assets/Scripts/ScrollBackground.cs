using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollBackground : MonoBehaviour {

	public int parallaxFactor = 100;
	public GameObject quadGameObject;

	private Renderer quadRenderer;
	private Transform plane;

	void Start(){
		quadRenderer = quadGameObject.GetComponent<Renderer>();
		plane = GameObject.FindGameObjectWithTag("Player").transform;
	}

	void Update(){
		Vector2 textureOffset = new Vector2(plane.transform.position.x/parallaxFactor, plane.transform.position.z/parallaxFactor);
		quadRenderer.material.mainTextureOffset = textureOffset;
	}
		
}
