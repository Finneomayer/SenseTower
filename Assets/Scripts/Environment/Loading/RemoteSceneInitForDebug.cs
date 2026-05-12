using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RemoteSceneInitForDebug : MonoBehaviour
{
    void Awake()
    {
        SceneManager.LoadSceneAsync("InfrastructureScene", LoadSceneMode.Additive);
    }
}
