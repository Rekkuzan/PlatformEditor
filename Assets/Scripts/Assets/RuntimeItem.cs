using PlatformEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformEditor
{
    /// <summary>
    /// Applied on every instantiated gameObject to keep track and hold reference to data
    /// </summary>
    public class RuntimeItem : MonoBehaviour
    {
        public AssetData Data;

        public BoxCollider BoundingBoxCollider { get; private set; }

        [SerializeField]
        private Transform RotationParent;

        private void OnEnable()
        {
            BoundingBoxCollider = GetComponent<BoxCollider>();
        }

        /// <summary>
        /// Get a serialized version of the runtime object
        /// </summary>
        /// <returns></returns>
        public SerializedItem GetSerializedItem()
        {
            return new SerializedItem()
            {
                AssetNameId = Data.NameId,
                Position = transform.position,
                Rotation = RotationParent.rotation.eulerAngles,
                Scale = transform.localScale
            };
        }

#if UNITY_EDITOR
        private static readonly List<Renderer> _Renderer = new List<Renderer>();
        /// <summary>
        /// Editor Only - Assign gameObject to Asset Prefab and remove all colliders
        /// </summary>
        /// <param name="item"></param>
        public void AssignObject(GameObject item)
        {
            _Renderer.Clear();
            item.GetComponentsInChildren(_Renderer);
            _Renderer.AddRange(item.GetComponents<Renderer>());

            Bounds bounds = _Renderer[0].bounds;
            foreach (Renderer rend in _Renderer)
            {
                bounds.Encapsulate(rend.bounds);
            }
            Vector3 center = bounds.center;
            BoundingBoxCollider.center = center;
            BoundingBoxCollider.size = bounds.size;

            var collider = item.GetComponent<Collider>();
            if (collider)
                DestroyImmediate(collider);
        }

#endif

        /// <summary>
        /// Get rotation of runtimeItem
        /// </summary>
        /// <returns></returns>
        public Quaternion GetRotation()
        {
            return RotationParent.rotation;
        }

        /// <summary>
        /// Get position of runtimeItem
        /// </summary>
        /// <returns></returns>
        public Vector3 GetPosition()
        {
            return transform.position;
        }

        /// <summary>
        /// Get LocalScale of runtimeItem
        /// </summary>
        /// <returns></returns>
        public Vector3 GetScale()
        {
            return transform.localScale;
        }

        /// <summary>
        /// Apply position to rutimeItem
        /// </summary>
        /// <param name="position"></param>
        public void ApplyPosition(Vector3 position)
        {
            transform.position = position;
        }

        /// <summary>
        /// Apply localScale to runtimeItem
        /// </summary>
        /// <param name="scale"></param>
        public void ApplyScale(Vector3 scale)
        {
            transform.localScale = scale;
        }

        /// <summary>
        /// Apply rotation to runtimeItem
        /// </summary>
        /// <param name="rotation"></param>
        public void ApplyRotation(Quaternion rotation)
        {
            RotationParent.rotation = rotation;
        }

        /// <summary>
        /// Return the center of the runtimeItem
        /// </summary>
        /// <returns></returns>
        public Vector3 GetCenterPosition()
        {
            if (!BoundingBoxCollider)
                return transform.position;
            return transform.position + transform.rotation * (BoundingBoxCollider.center * transform.localScale.x);
        }


        /// <summary>
        /// Return the bottom of the runtimeItem
        /// </summary>
        /// <returns></returns>
        public Vector3 GetBottomPoint()
        {
            return transform.position;
        }

        /// <summary>
        /// Return the right of the runtimeItem
        /// </summary>
        /// <returns></returns>
        public Vector3 GetRightPosition()
        {
            if (!BoundingBoxCollider)
                return transform.position;
            return transform.position + transform.rotation * ((BoundingBoxCollider.center + 0.5F * BoundingBoxCollider.size.x * Vector3.right) * transform.localScale.x);
        }
    }
}