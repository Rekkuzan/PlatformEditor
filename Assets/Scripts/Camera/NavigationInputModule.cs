using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformEditor
{
    /// <summary>
    /// Handle behaviour of camera based on Input
    /// </summary>
    [RequireComponent(typeof(InputBasicGestureModule))]
    public class NavigationInputModule : MonoBehaviour
    {
        [SerializeField] CameraHandler CameraH;

        [SerializeField] float ZoomPinchSpeed = 1.5f;
        [SerializeField] float RotatePinchSpeed = 0.1f;
        [SerializeField] float PanSpeed = 1;
        [SerializeField] float MinDistanceFromGround = 2;

        private void OnEnable()
        {
            InputBasicGestureModule.OnPinch += OnPinch;
            InputBasicGestureModule.OnRotate += OnRotate;
            InputBasicGestureModule.OnPan += OnPan;
        }

        private void OnDisable()
        {
            InputBasicGestureModule.OnPinch -= OnPinch;
            InputBasicGestureModule.OnRotate -= OnRotate;
            InputBasicGestureModule.OnPan -= OnPan;
        }

        private void OnRotate(float signedDeltaAngle)
        {
            if (ObjectSelectionTransform.Instance.CurrentState != ObjectSelectionTransform.State.NoSelection)
                return;
            CameraH.AngleRotationToApply -= signedDeltaAngle * RotatePinchSpeed;
        }

        private void OnPinch(float signedDeltaDistance)
        {
            if (ObjectSelectionTransform.Instance.CurrentState != ObjectSelectionTransform.State.NoSelection)
                return;

            var currentDistance = Vector3.Distance(CameraH.transform.position, CameraH.LookAtPlanePoint);

            if (currentDistance <= MinDistanceFromGround && signedDeltaDistance > 0)
                CameraH.ForwardDistanceToApply = 0;
            else
                CameraH.ForwardDistanceToApply -= signedDeltaDistance * ZoomPinchSpeed;
        }

        private void OnPan(Vector2 dir)
        {
            if (ObjectSelectionTransform.Instance.CurrentState != ObjectSelectionTransform.State.NoSelection)
                return;
            CameraH.DirectionToApply -= dir * PanSpeed;
        }
    }
}