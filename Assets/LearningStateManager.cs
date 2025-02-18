using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class LearningStateManager : MonoBehaviour
{
    private List<(string Sign, string Hand)> learningStates = new List<(string, string)>
    {
        ("g", "Left"),
        ("f", "Right"),
        ("s", "Any"),
        ("f", "Right"),
        ("i", "Right")
    };
    private bool mirrowed = true;
    private string restPoseName = "RESTPOSE";
    private int currentStateIndex = 0;

    public HandDataLoader handDataLoader;
    public Transform leftHand;
    public Transform rightHand;

    public (string Sign, string Hand) CurrentState { get; private set; }

    void Start()
    {
        StartCoroutine(InitializeHandData());
    }

    private IEnumerator InitializeHandData()
    {
        yield return StartCoroutine(handDataLoader.LoadAllHandData());
        ApplyHandShape();
    }

    private void ApplyHandShape()
    {

        CurrentState = learningStates[currentStateIndex];
        
        ApplyRestPosition(rightHand);
        ApplyRestPosition(leftHand);

        if (CurrentState.Hand == "Right" || CurrentState.Hand == "Any")
        {
            handDataLoader.LoadHandData(mirrowed ? leftHand : rightHand, CurrentState.Sign);
            return;
        }

        if (CurrentState.Hand == "Left" || CurrentState.Hand == "Any")
        {
            handDataLoader.LoadHandData(mirrowed ? rightHand : leftHand, CurrentState.Sign);
            return;
        }
    }

    private void ApplyRestPosition(Transform hand)
    {
        handDataLoader.LoadHandData(hand, restPoseName);
    }

    public void ChangeState(string performedSign, string performedHand)
    {
        if (currentStateIndex >= learningStates.Count-1) return;

        var expectedState = learningStates[currentStateIndex];

        if (performedSign == expectedState.Sign &&
            (performedHand == expectedState.Hand || expectedState.Hand == "Any"))
        {
            currentStateIndex++;
            ApplyHandShape();
            
        }
        //else
        //{
        //    Log.L($"Incorrect gesture. Expected: {expectedState.Sign} on {expectedState.Hand}, but got: {performedSign} on {performedHand}");
        //}
    }
}
