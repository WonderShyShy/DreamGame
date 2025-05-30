# VR UI 交互系统使用说明

## 📋 概述

这套VR UI交互系统专门为虚拟现实环境设计，提供了完整的UI交互解决方案，特别适用于心理健康评估等应用场景。

## 🎯 核心组件

### 1. VRUIManager（VR UI管理器）
主要管理VR环境中的UI系统和问卷流程。

**主要功能：**
- 管理Canvas在VR空间中的位置
- 控制问卷系统流程
- 处理用户答案和结果计算
- 提供心理健康评估逻辑

**关键设置：**
- `canvasDistance`: UI距离用户的距离（推荐2-3米）
- `vrCamera`: VR摄像机引用
- UI面板：主菜单、问题面板、结果面板

### 2. VRUIButton（VR优化按钮）
提供丰富视觉反馈的VR按钮组件。

**特色功能：**
- 悬停缩放效果
- 颜色渐变动画
- 发光效果
- 音效反馈
- 触觉震动反馈

**使用方法：**
```csharp
// 设置按钮交互状态
vrButton.SetInteractable(true);

// 自定义按钮颜色
vrButton.SetColors(Color.white, Color.cyan, Color.yellow);
```

### 3. VRUISlider（VR滑块组件）
专为VR优化的滑块，支持评分选择。

**特色功能：**
- 分段显示（适合评分量表）
- 整数捕捉
- 实时数值显示
- 平滑动画效果

**配置示例：**
```csharp
// 设置评分范围
slider.SetValueRange(1, 10);

// 监听数值变化
slider.OnValueChanged.AddListener(OnScoreChanged);
```

### 4. VRUIPanelAnimator（面板动画管理器）
控制UI面板的显示隐藏动画。

**动画类型：**
- 淡入淡出
- 缩放动画
- 滑动动画（上下左右）
- 旋转动画
- 自定义动画

**使用方法：**
```csharp
// 显示面板
panelAnimator.Show();

// 隐藏面板
panelAnimator.Hide();

// 切换显示状态
panelAnimator.Toggle();
```

## 🚀 快速开始

### 第一步：设置基础Canvas
1. 创建一个Canvas，设置为World Space模式
2. 添加`VRUIManager`脚本
3. 配置VR摄像机引用
4. 设置Canvas距离（推荐2米）

### 第二步：创建UI面板
1. 在Canvas下创建面板GameObject
2. 添加`VRUIPanelAnimator`组件
3. 选择合适的动画类型
4. 配置动画参数

### 第三步：添加交互元素
1. 为按钮添加`VRUIButton`组件
2. 为滑块添加`VRUISlider`组件
3. 配置视觉效果和音效
4. 设置事件监听

### 第四步：配置XR交互
1. 确保场景中有XR Origin
2. 添加XR Ray Interactor到控制器
3. 在Canvas上添加GraphicRaycaster组件
4. 添加EventSystem到场景

## 💡 心理健康评估实现

### 问卷系统设置
```csharp
// 在VRUIManager中添加问题
questions.Add(new QuestionData
{
    questionText = "您最近的情绪状态如何？",
    answers = new string[] { "很好", "较好", "一般", "较差", "很差" },
    scores = new int[] { 5, 4, 3, 2, 1 }
});
```

### 评估结果计算
系统会自动计算总分并生成评估报告：
- 80%以上：心理健康状态良好
- 60-80%：基本正常
- 40-60%：存在压力
- 40%以下：建议寻求专业帮助

## 🎮 交互方式

### VR控制器交互
- **射线指向**：使用控制器射线指向UI元素
- **触发选择**：按下扳机键选择
- **握拳拖拽**：握拳时可拖拽滑块

### 手部追踪交互
- **直接触摸**：手指直接触摸UI元素
- **悬停反馈**：手指接近时显示高亮
- **手势确认**：点击手势确认选择

### 键盘测试（开发阶段）
- **空格键**：切换主面板显示
- **数字键1**：显示主菜单
- **数字键2**：显示设置面板
- **ESC键**：隐藏所有面板

## 🔧 自定义配置

### 视觉效果定制
```csharp
// 自定义按钮颜色
[Header("按钮颜色设置")]
public Color normalColor = Color.white;
public Color hoverColor = Color.cyan;
public Color pressColor = Color.yellow;

// 自定义动画曲线
[Header("动画设置")]
public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
public float animationDuration = 0.2f;
```

### 音效配置
```csharp
[Header("音效设置")]
public AudioClip hoverSound;
public AudioClip clickSound;
public AudioClip valueChangeSound;
```

### 触觉反馈设置
```csharp
[Header("触觉反馈")]
public bool enableHapticFeedback = true;
public float hapticIntensity = 0.3f;
public float hapticDuration = 0.1f;
```

## 📱 最佳实践

### UI布局建议
- UI元素距离用户1.5-3米
- 按钮最小尺寸：世界空间中0.3x0.3米
- 文字大小：至少24号字体
- 颜色对比度：确保在VR中清晰可见

### 性能优化
- 使用对象池管理UI元素
- 合理控制同时显示的UI数量
- 优化材质和纹理大小
- 避免频繁的UI布局重计算

### 用户体验
- 提供清晰的视觉反馈
- 添加音效和触觉反馈
- 保持一致的交互方式
- 提供操作指引和帮助

## 🐛 常见问题

### Q: UI无法点击？
A: 检查以下项目：
- Canvas是否有GraphicRaycaster组件
- 场景中是否有EventSystem
- XR Ray Interactor是否正确配置
- UI元素是否在可交互层级

### Q: 动画效果不流畅？
A: 建议：
- 检查animationDuration设置
- 确认AnimationCurve配置
- 减少同时播放的动画数量
- 优化Update方法调用频率

### Q: 音效无法播放？
A: 检查：
- AudioSource组件是否添加
- 音频文件是否正确导入
- 音量设置是否合适
- 是否在VR中音频空间化设置

## 📧 技术支持

如果您在使用过程中遇到问题，请检查：
1. Unity版本兼容性（推荐2022.3 LTS）
2. XR Interaction Toolkit版本
3. 相关依赖包是否完整安装
4. VR设备驱动是否正常

---

**注意：** 此系统专为VR环境设计，在普通桌面环境下可能需要额外的鼠标/键盘适配。 