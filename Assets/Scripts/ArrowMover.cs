using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMover : MonoBehaviour {

    public  float lifeTimeAfterCollision;
    private Rigidbody2D arrowBody;
    private bool collided;
    private GameController gameController;


	void Start () {
        arrowBody = GetComponent<Rigidbody2D>();
        collided = false;
        GameObject gameControllerObj = GameObject.FindWithTag("GameController");
        if (gameControllerObj)
        {
            gameController = gameControllerObj.GetComponent<GameController>();
        }
        else
        {
            Debug.Log("Error, no game controller!");
            gameController = null;
        }
    }
	
	void FixedUpdate () {
        if (arrowBody != null)
        {
            if (!collided)
                transform.right = Vector3.Slerp(transform.right, arrowBody.velocity.normalized, Time.deltaTime * arrowBody.velocity.magnitude);
            else if (arrowBody.velocity.magnitude > 35)
                transform.right = Vector3.Slerp(transform.right, arrowBody.velocity.normalized, Time.deltaTime);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        collided = true;
        Destroy(this.gameObject, lifeTimeAfterCollision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Target"))
        {
            Destroy(collision.gameObject);
            if (gameController)
            {
                gameController.SpawnTarget();
            }
        }
    }

}
