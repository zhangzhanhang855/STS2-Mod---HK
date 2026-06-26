- 📖 Godot Mod 场景文件使用说明书 -
本文档详细介绍了为你生成的 7 个 Godot 4 场景（.tscn）文件的功能、结构、动画配置以及如何在 Mod 开发中使用它们。所有场景均已针对 **Godot 4** 进行了优化，并完美契合**“空洞骑士”（Hollow Knight）**主题。
- 核心指南
在开始之前，请记住一个关键步骤：**所有文件均已保存为文本格式。请将代码块中的内容完整复制，并保存为对应的 .tscn 文件，放入你的项目文件夹中。**
- 战斗角色场景 (HK_Character.tscn)
【提示词】：角色使用的默认待机动画场景（Hollow Knight主题）
HK_Character.tscn
1. 场景功能
这是一个基于 Node2D 的基础角色战斗场景，包含了空洞骑士角色的贴图、碰撞和默认待机动画。它利用简单的缩放和位移来模拟呼吸感，让战斗角色看起来不再僵硬。
2. 场景结构
 * **HK_Character** (Node2D)：根节点。
   * **Sprite2D** (Sprite2D)：角色贴图。
   * **CollisionShape2D** (CollisionShape2D)：战斗碰撞体（预设 64x64 正方形）。
   * **Camera2D** (Camera2D)：自带战斗摄像机（可选）。
   * **AnimationPlayer** (AnimationPlayer)：动画控制器。
3. 动画配置
 * **Animation: idle** (2s, 循环播放, 缓动模式：In/Out)
   * **Sprite2D:scale** (Vector2): 1 -> 1.02, 0.98 -> 1 (呼吸缩放)
   * **Sprite2D:position** (Vector2): 0,0 -> 0,-4 -> 0,0 (轻微上下浮动)
4. 使用说明
 1. 选中 Sprite2D 节点。
 2. 在右侧检查器中的 Texture 属性里放入你的**空洞骑士（小骑士）贴图**。
 3. 如果美术素材中心点不对，可以调整 Offset。
- 头像缩略图图标场景 (HK_Portrait.tscn)
【提示词】：角色的头像缩略图图标场景（Control节点）
HK_Portrait.tscn
1. 场景功能
这是一个用于角色选择界面或 UI 顶部的头像场景，基于 Control 节点家族。它包含一个可选的边框、一个居中的头像层，以及一个默认自动播放的脉冲呼吸特效，让头像在 UI 里看起来更有高级感。
2. 场景结构
 * **HK_Portrait** (Control)：UI 根节点，尺寸 200x200，锚点居中。
   * **Frame** (TextureRect)：头像边框层（预设 100% 空间）。
   * **Avatar** (TextureRect)：实际头像层（预设 15px 内边距，等比缩放并居中）。
   * **AnimationPlayer** (AnimationPlayer)：UI 动画控制器。
3. 动画配置
 * **Animation: pulse** (2s, 循环播放, 缓动模式：In/Out)
   * **Avatar:scale** (Vector2): 1 -> 1.05 -> 1 (脉冲放大)
   * **Avatar:modulate** (Color): 白色 -> 亮微蓝光 -> 白色 (泛光效果)
4. 使用说明
 1. 选中 Avatar 节点，将你的头像切图放入 Texture 属性。
 2. 如果有边框图，将其放入 Frame 节点的 Texture 属性；如果没有，可以将其隐藏。
 3. 此场景尺寸为 200x200，可以随意放置在其他 UI 容器中。
- 灵魂容器能量计数器场景 (HK_EnergyCounter.tscn)
【提示词】：角色的能量计数器场景（灵魂容器）
HK_EnergyCounter.tscn
1. 场景功能
这是一个基于 Control 的 UI 元素，通过 TextureProgressBar 模拟空洞骑士中**“灵魂容器”（Soul Vessel）**的外观。它通过动画模拟灵魂液体从下往上填充，并在获得能量时播放带感的泛光特效。
2. 场景结构
 * **HK_EnergyCounter** (Control)：根节点，尺寸 120x120，pivot 在中心点。
   * **VesselBackground** (TextureRect)：空的“灵魂容器”背景。
   * **EnergyFill** (TextureProgressBar)：灵魂液体进度条。
   * **EnergyText** (Label)：显示文字 (如 "3/3")。
   * **AnimationPlayer** (AnimationPlayer)：能量特效控制器。
3. 动画配置
 * **Animation: gain_energy** (0.5s, 单次播放)
   * **.:scale** (Vector2): 1 -> 1.2 -> 1 (瞬间放大 20%)
   * **EnergyText:modulate** (Color): 白色 -> 过曝泛白 -> 白色 (强泛光效果)
4. 使用说明
 1. **贴图配置**：
   * 将容器背景图放入 VesselBackground.texture。
   * 将液体贴图放入 EnergyFill.texture_progress。
   * 将玻璃边框放入 EnergyFill.texture_over（可选）。
 2. **代码控制**：通过脚本控制 EnergyFill.value（当前能量）和 EnergyText.text（文本），并调用 $AnimationPlayer.play("gain_energy")。
