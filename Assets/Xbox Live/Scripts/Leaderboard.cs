// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

#if ENABLE_WINMD_SUPPORT
using Microsoft.Xbox.Services.Leaderboard;
using Microsoft.Xbox.Services.Social.Manager;
using Microsoft.Xbox.Services.Statistics.Manager;
#endif

[Serializable]
public class Leaderboard : MonoBehaviour
{
    private string socialGroup;

    public StatBase stat;

    public LeaderboardTypes leaderboardType;

    [Range(1, 100)]
    public uint entryCount = 10;

    public Text headerText;

    [HideInInspector]
    public uint currentPage;

    [HideInInspector]
    public uint totalPages;

    [HideInInspector]
    public Text pageText;

    [HideInInspector]
    public Button firstButton;

    [HideInInspector]
    public Button previousButton;

    [HideInInspector]
    public Button nextButton;

    [HideInInspector]
    public Button lastButton;

    public string firstControllerButton;

    public string lastControllerButton;

    public string nextControllerButton;

    public string prevControllerButton;

    public string refreshControllerButton;

    public string verticalScrollInputAxis;

    public Transform contentPanel;

    public ScrollRect scrollRect;

    public XboxLiveUserInfo XboxLiveUser;

    public float scrollSpeedMultiplier = 0.1f;

#if ENABLE_WINMD_SUPPORT
    private LeaderboardResult leaderboardData;
    private XboxSocialUserGroup userGroup;
#endif
#pragma warning disable 414
    private ObjectPool entryObjectPool;
#pragma warning restore 414

    private bool isLocalUserAdded
    {
        get
        {
            return statsAddedLocalUser && socialAddedLocalUser;
        }
    }
    private bool statsAddedLocalUser, socialAddedLocalUser;

    private void Awake()
    {
        this.EnsureEventSystem();
        XboxLiveServicesSettings.EnsureXboxLiveServicesSettings();

        if (this.stat == null)
        {
            if (XboxLiveServicesSettings.Instance.DebugLogsOn)
            {
                Debug.LogFormat("Leaderboard '{0}' does not have a stat configured and will not function properly.", this.name);
            }
            return;
        }

        this.headerText.text = this.stat.DisplayName;
        this.entryObjectPool = this.GetComponent<ObjectPool>();
        this.UpdateButtons();

#if ENABLE_WINMD_SUPPORT
        SocialManagerComponent.Instance.EventProcessed += this.SocialManagerEventProcessed;
#endif
        StatsManagerComponent.Instance.LocalUserAdded += this.LocalUserAdded;
        StatsManagerComponent.Instance.GetLeaderboardCompleted += this.GetLeaderboardCompleted;
        this.statsAddedLocalUser = false;
        this.socialAddedLocalUser = false;
    }

#if ENABLE_WINMD_SUPPORT
    private void Start()
    {
        if (this.XboxLiveUser == null
            && XboxLiveUserManager.Instance.SingleUserModeEnabled
            && XboxLiveUserManager.Instance.GetSingleModeUser() != null
            && XboxLiveUserManager.Instance.GetSingleModeUser().User != null
            && XboxLiveUserManager.Instance.GetSingleModeUser().User.IsSignedIn)
        {
            this.XboxLiveUser = XboxLiveUserManager.Instance.GetSingleModeUser();
            this.statsAddedLocalUser = true;
            this.socialAddedLocalUser = true;
            this.UpdateData(0);
        }
    }
#endif

    public void RequestFlushToService(bool isHighPriority)
    {
#if ENABLE_WINMD_SUPPORT
        StatsManagerComponent.Instance.RequestFlushToService(this.XboxLiveUser.User, isHighPriority);
#endif
    }

    void Update()
    {
        if (!string.IsNullOrEmpty(this.refreshControllerButton) && Input.GetKeyDown(this.refreshControllerButton))
        {
            this.Refresh();
        }

        if (this.currentPage != 0 && !string.IsNullOrEmpty(this.prevControllerButton) && Input.GetKeyDown(this.prevControllerButton))
        {
            this.PreviousPage();
        }

        if (this.currentPage != this.totalPages && !string.IsNullOrEmpty(this.nextControllerButton) && Input.GetKeyDown(this.nextControllerButton))
        {
            this.NextPage();
        }

        if (!string.IsNullOrEmpty(this.lastControllerButton) && Input.GetKeyDown(this.lastControllerButton))
        {
            this.LastPage();
        }

        if (!string.IsNullOrEmpty(this.firstControllerButton) && Input.GetKeyDown(this.firstControllerButton))
        {
            this.FirstPage();
        }

        if (!string.IsNullOrEmpty(this.verticalScrollInputAxis) && Input.GetAxis(this.verticalScrollInputAxis) != 0)
        {
            var inputValue = Input.GetAxis(this.verticalScrollInputAxis);
            this.scrollRect.verticalScrollbar.value = this.scrollRect.verticalNormalizedPosition + inputValue * scrollSpeedMultiplier;
        }
    }

    public void Refresh()
    {
        this.FirstPage();
    }

    public void NextPage()
    {
        this.UpdateData(this.currentPage + 1);
    }

    public void PreviousPage()
    {
        if (this.currentPage > 0)
        {
            this.UpdateData(this.currentPage - 1);
        }
    }

