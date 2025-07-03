using UnityEngine;
using Lofelt.NiceVibrations;

namespace UI
{
    /// <summary>
    /// 震动测试辅助工具
    /// 可以在编辑器中或运行时测试不同的震动效果
    /// </summary>
    public class VibrationTestHelper : MonoBehaviour
    {
        [Header("震动测试")]
        [SerializeField] private bool enableDebugLog = true;
        
        [Header("快捷测试按钮")]
        [SerializeField] private KeyCode testSelectionKey = KeyCode.Alpha1;
        [SerializeField] private KeyCode testLightImpactKey = KeyCode.Alpha2;
        [SerializeField] private KeyCode testMediumImpactKey = KeyCode.Alpha3;
        [SerializeField] private KeyCode testSuccessKey = KeyCode.Alpha4;
        [SerializeField] private KeyCode testHeavyImpactKey = KeyCode.Alpha5;
        
        void Update()
        {
            // 仅在调试模式下启用键盘测试
            if (enableDebugLog && Application.isEditor)
            {
                if (Input.GetKeyDown(testSelectionKey))
                {
                    TestVibration(HapticPatterns.PresetType.Selection, "格子吸附震动");
                }
                
                if (Input.GetKeyDown(testLightImpactKey))
                {
                    TestVibration(HapticPatterns.PresetType.LightImpact, "快速跨越震动");
                }
                
                if (Input.GetKeyDown(testMediumImpactKey))
                {
                    TestVibration(HapticPatterns.PresetType.MediumImpact, "方块放置震动");
                }
                
                if (Input.GetKeyDown(testSuccessKey))
                {
                    TestVibration(HapticPatterns.PresetType.Success, "单行消除震动");
                }
                
                if (Input.GetKeyDown(testHeavyImpactKey))
                {
                    TestVibration(HapticPatterns.PresetType.HeavyImpact, "多行消除/特殊方块震动");
                }
            }
        }
        
        /// <summary>
        /// 测试指定类型的震动
        /// </summary>
        public void TestVibration(HapticPatterns.PresetType presetType, string description)
        {
            try
            {
                HapticPatterns.PlayPreset(presetType);
                
                if (enableDebugLog)
                {
                    Debug.Log($"🎮 震动测试: {description} ({presetType})");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"震动测试失败: {e.Message}");
            }
        }
        
        /// <summary>
        /// 测试所有震动类型
        /// </summary>
        [ContextMenu("测试所有震动类型")]
        public void TestAllVibrations()
        {
            StartCoroutine(TestAllVibrationsCoroutine());
        }
        
        private System.Collections.IEnumerator TestAllVibrationsCoroutine()
        {
            var vibrationTypes = new[]
            {
                (HapticPatterns.PresetType.Selection, "Selection - 选择"),
                (HapticPatterns.PresetType.LightImpact, "LightImpact - 轻度冲击"),
                (HapticPatterns.PresetType.MediumImpact, "MediumImpact - 中度冲击"),
                (HapticPatterns.PresetType.HeavyImpact, "HeavyImpact - 重度冲击"),
                (HapticPatterns.PresetType.Success, "Success - 成功"),
                (HapticPatterns.PresetType.Warning, "Warning - 警告"),
                (HapticPatterns.PresetType.Failure, "Failure - 失败")
            };
            
            foreach (var (type, description) in vibrationTypes)
            {
                TestVibration(type, description);
                yield return new WaitForSeconds(0.8f); // 间隔0.8秒
            }
        }
        
        void OnGUI()
        {
            if (!enableDebugLog) return;
            
            GUILayout.BeginArea(new Rect(10, 10, 300, 280));
            GUILayout.Label("震动测试面板", GUI.skin.box);
            
            GUILayout.Label("拖动震动:", GUI.skin.label);
            if (GUILayout.Button("格子吸附震动 (Selection)"))
            {
                TestVibration(HapticPatterns.PresetType.Selection, "格子吸附震动");
            }
            
            if (GUILayout.Button("快速跨越震动 (LightImpact)"))
            {
                TestVibration(HapticPatterns.PresetType.LightImpact, "快速跨越震动");
            }
            
            if (GUILayout.Button("方块放置震动 (MediumImpact)"))
            {
                TestVibration(HapticPatterns.PresetType.MediumImpact, "方块放置震动");
            }
            
            GUILayout.Space(10);
            GUILayout.Label("消除震动:", GUI.skin.label);
            if (GUILayout.Button("单行消除震动 (Success)"))
            {
                TestVibration(HapticPatterns.PresetType.Success, "单行消除震动");
            }
            
            if (GUILayout.Button("多行消除震动 (HeavyImpact)"))
            {
                TestVibration(HapticPatterns.PresetType.HeavyImpact, "多行消除震动");
            }
            
            if (GUILayout.Button("特殊方块消除震动 (HeavyImpact)"))
            {
                TestVibration(HapticPatterns.PresetType.HeavyImpact, "特殊方块消除震动");
            }
            
            GUILayout.Space(10);
            if (GUILayout.Button("测试所有震动类型"))
            {
                TestAllVibrations();
            }
            
            GUILayout.Space(10);
            GUILayout.Label("快捷键:");
            GUILayout.Label("1 - 格子吸附震动");
            GUILayout.Label("2 - 快速跨越震动");
            GUILayout.Label("3 - 方块放置震动");
            GUILayout.Label("4 - 单行消除震动");
            GUILayout.Label("5 - 多行消除震动");
            
            GUILayout.EndArea();
        }
    }
} 