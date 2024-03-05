using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "scriptables/UI_Data", fileName = "UI_Data")]
public class UI_Data : ScriptableObject
{
    // its gonna keep track of the current UI states
    // e.g whether the game is mute or not and other stuff like that

    [Tooltip("is mute or not")]
    public bool audioIsMute = true; // we gotta read this data during the game as well
    [Tooltip("Current coins the player has collected so far")]
    public int totalCoins = 0;

    [Tooltip("drop all the images (character selection images) to keep them remain unlocked each time")]
    public bool[] characterKartIndices = new bool[6];
    [Tooltip("indices of those karts that the player will be able to select for the gameplay")]
    public bool[] allSelectableKarts = new bool[6];
    // so if the 3rd kart is unlocked, its gonna be like
    // characterkartIndices[2] = true;


    // this data would be temporary just for counting how many enemies have been killed
    public int totalEnemiesKilled = 0;



    // data for controls, either joystick or arrow ones
    [Tooltip("if its false then the user is playing with joystick")]
    public bool isUsingArrows = true;
}
