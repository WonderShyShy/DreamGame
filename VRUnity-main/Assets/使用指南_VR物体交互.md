# 🎮 VR物体交互系统 - 使用指南

## 🎯 快速开始（5分钟配置）

### 第一步：创建交互管理器
1. 在Hierarchy中右键 → Create Empty
2. 命名为 `VR_Interaction_Manager`
3. 选中这个对象，在Inspector中点击 `Add Component`
4. 搜索并添加 `VRObjectInteraction` 脚本

### 第二步：配置VR引用
在VRObjectInteraction组件中设置：
```
VR设置:
- VR Camera: 拖入您的VR摄像机 [必需]
- Left Controller: 拖入左手控制器 [可选]
- Right Controller: 拖入右手控制器 [可选]

射线设置:
- Ray Distance: 10 (射线距离)
- Interactable Layer: Default

视觉反馈:
- Normal Ray Color: 蓝色
- Hit Ray Color: 绿色
```

### 第三步：让物体可交互
1. 选中场景中任何物体（桌子、椅子、花瓶等）
2. 在Inspector中点击 `Add Component`
3. 搜索并添加 `InteractableObject` 脚本
4. 配置物体名称和交互类型

## 🧪 测试交互

### 编辑器测试（开发阶段）
运行场景后，使用以下按键测试：

| 按键 | 功能 |
|------|------|
| **Q** | 模拟左手控制器点击 |
| **E** | 模拟右手控制器点击 |
| **Space** | 鼠标指向物体并按空格测试 |

### VR设备测试
1. 戴上VR头显
2. 用控制器射线指向可交互物体
3. 按下控制器的触发键（通常是食指按键）
4. 观察物体的视觉反馈

## 📱 交互效果说明

### 视觉反馈
- **正常状态**：物体保持原色
- **射线悬停**：物体发出黄色光芒
- **点击交互**：物体闪绿色光
- **射线颜色**：蓝色（正常）→ 绿色（指向物体）

### 交互类型
1. **OneTime**（一次性）：只能交互一次，适合开关、按钮
2. **Repeatable**（重复）：可以多次交互，适合敲击物体
3. **Toggle**（开关）：切换开/关状态，适合灯光控制
4. **Cooldown**（冷却）：有冷却时间，防止频繁点击

## 🎨 自定义配置

### 修改物体交互设置
选中可交互物体，在InteractableObject组件中：

```
交互设置:
- Object Name: "我的桌子"
- Interaction Text: "点击查看"

视觉反馈:
- Enable Hover Effect: ✅
- Hover Color: 黄色
- Interact Color: 绿色

交互类型:
- Interaction Type: Repeatable
- Cooldown Time: 1.0秒
```

### 添加音效
```
音效设置:
- Hover Sound: 拖入悬停音效文件
- Interact Sound: 拖入交互音效文件
```

### 添加自定义事件
在InteractableObject的"事件"部分：
```
On Interaction Event:
- 点击 + 添加新事件
- 拖入目标对象
- 选择要调用的方法
```

## 🔍 实际使用示例

### 示例1：创建可点击的桌子
1. 选中场景中的桌子模型
2. 添加InteractableObject脚本
3. 设置：
   - Object Name: "木制桌子"
   - Interaction Type: Repeatable
   - 悬停颜色设为木色调

### 示例2：创建灯光开关
1. 选中墙上的开关模型
2. 添加InteractableObject脚本
3. 设置：
   - Object Name: "房间灯开关"
   - Interaction Type: Toggle
4. 在事件中连接灯光的开关方法

### 示例3：创建信息展示物体
1. 选中画框或装饰品
2. 添加InteractableObject脚本
3. 设置：
   - Object Name: "古典画作"
   - Interaction Type: OneTime
4. 在事件中连接显示信息的UI

## 🛠️ 高级使用

### 创建自定义交互逻辑
创建新脚本，继承InteractableObject：

```csharp
public class MyCustomInteraction : InteractableObject
{
    [Header("自定义设置")]
    public GameObject targetObject;
    public float moveDistance = 1f;
    
    protected override void HandleInteraction(string interactorName)
    {
        // 您的自定义逻辑
        if (targetObject != null)
        {
            targetObject.transform.position += Vector3.up * moveDistance;
        }
        
        Debug.Log("执行了自定义交互!");
    }
}
```

### 批量设置可交互物体
1. 选中多个物体（按住Ctrl点击）
2. 统一添加InteractableObject脚本
3. 分别配置每个物体的具体设置

## 🔧 问题排查

### 常见问题及解决方案

**Q1: 射线看不见**
- 检查VR摄像机是否正确引用
- 确认控制器Transform是否正确设置

**Q2: 物体无法点击**
- 确保物体有Collider组件
- 检查物体Layer与设置是否一致
- 确认InteractableObject脚本已添加

**Q3: 没有视觉反馈**
- 确保物体有Renderer组件
- 检查Enable Hover Effect是否勾选
- 确认材质支持发光效果

**Q4: VR控制器不工作**
- 先用键盘Q、E键测试功能
- 检查VR设备连接状态
- 确认控制器引用正确

## 📋 完整配置检查清单

使用前请确保：
- [ ] VR_Interaction_Manager已创建
- [ ] VRObjectInteraction脚本已添加
- [ ] VR摄像机已引用
- [ ] 控制器Transform已设置（如果有VR设备）
- [ ] 目标物体有Collider组件
- [ ] 目标物体添加了InteractableObject脚本
- [ ] 交互类型已配置
- [ ] 已测试键盘输入功能
- [ ] Console无错误信息

## 💡 使用技巧

1. **开发阶段**：先用键盘测试，确保功能正常再用VR设备
2. **性能优化**：大场景中限制同时可交互的物体数量
3. **用户体验**：合理设置射线距离，避免误触
4. **视觉设计**：使用不同颜色区分不同类型的可交互物体
5. **音效设计**：为不同材质的物体设置合适的音效

---

现在您可以开始使用VR物体交互系统了！🎮 

有任何问题随时告诉我！ 