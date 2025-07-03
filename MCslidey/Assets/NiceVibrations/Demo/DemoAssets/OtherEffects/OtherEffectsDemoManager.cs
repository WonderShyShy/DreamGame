// Copyright (c) Meta Platforms, Inc. and affiliates. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lofelt.NiceVibrations
{
    public class OtherEffectsDemoManager : DemoManager
    {
        public HapticSource hapticSource;
        
        public Transform content;
        public GameObject togglePrefab;
        public List<Toggle> toggles;
        
        public ClipListSO clipList;
        public List<HapticClipWithName> hapticClips;
        
        private int _currentClipIndex = 0;

        protected virtual void Awake()
        {
            hapticClips = clipList.clips;
            List<string> options = new List<string>();
            foreach (HapticClipWithName clip in hapticClips)
            {
                options.Add(clip.name);
            }

            foreach (var hapticClip in hapticClips)
            {
                GameObject toggle = Instantiate(togglePrefab, content);
                toggle.SetActive(true);
                Toggle t = toggle.GetComponent<Toggle>();
                toggles.Add(t);
                t.onValueChanged.AddListener(v =>
                {
                    if (v)
                    {
                        _currentClipIndex = hapticClips.IndexOf(hapticClip);
                        toggles.ForEach(tg =>
                        {
                            if (tg != t)
                            {
                                tg.isOn = false;
                            }
                        });
                    }
                });
                
                Text text = toggle.GetComponentInChildren<Text>();
                text.text = hapticClip.name;
            }
            
            /*dropdown.ClearOptions();
            dropdown.AddOptions(options);
            dropdown.onValueChanged.AddListener(v=>
            {
                _currentClipIndex = v;
            });*/
        }

        public virtual void EffectButton()
        {
            hapticSource.clip = hapticClips[_currentClipIndex].clip;
            hapticSource.Play();
        }
    }
}
