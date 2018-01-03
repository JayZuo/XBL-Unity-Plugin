// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

#if ENABLE_WINMD_SUPPORT
using Microsoft.Xbox.Services.Statistics.Manager;
#endif

namespace XboxLivePrefab
{
    public class StatEventArgs : EventArgs
    {
#if ENABLE_WINMD_SUPPORT
        public StatEventArgs(StatisticEvent statEvent)
        {
            this.EventData = statEvent;
        }

        public StatisticEvent EventData { get; private set; }
#endif
    }
}
