using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class LearningStateManager : MonoBehaviour
{
    private List<(string Sign, string Hand)> learningStates = new List<(string, string)>
    {
        ("s", "Right"),
        ("l", "Right"),
        ("s", "Left"),
        ("l", "Left"),
        ("s", "Right"),
        ("l", "Right"),
        ("b", "Right"),
        ("a", "Left")
    };

    private bool mirrowed = true;
    private string restPoseName = "RESTPOSE";
    private int currentStateIndex = 0;

    public HandDataLoader handDataLoader;

    public Transform leftHand;
    public Transform rightHand;
    public Transform leftArm;
    public Transform leftForearm;
    public Transform rightArm;
    public Transform rightForearm;
    public Transform head;

    public (string Sign, string Hand) CurrentState { get; private set; }

    void Start()
    {

        BendArm(learningStates[0].Hand);
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

        if (currentStateIndex >= learningStates.Count - 1)
        {
            ResetToFinalPose();
            return;
        }

        var expectedState = learningStates[currentStateIndex];
        var nextState = learningStates[currentStateIndex + 1];

       
        if (performedSign == expectedState.Sign && (performedHand == expectedState.Hand || expectedState.Hand == "Any"))
        {

            //Add new learnt sign to player knowledge
            GameManager.Instance.AddNewSign(performedSign[0]);
            BendArm(nextState.Hand);

            currentStateIndex++;
            ApplyHandShape();

            
        }    

    }
    private void ResetToFinalPose()
    {
        head.localRotation = Quaternion.Euler(20.3406696f, 308.71405f, 356.256866f);
        leftArm.localRotation = Quaternion.Euler(44.6880379f, 321.45459f, 299.12439f);
        leftForearm.localRotation = Quaternion.Euler(22.5966377f, 29.5190468f, 23.2114391f);

        rightArm.localRotation = Quaternion.Euler(55.708477f, 305.393738f, 321.121948f);
        rightForearm.localRotation = Quaternion.Euler(22.5966377f, 29.5190468f, 23.2114391f);
    }

    private void BendArm(string handedness)
    {
        if (handedness == "Left")
        {
            //Bend left arm
            leftArm.localRotation = Quaternion.Euler(65, 35, 50);
            leftForearm.localRotation = Quaternion.Euler(-10, -101, -53);

            //Restore the right
            rightArm.localRotation = Quaternion.Euler(55.7084503f, 305.393738f, 337.076752f);
            rightForearm.localRotation = Quaternion.Euler(327.319855f, 5.64906311f, 115.05056f);
        }

        if (handedness == "Right")
        {
            //Bend right arm
            rightArm.localRotation = Quaternion.Euler(53, -48, -53);
            rightForearm.localRotation = Quaternion.Euler(24, 116, 70);
            //Restore left
            leftArm.localRotation = Quaternion.Euler(65.4945374f, 35.3435707f, 357.066284f);
            leftForearm.localRotation = Quaternion.Euler(316.427032f, 350.97879f, 260.720398f);
        }
    }
}
