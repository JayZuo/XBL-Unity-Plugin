// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using UnityEngine;

#if ENABLE_WINMD_SUPPORT
using Microsoft.Xbox.Services.Social.Manager;

public delegate void SocialEventHandler(object sender, SocialEvent socialEvent);
#endif

public class SocialManagerComponent : Singleton<SocialManagerComponent>
{
#if ENABLE_WINMD_SUPPORT
    public event SocialEventHandler EventProcessed;

    private SocialManager manager;
#endif

    protected SocialManagerComponent()
    {
    }

#if ENABLE_WINMD_SUPPORT
    /// <summary>
    /// Awake is called when the script instance is being loaded
    /// </summary>
    private void Awake()
    {
        this.manager = SocialManager.SingletonInstance;
    }

    private void Update()
    {
        try
        {
            var socialEvents = this.manager.DoWork();

            foreach (SocialEvent socialEvent in socialEvents)
            {
                if (XboxLiveServicesSettings.Instance.DebugLogsOn)
                {
                    Debug.LogFormat("[SocialManager] Processed {0} event.", socialEvent.EventType);
                }
                this.OnEventProcessed(socialEvent);
            }
        }
        catch (Exception e)
        {
            if (XboxLiveServicesSettings.Instance.DebugLogsOn)
            {
                Debug.Log("An Exception Occured: " + e.ToString());
            }
        }
    }

    protected virtual void OnEventProcessed(SocialEvent socialEvent)
    {
        this.EventProcessed?.Invoke(this, socialEvent);
    }
#endif
}
