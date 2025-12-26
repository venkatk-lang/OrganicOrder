using DG.Tweening;
using IACGGames;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class TutorialManager : Singleton<TutorialManager>
{

    [Header("UI")]
    [SerializeField] GameObject tutorialUIRoot;
    [SerializeField] Button skipTutorialButton;
    [SerializeField] TutorialTextUI tutorialTextUI;
    [SerializeField] Transform handImage;   // UI Image
    [SerializeField] float moveDuration = 1.2f;
    [SerializeField] float scaleDuration = 0.25f;

    Sequence handSequence;

    [Header("References")]
    LevelManager levelManager;
    ItemDatabaseSO itemDatabase;

    int currentTrialIndex;
    bool tutorialActive;

    Coroutine idleRoutine;
    Coroutine mainInstructionRoutine;

    const float IDLE_TIME = 5f;
    private string dragMessage;
    private string wrongMessage;
    private Item firstItem;


    protected override void Awake()
    {
        base.Awake();
        tutorialUIRoot.SetActive(false);
        handImage.gameObject.SetActive(false);
    }
    public void StartTutorial(LevelManager lm)
    {
        tutorialUIRoot.SetActive(true);
        tutorialActive = true;
        levelManager = lm;
        itemDatabase = lm.itemDatabase;
        RaycastInputManager.Instance.OnDragStart += OnDragStarted;
        currentTrialIndex = 0;
        skipTutorialButton.onClick.AddListener(() =>
        {
            EndTutorial();
        });
        StartCurrentTrial();
    }


    void StartCurrentTrial()
    {

        handImage.gameObject.SetActive(false);
        wrongMessage = "Oops! Place dishes according to the rule.";
        switch (currentTrialIndex)
        {
            case 0:
                SetupTrial1();
                break;

            case 1:
                SetupTrial2();
                break;

            case 2:
                SetupTrial3();
                break;

            case 3:
                SetupTrial4();
                break;

            default:
                EndTutorial();
                break;
        }

       
    }

  

    void SetupTrial1()
    {
        int positions = 2; //totalTargetpos
        var items = itemDatabase.items.GetRange(0, positions);
        ItemData itemA = items[0];
        int itemPos = 1;
        firstItem = itemA.itemType;
        RuntimeRule rule = new RuntimeRule
        {
            root = new InPositionCondition(itemA, itemPos)
        };
      
        levelManager.SetupTutorialLevel(
            positions,
            new[] { rule },
            items,
            Array.Empty<LockedSeed>()
        );
        dragMessage = $"Click and drag the {itemA.name} to position {itemPos+1}.\n" +
            "Release when the dish is near the number marker.";
        StartIdleTimer();
        ShowText("In this game you need to arrange dishes acorrding to rules.");

    }


    void SetupTrial2()
    {
       
        int positions = 3;
        var items = itemDatabase.items.GetRange(0, positions);

        ItemData itemA = itemDatabase.items[0];
        ItemData itemB = itemDatabase.items[1];
        ItemData itemC = itemDatabase.items[2];
        RuntimeRule rule1 = new RuntimeRule
        {
            root = new LeftOfCondition(itemA, itemB)
        };

        RuntimeRule rule2 = new RuntimeRule
        {
            root = new LeftOfCondition(itemA, itemC)
        };
        levelManager.SetupTutorialLevel(
            positions,
            rules: new[] { rule1,rule2 },
            items,
             new[]
            {
                new LockedSeed
                {
                    item = itemA,
                    position = 0
                }
            }
        );
      
        ShowText("Sometimes dishes are already placed, and you cannot move them.");

    }

    void SetupTrial3()
    {
        int positions = 3;
        var items = itemDatabase.items.GetRange(0, positions);

        ItemData itemB = itemDatabase.items[1];
        ItemData itemC = itemDatabase.items[2];
        RuntimeRule rule = new RuntimeRule
        {
            root = new IfThenCondition
                (new InPositionCondition(itemB, 1),
                new InPositionCondition(itemC, 2)
                )
        };

       
        levelManager.SetupTutorialLevel(
            positions,
            rules: new[] { rule },
            items,
            Array.Empty<LockedSeed>()
        );

        ShowText("Some rules have 2 parts, Place dishes according to the rule, then click Submit.");

    }

    void SetupTrial4()
    {
        int positions = 3;
        var items = itemDatabase.items.GetRange(0, positions);

   
        ItemData itemB = itemDatabase.items[1];
        ItemData itemC = itemDatabase.items[2];
        RuntimeRule rule = new RuntimeRule
        {
            root = new IfThenCondition
                (new InPositionCondition(itemB, 1),
                new InPositionCondition(itemC, 2)
                )
        };


        levelManager.SetupTutorialLevel(
            positions,
            rules: new[] { rule },
            items,
              new[]
            {
                new LockedSeed
                {
                    item = itemC,
                    position = 0
                }
            },
              true
        );

        ShowText("One more to go! This time, try to place the dishes before time ends.");
        wrongMessage = "Try placing dishes as if first part is not true";

    }

    private void ShowMainInstruction()
    {
        if(mainInstructionRoutine != null)
        {
            StopCoroutine(mainInstructionRoutine);
        }
        mainInstructionRoutine = StartCoroutine(MainInstruction());
    }
    IEnumerator MainInstruction()
    {
        yield return new WaitForSeconds(5f);
        ShowText("Place dishes according to the rule, then click Submit");
    }



    public void OnDragStarted()
    {
        if (!tutorialActive)
            return;
        StopHandTutorial();
        StopIdleTimer();
       ShowMainInstruction();
    }

    public void OnSubmit(bool correct)
    {
        if (correct)
        {
            AdvanceTrial();
        }
        else
        {
            ShowText(wrongMessage);
        }
    }


    void StartIdleTimer()
    {
        StopIdleTimer();
        idleRoutine = StartCoroutine(IdleHintRoutine());
    }

    void StopIdleTimer()
    {
        if (idleRoutine != null)
            StopCoroutine(idleRoutine);
    }

    IEnumerator IdleHintRoutine()
    {
        yield return new WaitForSeconds(IDLE_TIME);
        ShowText(dragMessage);
        PlayHandTutorial();
    }
    void PlayHandTutorial()
    {
        StopHandTutorial(); // safety

        Transform startPos = levelManager.GetItem(firstItem);
        Transform targetPos = levelManager.GetTargetPosition(1); // position index 1
        if(startPos ==null || targetPos == null)
        {
            return;
        }

        handImage.gameObject.SetActive(true);
        handImage.position = startPos.position;
        handImage.localScale = Vector3.one;

        handSequence = DOTween.Sequence();

        handSequence
            // Scale down
            .Append(handImage.DOScale(0.9f, scaleDuration))
            // Scale up
            .Append(handImage.DOScale(1f, scaleDuration))
            // Move to target
            .Append(handImage.DOMove(targetPos.position, moveDuration)
                .SetEase(Ease.InOutQuad))
            // Small pause
            .AppendInterval(0.4f)
     
            // Reset position
            .AppendCallback(() =>
            {
                handImage.position = startPos.position;
            })
            .SetLoops(-1, LoopType.Restart);
    }

    void StopHandTutorial()
    {
        if (handSequence != null && handSequence.IsActive())
        {
            handSequence.Kill();
            handSequence = null;
        }

        handImage.gameObject.SetActive(false);
    }


    void AdvanceTrial()
    {
      
        currentTrialIndex++;
        StartCurrentTrial();
    }

    void EndTutorial()
    {
        StopAllCoroutines();
        RaycastInputManager.Instance.OnDragStart -= OnDragStarted;
        skipTutorialButton.onClick.RemoveAllListeners();
        tutorialActive = false;
        tutorialTextUI.Hide();
        tutorialUIRoot.SetActive(false);
        GameManager.Instance.StartGame();
    }


    void ShowText(string text)
    {
        tutorialTextUI.Show(text);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
