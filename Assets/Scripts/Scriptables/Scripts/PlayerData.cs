using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName ="scriptables/playerData",fileName ="PlayerData")]
public class PlayerData : ScriptableObject
{
    [SerializeField] string playerName;
    [SerializeField] int totalCoins;
    [SerializeField] int totalGems;

    public void SetPlayerName(string name) => this.playerName = name;
    public void SetTotalCoins(int coins) => this.totalCoins = coins;
    public void SetTotalGems(int gems) => this.totalGems = gems;
}
