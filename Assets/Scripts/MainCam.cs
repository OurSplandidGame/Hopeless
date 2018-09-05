using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCam : MonoBehaviour {
    public GameObject target;
    public Vector3 offset;
	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = target.transform.position + offset;
	}
}
