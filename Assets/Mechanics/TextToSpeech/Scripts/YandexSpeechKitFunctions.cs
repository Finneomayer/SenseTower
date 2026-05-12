using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Proyecto26;
using UnityEngine.Networking;

namespace Assets.Mechanics.TextToSpeech
{
    public static class YandexSpeechKitFunctions
    {
        private static bool _isBusy;

        public static async UniTask<AudioClip> ConvertTextToSpeech(string text)
        {
            await UniTask.WaitWhile(() => _isBusy);
            _isBusy = true;

            var utcs = new UniTaskCompletionSource<AudioClip>();

            AudioClip speechData;

            try
            {
                speechData = await RequestTextToSpeechAudioClip(text);
            }
            catch (Exception)
            {
                Debug.LogError("ConvertTextToSpeech error");
                speechData = null;
            }

            _isBusy = false;

            utcs.TrySetResult(speechData);
            return await utcs.Task;
        }

        private static async UniTask<AudioClip> RequestTextToSpeechAudioClip(string text)
        {
            const string Token = "Api-Key AQVNztXFVA_eW5CeFASyBrmfpmaxexcA0K9gXB4v";
            const string FolderId = "b1g283hnblgcvrgk6d53";
            const string SpeechKitUrl = "https://tts.api.cloud.yandex.net/speech/v1/tts:synthesize";

            var utcs = new UniTaskCompletionSource<AudioClip>();

            var data = new WWWForm();
            data.AddField("folderId", FolderId);
            data.AddField("text", text);
            data.AddField("lang", "ru-RU");
            data.AddField("voice", "alena");
            data.AddField("emotion", "neutral");
            data.AddField("speed", "1");
            data.AddField("format", "mp3");
            data.AddField("sampleRateHertz", "48000");

            var options = new RequestHelper
            {
                Uri = SpeechKitUrl,
                FormData = data,
                DownloadHandler = new DownloadHandlerAudioClip(SpeechKitUrl, AudioType.MPEG),
            };
            options.Headers["Authorization"] = Token;

            RestClient.Post(options).Then(response =>
            {
                utcs.TrySetResult(((DownloadHandlerAudioClip)response.Request.downloadHandler).audioClip);
            })
            .Catch(err =>
            {
                Debug.LogWarning($"RequestTextToSpeechAudioClip. {err.Message}. Url: {options.Uri}");
                utcs.TrySetResult(null);
            });

            return await utcs.Task;
        }
    }
}
