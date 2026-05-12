using UnityEngine;
using Cysharp.Threading.Tasks;
using Assets.Mechanics.TextToSpeech;

namespace Assets.Mechanics.TextToSpeech
{
    public static class TextToSpeechConverter
    {
        public static async UniTask<AudioClip> SynthesizeAudioClip(string text)
        {
            var utcs = new UniTaskCompletionSource<AudioClip>();

            if (TextToSpeechSessionStorage.TryGetAudioClip(text, out AudioClip clip))
            {
                utcs.TrySetResult(clip);
                return await utcs.Task;
            }

            clip = await YandexSpeechKitFunctions.ConvertTextToSpeech(text);
            if (clip != null)
            {
                TextToSpeechSessionStorage.AddAudioClip(text, clip);
            }

            utcs.TrySetResult(clip);

            return await utcs.Task;
        }
    }
}
