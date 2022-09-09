using PlatformEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformEditor
{
    /// <summary>
    /// Handle the selection of runtimeItem/assetdata on the scene
    /// </summary>
    public class ObjectSelectionTransform : SingletonMonobehaviour<ObjectSelectionTransform>
    {
        public enum State
        {
            NoSelection,
            DataSelected,
            ItemSelected
        }

        public AssetData CurrentSelectedData { get; private set; }
        public GameObject CurrentSelectedItem { get; private set; }
        public State CurrentState { get; private set; }

        [SerializeField]
        private SelectionTransformHelper Helper;

        public static event System.Action OnObjectStartEditing;
        public static event System.Action OnObjectStopEditing;

        private void OnEnable()
        {
            CreateAction.OnCreateEvent += OnCreateItem;
            SetHelperEnable(false);
        }

        private void OnDisable()
        {
            CreateAction.OnCreateEvent -= OnCreateItem;
        }

        private void Update()
        {
            switch (CurrentState)
            {
                case State.NoSelection:
                    UpdateNoSelection();
                    break;
                case State.DataSelected:
                    UpdateDataSelected();
                    break;
                case State.ItemSelected:
                    UpdateItemSelected();
                    break;
            }
        }

        /// <summary>
        /// Current data will selected and put into DataSelected state
        /// </summary>
        /// <param name="data"></param>
        public void SelectNewItem(AssetData data)
        {
            CurrentSelectedData = data;
            SetState(State.DataSelected);
        }

        /// <summary>
        /// If user input, check if trying to select an item already instantiated
        /// </summary>
        private void UpdateNoSelection()
        {
            if (InputTouch.TouchCount != 1 || InputTouch.IsOverUI)
                return;

            if (InputTouch.GetTouchByIndex(0).phase != TouchPhase.Began)
                return;

            var hit = CameraHandler.Instance.PerformRaycast(InputTouch.GetTouchByIndex(0).position, CameraHandler.Instance.ItemMask);

            if (!hit.HasValue)
                return;

            var item = hit.Value.collider.GetComponent<RuntimeItem>();
            if (item == null)
            {
                item = hit.Value.collider.GetComponentInParent<RuntimeItem>();
                if (item == null)
                    return;
            }

            CurrentSelectedData = item.Data;
            CurrentSelectedItem = item.gameObject;
            SetState(State.ItemSelected);
        }

        /// <summary>
        /// User invited to click on the env to generate the item based on the data selected
        /// Waiting for input to instantiate and place item
        /// </summary>
        private void UpdateDataSelected()
        {
            if (InputTouch.TouchCount == 0 || InputTouch.IsOverUI)
                return;

            if (InputTouch.GetTouchByIndex(0).phase == TouchPhase.Ended)
            {
                var newPos = CameraHandler.Instance.PerformRaycastPoint(InputTouch.GetTouchByIndex(0).position, CameraHandler.Instance.EnvMask);

                if (!newPos.HasValue)
                    return;

                SetState(State.NoSelection);

                var action = CreateAction.Create(CurrentSelectedData, newPos.Value);
                action.Apply();
            }
        }

        /// <summary>
        /// User can perform : Translation, Rotation, Scale, Delete on the selected item
        /// </summary>
        private void UpdateItemSelected()
        {
            if (CurrentSelectedItem == null)
                return;

            if (InputTouch.TouchCount > 0 && InputTouch.GetTouchByIndex(0).phase == TouchPhase.Began && !InputTouch.IsOverUI)
            {
                if (Helper.CurrentState == SelectionTransformHelper.State.None)
                {
                    var hit = CameraHandler.Instance.PerformRaycast(InputTouch.GetTouchByIndex(0).position, CameraHandler.Instance.ItemMask);

                    if (hit.HasValue)
                    {
                        /*
                        var runtimeItemClicked = hit.Value.collider.GetComponent<RuntimeItem>();
                        if (runtimeItemClicked == null)
                            runtimeItemClicked = hit.Value.collider.GetComponentInParent<RuntimeItem>();

                        // clicked on the void or other item
                        if (runtimeItemClicked == null || runtimeItemClicked.gameObject != this.CurrentSelectedItem)
                        {
                            Debug.Log("NO SELECTION BECAUSE NOT SAME ITEM OR NULL");
                            SetState(State.NoSelection);
                        }
                        // clicked on the same item => moving it
                        else if (runtimeItemClicked.gameObject == this.CurrentSelectedItem)
                            Helper.SetState(SelectionTransformHelper.State.Translate);*/
                        return;
                    }

                    SetState(State.NoSelection);
                }
            }
        }

        /// <summary>
        /// Set new state and apply/invoke events related
        /// </summary>
        /// <param name="state"></param>
        private void SetState(State state)
        {
            switch (state)
            {
                case State.NoSelection:
                    SetItemSelected(false);
                    SetHelperEnable(false);
                    OnObjectStopEditing?.Invoke();
                    break;
                case State.DataSelected:
                    SetItemSelected(false);
                    SetHelperEnable(false);
                    break;
                case State.ItemSelected:
                    SetItemSelected(true);
                    SetHelperEnable(true);
                    OnObjectStartEditing?.Invoke();
                    break;
            }

            CurrentState = state;
        }

        /// <summary>
        /// Enable TransformHelper 
        /// </summary>
        /// <param name="enable"></param>
        private void SetHelperEnable(bool enable)
        {
            if (enable)
                Helper.ObjectTransform = CurrentSelectedItem.transform;
            else
                Helper.ObjectTransform = null;
        }

        /// <summary>
        /// Apply correct layer on item selected/unselected
        /// </summary>
        /// <param name="selected"></param>
        private void SetItemSelected(bool selected)
        {
            if (CurrentSelectedItem)
                CurrentSelectedItem.layer = selected ? 7 : 6;
            if (!selected)
                CurrentSelectedItem = null;
        }

        /// <summary>
        /// On item created event, saving references and changing state
        /// </summary>
        /// <param name="obj"></param>
        private void OnCreateItem(GameObject obj)
        {
            var runtimeItem = obj.GetComponent<RuntimeItem>();

            if (runtimeItem == null)
                return;

            CurrentSelectedData = runtimeItem.Data;
            CurrentSelectedItem = runtimeItem.gameObject;
            SetState(State.ItemSelected);
        }

        /// <summary>
        /// Delete the current item and switch back to NoSelectionState
        /// </summary>
        public void DeleteItem()
        {
            if (CurrentSelectedItem == null)
                return;

            var deleteAction = DeleteAction.Create(CurrentSelectedData, CurrentSelectedItem.GetComponent<RuntimeItem>());
            deleteAction.Apply();

            SetState(State.NoSelection);
        }
    }
}