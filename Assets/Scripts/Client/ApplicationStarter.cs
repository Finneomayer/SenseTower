using Assets.Scripts.Space;
using UnityEngine;

namespace Assets.Scripts.Client
{
    public class ApplicationStarter : MonoBehaviour
    {
        private const string ApplicationStarterTag = "ApplicationStarter";

        private void Awake()
        {
            Debug.Log($"{ApplicationStarterTag}: Awake");
            if (GameObject.FindGameObjectsWithTag(ApplicationStarterTag).Length > 1)
            {
                Debug.Log($"{ApplicationStarterTag} double creation" + " " + GameObject.FindGameObjectsWithTag(ApplicationStarterTag).Length);
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            //Debug.Log($"{ApplicationStarterTag}: Start");
            //switch (StaticStateKeeper.CurrentSpace)
            //{
            //    case SpaceType.Null:
            //        ClientGameStateManager.ClientState.Initialize(ClientGameStateManager.StateEnter);
            //        break;
            //    case SpaceType.EnterScene:
            //        ClientGameStateManager.ClientState.Initialize(ClientGameStateManager.StateEnter);
            //        break;
            //    case SpaceType.HallScene:
            //        ClientGameStateManager.ClientState.Initialize(ClientGameStateManager.StateAuthorized);
            //        break;
            //    case SpaceType.MyPlace:
            //        ClientGameStateManager.ClientState.Initialize(ClientGameStateManager.StateInRoom);
            //        break;
            //    case SpaceType.LectureScene:
            //        ClientGameStateManager.ClientState.Initialize(ClientGameStateManager.StateInRoom);
            //        break;
            //    case SpaceType.StandupScene:
            //        ClientGameStateManager.ClientState.Initialize(ClientGameStateManager.StateInRoom);
            //        break;
            //    case SpaceType.LoadingScene:
            //        ClientGameStateManager.ClientState.Initialize(ClientGameStateManager.StateEnter);
            //        break;
            //}
        }
    }
}