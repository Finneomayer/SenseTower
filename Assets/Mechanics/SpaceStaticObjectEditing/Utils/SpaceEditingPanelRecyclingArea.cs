using System.Collections.Generic;
using Mechanics.SpaceStaticObjectEditing.Model;
using Mechanics.SpaceStaticObjectEditing.UI;
using UI;
using UnityEngine;

namespace Mechanics.SpaceStaticObjectEditing.Utils
{
    public class SpaceEditingPanelRecyclingArea : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private SpaceEditingPanel _spaceEditingPanel;
        [SerializeField] private MaterialColorChanger _materialColorChanger;
        [SerializeField, Range(0f, 1f)] private float MinAlpha = 0;
        [SerializeField, Range(0f, 1f)] private float MaxAlpha = 0.2f;

        #endregion

        [SerializeField] private List<SpaceStaticObjectModel> _objectModels = new();

        private void Start()
        {
            _materialColorChanger.SetAlphaInstantly(MinAlpha);
        }

        private void OnDestroy()
        {
            foreach (SpaceStaticObjectModel spaceStaticObjectModel in _objectModels)
            {
                RemoveObject(spaceStaticObjectModel);
            }
        }

        private void LateUpdate()
        {
            _materialColorChanger.SetAlphaInstantly( MinAlpha);

            for (int i = 0; i < _objectModels.Count; i++)
            {
                SpaceStaticObjectModel tempObjectModel = _objectModels[i];

                if (tempObjectModel == null)
                {
                    _objectModels.RemoveAt(i);
                    continue;
                }

                if (tempObjectModel.HitTheGround)
                    return;
                
                _materialColorChanger.SetAlphaInstantly(_objectModels.Count > 0 ? MaxAlpha : MinAlpha);

                //TODO change this after full realization
                if (tempObjectModel.TriggerGrabInteractable != null)
                {
                    if (tempObjectModel.RightTriggerGrabInteractable != null)
                    {
                        if (!tempObjectModel.TriggerGrabInteractable.IsGrabbing &&
                            !tempObjectModel.RightTriggerGrabInteractable.IsGrabbing)
                        {
                            _spaceEditingPanel.TryDestroyObject(tempObjectModel);
                            RemoveObject(tempObjectModel);
                        }
                    }
                    else
                    {
                        if (!tempObjectModel.TriggerGrabInteractable.IsGrabbing)
                        {
                            _spaceEditingPanel.TryDestroyObject(tempObjectModel);
                            RemoveObject(tempObjectModel);
                        }
                    }
                }
                else
                {
                    _spaceEditingPanel.TryDestroyObject(tempObjectModel);
                    RemoveObject(tempObjectModel);
                }
            }

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out SpaceStaticObjectModel spaceStaticObjectModel))
            {
                AddObject(spaceStaticObjectModel);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out SpaceStaticObjectModel spaceStaticObjectModel))
            {
                RemoveObject(spaceStaticObjectModel);
            }
        }

        public void ForseDisable()
        {
            _objectModels.Clear();
        }

        private void AddObject(SpaceStaticObjectModel spaceStaticObjectModel)
        {
            _objectModels.Add(spaceStaticObjectModel);
        }

        private void RemoveObject(SpaceStaticObjectModel spaceStaticObjectModel)
        {
            if (_objectModels.Contains(spaceStaticObjectModel))
            {
                _objectModels.Remove(spaceStaticObjectModel);
            }
        }
    }
}