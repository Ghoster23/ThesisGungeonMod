using UnityEngine;
using System.Collections.Generic;
using MonoMod;

namespace DynamicRandomness
{
    class DynamicRandomnessController : MonoBehaviour
    {
        private bool _init = false;


        private void Start()
        {
            ETGModConsole.Log("Started the Dynamic Randomness Controller - Order " + Module.Order);
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
