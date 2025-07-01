using System;
using System.Collections.Generic;
using Models;
using Other;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ReadyGroup : MonoBehaviour
    {
        public GameObject readyPrefab;
        public Sprite specialSpriteFrame;
        public Sprite specialBronzeSpriteFrame;
        public Sprite specialGoldSpriteFrame;
        public Sprite normalSpriteFrame;

        private readonly List<GameObject> _itemList = new List<GameObject>();
        private ObjectPool _readyItemsPool;
        private Color[] _readyItemColors = null;
        private Color _bronzeColor = new Color(198 / 255f, 43 / 255f, 166 / 255f, 255 / 255f);
        private Color _goldColor = new Color(181 / 255f, 37 / 255f, 25 / 255f, 255 / 255f);
        private readonly Color _stoneColor = new Color(208 / 255f, 205 / 255f, 246 / 255f, 255 / 255f);
        private int _itemOffsetY = -18;

        public void InitRes()
        {
            if (_readyItemsPool == null)
            {
                _readyItemsPool = new ObjectPool();
                if (Constant.SceneVersion == "1" || Constant.SceneVersion == "2")
                {
                    _itemOffsetY = -12;
                } else if (Constant.SceneVersion == "3")
                {
                    _goldColor = new Color(211 / 255f, 154 / 255f, 53 / 255f, 1f);
                    _bronzeColor = new Color(98 / 255f, 215 / 255f, 241 / 255f, 1f);
                }
                _readyItemsPool.CreateObject(readyPrefab, 4);
            }
        }
        
        public void UpdateReadyGroup(List<int[]> blocksData)
        {
            if (_readyItemColors == null)
            {
                _readyItemColors = new Color[Enum.GetNames(typeof(Blocks.Color)).Length + 1];
                _readyItemColors[0] = new Color(255 / 255f, 255 / 255f, 255 / 255f, 1);
                _readyItemColors[1] = new Color(2 / 255f, 91 / 255f, 255 / 255f, 1);
                _readyItemColors[2] = new Color(126 / 255f, 255 / 255f, 0 / 255f, 1);
                _readyItemColors[3] = new Color(255 / 255f, 52 / 255f, 239 / 255f, 1);
                _readyItemColors[4] = new Color(255 / 255f, 71 / 255f, 16 / 255f, 1);
                _readyItemColors[5] = new Color(255 / 255f, 236 / 255f, 19 / 255f, 1);
            }
            
            if (Constant.SceneVersion == "3")
            {
                _readyItemColors[0].r = 19 / 255f;
                _readyItemColors[0].g = 130 / 255f;
                _readyItemColors[0].b = 1f;
                _readyItemColors[0].a = 1f;
            }
            
            foreach (var item in _itemList)
            {
                _readyItemsPool.Put(item);
            }
            _itemList.Clear();
            
            foreach (var data in blocksData)
            {
                var item = _readyItemsPool.Get();
                item.transform.SetParent(transform);
                item.transform.Find("icon").gameObject.SetActive(false);
                item.transform.localScale = Vector2.one;
                item.transform.localPosition = new Vector2(Constant.BlockGroupEdgeLeft + Constant.BlockWidth * data[(int)Blocks.Key.Pos], _itemOffsetY);
                item.GetComponent<RectTransform>().sizeDelta = new Vector2(Constant.BlockWidth * data[(int)Blocks.Key.Length], item.GetComponent<RectTransform>().sizeDelta.y);
                
                if (data[(int) Blocks.Key.Special] != 0)
                {
                    switch (data[(int) Blocks.Key.Special])
                    {
                        case (int) Blocks.Special.Rainbow:
                            item.GetComponent<Image>().sprite = specialSpriteFrame;
                            item.GetComponent<Image>().color = _readyItemColors[0];
                            break;
                        case (int) Blocks.Special.Bronze:
                            item.GetComponent<Image>().sprite = normalSpriteFrame;
                            item.GetComponent<Image>().color = _bronzeColor;
                            
                            item.transform.Find("icon").gameObject.SetActive(true);
                            item.transform.Find("icon").GetComponent<Image>().sprite = specialBronzeSpriteFrame;
                            item.transform.Find("icon").GetComponent<Image>().SetNativeSize();
                            break;
                        case (int) Blocks.Special.Gold:
                            item.GetComponent<Image>().sprite = normalSpriteFrame;
                            item.GetComponent<Image>().color = _goldColor;
                            
                            item.transform.Find("icon").gameObject.SetActive(true);
                            item.transform.Find("icon").GetComponent<Image>().sprite = specialGoldSpriteFrame;
                            item.transform.Find("icon").GetComponent<Image>().SetNativeSize();
                            break;
                        case (int) Blocks.Special.Stone:
                            item.GetComponent<Image>().sprite = normalSpriteFrame;
                            item.GetComponent<Image>().color = _stoneColor;
                            break;
                    }
                }
                else
                {
                    item.GetComponent<Image>().sprite = normalSpriteFrame;

                    if (Constant.SceneVersion != "3")
                    {
                        item.GetComponent<Image>().color = _readyItemColors[data[(int)Blocks.Key.Color]];
                    }
                    else
                    {
                        item.GetComponent<Image>().color = _readyItemColors[0];
                    }
                }
                
                _itemList.Add(item);
            }
        }
    }
}
