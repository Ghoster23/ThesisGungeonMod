using System;

namespace DynamicRandomness.Data_Logging
{
    [Serializable]
    class BattleData
    {
        private double _duration;

        private bool _victory;

        private int _damageReceived;

        private float _damageDone;

        private int _variant;


        private DateTime _battleStart;


        public BattleData(DateTime battleStart, int variant)
        {
            _battleStart = battleStart;

            _variant = variant;
        }


        public void BattleEnd(DateTime battleEnd, bool victory)
        {
            _duration = battleEnd.Subtract(_battleStart).TotalSeconds;

            _victory = victory;
        }
    }
}
