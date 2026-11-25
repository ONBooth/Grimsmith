using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueManager", menuName = "Scriptable Objects/DialogueManager")]
#region DialogueManager
public class DialogueManager : ScriptableObject
{
    [Header("DialogueManager")]
    [Space]
    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue;
    
    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkjSON;


    public void EnterDialogue(TextAsset inkJSON , Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<StaffID>();
            other.GetComponent<Dialogue>();
        }
    }

    public void ExitDialogue(TextAsset inkJSON)
    {
    
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerMovement>();
        }
    }
    private void OnTriggerExit(Collider other)
    {

    }

}
#endregion 