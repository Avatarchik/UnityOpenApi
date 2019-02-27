﻿using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Microsoft.OpenApi.Readers;
using OAS = Microsoft.OpenApi;
using System.IO;
using Microsoft.OpenApi.Validations;

namespace HttpMono
{
    [CreateAssetMenu(menuName = "Unity Open API/Parser")]
    public class OpenApiParser : ScriptableObject
    {
        [Header("Dependencies")]
        [SerializeField] private HttpAsset http = null;

        

        public void Parse(string json)
        {
            var stream = CreateStream(json);
            var parsed = new OpenApiStreamReader(new OpenApiReaderSettings
            {
                ReferenceResolution = ReferenceResolutionSetting.ResolveLocalReferences,
                RuleSet = ValidationRuleSet.GetDefaultRuleSet()
            }).Read(stream, out var openApiDiagnostic);
            Debug.Log("Parsed");
        }

        public void ParseFromUrl(string url)
        {
            http.HttpMono.Get(url, null, result =>
            {
                if(result.Ok)
                {
                    Parse(result.Text);
                } else
                {
                    Debug.Log(result.Error.Message);
                }
            });
        }

        private MemoryStream CreateStream(string text)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            writer.Write(text);
            writer.Flush();
            stream.Position = 0;

            return stream;
        }

    }

    [CustomEditor(typeof(OpenApiParser))]
    public class OpenApiParserEditor : Editor
    {
        string url = "https://raw.githubusercontent.com/OAI/OpenAPI-Specification/master/examples/v3.0/uspto.yaml";
        TextAsset textAsset;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10);

            var t = (OpenApiParser)target;

            GUILayout.BeginVertical();
            textAsset = (TextAsset)EditorGUILayout.ObjectField("API Asset", textAsset, typeof(TextAsset), false);
            if (GUILayout.Button("Parse From Asset"))
            {
                t.Parse(textAsset.text);
            }
            GUILayout.EndVertical();

            GUILayout.Space(10);

            GUILayout.BeginVertical();
            url = EditorGUILayout.TextField(new GUIContent("URL"), url);
            if (GUILayout.Button("Parse From Url"))
            {
                t.ParseFromUrl(url);
            }
            GUILayout.EndVertical();

        }
    }
}
