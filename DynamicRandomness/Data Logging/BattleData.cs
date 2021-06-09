using System;

namespace DynamicRandomness.Data_Logging
{
    [Serializable]
    public class BattleData
    {
        public double duration;

        public bool victory;

        public float playerHealth;

        public float bossHealth;

        public int variant;

        public int[] battleStates;


        public BattleData(int variant)
        {
            this.variant = variant;
        }


        public void BattleEnd(double duration, float playerHealth, float bossHealth)
        {
            this.duration = duration;

            this.playerHealth = playerHealth;

            this.bossHealth = bossHealth;

            this.victory = this.playerHealth >= 0 && this.bossHealth <= 0;
        }

        public void UpdateBattleState(int stateId)
        {
            if (variant != 2) return;

            if (battleStates == null) battleStates = new int[1] { stateId };

            else
            {
                var newArray = new int[battleStates.Length + 1];

                battleStates.CopyTo(newArray, 0);
                newArray[battleStates.Length] = stateId;

                battleStates = newArray;
            }
        }
    }
}
