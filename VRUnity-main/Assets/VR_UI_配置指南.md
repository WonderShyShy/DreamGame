# 🎮 VR UI 配置详细指南

## 📋 总览
本指南将教您如何配置VR心理健康评估系统的UI界面。

## 🚀 快速配置（推荐）

### 方法一：使用自动配置脚本

1. **在Hierarchy中创建空GameObject**
   - 右键Hierarchy → Create Empty
   - 命名为 `VR UI Setup`

2. **添加VRUISetup组件**
   - 选中 `VR UI Setup` 对象
   - Inspector → Add Component → 搜索 `VRUISetup`
   - 添加组件

3. **配置参数**
   ```
   配置选项:
   ✅ Create Main Menu Panel (创建主菜单面板)
   ✅ Create Question Panel (创建问题面板)  
   ✅ Create Result Panel (创建结果面板)
   ✅ Create Event System (创建事件系统)
   
   样式设置:
   Primary Color: 青色 (Cyan)
   Secondary Color: 白色 (White)
   Background Color: 半透明黑色
   
   字体设置:
   Title Font Size: 48
   Button Font Size: 32
   Text Font Size: 28
   ```

4. **执行自动配置**
   - 在Inspector中找到 `VRUISetup` 组件
   - 右键组件标题 → `自动配置VR UI`
   - 等待配置完成（查看Console日志）

5. **设置VR摄像机引用**
   - 找到生成的Canvas对象
   - 选中Canvas → VRUIManager组件
   - 将您的VR摄像机拖入 `VR Camera` 字段

## 🛠️ 手动配置（详细步骤）

### 第一步：创建Canvas

1. **创建Canvas**
   - Hierarchy → 右键 → UI → Canvas
   - 命名为 `VR UI Canvas`

2. **配置Canvas组件**
   ```
   Canvas:
   - Render Mode: World Space ⭐
   - Event Camera: [拖入您的VR摄像机]
   
   Canvas Scaler:
   - UI Scale Mode: Constant Physical Size
   - Physical Unit: Meters
   - Reference Pixels Per Unit: 100
   
   Graphic Raycaster:
   - 保持默认设置
   ```

3. **设置Canvas Transform**
   ```
   Transform:
   - Position: (0, 2, 3)  // 距离用户3米，高度2米
   - Rotation: (0, 0, 0)
   - Scale: (0.001, 0.001, 0.001)  // 重要！VR适配尺寸
   ```

### 第二步：创建EventSystem

1. **检查EventSystem**
   - 在Hierarchy中查找是否已有EventSystem
   - 如果没有：右键Hierarchy → UI → Event System

### 第三步：创建主菜单面板

1. **创建面板**
   - 右键Canvas → UI → Panel
   - 命名为 `MainMenuPanel`

2. **配置面板**
   ```
   RectTransform:
   - Anchor Presets: Stretch (全屏填充)
   - Left, Top, Right, Bottom: 0
   
   Image:
   - Color: (0.1, 0.1, 0.1, 0.8)  // 半透明黑色背景
   ```

3. **添加标题文本**
   - 右键MainMenuPanel → UI → Text
   - 命名为 `Title`
   - 文本内容：`心理健康评估系统`
   - Position: (0, 200, 0)
   - 字体大小：48

4. **添加按钮**
   - 右键MainMenuPanel → UI → Button
   - 创建三个按钮：
     - `开始评估` (Position: 0, 0)
     - `设置` (Position: 0, -100)  
     - `退出` (Position: 0, -200)

### 第四步：创建问题面板

1. **创建面板**
   - 复制MainMenuPanel
   - 命名为 `QuestionPanel`
   - 默认设为不激活 (取消勾选左上角复选框)

2. **添加问题元素**
   ```
   元素列表:
   - 问题文本 (Position: 0, 250)
   - 进度文本 (Position: -300, 180) 
   - 进度条 (Position: 0, 180)
   - 5个答案按钮 (垂直排列，间距70)
   ```

### 第五步：创建结果面板

1. **创建面板**
   - 复制MainMenuPanel
   - 命名为 `ResultPanel`
   - 默认设为不激活

2. **添加结果元素**
   ```
   元素列表:
   - 结果标题 (Position: 0, 250)
   - 结果图标 (Position: 0, 150)
   - 结果文本 (Position: 0, 0)
   - 重新评估按钮 (Position: -150, -200)
   - 返回主菜单按钮 (Position: 150, -200)
   ```

