using System;
using System.Collections;
using Assets.Scripts.Models;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Proyecto26;
using UnityEngine;

namespace Assets.Scripts.WebUtils
{
    public class RequestDto<T>
    {
        public T Result;
        public string Error;
    }

    public class PatchRequestDto
    {
        public int OperationType;
        public string Path;
        public string Op;
        public string From;
        public object Value;
    }

    public static class WebRequestFunctions
    {
        public static async UniTask<HttpResponse<string>> Get(RequestHelper options)
        {
            var utcs = new UniTaskCompletionSource<HttpResponse<string>>();

            HttpResponse<string> httpResponse = null;
            
            if(!options.Headers.ContainsKey("Accept-Encoding"))
                options.Headers.Add("Accept-Encoding","gzip");
            
            RestClient.Get(options).Then(response =>
            {
                httpResponse = new HttpResponse<string>(response.Text, response.StatusCode);
                utcs.TrySetResult(httpResponse);
            })
            .Catch(err =>
            {
                RequestException error = err as RequestException;

                Debug.LogWarning($"{typeof(WebRequestFunctions).Name}. {nameof(Get)}. {error.Message}. {error.Response}. Url: {options.Uri}");
                httpResponse = new HttpResponse<string>(default, error.StatusCode);
                utcs.TrySetResult(httpResponse);
            });

            return await utcs.Task;
        }

        public static async UniTask<HttpResponse<string>> Post(RequestHelper options)
        {
            var utcs = new UniTaskCompletionSource<HttpResponse<string>>();
            
            if(!options.Headers.ContainsKey("Accept-Encoding"))
                options.Headers.Add("Accept-Encoding","gzip");

            HttpResponse<string> httpResponse = null;

            RestClient.Post(options).Then(response =>
            {
                httpResponse = new HttpResponse<string>(response.Text, response.StatusCode);
                utcs.TrySetResult(httpResponse);
            })
            .Catch(err =>
            {
                Debug.LogWarning($"{typeof(WebRequestFunctions).Name}. " +
                    $"{nameof(Post)}. {err.Message}. Url: {options.Uri}");
                httpResponse = new HttpResponse<string>(default,
                    (err as RequestException).StatusCode);
                utcs.TrySetResult(httpResponse);
            });

            return await utcs.Task;
        }

        public static async UniTask<HttpResponse<string>> Post(string url, string accessToken = null)
        {
            var options = new RequestHelper
            {
                Uri = url,
            };

            if (!string.IsNullOrEmpty(accessToken))
            {
                options.Headers["Authorization"] = $"Bearer {accessToken}";
            }
            return await Post(options);
        }

        public static async UniTask<HttpResponse<string>> Post<TIn>
            (string url, TIn inObject, string accessToken = null)
        {
            var options = new RequestHelper
            {
                Uri = url,
                BodyString = JsonConvert.SerializeObject(inObject),
            };

            if (!string.IsNullOrEmpty(accessToken))
            {
                options.Headers["Authorization"] = $"Bearer {accessToken}";
            }

            return await Post(options);
        }
        public static async UniTask<HttpResponse<T>> DeleteWithDeserialization<T>(RequestHelper options)
        {
            var utcs = new UniTaskCompletionSource<HttpResponse<T>>();

            HttpResponse<T> httpResponse = null;
            if(!options.Headers.ContainsKey("Accept-Encoding"))
                options.Headers.Add("Accept-Encoding","gzip");

            RestClient.Delete(options).Then(response =>
                {
                    httpResponse = new HttpResponse<T>(DeserializeData<T>(response.Text), response.StatusCode);
                    utcs.TrySetResult(httpResponse);
                })
                .Catch( err =>
                {
                    RequestException error = err as RequestException;

                    Debug.LogWarning($"{typeof(WebRequestFunctions).Name}. {nameof(DeleteWithDeserialization)}. {error.Message}. {error.Response}. Url: {options.Uri}");
                    httpResponse = new HttpResponse<T>(DeserializeData<T>(error.Response), error.StatusCode);
                    utcs.TrySetResult(httpResponse);
                });

            return await utcs.Task;
        }
        public static async UniTask<HttpResponse<T>> PostWithDeserialization<T>(RequestHelper options)
        {
            var utcs = new UniTaskCompletionSource<HttpResponse<T>>();

            HttpResponse<T> httpResponse = null;
            if(!options.Headers.ContainsKey("Accept-Encoding"))
                options.Headers.Add("Accept-Encoding","gzip");

            RestClient.Post(options).Then(response =>
            {
                httpResponse = new HttpResponse<T>(DeserializeData<T>(response.Text), response.StatusCode);
                utcs.TrySetResult(httpResponse);
            })
            .Catch( err =>
            {
                RequestException error = err as RequestException;

                Debug.LogWarning($"{typeof(WebRequestFunctions).Name}. {nameof(PostWithDeserialization)}. {error.Message}. {error.Response}. Url: {options.Uri}");
                httpResponse = new HttpResponse<T>(DeserializeData<T>(error.Response), error.StatusCode);
                utcs.TrySetResult(httpResponse);
            });

            return await utcs.Task;
        }


        public static async UniTask<HttpResponse<T>> PutWithDeserialization<T>(RequestHelper options) where T : class
        {
            var utcs = new UniTaskCompletionSource<HttpResponse<T>>();

            HttpResponse<T> httpResponse = null;
            if(!options.Headers.ContainsKey("Accept-Encoding"))
                options.Headers.Add("Accept-Encoding","gzip");

            RestClient.Put(options).Then(response =>
            {
                httpResponse = new HttpResponse<T>(DeserializeData<T>(response.Text), response.StatusCode);
                utcs.TrySetResult(httpResponse);
            })
            .Catch( err =>
            {
                RequestException error = err as RequestException;

                Debug.LogWarning($"{typeof(WebRequestFunctions).Name}. {nameof(PostWithDeserialization)}. {error.Message}. {error.Response}. Url: {options.Uri}");
                httpResponse = new HttpResponse<T>(DeserializeData<T>(error.Response), error.StatusCode);
                utcs.TrySetResult(httpResponse);
            });

            return await utcs.Task;
        }

