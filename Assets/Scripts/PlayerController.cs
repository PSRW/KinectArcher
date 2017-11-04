using System.Collections; 
using System.Collections.Generic;
using System.Linq;
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

                var trackedBody = bodyPartsData.SingleOrDefault(x => x.IsTracked);

                if (trackedBody != null)
                {
                    Vector2 leftWristPos = new Vector2(trackedBody.Joints[JointType.WristLeft].Position.X,
                                   trackedBody.Joints[JointType.WristLeft].Position.Y);
                    Vector2 rightWristPos = new Vector2(trackedBody.Joints[JointType.WristRight].Position.X,
                                                    trackedBody.Joints[JointType.WristRight].Position.Y);
                    Vector2 ortographicPoint = new Vector2(rightWristPos.x, leftWristPos.y);

                    float wristToWristDistance = (rightWristPos - leftWristPos).magnitude;

                    float pointingWristToOrtographicPoint = (rightWristPos - ortographicPoint).magnitude;

                    float angle = Mathf.Asin(wristToWristDistance / pointingWristToOrtographicPoint);
                    angle *= Mathf.Deg2Rad;

                    var a = 1;
                }
            }
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

