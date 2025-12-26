using DG.Tweening;
using IACGGames;
using IACGGames.UISystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;


public class LevelManager : MonoBehaviour
{

    [Header("Data")]
    public ItemDatabaseSO itemDatabase;

    [Header("Level Parameters")]
    [Range(2, 6)]
    public int numberOfPositions = 4; //change later

    [Range(1, 3)]
    public int numberOfRules = 2; //change later

    [Range(0, 2)]
    public int maxLockedSeeds = 1;
    public int totalTrials = 7;
    int currentTrialIndex = 0;
    int correctTrialCount = 0;
    int attemptsThisTrial = 0;
    const int maxAttempts = 2;


    GeneratedLevel currentLevel;
    LevelGenerator generator;

    [Header("Target Slot Spawning")]
    public Transform slotParent;
    public TargetSlot slotPrefab;
    public float slotSpacing = 1.5f;
    public Vector2 slotColliderSize = new Vector2(1f, 1f);


    [Header("Item Spawning")]
    public ItemDraggable itemPrefab;
    public Transform itemSpawnParent;

    public List<SpawnSlot> spawnSlots = new();
    public List<TargetSlot> targetSlots = new();
    List<ItemDraggable> spawnedItems = new();

    [Header("Rule")]
    public Transform ruleContainer;
    public RuleUI ruleUIPrefab;
    Dictionary<RuntimeRule, RuleUI> ruleMap;

    public int pointsPerCorrect = 100; // this will be changed as per time

    public Action<bool> OnItemPlacedInTarget; 
    public Action OnShowTrial; 
    public Action OnFailTrial; 
    public Action<SubmitResult> OnSubmit;  // answer correct, que impossible , click sumbit


    public PlayerProgress progress;

    public TrialTimer trialTimer;
    public BackgroundLightController bgLight;

    [SerializeField] const float TRIALTIME = 25;

    bool isTutorialLevel;
    bool isTimerTutorial;
    public void LevelSetup()
    {
        RaycastInputManager.Instance.DisableInput();
        Debug.Log("Start Game level");
        generator = new LevelGenerator();
        progress = new PlayerProgress();
    }


    public void StartGameLevel()
    {
        AudioManager.Instance.PlayBGM(BGMAudioID.Gameplay, true);
        isTimerTutorial = false;
        isTutorialLevel = false;
        correctTrialCount = 0;
        currentTrialIndex = 0;
        attemptsThisTrial = 0;
        progress.Initialize();
        StartTrial();
        UIManager.Instance.gameHUD.SetupHUD(false);
        
    }
    void ClearAllItems()
    {
        if (spawnedItems != null)
        {
            foreach (var item in spawnedItems)
                Destroy(item.gameObject);

            spawnedItems.Clear();
        }
        if (targetSlots != null)
        {
            foreach (var item in targetSlots)
            {
                Destroy(item.gameObject);
            }
            targetSlots.Clear();

        }
        
        if(spawnSlots != null)
        {
            foreach (var item in spawnSlots)
            {
                item.Clear();
            }
        }
    }

    void StartTrial()
    {
      
        ClearAllItems();
        currentTrialIndex++;
        UIManager.Instance.gameHUD.ShowTrialText(currentTrialIndex, totalTrials);
        GenerateAndDebugLevel();
        attemptsThisTrial = 0;
        OnShowTrial?.Invoke();
       // bgLight.ResetLight();
    }

