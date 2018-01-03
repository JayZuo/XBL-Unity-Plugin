// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

#if ENABLE_WINMD_SUPPORT
using Microsoft.Xbox.Services.Statistics.Manager;
using Microsoft.Xbox.Services.System;
#endif

/// <summary>
/// The actual integer value of the for the stat.
/// </summary>
/// <remarks>
/// Yes, this should be a long, but Unity doesn't save seem to properly serialize long values.
/// </remarks>
[Serializable]
public class IntegerStat : StatBase<int>
{
#if ENABLE_WINMD_SUPPORT
    protected override void HandleGetStat(XboxLiveUser user, string statName)
    {
        this.isLocalUserAdded = true;
        try
        {
            var statValue = StatisticManager.SingletonInstance.GetStatistic(user, statName);
            if (statValue != null)
            {
                this.Value = (int)statValue.AsInteger;
            }
        }
        catch (Exception ex)
        {
            if (XboxLiveServicesSettings.Instance.DebugLogsOn)
            {
                UnityEngine.Debug.LogError(ex.Message);
            }
        }
    }
#endif

    public void Increment()
    {
        this.Value = this.Value + 1;
    }

    public void Decrement()
    {
        this.Value = this.Value - 1;
    }

    public override int Value
    {
        get
        {
            return base.Value;
        }
        set
        {
            if (this.isLocalUserAdded)
            {
                base.Value = value;
#if ENABLE_WINMD_SUPPORT
                StatisticManager.SingletonInstance.SetStatisticNumberData(this.XboxLiveUser.User, this.ID, value);
                this.StatChanged?.Invoke();
#endif
            }
        }
    }
}
