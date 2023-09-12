using UnityEngine;

namespace Source.Scripts
{
    public class TestClass
    {
        public void Log(string log, LogType logType = LogType.Normal)
        {
#if UNITY_EDITOR
            switch (logType)
            {
                case LogType.Normal:
                    Debug.Log(log);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(log);
                    break;
                case LogType.Error:
                    Debug.LogError(log);
                    break;
            }
#endif
        }
    }

    public enum LogType
    {
        Normal = 0,
        Warning = 1,
        Error =2
    }
}