using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;


public class PlayerController : MonoBehaviour {


    public GameObject arrowTemplate;
    public GameObject arrowSpawn;
    public GameObject bowString;

    private LineRenderer bowStringRenderer;

    private Vector3 bowStringMiddlePointPosition = new Vector3();
    private Vector3 bowStringMiddlePointInitialPosition = new Vector3();
    private Vector3 arrowSpawnInitialPosition = new Vector3();

    private KinectSensor kinectSensor;
    public float speedTimeFactor;
    private float speedCoefficient = 0;
    void Start () {
        bowStringRenderer = bowString.GetComponent<LineRenderer>();
        arrowSpawnInitialPosition = arrowSpawn.transform.localPosition;

        bowStringMiddlePointInitialPosition = bowStringRenderer.GetPosition(1);
	}

    void FixedUpdate () {
       float bowRotation = Input.GetAxis("Vertical");
        this.transform.rotation *= Quaternion.Euler(0, 0, bowRotation);

        
        if(Input.GetKey(KeyCode.Space))
        {
            DrawBow();
        }
        else if(Input.GetKeyUp(KeyCode.Space))
        {
            ReleaseArrow();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            arrowSpawn.SetActive(true);
        }
    }

    private void SpawnArrow(float speedFactor)
    {
        GameObject arrow = Instantiate(arrowTemplate, arrowSpawn.transform.position, arrowSpawn.transform.rotation);
        Rigidbody2D arrowBody = arrow.GetComponent<Rigidbody2D>();
        float angle = arrow.transform.eulerAngles.z * Mathf.Deg2Rad;

        Vector2 arrowVelocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speedFactor;
        arrowBody.velocity = arrowVelocity;

        //arrow.transform.localScale = arrowSpawn.transform.lossyScale;
        arrowSpawn.SetActive(false);
    }

    private void DrawBow()
    {
        const float drawSpeedFactor = 0.05f;
        const float drawEndString = 7.0f;
        float drawEndArrow = arrowSpawnInitialPosition.x - drawEndString;
        if (arrowSpawn.transform.localPosition.x > drawEndArrow)
        {
            arrowSpawn.transform.Translate(-drawSpeedFactor, 0, 0);
            bowStringMiddlePointPosition = bowStringRenderer.GetPosition(1);
            bowStringMiddlePointPosition.y += drawSpeedFactor;
            bowStringRenderer.SetPosition(1, bowStringMiddlePointPosition);
            speedCoefficient += speedTimeFactor;
        }
     }

    private void ReleaseArrow()
    {
        SpawnArrow(speedCoefficient);
        bowStringRenderer.SetPosition(1, bowStringMiddlePointInitialPosition);
        arrowSpawn.transform.localPosition = arrowSpawnInitialPosition;
        speedCoefficient = 0;
    }
}

