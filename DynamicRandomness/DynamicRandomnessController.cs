using UnityEngine;
using MonoMod;

namespace DynamicRandomness
{
    class DynamicRandomnessController : MonoBehaviour
    {
        private int _order = 0;

        private PlayerStats _playerStats;

        private bool _init = false;


        private void Start()
        {
            _order = Random.Range(0, 2);

            ETGModConsole.Log("Started the Dynamic Randomness Controller - Order " + _order);
        }

        private void Update()
        {

        }


        public void Init()
        {
            if (_init) return;

            _init = true;
        }
    }
}
