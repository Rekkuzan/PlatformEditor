using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformEditor
{
    /// <summary>
    /// Behaviour to handle transform fonctions of the selected item
    /// Create an IAction based on the input
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class SelectionTransformHelper : MonoBehaviour
    {
        public enum State
        {
            None = 0,
            RotationY,
            RotationX,
            Scale,
            Translate,
        }

        [Header("Settings")]
        [SerializeField]
        private float RotationYMaxDistance360 = 0.6f;
        [SerializeField]
        private float RotationXMaxDistance360 = 0.6f;
        [SerializeField]
        private float ScaleDistanceFactor = 6;
        [SerializeField]
        private Vector2 MinMaxScale = new Vector2(0.25f, 3.0F);

        [Header("Debug")]
        [SerializeField]
        private State _state = State.None;
        public State CurrentState { get { return _state; } }

        private Transform _objectTransform;
        public Transform ObjectTransform
        {
            get { return _objectTransform; }
            set
            {
                _objectTransform = value;
                _runtimeItem = _objectTransform ? _objectTransform.GetComponent<RuntimeItem>() : null;
            }
        }

        [SerializeField]
        private RuntimeItem _runtimeItem;

        private RectTransform _rectTransform;
        private Vector2 _startScreenPos;
        private Vector2 _deltaTotalScreenPos;

        private Vector3 _startPosition;
        private Quaternion _startingRotation;
        private Vector3 _startingScale;


        private void OnEnable()
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
            _state = State.None;
        }

        private void Update()
        {
            if (ObjectTransform == null)
                return;

            EnsureRuntimeItemIsFetched();
            UIFollowingTransform();

            if (InputTouch.TouchCount == 0 || InputTouch.GetTouchByIndex(0).phase == TouchPhase.Ended)
            {
                if (CurrentState != State.None)
                    SetState(State.None);
                return;
            }

            CalculateDelaPositionScreen();

            // Ensure to avoid unexpected behaviour with value very close to 0
            if (_deltaTotalScreenPos.magnitude < 0.01f)
                return;

            switch (_state)
            {
                case State.None:
                    break;
                case State.RotationY:
                    ApplyRotationYAxis();
                    break;
                case State.Scale:
                    ApplyScale();
                    break;
                case State.RotationX:
                    ApplyRotationXAxis();
                    break;
                case State.Translate:
                    ApplyTranslation();
                    break;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (ObjectTransform == null)
                return;

            if (_runtimeItem == null)
                _runtimeItem = ObjectTransform.gameObject.GetComponent<RuntimeItem>();
            var centerItem = _runtimeItem.GetCenterPosition();

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(centerItem, .2f);
        }
#endif

        /// <summary>
        /// Ensure runtimeItem is not null and not different from ObjectTransform
        /// </summary>
        private void EnsureRuntimeItemIsFetched()
        {
            if (_runtimeItem == null || _runtimeItem.transform != ObjectTransform)
                _runtimeItem = ObjectTransform.gameObject.GetComponent<RuntimeItem>();
        }

        /// <summary>
        /// UI following the 3D transform on screen
        /// </summary>
        private void UIFollowingTransform()
        {
            var viewport = CameraHandler.Instance.WorldToScreenPoint(_runtimeItem.GetCenterPosition());
            _rectTransform.position = viewport;

            // UI fit more the size of the item
            var bottom = CameraHandler.Instance.WorldToScreenPoint(_runtimeItem.GetBottomPoint());
            var right = CameraHandler.Instance.WorldToScreenPoint(_runtimeItem.GetRightPosition());

            var sizeDelta = _rectTransform.sizeDelta;
            sizeDelta.y = Mathf.Max(Mathf.Abs(viewport.y - bottom.y) * 2, 550);
            sizeDelta.x = Mathf.Max(Mathf.Abs(viewport.x - right.x) * 2, 550);

            _rectTransform.sizeDelta = sizeDelta;
        }

        /// <summary>
        /// Calculate the total delta position on screen
        /// </summary>
        private void CalculateDelaPositionScreen()
        {
            _deltaTotalScreenPos = _startScreenPos - InputTouch.GetTouchByIndex(0).position;

            // Make sure this is percentage of screen and not pixel
            _deltaTotalScreenPos.x /= Screen.width;
            _deltaTotalScreenPos.y /= Screen.height;
        }

        /// <summary>
        /// Apply rotation on X axis based on delta position on screen
        /// </summary>
        private void ApplyRotationXAxis()
        {
            var cameraXaxis = CameraHandler.Instance.transform.right;
            _runtimeItem.ApplyRotation(Quaternion.AngleAxis(-_deltaTotalScreenPos.y * (360.0F / RotationXMaxDistance360), cameraXaxis) * _startingRotation);
        }

        /// <summary>
        /// Apply rotation on Y axis based on delta position on screen
        /// </summary>
        private void ApplyRotationYAxis()
        {
            _runtimeItem.ApplyRotation(Quaternion.AngleAxis(_deltaTotalScreenPos.x * (360.0F / RotationYMaxDistance360), Vector3.up) * _startingRotation);
        }

        /// <summary>
        /// Apply the scale based on delta position on screen
        /// </summary>
        private void ApplyScale()
        {
            var valueScreen = _deltaTotalScreenPos.x + _deltaTotalScreenPos.y;
            var value = _startingScale.x - valueScreen * ScaleDistanceFactor;
            value = Mathf.Clamp(value, MinMaxScale.x, MinMaxScale.y);

            _runtimeItem.ApplyScale(Vector3.one * value);
        }

        /// <summary>
        /// Apply new transformation to runtimeItem by raycasting on the scene
        /// </summary>
        private void ApplyTranslation()
        {
            var newPos = CameraHandler.Instance.PerformRaycastPoint(InputTouch.GetTouchByIndex(0).position, CameraHandler.Instance.EnvMask);

            if (!newPos.HasValue)
                return;

            _runtimeItem.ApplyPosition(newPos.Value);
        }

        /// <summary>
        /// Will set a new state to the helper, creating an IAction and saving the state of ObjectTransform
        /// </summary>
        /// <param name="state"></param>
        public void SetState(State state)
        {
            if (_state == state)
                return;

            if (_state != State.None)
            {
                CreateActionBasedFromState(_state);
            }

            _state = state;
            _startScreenPos = InputTouch.GetTouchByIndex(0).position;
            _startPosition = _runtimeItem.GetPosition();
            _startingRotation = _runtimeItem.GetRotation();
            _startingScale = _runtimeItem.GetScale();
        }

        /// <summary>
        /// Will create an action based on the state given
        /// </summary>
        /// <param name="state"></param>
        private void CreateActionBasedFromState(State state)
        {
            IAction action = null;
            switch (state)
            {
                case State.None:
                    break;
                case State.RotationX:
                case State.RotationY:
                    action = RotateAction.Create(_runtimeItem, _startingRotation, _runtimeItem.GetRotation());
                    break;
                case State.Scale:
                    action = ScaleAction.Create(_runtimeItem, _startingScale, _runtimeItem.GetScale());
                    break;
                case State.Translate:
                    action = TranslateAction.Create(_runtimeItem, _startPosition, _runtimeItem.GetPosition());
                    break;
            }

            action?.Apply();
        }

        public void StartRotationYLocalAxis()
        {
            SetState(State.RotationY);
        }

        public void StartRotationXWorldAxis()
        {
            SetState(State.RotationX);
        }

        public void StartScaling()
        {
            SetState(State.Scale);
        }

        public void StartTranslate()
        {
            SetState(State.Translate);
        }
    }
}