// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

#if ENABLE_WINMD_SUPPORT
using Microsoft.Xbox.Services.System;
#endif

public class XboxLiveUserEventArgs : EventArgs
{
#if ENABLE_WINMD_SUPPORT
    public XboxLiveUserEventArgs(XboxLiveUser user)
    {
        this.User = user;
    }

    public XboxLiveUser User { get; private set; }
#endif
}
