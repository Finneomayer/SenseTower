using System.Linq;
using UnityEngine;

namespace Assets.Mechanics.Mafia
{
    public class PlayerSeatsControllerLocalDebug: MonoBehaviour
    {
    //    [SerializeField]
    //    private PlayerSeat[] PlayerSeats;
    //    [SerializeField]
    //    private PlayerState[] PlayerStates;

    //    private MafiaEventMediatorClient _eventMediator;


    //    private void Awake()
    //    {
    //        _eventMediator = new MafiaEventMediatorClient();
    //        for (int i = 0; i < PlayerSeats.Length; i++)
    //        {
    //            PlayerSeats[i].Init(_eventMediator, PlayerStates[i], true, true);
    //            PlayerSeats[i].SetSelected(false);
    //        }
    //    }

    //    private void OnEnable()
    //    {
    //        _eventMediator.SelectPlayerRequested += _eventMediator_SelectPlayerRequested;
    //    }

    //    private void OnDisable()
    //    {
    //        _eventMediator.SelectPlayerRequested -= _eventMediator_SelectPlayerRequested;
    //    }

    //    private void _eventMediator_SelectPlayerRequested(int seatNumber)
    //    {
    //        PlayerState currentPlayerState = GetCurrentPlayerState();
    //        PlayerState otherPlayerState = GetPlayerState(seatNumber);

    //        currentPlayerState.SelectedNumberOfOtherPlayer = otherPlayerState.Number;
    //        currentPlayerState.SelectedNameOfOtherPlayer = otherPlayerState.PlayerName;
            

    //        for (int i = 0; i < PlayerSeats.Length; i++)
    //        {
    //            PlayerStates[i].VoteCount = GetVoteCount(PlayerStates[i].Number);
    //            PlayerSeats[i].Init(_eventMediator, PlayerStates[i], true, true);
    //            PlayerSeats[i].SetSelected(PlayerStates[i].Number == currentPlayerState.SelectedNumberOfOtherPlayer);
    //        }
    //    }

    //    private PlayerState GetCurrentPlayerState()
    //    {
    //        return null;//PlayerStates.FirstOrDefault(x => x.IsCurrentUser);
    //    }

    //    private PlayerState GetPlayerState(int seatNumber)
    //    {
    //        return PlayerStates.FirstOrDefault(x => x.Number == seatNumber);
    //    }

    //    private int GetVoteCount(int seatNumber)
    //    {
    //        return PlayerStates.Count(x => x.SelectedNumberOfOtherPlayer == seatNumber);
    //    }
    }
}
