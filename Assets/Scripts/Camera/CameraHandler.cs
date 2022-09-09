using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformEditor
{
    /// <summary>
    /// Handler to perform transform functions on the camera
    /// Give access to Raycast method from camera
    /// </summary>
    public class CameraHandler : SingletonMonobehaviour<CameraHandler>
    {
        [Header("Settings")]
        [SerializeField]
        private MeshCollider PlaneReference;

        public LayerMask EnvMask;
        public LayerMask ItemMask;


        private Vector3? _lookAtPointOnPlane;
        public Vector3 LookAtPlanePoint
        {
            get
            {
                if (_lookAtPointOnPlane == null)
                {
                    UpdateLookAtPoint();
                }

                return _lookAtPointOnPlane ?? Vector3.zero;
            }
            private set
            {
                _lookAtPointOnPlane = value;
            }
        }

        private Camera _camera;
        private Vector3 _prevPosition;
        private Quaternion _prevRotation;

        [HideInInspector]
        public float ForwardDistanceToApply;
        [HideInInspector]
        public float AngleRotationToApply;
        [HideInInspector]
        public Vector2 DirectionToApply;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _prevPosition = transform.position;
            _prevRotation = transform.rotation;
            ForwardDistanceToApply = 0;
            AngleRotationToApply = 0;
            DirectionToApply = Vector2.zero;
        }

        private void LateUpdate()
        {
            // Apply camera movement
            ApplyAngleRotation();
            ApplyForwardDistance();
            ApplyDirection();
            //Update the looking point
            UpdateLookAtPoint();
        }

        /// <summary>
        /// Apply the distance from the LookAtPoint 
        /// </summary>
        private void ApplyForwardDistance()
        {
            if (Mathf.Abs(ForwardDistanceToApply) <= float.Epsilon)
            {
                // no change, no need to update the forward distance
                return;
            }


            float toApply = Mathf.Lerp(ForwardDistanceToApply, 0, Time.deltaTime * 5);
            ForwardDistanceToApply -= toApply;
            transform.position -= transform.forward * toApply;
        }

        /// <summary>
        /// Apply the angle rotation from the LookAtPlanePoint
        /// </summary>
        private void ApplyAngleRotation()
        {
            if (Mathf.Abs(AngleRotationToApply) <= float.Epsilon)
            {
                // no change, no need to update angle rotation
                return;
            }

            float toApply = Mathf.Lerp(AngleRotationToApply, 0, Time.deltaTime * 5);
            AngleRotationToApply -= toApply;
            transform.RotateAround(LookAtPlanePoint, Vector3.up, toApply);
        }


        /// <summary>
        /// Apply the translation movement of the Camera
        /// </summary>
        private void ApplyDirection()
        {
            if (DirectionToApply.magnitude <= float.Epsilon)
            {
                // no change, no need to update position or rotation
                return;
            }

            var toApply = Vector2.Lerp(DirectionToApply, Vector2.zero, Time.deltaTime * 5);
            DirectionToApply -= toApply;
            transform.position += Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * (new Vector3(DirectionToApply.x, 0, DirectionToApply.y));
        }

        private RaycastHit _hit;
        /// <summary>
        /// Perform a raycast to update LookAtPoint position
        /// </summary>
        private void UpdateLookAtPoint()
        {
            if (_lookAtPointOnPlane.HasValue)
            {
                if (!(Vector3.Distance(_prevPosition, transform.position) > float.Epsilon || Quaternion.Angle(_prevRotation, transform.rotation) > float.Epsilon))
                {
                    // no change, no need to update looking point with raycast
                    return;
                }
            }

            if (PlaneReference.Raycast(_camera.ViewportPointToRay(Vector2.one * 0.5f), out _hit, _camera.farClipPlane))
            {
                _lookAtPointOnPlane = _hit.point;
            }

            _prevPosition = transform.position;
            _prevRotation = transform.rotation;
        }

        /// <summary>
        /// Perform a raycast from the camera
        /// </summary>
        /// <param name="screenPoint">Screen mouse/touch position</param>
        /// <param name="layer">LayerMask to apply on Raycast</param>
        /// <returns>RaycastHit result from the raycast</returns>
        public RaycastHit? PerformRaycast(Vector2 screenPoint, LayerMask? layer = null)
        {
            if (layer.HasValue)
            {
                if (!Physics.Raycast(_camera.ScreenPointToRay(screenPoint), out _hit, _camera.farClipPlane, layer.Value))
                {
                    return null;
                }
            }
            else
            {
                if (!Physics.Raycast(_camera.ScreenPointToRay(screenPoint), out _hit, _camera.farClipPlane))
                {
                    return null;
                }
            }


            return _hit;
        }

        /// <summary>
        /// Perform a raycast from the camera
        /// </summary>
        /// <param name="screenPoint">Screen mouse/touch position</param>
        /// <param name="layer">LayerMask to apply on Raycast</param>
        /// <returns>Contact point of the raycast</returns>
        public Vector3? PerformRaycastPoint(Vector2 screenPoint, LayerMask? layer = null)
        {
            var h = PerformRaycast(screenPoint, layer);
            return h?.point;
        }

        /// <summary>
        /// Get the screen position from world position 
        /// </summary>
        /// <param name="World">World position</param>
        /// <returns>Screen position</returns>
        public Vector2 WorldToScreenPoint(Vector3 World)
        {
            return _camera.WorldToScreenPoint(World);
        }
    }
}