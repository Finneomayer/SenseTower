using Unity.Netcode;
using UnityEngine;

public class ClientObjectsDisablerForServer : MonoBehaviour
{
#if UNITY_SERVER    
    private void Start()
    {
        Transform[] gameObjects = GetComponentsInChildren<Transform>();

        //List<MonoBehaviour> networkObjects = new();
        
        //gameObjects.Select(((x) => x.TryGetComponent(out NetworkBehaviour _) && x.TryGetComponent(out NetworkObject _));
        foreach (Transform obj in gameObjects)
        {
            if (!obj.GetComponentInChildren<NetworkObject>() && !obj.GetComponentInChildren<NetworkBehaviour>())
            {
                obj.gameObject.SetActive(false);
            }
            //if (obj.TryGetComponent(out NetworkBehaviour _) || obj.TryGetComponent(out NetworkObject _))
            //{
            //    networkObjects.Add(obj);
            //    //obj.gameObject.SetActive(false);
            //}
            
            //obj.gameObject.SetActive(false);
        }

        //foreach (MonoBehaviour obj in gameObjects) 
        //{
        //    if (!obj.TryGetComponent(out NetworkBehaviour _) && !obj.TryGetComponent(out NetworkObject _))
        //    {
        //        obj.gameObject.SetActive(false);
        //    }
        //}
        //foreach (MonoBehaviour obj in gameObjects)
        //{
        //    if (!obj.TryGetComponent(out NetworkObject _))
        //    {
        //        obj.gameObject.SetActive(false);
        //    }
        //}
    }
#endif
}
