// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using UnityEngine;

#if ENABLE_WINMD_SUPPORT
using Microsoft.Xbox.Services.Statistics.Manager;
using Microsoft.Xbox.Services.System;
#endif

public class StatsManagerComponent : Singleton<StatsManagerComponent>
{
    public event EventHandler<XboxLiveUserEventArgs> LocalUserAdded;

    public event EventHandler<XboxLiveUserEventArgs> LocalUserRemoved;

    public event EventHandler<XboxLivePrefab.StatEventArgs> GetLeaderboardCompleted;

    public event EventHandler StatUpdateComplete;

#if ENABLE_WINMD_SUPPORT
    private StatisticManager manager;
#endif

    protected StatsManagerComponent()
    {
    }

#if ENABLE_WINMD_SUPPORT
    private void Awake()
    {
        this.manager = StatisticManager.SingletonInstance;
    }

    private void Update()
    {
        if (this.manager == null && XboxLiveServicesSettings.Instance.DebugLogsOn)
        {
            Debug.LogWarning("Somehow the manager got nulled out.");
            return;
        }
        var events = this.manager.DoWork();
        foreach (StatisticEvent statEvent in events)
        {
            if (XboxLiveServicesSettings.Instance.DebugLogsOn)
            {
                Debug.LogFormat("[StatsManager] Processed {0} event for {1}.", statEvent.EventType, statEvent.User.Gamertag);
            }

            switch (statEvent.EventType)
            {
                case StatisticEventType.LocalUserAdded:
                    this.OnLocalUserAdded(statEvent.User);
                    break;
                case StatisticEventType.LocalUserRemoved:
                    this.OnLocalUserRemoved(statEvent.User);
                    break;
                case StatisticEventType.StatisticUpdateComplete:
                    this.OnStatUpdateComplete();
                    break;
                case StatisticEventType.GetLeaderboardComplete:
                    this.OnGetLeaderboardCompleted(new XboxLivePrefab.StatEventArgs(statEvent));
                    break;
            }
        }
    }

    public void RequestFlushToService(XboxLiveUser user, bool isHighPriority)
    {
        this.manager.RequestFlushToService(user, isHighPriority);
    }

    protected virtual void OnLocalUserAdded(XboxLiveUser user)
    {
        this.LocalUserAdded?.Invoke(this, new XboxLiveUserEventArgs(user));
    }

    protected virtual void OnLocalUserRemoved(XboxLiveUser user)
    {
        this.LocalUserRemoved?.Invoke(this, new XboxLiveUserEventArgs(user));
    }

    protected virtual void OnStatUpdateComplete()
    {
        this.StatUpdateComplete?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnGetLeaderboardCompleted(XboxLivePrefab.StatEventArgs statEvent)
    {
        this.GetLeaderboardCompleted?.Invoke(this, statEvent);
    }
#endif
}
