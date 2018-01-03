using System;
using UnityEngine;

#if ENABLE_WINMD_SUPPORT
using Microsoft.Xbox.Services.Achievements;
using System.Threading.Tasks;
#endif

public abstract class AchievementBase : MonoBehaviour
{
    public XboxLiveUserInfo XboxLiveUser;

    [Tooltip("The achievement ID.")]
    public string ID;

    void Update()
    {
        if (this.XboxLiveUser == null)
        {
            this.XboxLiveUser = XboxLiveUserManager.Instance.GetSingleModeUser();
        }
    }

    public abstract uint CalculateProgress();

    public void UnlockAchievement()
    {
#if ENABLE_WINMD_SUPPORT
        XboxLiveUser.XboxLiveContext?.AchievementService.UpdateAchievementAsync(XboxLiveUser.User.XboxUserId, ID, CalculateProgress());
#endif
    }

#if ENABLE_WINMD_SUPPORT
    public Task<Achievement> GetAchievementAsync()
    {
        return XboxLiveUser.XboxLiveContext?.AchievementService.GetAchievementAsync(XboxLiveUser.User.XboxUserId, Microsoft.Xbox.Services.XboxLiveAppConfiguration.SingletonInstance.ServiceConfigurationId, ID).AsTask();
    }
#endif
}
