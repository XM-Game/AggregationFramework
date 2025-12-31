#if ENABLE_UNITYWEBREQUEST && (!UNITY_2019_1_OR_NEWER || UNITASK_WEBREQUEST_SUPPORT)

using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Cysharp.Threading.Tasks
{
    public class UnityWebRequestException : Exception
    {
        public UnityWebRequest UnityWebRequest { get; } // 获取 UnityWebRequest 实例
#if UNITY_2020_2_OR_NEWER
        public UnityWebRequest.Result Result { get; } // 获取 UnityWebRequest 结果
#else
        public bool IsNetworkError { get; } // 获取是否网络错误
        public bool IsHttpError { get; } // 获取是否 HTTP 错误
#endif
        public string Error { get; } // 获取错误信息
        public string Text { get; } // 获取文本
        public long ResponseCode { get; } // 获取响应代码
        public Dictionary<string, string> ResponseHeaders { get; } // 获取响应头

        string msg; // 错误信息

        public UnityWebRequestException(UnityWebRequest unityWebRequest) // 构造函数，初始化 UnityWebRequestException 实例
        {
            this.UnityWebRequest = unityWebRequest;
#if UNITY_2020_2_OR_NEWER
            this.Result = unityWebRequest.result;
#else
            this.IsNetworkError = unityWebRequest.isNetworkError;
            this.IsHttpError = unityWebRequest.isHttpError;
#endif
            this.Error = unityWebRequest.error;
            this.ResponseCode = unityWebRequest.responseCode; // 获取响应代码
            if (UnityWebRequest.downloadHandler != null)
            {
                if (unityWebRequest.downloadHandler is DownloadHandlerBuffer dhb)
                {
                    this.Text = dhb.text; // 获取文本
                }
            }
            this.ResponseHeaders = unityWebRequest.GetResponseHeaders(); // 获取响应头
        }

        public override string Message // 获取错误信息
        {
            get
            {
                if (msg == null) // 如果错误信息为空
                {
                    if(!string.IsNullOrWhiteSpace(Text))
                    {
                        msg = Error + Environment.NewLine + Text; // 错误信息为错误信息和文本
                    }
                    else
                    {
                        msg = Error; // 错误信息为错误信息
                    }
                }
                return msg; // 返回错误信息
            }
        }
    }
}

#endif