- 商店待机动画场景 (HK_ShopIdle.tscn)
【提示词】：角色在商店的待机动画场景
HK_ShopIdle.tscn
1. 场景功能
这是一个更悠闲放松的战斗待机变体，用于商店中，不包含碰撞体。它引入了轻微的旋转，模拟角色在商店里放松和四处张望的动作。
2. 场景结构
 * **HK_ShopIdle** (Node2D)：根节点。
   * **Sprite2D** (Sprite2D)：角色贴图。
   * **AnimationPlayer** (AnimationPlayer)：商店待机动画。
3. 动画配置
 * **Animation: shop_idle** (4s, 循环播放, 缓动模式：In/Out)
   * **Sprite2D:scale** (Vector2): 1 -> 1.01 -> 1 (极细微呼吸)
   * **Sprite2D:rotation** (float): 0 -> 0.05rad -> 0 -> -0.05rad -> 0 (轻微左右摇晃/张望)
4. 使用说明
 1. 选中 Sprite2D 节点挂载贴图，动画会自动播放。
 2. 适合直接实例化放入商店场景中。
- 营地休息动画场景 (HK_CampfireRest.tscn)
【提示词】：角色休息时坐在火堆旁的动画场景
HK_CampfireRest.tscn
1. 场景功能
这是一个基于 Node2D 的营地休息场景。它使用简单的位移动画模拟角色坐在长椅上低头。为了增加氛围，它不仅使用了**橘黄色点光源 (PointLight2D)**，还为 Sprite2D 添加了**暖色调颜色调制动画**。
2. 场景结构
 * **HK_CampfireRest** (Node2D)：根节点。
   * **Sprite2D** (Sprite2D)：休息贴图。
   * **AnimationPlayer** (AnimationPlayer)：动画控制器。
   * **FireLight** (PointLight2D)：橘黄色环境点光源（预设能量 0.8）。
3. 动画配置
 * **Animation: rest** (3s, 循环播放, 缓动模式：In/Out)
   * **Sprite2D:position** (Vector2): 0,0 -> 0,2 -> 0,0 (轻微低头动作)
   * **Sprite2D:modulate** (Color): 暖黄 -> 橘红 -> 暖黄 (模拟火光闪烁)
4. 使用说明
 1. 选中 Sprite2D 挂载“空洞骑士休息”贴图，动画会自动播放。
 2. 如果你使用 Godot 的 2D 光照系统，FireLight 会自动生效。
- 卡牌拖尾特效场景 (HK_CardTrail.tscn)
【提示词】：角色卡牌拖尾特效场景
HK_CardTrail.tscn
1. 场景功能
这是一个基于 CPUParticles2D 的粒子特效场景。它生成一串缓慢上升、大小和颜色渐变（从白色变蓝再变透明）的粒子，模仿**“灵魂”液体**的质感，非常适合作为卡牌在移动时的拖尾特效。
2. 场景结构
 * **HK_CardTrail** (Node2D)：根节点。
   * **SoulParticles** (CPUParticles2D)：灵魂粒子特效发射器。
3. 粒子配置
 * **Amount**: 30
 * **Lifetime**: 0.6s
 * **Emission Shape**: Sphere (radius: 20)
 * **Gravity**: (0, -98) (缓慢上升)
 * **Scale Amount Curve**: 4 -> 8 -> 0 (粒子变大后消失)
 * **Color Ramp**: 白色 -> 淡蓝 -> 蓝紫色 -> 0 透明 (灵魂颜色渐变)
4. 使用说明
 1. **挂载方式**：将此场景作为卡牌子节点，或通过脚本控制其位置随卡牌移动。
 2. **代码调用**：当卡牌移动时，可以设置粒子处于发射状态；停止移动时，停止发射。
- 角色选择界面背景场景 (HK_SelectBg.tscn)
【提示词】：角色选择界面的背景场景
HK_SelectBg.tscn
1. 场景功能
这是一个基于 Control 的全屏 UI背景场景。它包含一个全屏大图、一个缓慢飘动的雾气层动画，以及一个暗色压暗遮罩。非常适合用于角色选择界面的背景，并为其提供一个空洞骑士风格的环境氛围。
2. 场景结构
 * **HK_SelectBg** (Control)：UI 背景根节点，全屏锚点。
   * **BackgroundImage** (TextureRect)：全屏背景大图。
   * **FogLayer** (TextureRect)：半透明雾气层（预设 offset_right 100）。
   * **DarkOverlay** (ColorRect)：暗色压暗遮罩。
   * **AnimationPlayer** (AnimationPlayer)：动画控制器。
3. 动画配置
 * **Animation: bg_float** (10s, 循环播放)
   * **FogLayer:position** (Vector2): 0,0 -> -50,20 -> 0,0 (全屏雾气缓慢飘动)
4. 使用说明
 1. **贴图配置**：分别在 BackgroundImage 和 FogLayer 中放入你的背景和雾气贴图。
 2. **层级**：此场景应该作为你整个角色选择 UI 的最底层。
- 🎨 贴图与美术资产注意事项
 * **贴图大小**：场景中的节点尺寸和动画参数是基于预设值（如 200x200、120x120）设计的。如果你的美术贴图尺寸不同，你可能需要在 Godot 编辑器中**调整相应节点的 scale、offset 或动画轨道的关键帧**。
 * **贴图格式**：为了支持半透明效果，强烈建议使用 **.png** 或 **.webp** 格式。
 * **Pivot 设置**：UI 特效动画（如放大）非常依赖中心点 (pivot_offset)。我们已经为大多数特效场景预设了中心点位置，如果调整了尺寸，请记得重新计算并设置中心点位置。
