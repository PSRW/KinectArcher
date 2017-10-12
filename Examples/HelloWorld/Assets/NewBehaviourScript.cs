using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Kinect;
using Windows.Kinect;

public class NewBehaviourScript : MonoBehaviour {

    private KinectSensor _Sensor;
    private BodyFrameReader _Reader;
    private Body[] _Data = null;
    private float horizontal;
    private float vertical;
    private float firstdeep;
    private float deep;
    private float angley;
    private float anglex;
    private float anglez;
    private float cameraSize;

    bool CheckBoundaries()
    {
        if (Math.Abs(this.gameObject.transform.position.x) > cameraSize)
        {
            return false;
        } 
        return true;
    }
    // Use this for initialization
    void Start () {
        _Sensor = KinectSensor.GetDefault();

        if (_Sensor != null)
        {
            _Reader = _Sensor.BodyFrameSource.OpenReader();

            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
            }
        }

        cameraSize = Camera.main.orthographicSize;

    }
	
	// Update is called once per frame
void Update()
{
    if (_Reader != null)
    {
        var frame = _Reader.AcquireLatestFrame();

        if (frame != null)
        {
            if (_Data == null)
            {
                _Data = new Body[_Sensor.BodyFrameSource.BodyCount];
            }

            frame.GetAndRefreshBodyData(_Data);

            frame.Dispose();
            frame = null;

            int idx = -1;
            for (int i = 0; i < _Sensor.BodyFrameSource.BodyCount; i++)
            {
                if (_Data[i].IsTracked)
                {
                    idx = i;
                }
            }
            if (idx>-1)
            {
                if (_Data[idx].HandRightState != HandState.Closed)
                {
                    horizontal = 
                        (float)(_Data[idx].Joints[JointType.HandRight].Position.X 
                        * 1);
                    vertical = 
                        (float)(_Data[idx].Joints[JointType.HandRight].Position.Y 
                        * 1);

                    if (firstdeep == -1)
                    {
                        firstdeep = 
                            (float)(_Data[idx].Joints[JointType.HandRight].Position.Z 
                            * 0.1);
                    }
                    deep = 
                        (float)(_Data[idx].Joints[JointType.HandRight].Position.Z 
                        * 0.1) - firstdeep;

                        if (CheckBoundaries())
                        {
                            this.gameObject.transform.position = new Vector3(
                                this.gameObject.transform.position.x + horizontal,
                                this.gameObject.transform.position.y + vertical,
                                this.transform.position.z + deep);
                        }

                        else 
                        {
                         
                            this.gameObject.transform.position = new Vector3(
                            this.gameObject.transform.position.x - horizontal,
                            this.gameObject.transform.position.y - vertical,
                            this.transform.position.z + deep);

                        }
                }
                if (_Data[idx].HandLeftState != HandState.Closed)
                {
                    angley = 
                        (float)(_Data[idx].Joints[JointType.HandLeft].Position.X );
                    anglex = 
                        (float)(_Data[idx].Joints[JointType.HandLeft].Position.Y);
                    anglez = 
                        (float)(_Data[idx].Joints[JointType.HandLeft].Position.Z);

          
                   
                }

                if(_Data[idx].HandRightState == HandState.Closed)
                    {
                        this.gameObject.transform.position = new Vector3(0, 0, this.transform.position.z);
                    }
            }
        }
    }
}

    void OnApplicationQuit()
    {
        if (_Reader != null)
        {
            _Reader.Dispose();
            _Reader = null;
        }

        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }
            _Sensor = null;
        }
    }
}
