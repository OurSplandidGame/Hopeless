using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour {
    public int secondsPerDay = 1;

    private float speed;
	// Use this for initialization
	void Start () {
        speed = 360.0f / secondsPerDay / 60.0f;
    }
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Time.deltaTime * speed, 0, 0);
	}
}
