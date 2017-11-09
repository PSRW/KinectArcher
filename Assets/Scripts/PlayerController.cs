﻿using Assets.Scripts;
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
    private float speedCoefficient = 0;

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

    private void Update()
    {
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
                }
            }
        }

    }
    void FixedUpdate() {
        float bowRotation = Input.GetAxis("Vertical");
        this.transform.rotation *= Quaternion.Euler(0, 0, bowRotation);


        if (Input.GetKey(KeyCode.Space))
        {
            if(arrowSpawn.activeInHierarchy)
                DrawBow();
        }
        else if(Input.GetKeyUp(KeyCode.Space) && arrowSpawn.activeInHierarchy)
        {
            if(arrowSpawn.activeInHierarchy)
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
