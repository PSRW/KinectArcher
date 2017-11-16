using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Windows.Kinect;

[System.Serializable]
public class Bow
{
    public GameObject bowObject;
    public Arrow arrow;
    public BowString bowString;

    public bool ReadyToDraw { get { return arrow.spawnObject.activeInHierarchy; } }
    private float currentDrawForce = 0;

    public Bow()
    {
        arrow = new Arrow();
        bowString = new BowString();
    }

    public void Initialize()
    {
        arrow.Initialize();
        bowString.Initialize();
    }

    public void Rotate(float angle) { bowObject.transform.Rotate(0, 0, angle); }

    public void SetRotation(float angle) { bowObject.transform.eulerAngles = new Vector3(0, 0 , angle); }

    public void ReleaseArrow()
    {
        arrow.Spawn(currentDrawForce);
        bowString.ResetDraw();
        arrow.ResetSpawnPosition();
        currentDrawForce = 0;
    }

    public void LoadArrow()
    {
        arrow.spawnObject.SetActive(true);
    }
    
    public void Draw()
    {
        float drawEndArrow = -(bowString.maxDrawPoint - bowString.StringInitialPosition.y);

        if (arrow.spawnObject.transform.localPosition.x > drawEndArrow)
        {
            arrow.spawnObject.transform.Translate(-bowString.drawSpeedFactor, 0, 0);
            bowString.DrawOverTime();
            
            currentDrawForce += bowString.drawForceTimeFactor;
        }
    }

    public void Draw(float wristDistance)
    {
       float drawRatio = bowString.DrawByDistance(wristDistance);
        arrow.spawnObject.transform.localPosition = new Vector3(arrow.SpawnInitialPosition.x + bowString.StringInitialPosition.y - bowString.StringPosition.y,
                                                                arrow.spawnObject.transform.localPosition.y,
                                                                arrow.spawnObject.transform.localPosition.z);
       currentDrawForce = arrow.maxSpeed * drawRatio;
    }

}

[System.Serializable]
public class BowString
{
    public GameObject stringObject;
    public float wristMinDistance = 0.1f;
    public float wristMaxDistance = 0.7f;
    public float maxDrawPoint = 15.0f;
    public float drawSpeedFactor = 0.2f;
    public float drawForceTimeFactor = 6.5f;

    private LineRenderer bowStringRenderer;
    private Vector3 stringPosition = new Vector3();
    public Vector3 StringPosition { get { return stringPosition; } }
    private Vector3 stringInitialPosition = new Vector3();
    public Vector3 StringInitialPosition { get { return stringInitialPosition; } }

    public void ResetDraw ()
    {
        bowStringRenderer.SetPosition(1, stringInitialPosition);
    }

    public void DrawOverTime()
    {
        stringPosition = bowStringRenderer.GetPosition(1);
        stringPosition.y += drawSpeedFactor * 2;
        bowStringRenderer.SetPosition(1, stringPosition);
    }

    public float DrawByDistance(float wristDistance)
    {
        float drawRatio = (wristDistance - wristMinDistance) / (wristMaxDistance - wristMinDistance);
        stringPosition = stringInitialPosition;
        stringPosition.y = drawRatio * (maxDrawPoint - stringInitialPosition.y) + stringInitialPosition.y;
        bowStringRenderer.SetPosition(1, stringPosition);

        return drawRatio;
    }
    public void Initialize()
    {
        bowStringRenderer = stringObject.GetComponent<LineRenderer>();
        stringInitialPosition = bowStringRenderer.GetPosition(1);
    }


}

[System.Serializable]
public class Arrow
{
    public GameObject template;
    public GameObject spawnObject;
    private Vector3 spawnInitialPosition = new Vector3();
    public Vector3 SpawnInitialPosition { get { return spawnInitialPosition; } }
    public float maxSpeed;
    
    public void Spawn(float arrowSpeed)
    {
        GameObject arrow = UnityEngine.Object.Instantiate(template, spawnObject.transform.position, spawnObject.transform.rotation);
        Rigidbody2D arrowBody = arrow.GetComponent<Rigidbody2D>();
        float angle = arrow.transform.eulerAngles.z * Mathf.Deg2Rad;

        Vector2 arrowVelocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * arrowSpeed;
        arrowBody.velocity = arrowVelocity;

        Debug.Log(arrowBody.velocity);
        spawnObject.SetActive(false);
    }

    public void ResetSpawnPosition()
    {
        spawnObject.transform.localPosition = spawnInitialPosition;
    }
    
    public void Initialize()
    {
        spawnInitialPosition = spawnObject.transform.localPosition;
    }
    

}


public class PlayerController : MonoBehaviour {

    public Bow playerBow = new Bow();

    private KinectSensor kinectSensor;
    private Body[] bodyPartsData = null;
    private BodyFrameReader bodyReader;

    
    void Start () {

        playerBow.Initialize();
        kinectSensor = KinectSensor.GetDefault();
        if(kinectSensor != null)
        {
            bodyReader = kinectSensor.BodyFrameSource.OpenReader();
            if(!kinectSensor.IsOpen)
                kinectSensor.Open();
        }
	}

    void FixedUpdate() {
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
                    if(wristToWristDistance > 0.2)
                        playerBow.SetRotation(angle);
                    Debug.Log(String.Format("{0} {1}", wristToWristDistance, angle));

                    if (wristToWristDistance < 0.2)
                        playerBow.LoadArrow();
                    if (playerBow.ReadyToDraw)
                    {
                        playerBow.Draw(wristToWristDistance);
                    }
                    if (trackedBody.IsHandOpened() && playerBow.ReadyToDraw)
                        playerBow.ReleaseArrow();
                }
            }
        }
        getKeyboardInputs();

    }

    private void getKeyboardInputs()
    {
        float bowRotation = Input.GetAxis("Vertical");
        playerBow.Rotate(bowRotation);

        if (Input.GetKey(KeyCode.Space) && playerBow.ReadyToDraw)
        {
            playerBow.Draw();
        }
        else if (Input.GetKeyUp(KeyCode.Space) && playerBow.ReadyToDraw)
        {
            playerBow.ReleaseArrow();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            playerBow.LoadArrow();
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            playerBow.SetRotation(90);
        }
    }

}
