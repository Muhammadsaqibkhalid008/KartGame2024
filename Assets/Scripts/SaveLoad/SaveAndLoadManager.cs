using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.SaveLoad;
using System.Linq;

public class SaveAndLoadManager : MonoBehaviour
{
    [SerializeField] string filename = "";
    [SerializeField] UI_Data ui_data;
    List<PlayerData_2> players = new List<PlayerData_2>();



    private void Start()
    {
        /*bool[] values = { true, false };
        bool[] values_2 = { true, false, false, true };
        PlayerData_2 data = new PlayerData_2(2, false, values, values_2, 5, false);

        players.Add(data);

        FileHandler.SaveToJSON(players, filename);*/

        //Debug.Log($"reading data {FileHandler.ReadFromJSON<PlayerData_2>(filename)}");
        /* List<PlayerData_2> pd = FileHandler.ReadFromJSON<PlayerData_2>(filename);
         foreach (var item in pd)
         {
             Debug.Log($"{item.totalCoins}, {item.muteState}");
         }*/
    }

    public void SaveData()
    {
        PlayerData_2 playerData = new PlayerData_2(
            ui_data.totalCoins,
            ui_data.audioIsMute,
            ui_data.characterKartIndices,
            ui_data.allSelectableKarts,
            ui_data.totalEnemiesKilled,
            ui_data.isUsingArrows
            );
        players.Add(playerData);
        FileHandler.SaveToJSON<PlayerData_2>(players, filename);
    }
    public void SaveInitial()
    {
        bool[] values = { false, false, false, false, false, false };
        bool[] values_2 = { false, false, false, false, false, false };
        PlayerData_2 playerData = new PlayerData_2(
            0,
            false,
            values,
            values_2,
            0,
            false
            );
        players.Add(playerData);
        FileHandler.SaveToJSON<PlayerData_2>(players, filename);
    }
    public void LoadData()
    {
        List<PlayerData_2> data = FileHandler.ReadFromJSON<PlayerData_2>(filename);
        PlayerData_2 mainData = data[0];
        ui_data.totalCoins = mainData.totalCoins;
        ui_data.audioIsMute = mainData.muteState;
        ui_data.characterKartIndices = mainData.characterKartIndices;
        ui_data.allSelectableKarts = mainData.allSelectableKarts;
        ui_data.isUsingArrows = mainData.isUsingArrows;
    }
}
