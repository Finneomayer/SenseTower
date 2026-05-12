using System.Collections;
using Assets.Scripts.Player;
using UnityEngine;

public class PlacePositionCorrector: MonoBehaviour
{
    [SerializeField] 
    private Place Place;

    private Coroutine _playerPositionCorrectionRoutine;

    private void Awake()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            enabled = false;
        }
    }

    private void OnEnable()
    {
        Place.OccupiedByLocalPlayer += OnPlaceOccupiedByLocalPlayer;
        Place.UnoccupiedByLocalPlayer += OnPlaceUnoccupiedByLocalPlayer;
    }

    private void OnDisable()
    {
        Place.OccupiedByLocalPlayer -= OnPlaceOccupiedByLocalPlayer;
        Place.UnoccupiedByLocalPlayer -= OnPlaceUnoccupiedByLocalPlayer;
    }

    private void OnPlaceOccupiedByLocalPlayer(PlayerLogic player)
    {
        if (_playerPositionCorrectionRoutine != null)
        {
            StopCoroutine(_playerPositionCorrectionRoutine);
        }
        _playerPositionCorrectionRoutine = StartCoroutine(PlayerPositionCorrection(player));
    }

    private void OnPlaceUnoccupiedByLocalPlayer()
    {
        if (_playerPositionCorrectionRoutine != null)
        {
            StopCoroutine(_playerPositionCorrectionRoutine);
            _playerPositionCorrectionRoutine = null;
        }
    }

    private IEnumerator PlayerPositionCorrection(PlayerLogic player)
    {
        const float PlaceHeight = 1.15f;
        const float PlaceMinHeight = 1f;
        const float PlaceMaxHeight = 1.3f;
        const float MaxSqrDistanceFromPlace = 9;

        if (player == null)
        {
            Debug.LogWarning("PlacePositionCorrection: player == null. Place position correction disabled");
            _playerPositionCorrectionRoutine = null;
            yield break;
        }
        Vector3 anchorPosition = Place.TeleportAnchor.TeleportAnchorTransform.position;
        Transform camera = Camera.main.transform;

        yield return null;
        player.SetY(-camera.localPosition.y + anchorPosition.y + PlaceHeight);

        while (true)
        {
            yield return new WaitForFixedUpdate();

            if (player == null || Place == null || Place.TeleportAnchor == null 
                || Place.TeleportAnchor.TeleportAnchorTransform == null)
            {
                _playerPositionCorrectionRoutine = null;
                yield break;
            }

            anchorPosition = Place.TeleportAnchor.TeleportAnchorTransform.position;

            if (camera.position.y < anchorPosition.y + PlaceMinHeight)
            {
                player.SetY(-camera.localPosition.y + anchorPosition.y + PlaceMinHeight);
            }
            else if (camera.position.y > anchorPosition.y + PlaceMaxHeight)
            {
                player.SetY(-camera.localPosition.y + anchorPosition.y + PlaceMaxHeight);
            }

            if (Vector3.SqrMagnitude(camera.position - anchorPosition) > MaxSqrDistanceFromPlace)
            {
                Debug.LogWarning("Returning player to place anchorPosition");
                player.transform.position = anchorPosition;
                player.transform.rotation = Place.TeleportAnchor.TeleportAnchorTransform.rotation;
            }
        }
    }
}
