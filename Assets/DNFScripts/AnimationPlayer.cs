using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPlayer : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Press Space to trigger animation
        {
            animator.Play("CINEMA_4D_Main"); // Replace with your animation name
        }
    }
}
