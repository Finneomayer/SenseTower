using UnityEngine;
using System.IO;

namespace Assets.Scripts
{
    public class LogToFileWriter : MonoBehaviour
    {
        private const string Filename = "SenseTower.log";

        private static LogToFileWriter _instance;

        private void Awake()
        {
#if UNITY_SERVER
            Destroy(gameObject);
            return;
#endif
            if (Application.platform == RuntimePlatform.Android)
            {
                Destroy(gameObject);
                return;
            }

            if (_instance == null)
            {
                _instance = this;
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
                InitLogging();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            Application.logMessageReceived += LogMessageReceived;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= LogMessageReceived;
        }

        private void InitLogging()
        {
            if (File.Exists(Filename))
            {
                File.Delete(Filename);
            }
        }

        private void LogMessageReceived(string logString, string stackTrace, LogType logType)
        {
            WriteLog(logString, stackTrace, logType);
        }

        private void WriteLog(string logString, string stackTrace, LogType logType)
        {           
            TextWriter tw = new StreamWriter(Filename, true);
            tw.WriteLine($"[{System.DateTime.Now}] [{logType}] {logString}");
            if (logType != LogType.Log && !string.IsNullOrEmpty(stackTrace))
            {
                tw.WriteLine($"Stacktrace: {stackTrace}");
            }
            tw.Close();
        }
    }
}
