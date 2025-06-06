using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models;
using Other;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Manager
{
    public static class ManagerAudio
    {
        private static GameObject _audioGameObject;
        
        private static string _musicEnabled = "on";
        private static float _musicVolume = 1.0f;
        private static string _soundEnabled = "on";
        private static float _soundVolume = 1.0f;

        private static AudioSource _musicAudioSource;
        private static List<AudioSource> _unusedAudioSources;
        private static List<AudioSource> _usedAudioSources;
        private static Dictionary<string, AudioClip> _audioClips;
        private const int MaxCount = 7;

        public static void InitData(GameObject rootNode)
        {
            _unusedAudioSources = new List<AudioSource>();
            _usedAudioSources = new List<AudioSource>();
            _audioClips = new Dictionary<string, AudioClip>();
            
            if (ManagerLocalData.HaveData(ManagerLocalData.MUSIC_ON_OFF))
            {
                _musicEnabled = ManagerLocalData.GetStringData(ManagerLocalData.MUSIC_ON_OFF);
                _musicVolume = ManagerLocalData.GetFloatData(ManagerLocalData.MUSIC_VOLUME);
                _soundEnabled = ManagerLocalData.GetStringData(ManagerLocalData.SOUND_ON_OFF);
                _soundVolume = ManagerLocalData.GetFloatData(ManagerLocalData.SOUND_VOLUME);
            }
            else
            {
                SetMusicOn(true);
                SetSoundOn(true);
                SetMusicVolume(_musicVolume);
                SetSoundVolume(_soundVolume);
            }

            _audioGameObject = rootNode;
        }

        private static void UsedToUnused(AudioSource audioSource)
        {
            if (audioSource != null)
            {
                if (_usedAudioSources.Contains(audioSource))
                {
                    _usedAudioSources.Remove(audioSource);
                }

                if (!_unusedAudioSources.Contains(audioSource))
                {
                    if (_unusedAudioSources.Count >= MaxCount)
                    {
                        Object.Destroy(audioSource);
                    }
                    else
                    {
                        _unusedAudioSources.Add(audioSource);
                    }
                }
            }
        }

        private static AudioSource UnusedToUsed()
        {
            AudioSource audioSource;
            if (_unusedAudioSources.Count > 0)
            {
                audioSource = _unusedAudioSources[0];
                _unusedAudioSources.RemoveAt(0);
            }
            else
            {
                audioSource = _audioGameObject.AddComponent<AudioSource>();
            }
            _usedAudioSources.Add(audioSource);
            return audioSource;
        }

        private static IEnumerator WaitPlayEnd(AudioSource audioSource, Action action = null)
        {
            yield return new WaitUntil(() => !audioSource.isPlaying);
            UsedToUnused(audioSource);
            action?.Invoke();
        }

        public static async void PlayMusic(string bgmPath, float volume = 0.0f)
        {
            _musicAudioSource = UnusedToUsed();

            var clip = Resources.Load<AudioClip>(bgmPath);
            _musicAudioSource.clip = clip;
            _musicAudioSource.playOnAwake = false;
            _musicAudioSource.loop = true;
            _musicAudioSource.volume = volume > 0.0f ? volume : _musicVolume;
            _musicAudioSource.Play();
    
            if (!GetMusicEnabled())
            {
                _musicAudioSource.mute = true;
                _musicAudioSource.Pause();
            }
        }
        
        private static void sendAudioClickAF()
        {
            if (Constant.AFData.setting_sound)
            {
                return;
            }
            
            Constant.AFData.setting_sound = true;
            ManagerLocalData.SetTableData(ManagerLocalData.AF, Constant.AFData);
        }

        public static async void PlaySound(string soundPath, float volume = 0.0f)
        {
            if (!GetSoundEnabled()) return;
            
            var audioSource = UnusedToUsed();

            if (!_audioClips.ContainsKey(soundPath))
            {
                var clip = Resources.Load<AudioClip>(soundPath);
                if (!_audioClips.ContainsKey(soundPath))
                {
                    _audioClips.Add(soundPath, clip);
                }
            }

            var audioClip = _audioClips[soundPath];
            audioSource.clip = audioClip;
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.volume = volume > 0.0f? volume: _soundVolume;
            audioSource.Play();
            Constant.GamePlayScript.StartCoroutine(WaitPlayEnd(audioSource));
        }

        public static void PauseBgm()
        {
            if (_musicAudioSource != null && _musicAudioSource.isPlaying)
            {
                _musicAudioSource.Pause();
            }
        }

        public static void ResumeBgm()
        {    
            if (_musicAudioSource != null && !_musicAudioSource.isPlaying)
            {
                _musicAudioSource.Play();
            }
        }

        public static void SetMusicOn(bool isInit = false)
        {
            _musicEnabled = "on";
            ManagerLocalData.SetStringData(ManagerLocalData.MUSIC_ON_OFF, _musicEnabled);
            ManagerLocalData.SaveData();

            if (_musicAudioSource != null)
            {
                _musicAudioSource.mute = false;
                _musicAudioSource.Play();
            }

            if (!isInit)
            {
                sendAudioClickAF();
            }

        }

        public static void SetMusicOff()
        {
            _musicEnabled = "off";
            ManagerLocalData.SetStringData(ManagerLocalData.MUSIC_ON_OFF, _musicEnabled);
            ManagerLocalData.SaveData();
            
            if (_musicAudioSource != null)
            {
                _musicAudioSource.mute = true;
                _musicAudioSource.Pause();
            }

            sendAudioClickAF();
        }

        public static bool GetMusicEnabled()
        {
            return _musicEnabled == "on";
        }

        public static void SetSoundOn(bool isInit = false)
        {
            _soundEnabled = "on";
            ManagerLocalData.SetStringData(ManagerLocalData.SOUND_ON_OFF, _soundEnabled);
            ManagerLocalData.SaveData();

            if (!isInit)
            {
                sendAudioClickAF();
            }
        }

        public static void SetSoundOff()
        {
            _soundEnabled = "off";
            ManagerLocalData.SetStringData(ManagerLocalData.SOUND_ON_OFF, _soundEnabled);
            ManagerLocalData.SaveData();
            sendAudioClickAF();
        }

        public static bool GetSoundEnabled()
        {
            return _soundEnabled == "on";
        }
        
        public static void SetMusicVolume(float volume)
        {
            _musicVolume = volume;
            if (_musicAudioSource != null && _musicAudioSource.isPlaying)
            {
                _musicAudioSource.volume = _musicVolume;
            }
            ManagerLocalData.SetFloatData(ManagerLocalData.MUSIC_VOLUME, _musicVolume);
            ManagerLocalData.SaveData();
        }

        public static void SetSoundVolume(float volume)
        {
            _soundVolume = volume;
            ManagerLocalData.SetFloatData(ManagerLocalData.SOUND_VOLUME, _musicVolume);
            ManagerLocalData.SaveData();
        }

        public static async Task<bool> LoadSoundRes(string soundAddressableName)
        {
            var tmpClip = Resources.Load<AudioClip>(soundAddressableName);
            if (!_audioClips.ContainsKey(soundAddressableName))
            {
                _audioClips.Add(soundAddressableName, tmpClip);
            }
            return true;
        }
    }
}
