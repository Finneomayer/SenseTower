using Assets.Scripts.Player;
using System;
using System.Collections.Generic;
using TMPro;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Mechanics.SpaceStaticObjectEditing.UI
{
    public class SpaceStaticObjectView : MonoBehaviour
    {
        [SerializeField] private BoxCollider ViewCollider;
        [SerializeField] private GameObject TextContent;
        [SerializeField] private TMP_Text ItemNameText;

        private SpaceEditingPlace _spacePlace;
        private IXRInteractor[] _interactors;

        private void Awake()
        {
            SetActiveText(false);
        }

        private void FixedUpdate()
        {
            if (_spacePlace == null)
                return;

            if (_spacePlace.PlaceGrabbable.AreHandsInPlace)
            {
                SetActiveText(true);
                return;
            }

            SetActiveText(XrColliderHovering.IsHovering(_interactors, ViewCollider));
        }

        public void Init(SpaceEditingPlace spacePlace, IXRInteractor[] interactors)
        {
            _spacePlace = spacePlace;
            _interactors = interactors;
        }

        public void DeInit()
        {
            _spacePlace = null;
            _interactors = null;
            SetActiveText(false);
        }

        private void SetActiveText(bool active)
        {
            if (!active)
            {
                TextContent.SetActive(false);
                return;
            }

            if (_spacePlace == null )
            {
                TextContent.SetActive(false);
                return;
            }

            ItemNameText.text = $"{_spacePlace.ItemModel.PrefabObjectType}";
            TextContent.SetActive(true);
        }
    }
}