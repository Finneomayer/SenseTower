using Assets.Scripts.Data;
using Mechanics.LoadSceneObjects.Models;
using UnityEngine;

namespace Mechanics.SpaceStaticObjectEditing.Interaction
{
    public class ObjectScalling : MonoBehaviour
    {
        [SerializeField] private Transform _leftTopCorner;
        [SerializeField] private Transform _rightDownCorner;
        [SerializeField] private Transform _objectGraphcs;

        Vector3 ball1XZPosition;
        Vector3 ball2XZPosition;

        float InitialDistXZ;
        float InitialDistY;
        Vector3 InitialScale;

        private void Start()
        {
            _objectGraphcs.transform.rotation = Quaternion.identity;
            CalculateFirstPosition();
            ball1XZPosition = new Vector3(_leftTopCorner.position.x, 0, _leftTopCorner.position.z);
            ball2XZPosition = new Vector3(_rightDownCorner.position.x, 0, _rightDownCorner.position.z);

            InitialDistXZ = Vector3.Distance(ball1XZPosition, ball2XZPosition);
            InitialDistY = _leftTopCorner.position.y - _rightDownCorner.position.y;
            InitialScale = _objectGraphcs.localScale;
        }

        private void Update()
        {
            _objectGraphcs.transform.rotation = Quaternion.identity;

            CalculateFirstPosition();
        }

        private void CalculateFirstPosition()
        {
            StaticObject scalableObject = GetScalableObject();
            
            _objectGraphcs.position = DataExtensions.CalculateMiddlePosition(scalableObject);
            _objectGraphcs.transform.rotation =
                DataExtensions.CalculateRotation(_objectGraphcs.transform, scalableObject);
            
            ball1XZPosition = new Vector3(_leftTopCorner.position.x, 0, _leftTopCorner.position.z);
            ball2XZPosition = new Vector3(_rightDownCorner.position.x, 0, _rightDownCorner.position.z);
            
            float CurrentDistXZ = Vector3.Distance(ball1XZPosition, ball2XZPosition);
            float CurrentDistY = _leftTopCorner.position.y - _rightDownCorner.position.y;
            
            Debug.Log($"{_leftTopCorner.position.y - _rightDownCorner.position.y} is scale Y: {CurrentDistY}, scale X: {CurrentDistXZ}");
            _objectGraphcs.localScale = new Vector3(_objectGraphcs.localScale.x, CurrentDistY,CurrentDistXZ );

        }

        private void CalculateTransform()
        {
            ball1XZPosition = new Vector3(_leftTopCorner.position.x, 0, _leftTopCorner.position.z);
            ball2XZPosition = new Vector3(_rightDownCorner.position.x, 0, _rightDownCorner.position.z);

            float CurrentDistXZ = Vector3.Distance(ball1XZPosition, ball2XZPosition);
            float CurrentDistY = _leftTopCorner.position.y - _rightDownCorner.position.y;

            float scaleZ = InitialScale.z * CurrentDistXZ / InitialDistXZ;
            float scaleY = InitialScale.y * CurrentDistY / InitialDistY;

            _objectGraphcs.localScale = new Vector3(_objectGraphcs.localScale.x, scaleY, scaleZ);

            StaticObject scalableObject = GetScalableObject();

            _objectGraphcs.position = DataExtensions.CalculateMiddlePosition(scalableObject);
            _objectGraphcs.transform.rotation =
                DataExtensions.CalculateRotation(_objectGraphcs.transform, scalableObject);
        }

        private StaticObject GetScalableObject()
        {
            StaticObject scalableObject = new();
            scalableObject.LeftTopX = _leftTopCorner.position.x;
            scalableObject.LeftTopY = _leftTopCorner.position.y;
            scalableObject.LeftTopZ = _leftTopCorner.position.z;

            scalableObject.RightDownX = _rightDownCorner.position.x;
            scalableObject.RightDownY = _rightDownCorner.position.y;
            scalableObject.RightDownZ = _rightDownCorner.position.z;

            return scalableObject;
        }
    }
}