### 第六步：配置VRUIManager

1. **添加VRUIManager组件**
   - 选中Canvas
   - Add Component → VRUIManager

2. **配置引用**
   ```
   UI Canvas设置:
   - UI Canvas: [拖入Canvas]
   - Canvas Distance: 2
   - VR Camera: [拖入VR摄像机]
   
   UI面板:
   - Main Menu Panel: [拖入主菜单面板]
   - Question Panel: [拖入问题面板]
   - Result Panel: [拖入结果面板]
   
   问题系统:
   - Question Text: [拖入问题文本组件]
   - Answer Buttons: [拖入所有答案按钮]
   - Progress Text: [拖入进度文本]
   - Progress Slider: [拖入进度条]
   
   结果显示:
   - Result Text: [拖入结果文本]
   - Result Icon: [拖入结果图标Image]
   ```

## 🎨 高级配置

### 美化UI外观

1. **按钮样式**
   - 添加 `VRUIButton` 组件到每个按钮
   - 配置颜色：
     - Normal Color: 青色
     - Hover Color: 白色
     - Press Color: 黄色

2. **面板动画**
   - 添加 `VRUIPanelAnimator` 到每个面板
   - 选择动画类型：Scale、Fade、Slide等

3. **音效配置**
   - 在Canvas上添加 AudioSource
   - 导入按钮点击音效
   - 在VRUIManager中设置Audio Clip

### VR控制器适配

1. **XR Interaction Toolkit配置**
   ```
   如果使用XR Toolkit:
   - 确保XR Ray Interactor正常工作
   - Canvas需要GraphicRaycaster组件
   - 按钮需要可选中(Selectable)
   ```

2. **手部追踪支持**
   - 配置手部射线投射
   - 测试UI交互响应

## ✅ 测试配置

### 编辑器测试

1. **键盘测试**
   - 按 T 键：测试面板切换
   - 按 1-5 键：测试答案选择
   - 按 R 键：重置测试

2. **鼠标测试**
   - 直接点击UI按钮
   - 测试悬停效果

### VR设备测试

1. **确保VR设备连接正常**
2. **测试控制器射线是否能点击UI**
3. **检查UI距离和大小是否合适**

## 🔧 常见问题解决

### Canvas不显示
```
检查项目:
- Render Mode 是否为 World Space
- Canvas位置是否正确 (0, 2, 3)
- Canvas缩放是否为 (0.001, 0.001, 0.001)
- VR Camera是否正确引用
```

### 按钮无法点击
```
检查项目:
- 是否有 GraphicRaycaster 组件
- 是否有 EventSystem
- 按钮是否有 Button 组件
- VR控制器射线是否正常工作
```

### UI太大或太小
```
解决方案:
- 调整Canvas的Scale值
- 修改Canvas Distance
- 调整字体大小
```

### 没有声音
```
检查项目:
- AudioSource组件是否存在
- Audio Clip是否正确设置
- 音频文件格式是否正确
- 系统音量设置
```

## 📁 文件结构

配置完成后，您的项目结构应该是：

```
Assets/
├── Scripts/
│   ├── VRUIManager.cs        // 主要UI管理器
│   ├── VRUIButton.cs         // VR按钮组件
│   ├── VRUISlider.cs         // VR滑块组件
│   ├── VRUIPanelAnimator.cs  // 面板动画器
│   ├── VRUIExample.cs        // 使用示例
│   └── VRUISetup.cs          // 自动配置工具
├── Audio/
│   └── button_click.wav      // 按钮点击音效
└── Scenes/
    └── VR_UI_Scene.unity     // VR UI场景
```

## 🎯 最终检查清单

- [ ] Canvas配置为World Space
- [ ] VR摄像机正确引用  
- [ ] 三个主要面板创建完成
- [ ] VRUIManager组件正确配置
- [ ] EventSystem存在
- [ ] 所有UI元素正确引用
- [ ] 按钮点击事件正常工作
- [ ] VR控制器能够交互
- [ ] 音效播放正常
- [ ] 面板动画效果正常

## 💡 使用建议

1. **首次配置建议使用自动配置脚本**
2. **根据实际VR设备调整Canvas距离和大小**
3. **可以自定义问题内容和评估逻辑**
4. **建议添加更多视觉反馈效果**
5. **考虑添加多语言支持**

---

配置完成后，您就拥有了一个功能完整的VR心理健康评估UI系统！🎉 