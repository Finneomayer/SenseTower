using System;
using Assets.Mechanics.Network.Scripts;
using Assets.Scripts.Client;
using Assets.Scripts.Space;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Assets.Scripts.Shared
{
    public class NetworkStarter :  MonoBehaviour
    {
        [SerializeField] private bool _startDelayEnabled = false;
        [SerializeField] private NetworkStartInvoker _invoker = null;

        private IClientData _clientData;
        private ISpaceManager _spaceManager;
        private const string SenseTowerSettingsPortKey = "Port";
        
        [Inject]
        public void Init(IClientData clientData)
        {
            _clientData = clientData;
        }

        [Inject]
        public void Init(ISpaceManager spaceManager)
        {
            _spaceManager = spaceManager;
        }

        private void Awake()
        {
            NetworkStarter[] oldStarters = FindObjectsOfType<NetworkStarter>();
            foreach (var item in oldStarters)
            {
                if (item != this)
                {
                    Destroy(item.gameObject);
                }
            }
        }

        private void Start()
        {
            Init();
        }
        
        public void Init()
        {
            NetworkManager.Singleton.NetworkConfig.EnableNetworkLogs = true;

            var transport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
            transport.ConnectionData.Port = Port;
            transport.ConnectionData.Address = UnityServerIp;

            if (IsServer)
            {
                StartServer(transport);
            }
            else
            {
                if (_startDelayEnabled && _invoker != null)
                {
                    if (_invoker.NetworkStartRequested)
                    {
                        StartClient(transport);
                    }
                    else
                    {
                        Debug.LogWarning("Waiting for Network Start invoker...");
                        _invoker.OnStartNetwork += () =>
                        {
                            StartClient(transport);
                        };
                    }
                }
                else
                {
                    StartClient(transport);
                }
            }
        }


        private void StartClient(NetworkTransport transport)
        {
            Debug.Log($"Start client {UnityServerIp}:{Port}");
            NetworkEventsManager.Singleton.OnClientConnectedCallback += (x) => Debug.Log("connected " + x);
            transport.OnTransportEvent += (eventType, clientId, payload, receiveTime) =>
            {
                if (eventType == NetworkEvent.Disconnect)
                {
                    Debug.Log("Disconnected");
                }
            };
            var startResult = NetworkManager.Singleton.StartClient();
            Debug.Log(startResult ? "Client opened successfully" : "Client opening failed");
        }

        private void StartServer(NetworkTransport transport)
        {
            Debug.Log($"Start server {UnityServerIp}:{Port}");
            NetworkManager.Singleton.OnServerStarted += () => Debug.Log($"Server started");
            NetworkEventsManager.Singleton.OnClientConnectedCallback += clientId => Debug.Log($"Client connected {clientId}");
            NetworkEventsManager.Singleton.OnClientDisconnectCallback += clientId => Debug.Log($"Client disconnected {clientId}");

            transport.OnTransportEvent += (eventType, clientId, payload, receiveTime) =>
            {
                if (eventType != NetworkEvent.Data)
                {
                    Debug.Log($"[{DateTime.Now:HH:mm:ss}] Transport event {eventType}: ClientID={clientId}");
                }
            };

            var startResult = NetworkManager.Singleton.StartServer();
            Debug.Log(startResult ? "Server opened successfully" : "Server opening failed");
        }

        private string UnityServerIp
        {
            get
            {
                if (IsServer)
                {
                    return "0.0.0.0";
                }

                var space = _spaceManager.CurrentTransitionTarget;

                if (space?.SpaceConnectionInfo != null)
                {
                    return space.SpaceConnectionInfo.Ip;
                }
                Debug.Log("scene not support networking " + SceneManager.GetActiveScene().name);
                throw new ApplicationException("wrong execution. scene not support networking " + SceneManager.GetActiveScene().name);
            }
        }

        private ushort Port
        {
            get
            {
                if (IsServer)
                {
                    var port = Environment.GetEnvironmentVariable(SenseTowerSettingsPortKey);
                    if (ushort.TryParse(port, out var parsedPort))
                    {
                        return parsedPort;
                    }
                }
                else
                {

                    var space = _spaceManager.CurrentTransitionTarget;

                    if (space?.SpaceConnectionInfo != null)
                    {
                        return (ushort)space.SpaceConnectionInfo.Port;
                    }
                }

                Debug.Log("scene not support networking " + SceneManager.GetActiveScene().name);
                throw new ApplicationException("wrong execution. scene not support networking " + SceneManager.GetActiveScene().name);
            }
        }

        private static bool IsServer
        {
            get
            {
#if UNITY_SERVER
                return true;
#else
                return false;
#endif
            }
        }
    }
}
