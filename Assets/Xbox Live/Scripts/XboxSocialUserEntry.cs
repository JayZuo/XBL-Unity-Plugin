﻿// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using UnityEngine;
using UnityEngine.UI;

#if ENABLE_WINMD_SUPPORT
using Microsoft.Xbox.Services.Social.Manager;
#endif

public class XboxSocialUserEntry : MonoBehaviour
{
    public Button entryButton;
    public Image gamerpicImage;
    public Image gamerpicMask;
    public Text gamertagText;
    public Text presenceText;

#if ENABLE_WINMD_SUPPORT
    private XboxSocialUser data;

    public XboxSocialUser Data
    {
        get
        {
            return this.data;
        }
        set
        {
            this.data = value;
            this.gamertagText.text = this.data.Gamertag;
            this.presenceText.text = this.data.PresenceRecord.UserState.ToString();
        }
    }
#endif
}