        public static async UniTask<HttpResponse<T>> GetWithDeserialization<T>(RequestHelper options)
        {
            var utcs = new UniTaskCompletionSource<HttpResponse<T>>();

            HttpResponse<T> httpResponse = null;
            
            if(!options.Headers.ContainsKey("Accept-Encoding"))
                options.Headers.Add("Accept-Encoding","gzip");

            RestClient.Get(options).Then(response =>
            {
                httpResponse = new HttpResponse<T>(DeserializeData<T>(response.Text), response.StatusCode);
                utcs.TrySetResult(httpResponse);
#if UNITY_EDITOR
                Debug.Log($"REQUEST - {typeof(T)} - {response.Text}");
#endif

            })
            .Catch(err =>
            {
                Debug.LogWarning($"{typeof(WebRequestFunctions).Name}. " +
                    $"{nameof(GetWithDeserialization)}. {err.Message}. Url: {options.Uri}");
                httpResponse = new HttpResponse<T>(default,
                    (err as RequestException).StatusCode);
                utcs.TrySetResult(httpResponse);
            });

            return await utcs.Task;
        }

        public static async UniTask<HttpResponse<T>> GetWithDeserialization<T>
            (string url, string accessToken = null) where T : class
        {
            return await GetWithDeserialization<T>(url, null, accessToken);
        }

        public static async UniTask<HttpResponse<T>> GetWithDeserialization<T>
            (string url, WWWForm formData, string accessToken = null) where T : class
        {
            var options = new RequestHelper
            {
                Uri = url,
                FormData = formData
            };
            
            if (!string.IsNullOrEmpty(accessToken))
            {
                options.Headers["Authorization"] = $"Bearer {accessToken}";
            }

            return await GetWithDeserialization<T>(options);
        }

        public static async UniTask<HttpResponse<T>> PostWithDeserialization<TIn, T>
            (string url, TIn inObject, string accessToken = null)
        {
            var options = new RequestHelper
            {
                Uri = url,
                BodyString = JsonConvert.SerializeObject(inObject),
            };
            if (!string.IsNullOrEmpty(accessToken))
            {
                options.Headers["Authorization"] = $"Bearer {accessToken}";
            }

            return await PostWithDeserialization<T>(options);
        }

        public static async UniTask<HttpResponse<string>> Patch(RequestHelper options)
        {
            var utcs = new UniTaskCompletionSource<HttpResponse<string>>();

            if (!options.Headers.ContainsKey("Accept-Encoding"))
                options.Headers.Add("Accept-Encoding", "gzip");

            HttpResponse<string> httpResponse = null;

            RestClient.Patch(options).Then(response =>
            {
                httpResponse = new HttpResponse<string>(response.Text, response.StatusCode);
                utcs.TrySetResult(httpResponse);
            })
            .Catch(err =>
            {
                RequestException error = err as RequestException;

                Debug.LogWarning($"{typeof(WebRequestFunctions).Name}. {nameof(Patch)}. {error.Message}. {error.Response}. Url: {options.Uri}");
                httpResponse = new HttpResponse<string>(default, error.StatusCode);
                utcs.TrySetResult(httpResponse);
            });

            return await utcs.Task;
        }

        public static async UniTask<HttpResponse<string>> Patch<TIn>
            (string url, TIn inObject, string accessToken = null) where TIn : class
        {
            var options = new RequestHelper
            {
                Uri = url,
                BodyString = JsonConvert.SerializeObject(inObject),
            };
            if (!string.IsNullOrEmpty(accessToken))
            {
                options.Headers["Authorization"] = $"Bearer {accessToken}";
            }

            return await Patch(options);
        }

        public static async UniTask<HttpResponse<string>> Delete(RequestHelper options)
        {
            var utcs = new UniTaskCompletionSource<HttpResponse<string>>();

            if (!options.Headers.ContainsKey("Accept-Encoding"))
                options.Headers.Add("Accept-Encoding", "gzip");

            HttpResponse<string> httpResponse = null;

            RestClient.Delete(options).Then(response =>
            {
                httpResponse = new HttpResponse<string>(response.Text, response.StatusCode);
                utcs.TrySetResult(httpResponse);
            })
            .Catch(err =>
            {
                RequestException error = err as RequestException;

                Debug.LogWarning($"{typeof(WebRequestFunctions).Name}. {nameof(Delete)}. {error.Message}. {error.Response}. Url: {options.Uri}");
                httpResponse = new HttpResponse<string>(default, error.StatusCode);
                utcs.TrySetResult(httpResponse);
            });

            return await utcs.Task;
        }

        public static async UniTask<HttpResponse<string>> Delete(string url, string accessToken = null)
        {
            var options = new RequestHelper
            {
                Uri = url,
            };
            if (!string.IsNullOrEmpty(accessToken))
            {
                options.Headers["Authorization"] = $"Bearer {accessToken}";
            }

            return await Delete(options);
        }

        public static T DeserializeData<T>(string textData)
        {
            T deserialized = default;
            try
            {
                deserialized = JsonConvert.DeserializeObject<T>(textData);
            }
            catch (Exception e)
            {
                Debug.Log($"<color=red>Error deserialize {typeof(T)}: </color>" + e);
            }

            return deserialized;
        }
    }
}