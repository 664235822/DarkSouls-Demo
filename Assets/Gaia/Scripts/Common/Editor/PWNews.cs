// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using System;
using UnityEngine;

namespace GaiaCommon1
{
    [Serializable]
    public class PWNews : RemoteContent
    {
        public string Title { get; private set; }
        public string Url { get; private set; }
        public string Body { get; private set; }

        public PWNews(AppConfig appConfig) : base(appConfig.NameSpace + "_pw_news", appConfig.NewsURL)
        {
            Title = "Procedural Worlds News";
            Url = "http://procedural-worlds.com";
            Body = "Welcome to Procedural Worlds.";
        }

        protected override void ProcessRemoteData(PWMessage message)
        {
#if PW_DEBUG
                Debug.LogFormat("[PWNews-{0}]: Processing remote data\n" +
                    "Title: '{1}'\n" +
                    "Url: '{2}'\n" +
                    "Body: '{3}'\n" +
                    ".", m_id, message.title, message.url, message.bodyContent); 
#endif
            Title = message.title;
            Url = message.url;
            Body = message.bodyContent;
        }

        public PWNews Load()
        {
#if PW_DEBUG
            Debug.LogFormat("[PWNews]: Loading news for '{0}'", m_id); 
#endif
            return (PWNews)LoadPath(m_path);
        }
    }
}
