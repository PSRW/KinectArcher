using System.Collections; 
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class PlayerController : MonoBehaviour {

    public GameObject arrowTemplate;
    public GameObject arrowSpawn;
    private KinectSensor kinectSensor;
    private Body[] bodyPartsData = null;

    private BodyFrameReader bodyReader;

    public float speedTimeFactor;
    private float speedCoefficient = 0;
    void Start () {
        kinectSensor = KinectSensor.GetDefault();
        if(kinectSensor != null)
        {
            bodyReader = kinectSensor.BodyFrameSource.OpenReader();
            if(!kinectSensor.IsOpen)
                kinectSensor.Open();
            
        }
		
	}

    void FixedUpdate () {
       float bowRotation = Input.GetAxis("Vertical");
        this.transform.rotation *= Quaternion.Euler(0, 0, bowRotation);

        var frame = bodyReader.AcquireLatestFrame();
        if(frame != null)
        {
            if (bodyPartsData == null)
                bodyPartsData = new Body[kinectSensor.BodyFrameSource.BodyCount];
            frame.GetAndRefreshBodyData(bodyPartsData);
            frame.Dispose();
            frame = null;

            Vector2 wristPos = new Vector2(bodyPartsData[1].Joints[JointType.WristLeft].Position.X,
                                           bodyPartsData[1].Joints[JointType.WristLeft].Position.Y);
            Vector2 spineBasePos = new Vector2(bodyPartsData[1].Joints[JointType.SpineBase].Position.X,
                                            bodyPartsData[1].Joints[JointType.SpineBase].Position.Y);
            Vector2 spineShoulderPos = new Vector2(bodyPartsData[1].Joints[JointType.SpineShoulder].Position.X,
                                                bodyPartsData[1].Joints[JointType.SpineShoulder].Position.Y);

            Debug.Log("wristPos: " + wristPos.ToString() + "spineBasePos: " + spineBasePos.ToString());

            float heigth = new Vector2(Mathf.Abs(wristPos.x - spineShoulderPos.x), spineShoulderPos.y).magnitude;
            Debug.Log("hegth: " + heigth.ToString());
        }
        
        if(Input.GetKey(KeyCode.Space))
        {
            speedCoefficient += speedTimeFactor;
        }
        else if(Input.GetKeyUp(KeyCode.Space))
        {
            SpawnArrow(speedCoefficient);
            speedCoefficient = 0;
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
}

