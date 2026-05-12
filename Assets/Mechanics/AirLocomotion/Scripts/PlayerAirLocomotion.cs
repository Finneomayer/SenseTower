using System.Collections;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Mechanics.AirLocomotion.Scripts
{
    public class PlayerAirLocomotion : MonoBehaviour
    {
        [SerializeField]
        private PlayerLogic Player;
        [SerializeField]
        private AirLocomotionHand[] AirLocomotionHands;

        private Coroutine _airLocomotionRoutine;
        private AirLocomotionHand _currentLocomotionHand;

        private void OnEnable()
        {
            foreach (var item in AirLocomotionHands)
            {
                item.AirLocomotionStarted += HandAirLocomotionStarted;
                item.AirLocomotionStopped += HandAirLocomotionStopped;
            }
        }

        private void OnDisable()
        {
            foreach (var item in AirLocomotionHands)
            {
                item.AirLocomotionStarted -= HandAirLocomotionStarted;
                item.AirLocomotionStopped -= HandAirLocomotionStopped;
            }
            HandAirLocomotionStopped(_currentLocomotionHand);
        }

        private void HandAirLocomotionStarted(AirLocomotionHand hand)
        {
            if (_airLocomotionRoutine != null)
            {
                StopCoroutine(_airLocomotionRoutine);
            }
            _currentLocomotionHand = hand;
            _airLocomotionRoutine = StartCoroutine(AirLocomotionRoutine(hand));
        }

        private void HandAirLocomotionStopped(AirLocomotionHand hand)
        {
            if (hand != _currentLocomotionHand)
            {
                return;
            }
            if (_airLocomotionRoutine != null)
            {
                StopCoroutine(_airLocomotionRoutine);
                _airLocomotionRoutine = null;
            }
        }

        private IEnumerator AirLocomotionRoutine(AirLocomotionHand hand)
        {
            Vector3 previousHandPosition = hand.CurrentPosition;
            while (true)
            {
                yield return new WaitForFixedUpdate();

                Vector3 currentHandDelta = hand.CurrentPosition - previousHandPosition;
                Player.transform.position -= currentHandDelta;

                previousHandPosition = hand.CurrentPosition;
            }
        }
    }
}
