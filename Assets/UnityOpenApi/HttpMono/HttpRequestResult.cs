﻿using System.Collections.Generic;
using UnityEngine.Networking;

namespace HttpMono
{
    /// <summary>
    /// Represent the unified response for a web requests
    /// </summary>
    public class HttpRequestResult
    {
        public bool Ok { get; }
        public bool HasText { get; }
        public HttpResultError Error { get; }
        public KeyValuePair<long, string> HttpCodeResponse { get; }
        public string Text { get; }

        /// <summary>
        /// Create Response using UnityWebRequest
        /// </summary>
        /// <param name="r">a UnityWebRequest</param>
        public HttpRequestResult(UnityWebRequest r)
        {
            Ok = true;

            if (r.isNetworkError || string.IsNullOrEmpty(r.error) == false) Ok = false;
            HttpCodeResponse = StandardHttpResponseCodes.GetCode(r.responseCode);
            if (HttpCodeResponse.Key < 200 || HttpCodeResponse.Key > 299) Ok = false;

            if (!Ok)
            {
                Error = new HttpResultError(r);
            }

            if (r.downloadHandler != null)
            {
                if (string.IsNullOrEmpty(r.downloadHandler.text) == false)
                {
                    HasText = true;
                    Text = r.downloadHandler.text;
                }
            }
        }

        public HttpRequestResult(string result)
        {
            Ok = true;
            HasText = true;
            Text = result;
        }

    }

}