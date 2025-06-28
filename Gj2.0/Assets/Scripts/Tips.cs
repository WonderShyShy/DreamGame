using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Tips:MonoBehaviour
    {
        public GameObject obj;
        public GameData data;

        private void Start()
        {
            data=GameData.Instance;
        }

        private void Update()
        {
            float minHeight = 0f;
            float displayHeight = Mathf.Max(minHeight, data.currentHeight);
            obj.transform.position = new Vector3(-1, displayHeight, 0);
        }
    }
}