using UnityEngine;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace Assets.Mechanics.TextToSpeech
{
    public static class TextToSpeechSessionStorage
    {
        private static Dictionary<string, AudioClip> _audioClipsMap;

        public static bool TryGetAudioClip(string text, out AudioClip audioClip)
        {
            audioClip = null;

            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            if (_audioClipsMap == null || _audioClipsMap.Count == 0)
            {
                return false;
            }

            string textKey = CreateMD5(text);
            return _audioClipsMap.TryGetValue(textKey, out audioClip);
        }

        public static void AddAudioClip(string text, AudioClip audioClip)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            string textKey = CreateMD5(text);

            if (_audioClipsMap == null)
            {
                _audioClipsMap = new();
            }
            _audioClipsMap[textKey] = audioClip;
        }

        private static string CreateMD5(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
    }
}
