using System;

public class ShootingMasterAchievement : AchievementBase
{
    public int targetNumber;

    private IntegerStat stat;

    // Use this for initialization
    void Start()
    {
        stat = GetComponent<IntegerStat>();
        if (stat != null)
        {
            stat.StatChanged += UnlockAchievement;
            base.XboxLiveUser = stat.XboxLiveUser;
        }
    }

    public override uint CalculateProgress()
    {
        return (uint)Math.Round(Convert.ToDouble(stat.Value) / targetNumber * 100);
    }
}
