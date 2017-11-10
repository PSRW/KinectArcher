using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private Body[] bodyPartsData = null;
    private BodyFrameReader bodyReader;

    public float speedTimeFactor;
    public float arrowMaxSpeed;
    private float arrowSpeedCoefficient = 0;
    
    void Start () {
        bowStringRenderer = bowString.GetComponent<LineRenderer>();
        arrowSpawnInitialPosition = arrowSpawn.transform.localPosition;

        bowStringMiddlePointInitialPosition = bowStringRenderer.GetPosition(1);
        kinectSensor = KinectSensor.GetDefault();
        if(kinectSensor != null)
        {
            bodyReader = kinectSensor.BodyFrameSource.OpenReader();
            if(!kinectSensor.IsOpen)
                kinectSensor.Open();
        }
	}

    void FixedUpdate() {
        float bowRotation = Input.GetAxis("Vertical");
        this.transform.rotation *= Quaternion.Euler(0, 0, bowRotation);

        using (var frame = bodyReader.AcquireLatestFrame())
        {
            if (frame != null)
            {
                if (bodyPartsData == null)
                    bodyPartsData = new Body[kinectSensor.BodyFrameSource.BodyCount];
                frame.GetAndRefreshBodyData(bodyPartsData);
                var trackedBody = bodyPartsData.FirstOrDefault(x => x.IsTracked);
                if (trackedBody != null)
                {
                    var wristToWristDistance = trackedBody.GetWristsDistance();
                    var angle = trackedBody.GetPointingAngle();
                    angle *= Mathf.Rad2Deg;
                    this.transform.eulerAngles = new Vector3(0, 0, angle);
                    Debug.Log(String.Format("{0} {1}", wristToWristDistance, angle));

                    if (wristToWristDistance < 0.2)
                        arrowSpawn.SetActive(true);
                    if (arrowSpawn.activeInHierarchy)
                    {
                        DrawBow(wristToWristDistance);
                    }
                    if (trackedBody.IsHandOpened() && arrowSpawn.activeInHierarchy)
                        ReleaseArrow();
                }
            }
        }

        getKeyboardInputs();

    }

    private void getKeyboardInputs()
    {
        if (Input.GetKey(KeyCode.Space) && arrowSpawn.activeInHierarchy)
        {
                DrawBow(0.2f);
        }
        else if (Input.GetKeyUp(KeyCode.Space) && arrowSpawn.activeInHierarchy)
        {
               ReleaseArrow();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            arrowSpawn.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            this.transform.eulerAngles = new Vector3(0, 0, 90);
        }
    }
    private void SpawnArrow(float speedFactor)
    {
        GameObject arrow = Instantiate(arrowTemplate, arrowSpawn.transform.position, arrowSpawn.transform.rotation);
        Rigidbody2D arrowBody = arrow.GetComponent<Rigidbody2D>();
        float angle = arrow.transform.eulerAngles.z * Mathf.Deg2Rad;

        Vector2 arrowVelocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speedFactor;
        arrowBody.velocity = arrowVelocity;

        Debug.Log(arrowBody.velocity);
        arrowSpawn.SetActive(false);
    }

    private void DrawBow(float wristDistance)
    {
        Extensions.BowDrawCharacteristics bowDrawCharacteristics = Extensions.GetDrawCharacteristics(wristDistance);

        arrowSpawn.transform.localPosition = new Vector3(-bowDrawCharacteristics.BowStringDraw + arrowSpawnInitialPosition.x + bowStringMiddlePointInitialPosition.y,
                                                         arrowSpawn.transform.localPosition.y,
                                                         arrowSpawn.transform.localPosition.z);

        bowStringMiddlePointPosition = bowStringRenderer.GetPosition(1);
        bowStringMiddlePointPosition.y = bowDrawCharacteristics.BowStringDraw;
        bowStringRenderer.SetPosition(1, bowStringMiddlePointPosition);

        arrowSpeedCoefficient = arrowMaxSpeed * bowDrawCharacteristics.BowStringDrawCoefficient;
     }

    private void DrawBow()
    {
        const float drawSpeedFactor = 0.2f;
        const float drawEndString = 15.0f;
        float drawEndArrow = -(drawEndString - bowStringMiddlePointInitialPosition.y);
        if (arrowSpawn.transform.localPosition.x > drawEndArrow)
        {
            arrowSpawn.transform.Translate(-drawSpeedFactor, 0, 0);
            bowStringMiddlePointPosition = bowStringRenderer.GetPosition(1);
            bowStringMiddlePointPosition.y += drawSpeedFactor*2;
            bowStringRenderer.SetPosition(1, bowStringMiddlePointPosition);
            arrowSpeedCoefficient += speedTimeFactor;
        }
    }

    private void ReleaseArrow()
    {
        SpawnArrow(arrowSpeedCoefficient);
        bowStringRenderer.SetPosition(1, bowStringMiddlePointInitialPosition);
        arrowSpawn.transform.localPosition = arrowSpawnInitialPosition;
        arrowSpeedCoefficient = 0;
    }
}
