using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[Serializable]
public class Stat
{
    [SerializeField]
    private int val;
    [SerializeField]
    private int min_value;
    [SerializeField]
    private int max_value;

    public int Value
    {
        get { return val; }
        set
        {
            if(val != value)
            {
                int prev = val;
                val = Mathf.Clamp(value, min_value, max_value);
                int diff = val - prev;
                OnUpdate?.Invoke(val, min_value, max_value);
                OnValueModified?.Invoke(val, diff);
            }
        }
    }

    public event Action<int, int, int> OnUpdate;
    public event Action<int, int> OnValueModified;

    public Stat (int startValue, int min_value, int max_value)
    {
        this.min_value = min_value;
        this.max_value = max_value;
        Value = startValue;
    }
}

public class PetStats : MonoBehaviour
{
    [SerializeField]
    private PetSO petData;

    [Header("DEBUG")]

    [SerializeField]
    private Stat feed;
    [SerializeField]
    private Stat love;

    public Stat Feed { get => feed; }
    public Stat Love { get => love; }

    bool initializedStats = false;

    private void Awake()
    {
        if (!initializedStats)
        {
            if (!InitializeStats()) { Destroy(gameObject); return; }
        }
    }

    private void Start()
    {
        if (feed.Value > 0) { Invoke(nameof(LoveDecrease), petData.LoveDecreaseRate); }
        if (love.Value > 0) { Invoke(nameof(FeedDecrease), petData.FeedDecreaseRate); }
    }

    private void OnEnable()
    {
        feed.OnValueModified += RestartFeedDecrease;
        love.OnValueModified += RestartLoveDecrease;
    }

    private void OnDisable()
    {
        feed.OnValueModified -= RestartFeedDecrease;
        love.OnValueModified -= RestartLoveDecrease;
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }

    private bool InitializeStats()
    {
        if (petData == null)
        {
            return false;
        }

        feed = new Stat(0, 0, petData.MaxFeed);
        love = new Stat(0, 0, petData.MaxLove);
        initializedStats = true;
        return true;
    }

    private void RestartFeedDecrease(int value, int diff)
    {
        if (diff < 0) { return; }
        CancelInvoke(nameof(FeedDecrease));
        Invoke(nameof(FeedDecrease), petData.FeedDecreaseRate);
    }

    private void RestartLoveDecrease(int value, int diff)
    {
        if (diff < 0) { return; }
        CancelInvoke(nameof(LoveDecrease));
        Invoke(nameof(LoveDecrease), petData.LoveDecreaseRate);
    }

    private void LoveDecrease()
    {
        love.Value -= petData.LoveDecreaseValue;
        Invoke(nameof(LoveDecrease), petData.LoveDecreaseRate);
    }

    private void FeedDecrease()
    {
        feed.Value -= petData.FeedDecreaseValue;
        Invoke(nameof(FeedDecrease), petData.FeedDecreaseRate);
    }

    IEnumerator StatCooldown(Stat stat, float cooldown, int delta)
    {
        while (stat.Value > 0)
        {
            yield return new WaitForSeconds(cooldown);
            stat.Value += delta;
        }
    }
}
