using System;
using System.Linq;

namespace Assets.Mechanics.Mafia
{
    public static class GameStateExtension
    {
        public static bool TryGetPlayerStateByName(this GameState gameState, string playerName, out PlayerState playerState)
        {
            playerState = null;
            if (gameState == null || gameState.PlayerStates == null)
            {
                return false;
            }

            playerState = gameState.PlayerStates.FirstOrDefault((x) => x.PlayerName == playerName);
            return playerState != null;
        }

        public static bool TryGetPlayerStateBySeatNumber(this GameState gameState, int seatNumber, out PlayerState playerState)
        {
            playerState = null;
            if (gameState == null || gameState.PlayerStates == null)
            {
                return false;
            }

            playerState = gameState.PlayerStates.FirstOrDefault((x) => x.Number == seatNumber);
            return playerState != null;
        }

        public static bool TryGetPlayerStateByUserId(this GameState gameState, string userId, out PlayerState playerState)
        {
            playerState = null;
            if (gameState == null || gameState.PlayerStates == null)
            {
                return false;
            }

            playerState = gameState.PlayerStates.FirstOrDefault((x) => x.PlayerId == Guid.Parse(userId));
            return playerState != null;
        }

        //public static bool TryGetVotedPlayer(this GameState gameState, int currentPlayer)
        //{

        //}
    }
}
