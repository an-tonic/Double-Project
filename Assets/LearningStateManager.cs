using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearningStateManager : MonoBehaviour
{
    private List<string> learningStates = new List<string> { "G", "F" };
    private int currentStateIndex = 0;
    public HandDataLoader handDataLoader;

    // Current state property
    public string CurrentState { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        
        CurrentState = learningStates[currentStateIndex];
        Debug.Log("Starting learning state: " + CurrentState);
        handDataLoader.LoadHandData(CurrentState);
        
    }

  

    public void ChangeState(string nextState)
    {
        // Check if the requested state is the current state + 1 in sequence
        if (learningStates.Contains(nextState))
        {
            int nextIndex = learningStates.IndexOf(nextState);

            if (nextIndex == currentStateIndex + 1)
            {
                currentStateIndex = nextIndex;
                CurrentState = learningStates[currentStateIndex];
                Debug.Log("Changed to learning state: " + CurrentState);
                handDataLoader.LoadHandData(CurrentState);
            }
            else
            {
                Debug.LogWarning("Cannot skip states. Current state is: " + CurrentState);
            }
        }
        else
        {
            Debug.LogWarning("Invalid state: " + nextState);
        }
    }

}
