using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_Interactable
{
    Interactable_Type InteractableType{
        get;
    }
    public bool Interact();
    public void DisplayInteractionPrompt(bool immediate = false);
    public void HideInteractionPrompt(bool immediate = false);
    public bool Interactable{
        get;
    }
}

public enum Interactable_Type{
    Generic,
    Chest,
    Key,
    Person,
    Sword
}