    [ContextMenu("Generate Level")]
    public void GenerateAndDebugLevel()
    {
        Debug.Log("===== GENERATING LEVEL =====");

        int playerLevel = progress.Level;

        numberOfPositions = DifficultyResolver.GetPositions(playerLevel);
        numberOfRules = DifficultyResolver.GetRuleCount(playerLevel);

        currentLevel = generator.Generate(  
            itemDatabase,
            numberOfPositions,
            numberOfRules,
            maxLockedSeeds
        );

        if (currentLevel == null)
        {
            Debug.LogError("Level generation FAILED.");
            return;
        }

       // DebugLevel(currentLevel);

        SpawnTargetSlots(currentLevel.positions);
        SpawnItemsToSpawnSlots(currentLevel);
        DisplayRules(currentLevel.rules);
    }

  
    void DebugLevel(GeneratedLevel level)
    {
        Debug.Log($"Positions: {level.positions}");
        Debug.Log($"Impossible Level: {level.isImpossible}");

        Debug.Log("---- ITEMS ----");
        for (int i = 0; i < level.items.Count; i++)
        {
            Debug.Log($"{i + 1}: {level.items[i].itemName}");
        }

        Debug.Log("---- LOCKED SEEDS ----");
        if (level.lockedSeeds.Count == 0)
        {
            Debug.Log("None");
        }
        else
        {
            foreach (var l in level.lockedSeeds)
            {
                Debug.Log(
                    $"{l.item.itemName} locked at position {l.position + 1}");
            }
        }

        Debug.Log("---- RULES ----");
        for (int i = 0; i < level.rules.Count; i++)
        {
            Debug.Log(
                $"Rule {i + 1}: {level.rules[i].GetText()}");
        }
        DebugSolutions(level);
        Debug.Log("============================");
    }
    void DebugSolutions(GeneratedLevel level)
    {
        var solutions = RuleSolver.GetSolutions(
            level.items,
            level.rules,
            level.lockedSeeds);

        if (solutions.Count == 0)
        {
            Debug.Log(" NO SOLUTIONS FOUND (IMPOSSIBLE LEVEL)");
            return;
        }

        Debug.Log($" SOLUTIONS FOUND: {solutions.Count}");

        for (int i = 0; i < solutions.Count; i++)
        {
            Debug.Log($"--- SOLUTION {i + 1} ---");
            Debug.Log(solutions[i].ToReadableString());
        
        }
    }
    public void SetupTutorialLevel(
    int positions,
    IEnumerable<RuntimeRule> rules,
    IEnumerable<ItemData> items,
    IEnumerable<LockedSeed> lockedSeeds,
    bool isTimerEnabled = false)
    {
        AudioManager.Instance.PlayBGM(BGMAudioID.Gameplay, true);
        isTimerTutorial = isTimerEnabled;
        isTutorialLevel = true;
        UIManager.Instance.gameHUD.SetupHUD(true);
        ClearAllItems();

      
        currentLevel = new GeneratedLevel
        {
            positions = positions,
            items = new List<ItemData>(items),
            rules = new List<RuntimeRule>(rules),
            lockedSeeds = new List<LockedSeed>(lockedSeeds),
            isImpossible = false
        };
       // DebugLevel(currentLevel);

        SpawnTargetSlots(currentLevel.positions);
        SpawnItemsToSpawnSlots(currentLevel);
        DisplayRules(currentLevel.rules);

        OnShowTrial?.Invoke();
    }

    #region Spawn Items
    public void SpawnItemsToSpawnSlots(GeneratedLevel level)
    {
        // Safety check
        if (spawnSlots.Count < level.items.Count)
        {
            Debug.LogError("Not enough spawn slots for items!");
            return;
        }

        foreach (var slot in spawnSlots)
            slot.Clear();

        // Place locked items directly into target slots
        foreach (var locked in level.lockedSeeds)
        {
            ItemDraggable item = CreateItem(locked.item,true);
            TargetSlot targetSlot = targetSlots[locked.position];

            targetSlot.PlaceItem(item);
        }

        List<(ItemDraggable item, SpawnSlot slot)> animItems =
      new List<(ItemDraggable, SpawnSlot)>();
        // Place remaining items into spawn slots (lowest index first)
        int spawnIndex = 0;

        foreach (var itemData in level.items)
        {
            bool isLocked = level.lockedSeeds.Exists(l => l.item == itemData);
            if (isLocked)
                continue;

            SpawnSlot spawnSlot = spawnSlots[spawnIndex++];
            ItemDraggable item = CreateItem(itemData,false);

            spawnSlot.PlaceItem(item);
            animItems.Add((item, spawnSlot));
        }

        AnimateSpawnedItems(animItems);
    }
    void AnimateSpawnedItems(
     List<(ItemDraggable item, SpawnSlot slot)> items)
    {
        float startOffsetX = 6f;
        float moveDuration = 0.45f;
        float overlapDelay = 0.1f; // time between starts

        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < items.Count; i++)
        {
            ItemDraggable item = items[i].item;
            Transform target = items[i].slot.transform;

            item.transform.DOKill();

            Vector3 targetPos = target.position;
            Vector3 startPos = targetPos + Vector3.right * startOffsetX;
            item.transform.position = startPos;

            float startTime = i * overlapDelay;

            seq.Insert(
                startTime,
                item.transform.DOMove(targetPos, moveDuration)
                    .SetEase(Ease.OutCubic)

            );
        }
            AudioManager.Instance.PlaySFX(SFXAudioID.Slide);
            AudioManager.Instance.PlaySFX(SFXAudioID.Bell);

