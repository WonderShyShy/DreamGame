using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BgGroupEff : MonoBehaviour
    {
        public GameObject bg;
        public GameObject moon;
        
        public Sprite[] bgSprites;
        public Sprite[] moonSprites;

        public void UpdateEffByLevel(int level)
        {
            var bgIndex = level % bgSprites.Length;
            var moonIndex = level % moonSprites.Length;

            bg.GetComponent<Image>().sprite = bgSprites[bgIndex];
            moon.GetComponent<Image>().sprite = moonSprites[moonIndex];
        }
    }
}
