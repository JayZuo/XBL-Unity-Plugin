// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using UnityEngine;

#if ENABLE_WINMD_SUPPORT
using Microsoft.Xbox.Services;
using Microsoft.Xbox.Services.Social.Manager;
using Microsoft.Xbox.Services.Statistics.Manager;
using Microsoft.Xbox.Services.System;
#endif

public class XboxLiveUserInfo : MonoBehaviour
{
#if ENABLE_WINMD_SUPPORT
    private Windows.UI.Core.CoreDispatcher coreDispatcher = null;

    public XboxLiveUser User { get; private set; }

    public Windows.System.User WindowsSystemUser { get; private set; }

    public XboxLiveContext XboxLiveContext { get; private set; }
#endif

    public void Awake()
    {
        DontDestroyOnLoad(this);
    }

#if ENABLE_WINMD_SUPPORT
    public void Start()
    {
        Windows.ApplicationModel.Core.CoreApplicationView mainView = Windows.ApplicationModel.Core.CoreApplication.MainView;
        Windows.UI.Core.CoreWindow cw = mainView.CoreWindow;

        coreDispatcher = cw.Dispatcher;
    }

    public void Initialize()
    {
        this.User = new XboxLiveUser();
    }

    public void Initialize(Windows.System.User systemUser)
    {
        this.WindowsSystemUser = systemUser;
        this.User = new XboxLiveUser(systemUser);
    }

    public IEnumerator SignInAsync()
    {
        SignInStatus signInStatus;
        var signInSilentlyTask = this.User.SignInSilentlyAsync(coreDispatcher).AsTask();
        yield return signInSilentlyTask.AsCoroutine();

        if (signInSilentlyTask.IsCompleted)
        {
            signInStatus = signInSilentlyTask.Result.Status;
            if (signInStatus == SignInStatus.UserInteractionRequired)
            {
                var signInTask = this.User.SignInAsync(coreDispatcher).AsTask();
                yield return signInTask.AsCoroutine();

                if (signInTask.IsCompleted)
                {
                    signInStatus = signInTask.Result.Status;
                }
            }

            if (signInStatus == SignInStatus.Success)
            {
                StatisticManager.SingletonInstance.AddLocalUser(this.User);
                SocialManager.SingletonInstance.AddLocalUser(this.User, SocialManagerExtraDetailLevel.PreferredColorLevel);
                this.XboxLiveContext = new XboxLiveContext(this.User);
            }
        }
    }
#endif
}
