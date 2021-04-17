using UnityEngine;

namespace DynamicRandomness
{
    class PlaythroughData
    {
        private int order;

        private float duration;

        
        public PlaythroughData(int order, float duration)
        {
            this.order = order;

            this.duration = duration;
        }


        public string GetJSON()
        {
            return JsonUtility.ToJson(this);
        }
    }
}
