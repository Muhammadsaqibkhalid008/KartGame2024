using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.SaveLoad
{
    [Serializable]
    public class PlayerData_2
    {
        public int totalCoins;
        public bool muteState;
        // private members
        public bool[] characterKartIndices;
        public bool[] allSelectableKarts;
        public int totalEnemiesKilled;
        public bool isUsingArrows;

        public PlayerData_2(int totalCoins, bool muteState, bool[] characterKartIndices, bool[] allSelectableKarts, int totalEnemiesKilled, bool isUsingArrows)
        {
            this.totalCoins = totalCoins;
            this.muteState = muteState;
            this.characterKartIndices = characterKartIndices;
            this.allSelectableKarts = allSelectableKarts;
            this.totalEnemiesKilled = totalEnemiesKilled;
            this.isUsingArrows = isUsingArrows;
        }
    }
}
