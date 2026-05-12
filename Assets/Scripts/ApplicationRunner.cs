using System;
using Assets.Scripts;
using Assets.Scripts.Server;
using Client;
using UnityEngine;
using Zenject;

public class ApplicationRunner : MonoBehaviour
{
    public ApplicationBootstrapper BootstrapperPrefab;
    private DiContainer _diContainer;

    public ApplicationBootstrapper _applicationBootstrapperInstance { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        var bootstrapper = FindObjectOfType<ApplicationBootstrapper>();
        if (bootstrapper != null)
        {
            _applicationBootstrapperInstance = bootstrapper;
            return;
        }
        
        Init();
    }

    private void Start()
    {
        if(_applicationBootstrapperInstance != null)
            _applicationBootstrapperInstance.InitApplication();
    }

    [Inject]
    public void Construct(DiContainer diContainer)
    {
        _diContainer = diContainer;
    }

    private void Init()
    {
        GameObject tempPrefab = _diContainer.InstantiatePrefab(BootstrapperPrefab);
        _applicationBootstrapperInstance = tempPrefab.GetComponent<ApplicationBootstrapper>();
    }
}
