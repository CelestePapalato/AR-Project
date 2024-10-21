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

    public bool isMax { get => val == max_value; } 

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
    [SerializeField]
    private Stat petting;

    public Stat Feed { get => feed; }
    public Stat Love { get => love; }
    public Stat Petting { get => petting; }

    private bool initializedStats = false;

    private Coroutine pettingCooldownCoroutine;

    public event Action<float, float> OnPettingCooldownUpdate;

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
        petting.OnValueModified += StartPettingDecrease;
    }

    private void OnDisable()
    {
        feed.OnValueModified -= RestartFeedDecrease;
        love.OnValueModified -= RestartLoveDecrease;
        petting.OnValueModified -= StartPettingDecrease;
    }

    private void OnDestroy()
    {
        CancelInvoke();
        StopAllCoroutines();
    }

    private bool InitializeStats()
    {
        if (petData == null)
        {
            return false;
        }

        feed = new Stat(0, 0, petData.MaxFeed);
        love = new Stat(0, 0, petData.MaxLove);
        petting = new Stat(0, 0, petData.MaxPettingCount);
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
        if(love.Value == 0) { return; }
        Invoke(nameof(LoveDecrease), petData.LoveDecreaseRate);
    }

    private void FeedDecrease()
    {
        feed.Value -= petData.FeedDecreaseValue;
        if (feed.Value == 0) { return; }
        Invoke(nameof(FeedDecrease), petData.FeedDecreaseRate);
    }

    private void StartPettingDecrease(int value, int diff)
    {
        if(diff < 0 || pettingCooldownCoroutine != null) { return; }
        pettingCooldownCoroutine = StartCoroutine(PetCountCleaner());
    }

    private IEnumerator PetCountCleaner()
    {
        float cooldown = petData.PettingCooldown;
        float startTime = cooldown;
        while (petting.Value > 0)
        {
            while(cooldown > 0)
            {
                yield return null;
                cooldown -= Time.deltaTime;
                OnPettingCooldownUpdate?.Invoke(cooldown, startTime);
            }
            petting.Value -= 1;
            cooldown = startTime;
        }
    }
}
