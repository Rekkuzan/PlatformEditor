using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformEditor
{
    /// <summary>
    /// Behaviour that handles UI on the environment scene
    /// </summary>
    public class EnvironmentUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject GeneralUI;
        [SerializeField]
        private GameObject EditingUI;
        [SerializeField]
        private GameObject SavingUI;

        private void OnEnable()
        {
            ObjectSelectionTransform.OnObjectStartEditing += OnStartEditingItem;
            ObjectSelectionTransform.OnObjectStopEditing += OnStopEditingItem;
            OnStopEditingItem();
        }

        private void OnDisable()
        {
            ObjectSelectionTransform.OnObjectStartEditing -= OnStartEditingItem;
            ObjectSelectionTransform.OnObjectStopEditing -= OnStopEditingItem;
        }

        /// <summary>
        /// Single runtime Item stop from being edited
        /// </summary>
        private void OnStopEditingItem()
        {
            GeneralUI.SetActive(true);
            EditingUI.SetActive(false);
        }

        /// <summary>
        /// Start editing single runtime Item
        /// </summary>
        private void OnStartEditingItem()
        {
            GeneralUI.SetActive(false);
            EditingUI.SetActive(true);
        }

        /// <summary>
        /// Request saving the scene to disk
        /// </summary>
        public void RequestSaveScene()
        {
            SavingUI.SetActive(true);
        }
    }
}