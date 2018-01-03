// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

#if ENABLE_WINMD_SUPPORT
using Microsoft.Xbox.Services.Statistics.Manager;
using Microsoft.Xbox.Services.System;
#endif

[Serializable]
public class DoubleStat : StatBase<double>
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
                this.Value = statValue.AsNumber;
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

    public void Multiply(float multiplier)
    {
        this.Value = this.Value * multiplier;
    }

    public void Square()
    {
        var value = this.Value;
        this.Value = value * value;
    }

    public override double Value
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
