using UnityEngine;
using System;
using System.Collections.Generic;

namespace DynamicRandomness.Data_Logging
{
    [Serializable]
    public class PlaythroughData
    {
        public int order;

        public float duration;


        public List<BattleData> athenaBattles;

        public List<BattleData> hermesBattles;

        public List<BattleData> aresBattles;

        public int deathCount = 0;


        public bool tutorialCompleted = false;


        private float startTime;


        private BattleData currentBattle;

        public PlaythroughData(int order, float start)
        {
            this.order = order;

            this.startTime = start;

            this.athenaBattles = new List<BattleData>();
            this.hermesBattles = new List<BattleData>();
            this.aresBattles   = new List<BattleData>();
        }


        public void StartBattleLog(int variant)
        {
            this.currentBattle = new BattleData(variant);

            switch(variant)
            {
                default:
                case 1:
                    this.athenaBattles.Add(this.currentBattle);
                    break;

                case 2:
                    this.hermesBattles.Add(this.currentBattle);
                    break;

                case 3:
                    this.aresBattles.Add(this.currentBattle);
                    break;
            }
        }

        public void EndBattleLog(float duration, float playerHealth, float bossHealth)
        {
            this.currentBattle.BattleEnd(duration, playerHealth, bossHealth);
        }

        public void UpdateBattleState(int stateId)
        {
            this.currentBattle.UpdateBattleState(stateId);
        }


        public void IncrementDeathCount()
        {
            this.deathCount++;
        }

        public void TutorialComplete()
        {
            this.tutorialCompleted = true;
        }


        public string GetJSON()
        {
            this.duration = Time.time - startTime;

            this.currentBattle = null;

            var json = JsonUtility.ToJson(this);
            json = json.Remove(json.Length - 1) + ",";

            if(athenaBattles.Count > 0)
                json += BattleListToJson("athenaBattles", athenaBattles) + ",";

            if(hermesBattles.Count > 0)
                json += BattleListToJson("hermesBattles", hermesBattles) + ",";

            if (aresBattles.Count > 0)
                json += BattleListToJson("aresBattles", aresBattles);
            else
                json = json.Remove(json.Length - 1);

            return json + "}\n";
        }

        private string BattleListToJson(string listName, List<BattleData> battles)
        {
            var jsonList = listName + ":" + "[";

            foreach (var battle in battles)
                jsonList += JsonUtility.ToJson(battle) + ",";

            jsonList = jsonList.Remove(jsonList.Length - 1);

            return jsonList + "]";
        }
    }
}
