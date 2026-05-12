using UnityEngine;

namespace Assets.Blackboard
{
    public class MarkerMovement
    {
        private BlackBoardMarker _marker;
        private BlackBoard _blackBoard;
        private Transform _markerHandAnchor;

        public void Init(BlackBoardMarker marker, Transform markerHandAnchor, BlackBoard blackBoard)
        {
            _blackBoard = blackBoard;
            _marker = marker;
            _markerHandAnchor = markerHandAnchor;
            ResetMarkerPosition();
        }

        public void RefreshPosition()
        {
            ResetMarkerPosition();

            if (!_marker.IsDrawing)
            {
                return;
            }

            if (Vector3.Angle(-_marker.Tip.up, _blackBoard.BlackboardPlane.normal) > 80)
            {
                return;
            }

            Vector3 tipProjection = _blackBoard.GetPointOnBlackBoard(_marker.Tip.position);
            Vector3 correctVector = tipProjection - _marker.Tip.position;

            if (Vector3.Dot(correctVector, _blackBoard.BlackboardPlane.normal) < 0
                && Vector3.Magnitude(correctVector) > 0.01f)
            {
                return;
            }

            Vector3 correctedMarkerPosition = _marker.transform.position + correctVector;

            _marker.transform.position = correctedMarkerPosition;
        }

        private void ResetMarkerPosition()
        {
            _marker.transform.SetPositionAndRotation(_markerHandAnchor.position, _markerHandAnchor.rotation);
        }

    }
}