    public void FirstPage()
    {
        this.UpdateData(0);
    }

    public void LastPage()
    {
        this.UpdateData(this.totalPages - 1);
    }

    private void UpdateData(uint newPage)
    {
#if ENABLE_WINMD_SUPPORT
        if (!this.isLocalUserAdded)
        {
            return;
        }

        if (this.stat == null)
        {
            return;
        }

        if (this.XboxLiveUser == null)
        {
            this.XboxLiveUser = XboxLiveUserManager.Instance.GetSingleModeUser();
        }

        LeaderboardQuery query;
        if (newPage == this.currentPage + 1 && this.leaderboardData != null && this.leaderboardData.HasNext)
        {
            query = this.leaderboardData.GetNextQuery();
        }
        else
        {
            switch (leaderboardType)
            {
                case LeaderboardTypes.Global:
                    socialGroup = string.Empty;
                    break;
                case LeaderboardTypes.Favorites:
                    socialGroup = "favorite";
                    break;
                case LeaderboardTypes.Friends:
                    socialGroup = "all";
                    break;
            }

            query = new LeaderboardQuery
            {
                SkipResultToRank = newPage == 0 ? 0 : (newPage * this.entryCount) + 1,
                MaxItems = this.entryCount,
            };
        }

        this.currentPage = newPage;

        if (socialGroup == string.Empty)
        {
            StatisticManager.SingletonInstance.GetLeaderboard(this.XboxLiveUser.User, this.stat.ID, query);
        }
        else
        {
            StatisticManager.SingletonInstance.GetSocialLeaderboard(this.XboxLiveUser.User, this.stat.ID, socialGroup, query);
        }
#endif
    }

    private void LocalUserAdded(object sender, XboxLiveUserEventArgs e)
    {
        this.statsAddedLocalUser = true;
        this.Refresh();
    }

    private void GetLeaderboardCompleted(object sender, XboxLivePrefab.StatEventArgs e)
    {
#if ENABLE_WINMD_SUPPORT
        if (e.EventData.ErrorCode != 0)
        {
            return;
        }

        LeaderboardResultEventArgs leaderboardArgs = (LeaderboardResultEventArgs)e.EventData.EventArgs;
        this.LoadResult(leaderboardArgs.Result);
#endif
    }

#if ENABLE_WINMD_SUPPORT
    private void SocialManagerEventProcessed(object sender, SocialEvent socialEvent)
    {
        if (socialEvent.EventType == SocialEventType.LocalUserAdded)
        {
            socialAddedLocalUser = true;
            this.Refresh();
        }
        else if (socialEvent.EventType == SocialEventType.SocialUserGroupLoaded) //&& ((SocialUserGroupLoadedEventArgs)socialEvent.EventArgs).SocialUserGroup == this.userGroup //TODO
        {
            var entries = this.contentPanel.GetComponentsInChildren<LeaderboardEntry>();
            for (int i = 0; i < entries.Length; i++)
            {
                XboxSocialUser user = userGroup.Users.FirstOrDefault(x => x.Gamertag == entries[i].gamertagText.text);
                if (user != null)
                {
                    entries[i].GamerpicUrl = user.DisplayPicUrlRaw + "&w=128";
                }
            }
        }
    }

    /// <summary>
    /// Load the leaderboard result data from the service into the view.
    /// </summary>
    /// <param name="result"></param>
    private void LoadResult(LeaderboardResult result)
    {
        if (this.stat == null || (result.HasNext && (this.stat.ID != result.GetNextQuery().StatName || this.socialGroup != result.GetNextQuery().SocialGroup)))
        {
            return;
        }

        this.leaderboardData = result;

        uint displayCurrentPage = this.currentPage + 1;
        if (this.leaderboardData.TotalRowCount == 0)
        {
            this.totalPages = 0;
            displayCurrentPage = 0;
        }
        else if (this.totalPages == 0)
        {
            this.totalPages = (uint)Mathf.Ceil(this.leaderboardData.TotalRowCount / this.entryCount);
        }

        this.pageText.text = string.Format("Page: {0} / {1}", displayCurrentPage, this.totalPages);

        while (this.contentPanel.childCount > 0)
        {
            var entry = this.contentPanel.GetChild(0).gameObject;
            this.entryObjectPool.ReturnObject(entry);
        }

        var xuids = new List<string>();
        foreach (LeaderboardRow row in this.leaderboardData.Rows)
        {
            xuids.Add(row.XboxUserId);

            GameObject entryObject = this.entryObjectPool.GetObject();
            LeaderboardEntry entry = entryObject.GetComponent<LeaderboardEntry>();

            entry.Data = row;

            entryObject.transform.SetParent(this.contentPanel);
        }
        userGroup = SocialManager.SingletonInstance.CreateSocialUserGroupFromList(XboxLiveUserManager.Instance.UserForSingleUserMode.User, xuids);

        // Reset the scroll view to the top.
        this.scrollRect.verticalNormalizedPosition = 1;
        this.UpdateButtons();
    }
#endif

    public void UpdateButtons()
    {
        this.firstButton.interactable = this.previousButton.interactable = this.currentPage != 0;
        this.nextButton.interactable = this.lastButton.interactable = this.totalPages > 1 && this.currentPage < this.totalPages - 1;
    }
}
