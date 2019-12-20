using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.Client.RpcMessages;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using PlasmaCore.RPC;

namespace Hoard.MVC.Unity
{
    public class UnityRpcClientAsync : ClientBase, PlasmaCore.RPC.IClient
    {
        private readonly string _url;
        public JsonSerializerSettings JsonSerializerSettings { get; set; }

        public UnityRpcClientAsync(Uri baseUrl)
        {
            this._url = baseUrl.ToString();
            JsonSerializerSettings = UnityDefaultJsonSerializerSettingsFactory.BuildDefaultJsonSerializerSettings();
        }

        /// <summary>
        /// Calls WebRequest using Unity model. Must be called from coroutine on main thread. Returns RpcResult<TResult>
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        private IEnumerator<T> SendRequest<T>(string route, string args) where T : class
        {
            var rpcRequestJson = args;
            var requestBytes = Encoding.UTF8.GetBytes(rpcRequestJson);
            var unityRequest = new UnityWebRequest(route, "POST");
            var uploadHandler = new UploadHandlerRaw(requestBytes);
            unityRequest.SetRequestHeader("Content-Type", "application/json");
            uploadHandler.contentType = "application/json";
            unityRequest.uploadHandler = uploadHandler;

            unityRequest.downloadHandler = new DownloadHandlerBuffer();

            var req = unityRequest.SendWebRequest();
            while (!req.isDone)
                yield return null;

            T result = null;

            if (unityRequest.error != null)
            {
#if DEBUG
                Debug.Log(unityRequest.error);
#endif
                throw new RpcClientUnknownException("Error occurred when trying to send rpc requests(s): " + unityRequest.error);
            }
            else
            {
                try
                {
                    byte[] results = unityRequest.downloadHandler.data;
                    var responseJson = Encoding.UTF8.GetString(results);
#if DEBUG
                    Debug.Log(responseJson);
#endif
                    result = JsonConvert.DeserializeObject<T>(responseJson, JsonSerializerSettings);
                }
                catch (Exception ex)
                {
                    Debug.Log("Error occurred when trying to send rpc requests(s):" + ex.Message);
                    throw new RpcClientUnknownException("Error occurred when trying to send rpc requests(s)", ex);
                }
            }

            yield return result;
        }

        protected override async Task<RpcResponseMessage> SendAsync(RpcRequestMessage request, string route = null)
        {
            var rpcRequestJson = JsonConvert.SerializeObject(request, JsonSerializerSettings);
            return await UnityCoroutineHandler.ExecuteCoroutineOnMainThread(SendRequest<RpcResponseMessage>(_url, rpcRequestJson));
        }

        public async Task<T> SendRequestAsync<T>(RPCRequest request)
        {
            var rpcRequestJson = JsonConvert.SerializeObject(request.Parameters, JsonSerializerSettings);
            var response = await UnityCoroutineHandler.ExecuteCoroutineOnMainThread(SendRequest<RPCResponse>(_url+request.Route, rpcRequestJson));
            return response.GetData<T>();
        }
    }
}
