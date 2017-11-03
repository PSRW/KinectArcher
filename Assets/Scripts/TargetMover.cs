using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMover : MonoBehaviour {

    private Rigidbody2D targetBody;
	// Use this for initialization
	void Start () {
        targetBody = GetComponent<Rigidbody2D>();
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        targetBody.velocity *= -1;
    }
}
