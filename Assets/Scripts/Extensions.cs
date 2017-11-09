using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Windows.Kinect;

namespace Assets.Scripts
{
    public static class Extensions
    {
        /// <summary>
        /// Get distance between wrists of the tracked body
        /// </summary>
        /// <param name="trackedBody"></param>
        /// <returns></returns>
        public static float GetWristsDistance(this Body trackedBody)
        {
            var leftWristPos = new Vector2(trackedBody.Joints[JointType.WristLeft].Position.X, trackedBody.Joints[JointType.WristLeft].Position.Y);
            var rightWristPos = new Vector2(trackedBody.Joints[JointType.WristRight].Position.X, trackedBody.Joints[JointType.WristRight].Position.Y);
            return (rightWristPos - leftWristPos).magnitude;
        }

        /// <summary>
        /// Get angle between wrist-to-wrist line and horizontal line in radians
        /// </summary>
        /// <param name="trackedBody"></param>
        /// <param name="isDominantHandRight"></param>
        /// <returns></returns>
        public static float GetPointingAngle(this Body trackedBody, bool isPointingHandRight = true)
        {
            var pointingHandJoint = isPointingHandRight ? trackedBody.Joints[JointType.WristRight] : trackedBody.Joints[JointType.WristLeft];
            var holdingHandJoint = pointingHandJoint == trackedBody.Joints[JointType.WristRight] ? trackedBody.Joints[JointType.WristLeft] : trackedBody.Joints[JointType.WristRight];

            var pointingHandJointVector = new Vector2(pointingHandJoint.Position.X, pointingHandJoint.Position.Y);
            var holdingHandJointVector = new Vector2(holdingHandJoint.Position.X, holdingHandJoint.Position.Y);

            Vector2 ortographicPoint = new Vector2(holdingHandJointVector.x, pointingHandJointVector.y);
            float pointingWristToOrtographicPoint = (holdingHandJointVector - ortographicPoint).magnitude;


            if(pointingHandJoint.Position.Y > holdingHandJoint.Position.Y )
                return Mathf.Asin(pointingWristToOrtographicPoint / trackedBody.GetWristsDistance());
            else
                return -Mathf.Asin(pointingWristToOrtographicPoint / trackedBody.GetWristsDistance());
        }
    }
}
