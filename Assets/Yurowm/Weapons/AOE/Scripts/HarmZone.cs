using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarmZone : MonoBehaviour {
    public GameObject owner;
    public string[] effectTagList = { };
    public float damagePerSecond = 10;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerStay(Collider other)
    {
        foreach(string s in effectTagList)
        {
            if(other.gameObject.tag == s)
            {
                Character script = other.gameObject.GetComponent<Character>();
                if (script != null && script.IsActive)
                {
                    script.TakeDamage(owner, 1, damagePerSecond * Time.deltaTime);
                    return;
                }
            }
        }
        
    }
}
