using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BoosterManager : Singleton<BoosterManager>
{
    public Button hintButton; // Booster button that will find the match-3.
    [SerializeField] private Button undoButton; // Booster button that will remove rightmost object from stack.
    [SerializeField] private Button freezeButton; // Booster button that will freeze the timer.
    public bool timeFreezed; // Hepler to check time is freezed or not.

    #region Hint Booster
    public void HintButton()
    {
        if (!GameManager.Instance.GameIsPlaying() || InputController.Instance.IsResolving || !hintButton.interactable) return;
        if (ReplicaSourceTools.Enabled) {
            if (ReplicaSourceTools.Shuffling || !ReplicaSourceTools.CanUse(1)) return;
            var items=ReplicaSourceTools.AutoFindCandidates(InputController.Instance.objectsOnStack,LevelGenerator.Instance.objectsOnScene);
            if(items.Count==0)return;
            ReplicaSourceTools.MarkUsed(1);ReplicaSourceAudio.Tool(25);
            if(Camera.main)ReplicaSourceHighlight.For(Camera.main).ShowAutoFind(items);
            foreach(var item in items){
                var stack=InputController.Instance.objectsOnStack;int index=stack.FindLastIndex(x=>x&&x.name==item.name);index=index<0?stack.Count:index+1;
                var slot=ReplicaTray.Instance?ReplicaTray.Instance.SlotPosition(index):new Vector3(-1+index*.333f,.3f,-1.62f);
                InputController.Instance.AddObjectOnStack(item,false);ReplicaSourceToolVisual.AutoFind(item,slot);
            }
            CheckHintInteractibility();return;
        }
        StartCoroutine(FindPossibleMatch());
    }
    private IEnumerator FindPossibleMatch()
    {
        hintButton.interactable = false;
        if (InputController.Instance.objectsOnStack.Count > 0) // Stack is not empty.
        {
            FindMatchForAlreadyCollectedObjects();
        }
        else // Stack is empty.
        {
            FindFirstMatchOnGameArea();
        }
        yield return new WaitForSeconds(.75f);
        hintButton.interactable = true;
    }
    private void FindMatchForAlreadyCollectedObjects() // Find the most logical match for already collected object.
    {
        List<List<GameObject>> groupedList = new List<List<GameObject>>();
        List<GameObject> notCollectedObjects = new List<GameObject>();
        groupedList = InputController.Instance.objectsOnStack.GroupBy(x => x.name).OrderByDescending(g => g.Count()).Select(v => v.ToList()).ToList();
        notCollectedObjects = LevelGenerator.Instance.objectsOnScene.Except(InputController.Instance.objectsOnStack).ToList();
        notCollectedObjects = notCollectedObjects.Where(n => n.name.Contains(groupedList[0][0].name)).ToList();
        for (int i = 0; i < 3 - groupedList[0].Count; i++)
        {
            InputController.Instance.AddObjectOnStack(notCollectedObjects[i]);
        }
    }
    private void FindFirstMatchOnGameArea() // Find the first match between objects on scene.
    {
        List<GameObject> firstMatchedList = LevelGenerator.Instance.objectsOnScene.GroupBy(x => x.name).Where(g => g.Count() > 2).FirstOrDefault().Take(3).ToList();
        for (int i = 0; i < firstMatchedList.Count; i++)
        {
            InputController.Instance.AddObjectOnStack(firstMatchedList[i]);
        }
    }
    public void ResetBoosters()
    {
        StopAllCoroutines(); timeFreezed = false; FrostEffect.FrostAmount = 0;
        BoostersInteractibility(false);
    }
    public void CheckHintInteractibility() // If there is one box and all object are same in stack, player cant use this booster.
    {
        hintButton.interactable = GameManager.Instance.GameIsPlaying();
        if(ReplicaSourceTools.Enabled){hintButton.interactable=hintButton.interactable&&ReplicaSourceTools.CanUse(1)&&ReplicaSourceTools.CanAutoFind(InputController.Instance.objectsOnStack);return;}
        if (InputController.Instance.arrivedObject.Count >= 6)
        {
            bool allObjectsDifferent = true;
            for (int i = 0; i < InputController.Instance.arrivedObject.Count; i++)
            {
                for (int j = i + 1; j < InputController.Instance.arrivedObject.Count; j++)
                {
                    if (InputController.Instance.arrivedObject[i].name == InputController.Instance.arrivedObject[j].name)
                    {
                        allObjectsDifferent = false;
                        break;
                    }
                }
                if (!allObjectsDifferent)
                {
                    break;
                }
            }
            hintButton.interactable = allObjectsDifferent ? false : true;
        }
    }
    #endregion

    #region Undo Booster
    public void Undo() // Remove the most recently collected object from the stack.
    {
        if(ReplicaSourceTools.Enabled)InputController.Instance.MoveSourceItemsToStore();
        else InputController.Instance.UndoLast();
    }
    private void CheckUndoInteractibility() // Check for undo button interactibility.
    {
        undoButton.interactable = InputController.Instance.arrivedObject.Count > 0 ?
         InputController.Instance.arrivedObject.Count == 3 && InputController.Instance.arrivedObject.All(x => x.name == InputController.Instance.arrivedObject[0].name) ? false : true : false;
    }
    #endregion

    #region Freeze Booster
    public void Freeze() // Freeze the timer.
    {
        if (!GameManager.Instance.GameIsPlaying() || timeFreezed) return;
        StartCoroutine(FreezeTimer());
    }
    private IEnumerator FreezeTimer()
    {
        freezeButton.interactable = false;
        timeFreezed = true;
        for (int i = 0; i < 16; i++)
        {
            FrostEffect.FrostAmount += Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        yield return new WaitForSeconds(5f);
        for (int i = 0; i < 16; i++)
        {
            FrostEffect.FrostAmount -= Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        freezeButton.interactable = true;
        timeFreezed = false;
    }
    #endregion

    private void Update()
    {
        if (GameManager.Instance.GameIsPlaying())
        {
            CheckUndoInteractibility();
        }
        else
        {
            BoostersInteractibility(false);
        }
    }
    public void BoostersInteractibility(bool status)
    {
        hintButton.interactable = status;
        undoButton.interactable = status;
        freezeButton.interactable = status;
    }
}