        seq.Play();
        seq.OnComplete(() =>
        {
            foreach (var item in items)
            {
                item.item.MakeInteractable(true);

            }
            if (!isTutorialLevel || isTimerTutorial)
            {
                trialTimer.StartTimer();
            }
          
            RaycastInputManager.Instance.EnableInput();
        });
    }


    ItemDraggable CreateItem(ItemData data,bool isLocked)
    {
        ItemDraggable item = Instantiate(itemPrefab);
        item.SetData(data, isLocked);
        spawnedItems.Add(item);
        return item;
    }

    #endregion
    #region Spawn Targets

    void SpawnTargetSlots(int slotCount)
    {
        if (slotParent == null || slotPrefab == null)
        {
            Debug.LogWarning("Slot parent or prefab missing.");
            return;
        }

        foreach (var slot in targetSlots)
            slot.Clear();
        // Clear old slots - USE HELPER
        for (int i = slotParent.childCount - 1; i >= 0; i--)
            Destroy(slotParent.GetChild(i).gameObject);

        float startX = -((slotCount - 1) * slotSpacing) / 2f;

        for (int i = 0; i < slotCount; i++)
        {
            float xPos = startX + i * slotSpacing;

            TargetSlot slot = Instantiate(slotPrefab, slotParent);
            slot.SetIndex(i);
            slot.OnItemUpdated += OnTargetFilled;

            slot.transform.localPosition = new Vector3(xPos, 0f, 0f);
            targetSlots.Add(slot);

        }

        Debug.Log($"Spawned {slotCount} target slots.");
    }

    private void OnTargetFilled()
    {
        bool full = true;
        foreach (var item in targetSlots)
        {
            if(item.CurrentItem == null)
            {
                full = false;
                break;
            }
        }
        OnItemPlacedInTarget?.Invoke(full);
    }
    #endregion

    void HandleCorrect()
    {
        RaycastInputManager.Instance.DisableInput();
        AudioManager.Instance.PlaySFX(SFXAudioID.Correct);
        correctTrialCount++;
        trialTimer.StopTimer();

        float GetScoreForLevel(int level)
        {
            float baseScore = 400f;
            float growth = 1.05f;

            return baseScore + Mathf.Pow(growth, level);

            
        }
        int RoundOffScore(float score)
        {
            return Mathf.RoundToInt(score / 10f) * 10;
        }
        float score = GetScoreForLevel(progress.Level);
       // GameManager.Instance.AddScore(progress.Level * pointsPerCorrect);
    

        Debug.Log("Correct " + attemptsThisTrial);
        if (attemptsThisTrial == 1)
        {
            GameManager.Instance.AddScore(RoundOffScore(score));
            int levelGained = LevelProgressCalculator.CalculateLevelGain(currentLevel.positions, trialTimer.ElapsedTime, TRIALTIME);
            Debug.Log("Level " + levelGained);
            progress.IncreaseLevel(levelGained);
       
        }
        else if (attemptsThisTrial == 2) 
        {
            score = score / 2;
            GameManager.Instance.AddScore(RoundOffScore(score));
        }

            StartCoroutine(NextTrial());
    }
    void HandleInCorrect(Dictionary<ItemData,int> placement)
    {
        AudioManager.Instance.PlaySFX(SFXAudioID.Wrong);
        DescreseLevel();
        HighlightFailedRules(placement);
        if (attemptsThisTrial >= maxAttempts)
        {
            RaycastInputManager.Instance.DisableInput();
            if (currentLevel.isImpossible)
            {
                
                OnFailTrial?.Invoke();
            }
            else
            {
                AnimateSolution();
            }
        }
    }
    #region Helper
    public SpawnSlot GetLowestFreeSpawnSlot()
    {
        return spawnSlots
            .OrderBy(s => s.index)
            .FirstOrDefault(s => !s.IsOccupied);
    }
  
    bool AreAllTargetSlotsFilled()
    {
        foreach (var slot in targetSlots)
            if (!slot.IsOccupied)
                return false;

        return true;
    }

    #endregion

    #region Buttons
    public void OnSubmitPressed()
    {
        if(isTutorialLevel)
        {
            if (!AreAllTargetSlotsFilled()) return; // extra 
            var p = BuildPlacementFromSlots();
            bool c = EvaluatePlacement(p);
            TutorialManager.Instance.OnSubmit(c);
            return; 
        }
        if (attemptsThisTrial >= maxAttempts)
        {
            StartCoroutine(NextTrial());
            return;
        }

        if (!AreAllTargetSlotsFilled()) return; // extra 
      
        var placement = BuildPlacementFromSlots();
        bool correct = EvaluatePlacement(placement);
        attemptsThisTrial++;
        if (correct)
        {
            OnSubmit?.Invoke(new SubmitResult(true,currentLevel.isImpossible,true,attemptsThisTrial==maxAttempts));
            HandleCorrect();
        
        }
        else
        {
            OnSubmit?.Invoke(new SubmitResult(false, currentLevel.isImpossible, true, attemptsThisTrial == maxAttempts));
            // ShowWrongMark();
            HandleInCorrect(placement);
         
        }

         
       
    }

    void DescreseLevel()
    {
        float timeTaken = trialTimer.ElapsedTime;

        int levelLoss = LevelProgressCalculator.CalculateLevelDecrease(
            timeTaken,
            fastThreshold: 2f
        );
        progress.DecreaseLevel(levelLoss);
      

    }

    Dictionary<ItemData, int> BuildPlacementFromSlots()
    {
        var map = new Dictionary<ItemData, int>();

        foreach (var slot in targetSlots)
        {
            if (!slot.IsOccupied)
                return null;

            map[slot.CurrentItem.Data] = slot.index;
        }

        return map;
    }
    bool EvaluatePlacement(Dictionary<ItemData, int> placement)
    {
        // check locked seeds
        foreach (var l in currentLevel.lockedSeeds)
        {
            if (placement[l.item] != l.position)
                return false;
        }

        // check rules
        foreach (var rule in currentLevel.rules)
        {
            if (!rule.IsSatisfied(placement))
                return false;
        }

        return true;
    }
    public void OnImpossiblePressed()
    {
        if (isTutorialLevel)
        {
            return;
        }
            attemptsThisTrial++;
        if (currentLevel.isImpossible)
        {
            OnSubmit?.Invoke(new SubmitResult(true, currentLevel.isImpossible, false, attemptsThisTrial == maxAttempts));
            HandleCorrect();
            HighlightImpossibleRules();
        }
        else
        {
            OnSubmit?.Invoke(new SubmitResult(false, currentLevel.isImpossible, false, attemptsThisTrial == maxAttempts));
            AudioManager.Instance.PlaySFX(SFXAudioID.Wrong);
            // ShowPopup("This trial is possible");
            if (attemptsThisTrial >= maxAttempts)
            {
                //next
                OnFailTrial?.Invoke();
                RaycastInputManager.Instance.DisableInput();
            }
        }
          
    }
    void HighlightImpossibleRules()
    {
        var impossibleRules = RuleSolver.GetImpossibleRules(
            currentLevel.items,
            currentLevel.rules,
            currentLevel.lockedSeeds);

        foreach (var rule in currentLevel.rules)
        {
            SetRuleHighlight(rule, false);
        }
        foreach (var rule in impossibleRules)
        {
            SetRuleHighlight(rule, true);
        }

    }

    void HighlightFailedRules(Dictionary<ItemData, int> placement)
    {
        foreach (var rule in currentLevel.rules)
        {
            bool satisfied = rule.IsSatisfied(placement);
            SetRuleHighlight(rule, !satisfied);
        }
    }
    public void SetRuleHighlight(RuntimeRule rule, bool highlight)
    {
        if (!ruleMap.ContainsKey(rule)) return;

        ruleMap[rule].Highlight(highlight);
    }
   
    void AnimateSolution()
    {
        var solutions = RuleSolver.GetSolutions(
            currentLevel.items,
            currentLevel.rules,
            currentLevel.lockedSeeds
        );

        if (solutions.Count == 0)
            return;

        var solution = solutions[0];

        StartCoroutine(AnimateItemsToSolution(solution.positions));
    }
    IEnumerator AnimateItemsToSolution(Dictionary<ItemData, int> solution)
    {
        foreach (var kvp in solution)
        {
            ItemDraggable item = FindItem(kvp.Key);
            TargetSlot slot = targetSlots[kvp.Value];

            item.transform.DOMove(slot.transform.position, 0.5f);
        }

        yield return new WaitForSeconds(0.6f);

        OnFailTrial?.Invoke();
    }
    ItemDraggable FindItem(ItemData data)
    {
        return spawnedItems.Find(i => i.Data == data);
    }
    WaitForSeconds trialBetweenTime =  new WaitForSeconds(1f);
    IEnumerator NextTrial()
    {
        yield return trialBetweenTime;
        if (currentTrialIndex >= totalTrials)
        {
            EndGame();
            yield return null;
        }

        StartTrial();
    }

    private void EndGame()
    {
        UIManager.Instance.LevelComplete.UpdateLevelText(progress.Level, correctTrialCount, totalTrials, GameManager.Instance.CurrentScore);
        UIManager.Instance.Show(UIState.LevelComplete,0.25f);
        progress.SaveLevelProgress();
    }
    #endregion
    #region Spawn Items
    public void DisplayRules(List<RuntimeRule> rules)
    {
        ClearRules();
        if (ruleMap == null)
        {
            ruleMap = new Dictionary<RuntimeRule, RuleUI>();
        }
  
        foreach (var rule in rules)
        {
            RuleUI ruleUI =
                Instantiate(ruleUIPrefab, ruleContainer);

            ruleUI.Init(rule.GetText());
            ruleMap.Add(rule, ruleUI);
        }
    }

    void ClearRules()
    {
        for (int i = ruleContainer.childCount - 1; i >= 0; i--)  //Helper
            Destroy(ruleContainer.GetChild(i).gameObject);

        if(ruleMap != null)
        {
            ruleMap.Clear();
        }
   
    }
    public void ResetLevel()
    {
        ClearAllItems();
        ClearRules();
        trialTimer.StopTimer();
    }
    public void PauseLevel()
    {
        trialTimer.PauseTimer();

    }
    public void ResumeLevel()
    {
        trialTimer.ResumeTimer();

    }
    #endregion

    //Tutorial helper
    public Transform GetItem(Item itemID)
    {
        foreach (var item in spawnedItems)
        {
            if (item.Data.itemType == itemID)
                return item.transform;
        }
        return null;
    }
    public Transform GetTargetPosition(int index)
    {
        foreach (var item in targetSlots)
        {
            if (item.index == index)
                return item.transform;
        }
        return null;
    }
}

public struct SubmitResult
{
    public bool isCorrect;
    public bool isImpossible;
    public bool clickedSubmit;
    public bool finalAttempt;

    public SubmitResult(bool isCorrect, bool isImpossibleAttempt, bool clickedSubmit,bool finalAttempt)
    {
        this.isCorrect = isCorrect;
        this.isImpossible = isImpossibleAttempt;
        this.clickedSubmit = clickedSubmit;
        this.finalAttempt = finalAttempt;
    }
}