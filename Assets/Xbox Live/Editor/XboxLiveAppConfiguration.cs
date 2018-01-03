// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using UnityEngine;

[Serializable]
public class XboxLiveAppConfiguration
{
    public const string FileName = "XboxServices.config";

    public string PublisherId;

    public string PublisherDisplayName;

    public string PackageIdentityName;

    public string DisplayName;

    public string AppId;

    public string ProductFamilyName;

    public string PrimaryServiceConfigId;

    public uint TitleId;

    public string Sandbox;

    public bool XboxLiveCreatorsTitle;

    public static XboxLiveAppConfiguration Load(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(string.Format("Unable to find Xbox Live app configuration file '{0}'.", path));
        }

        string content = File.ReadAllText(path);
        if (string.IsNullOrEmpty(content))
        {
            throw new Exception(string.Format("Xbox Live app configeration file '{0}' was empty.", path));
        }

        return JsonUtility.FromJson<XboxLiveAppConfiguration>(content);
    }
}
