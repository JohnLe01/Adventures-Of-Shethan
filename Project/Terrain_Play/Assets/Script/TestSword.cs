using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSword : MonoBehaviour {
    public GameObject character;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        this.gameObject.transform.Rotate(0, 0, 5);
	}

    void OnTriggerEnter(Collider other) {
        Destroy(this.gameObject);
    }
}
