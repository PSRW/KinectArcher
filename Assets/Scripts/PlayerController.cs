using Assets.Scripts;
using System.Collections; 
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Windows.Kinect;

public class PlayerController : MonoBehaviour {

    public GameObject arrowTemplate;
    public GameObject arrowSpan;

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

                var wristToWristDistance = trackedBody.GetWristsDistance();
                var angle = trackedBody.GetPointingAngle();
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
            arrowSpan.SetActive(true);
        }
    }

    private void SpawnArrow(float speedFactor)
    {
        GameObject arrow = Instantiate(arrowTemplate, arrowSpan.transform.position, arrowSpan.transform.rotation);
        Rigidbody2D arrowBody = arrow.GetComponent<Rigidbody2D>();
        float angle = arrow.transform.eulerAngles.z * Mathf.Deg2Rad;

        Vector2 arrowVelocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speedFactor;
        arrowBody.velocity = arrowVelocity;

        //arrow.transform.localScale = arrowSpawn.transform.lossyScale;
        arrowSpan.SetActive(false);
    }
}

