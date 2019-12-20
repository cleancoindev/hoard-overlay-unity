using UnityEngine;
using System;
using System.Threading.Tasks;

namespace Hoard.MVC.Unity
{
    /// <summary>
    ///   Starts the connection with the blockchain gateway point
    /// </summary>
    public class HoardServiceInitializer : MonoBehaviour
    {
        /// <summary>
        ///   Reference to the hoard configuration
        /// </summary>
        public TextAsset hoardConfig;
        public bool Initialized {get; private set;}

        public static HoardServiceInitializer Instance {get; private set;}

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Two instances of HoardServiceInitalizer");
                Destroy(this);
                return;
            }

            Instance = this;
        }
        public void Start()
        {
            var config = HoardServiceConfig.LoadFromStream(hoardConfig.text);
            var clientURL = ((EthereumClientConfig)config.BCClient).ClientUrl;
            var ethClient = new EthereumClientOptions(new UnityRpcClientAsync(new Uri(clientURL)));
            var options = new HoardServiceOptions(config, ethClient);
            HoardService.Instance.Initialize(options)
                .ContinueGUISynch(AfterInit);
        }

        private void AfterInit(Task t)
        {
            if (t.IsFaulted)
            {
                Debug.LogError("Hoard service failed to initialize! " + t.Exception);
                HoardService.Instance.Shutdown();
                Initialized = false;
                return;
            }
            Initialized = true;
            Debug.Log("Hoard Initialized");
        }

        public void OnDestroy()
        {
            HoardService.Instance.Shutdown();
        }
    }


    [System.Serializable]
    public class WhisperConfig : HoardServiceConfig
    {
        public string WsURL;
    }
}

