# FPS
基于C#和Unity3D的第一人称视角游戏，内涵源码以及生成的项目应用。

## 目录说明
注：此部分说明由ai生成
```tree
第一人称视角射击游戏/
├── .gitignore                      # Git 全局忽略文件 (极其重要：通常用于过滤掉 Library、Build 目录和 .vs、obj 等临时文件)
│
├── Build/                          # Unity Build Pipeline 最终输出的可执行游戏与运行期数据包 (打包产物)
│   └── \FPS可执行文件.zip 
│
└── FPS/                            # Unity 工程开发主干根目录
    ├── .vsconfig                   # Visual Studio 附加的工作负载和组件环境配置文件
    ├── Assembly-CSharp-Editor.csproj # C# 编辑器扩展工程项目文件 (定义了只在 Unity Editor 内运行的脚本引用，如自定义面板)
    ├── Assembly-CSharp.csproj      # 游戏主体运行态 C# 逻辑工程项目文件
    ├── FPS.sln                     # 统合和绑定所有工程文件的 Visual Studio 顶级解决方案结构文件
    ├── ignore.conf                 # Plastic SCM 版本控制 (Unity 官方的 VCS 解决方案) 专用忽略配置文件
    │
    ├── .plastic/                   # [版本控制] Unity Version Control (Plastic SCM) 的本地工作区记录状态
    │   ├── plastic.changes
    │   ├── plastic.selector
    │   ├── plastic.wktree
    │   └── plastic.workspace
    │
    ├── .vs/                        # Visual Studio 或 Rider 自动生成的本地分析缓存与诊断数据 (应被 Git 忽略)
    │
    ├── Assets/                     #  一切游戏创作资源与剧本资产存放区 (也是面试中主要盘问逻辑和层级划分的地方)
    │   ├── DefaultNetworkPrefabs.asset # 如果集成了 Netcode for GameObjects，这里负责保存所有可供网络端实例化的基础预制体表配置
    │   ├── DefaultNetworkPrefabs.asset.meta # 每个资源的对应的唯一 GUID 记录器，记录了该 Asset 在 Unity 背后的依赖关系引用 (绝不能删)
    │   │
    │   ├── Script/                 # [核心代码区] 所有自己手写的 C# 业务核心控制脚本存放目录
    │   ├── Prefabs/                # [核心装配区] 从场景中拖拽剥离出来的游戏对象预设体组合 (如玩家容器、怪物生成器、会死亡的 AI 模板)
    │   ├── Animations/             # 存放 Animation Clip (动画片段) 及最重要的 Animator Controller (利用状态机驱动动作流转)
    │   │
    │   ├── RPG_FPS_game_assets_industrial/ # 第三方/美术：末日废土或工业风格的 3D 场景与布景杂物模型集合
    │   ├── LowPolySoldiers/        # 第三方/角色：低多边形网格像素风 (Low Poly) 的玩家和敌人角色蒙皮模型及材质包
    │   ├── PostApocalypseGunsDemo/ # 第三方/武器：末日改装枪支与近战武器的 3D 资产库 (通常带有枪械的拆解蒙皮节点)
    │   │
    │   ├── Audios/                 # 自配的各类开火、受击、换弹夹等短促的 SE 音效和场景 BGM 导入目录
    │   ├── Image/                  # HUD, 十字准星、掉血红框特效、背包等 2D UI 精灵图界面图集资源
    │   ├── TextMesh Pro/           # 高级富文本渲染插件的核心依赖着色器资产组 (负责伤害飘字和清晰 HUD 高清渲染)
    │   ├── ThirdSource/            # 其他零碎从 GitHub 等外部拷贝引用的第三方 C# 工具框架或宏定义库
    │   ├── UnityTechnologies/      # 通常随外部 Package 导入携带的官方扩展示例及资源组
    │   ├── Weapons of Choice FREE - Komposite Sound/ # 高质量且分轨的枪械设计射孔环境音效合集资源
    │   │
    │   ├── _TerrainAutoUpgrade/    # 可能是自动生成的旧版 Unity 地形升阶过渡备份文件
    │   └── !GET_MORE_PAID_ASSETS_FOR_FREE/ # 第三方资产包附带的版权声明或广告文本
    │
    ├── Packages/                   # 本地项目依赖到的各种 Unity 官方模块包版本锁定区域 (UPM 包管理器)
    │   ├── manifest.json           # 核心依赖清单列表，详细罗列了例如 UGUI、Timeline、Cinemachine 等使用到的版本号
    │   └── packages-lock.json      # 包引用多级树深度锁定档 (类似于前端的 package-lock.json)
    │
    ├── ProjectSettings/            # [核心全局设置] 非常关键。定义了整个游戏世界的物理、输入、显示、烘焙规则的根环境
    │   ├── AudioManager.asset      # 定义全局声音管道混合与总线数量等属性
    │   ├── ClusterInputManager.asset # 设置了新旧 Input 系统的按键布局，如映射 W/A/S/D 到浮点轴线等重度交互配置
    │   ├── BurstAotSettings_StandaloneWindows.json # [性能考点] 针对 Windows 的 Burst 编译器预编阶段 AOT 指令生成性能调参优化 
    │   └── CommonBurstAotSettings.json
    ├── Logs/                       # 在运行、打包、或者 Unity Crash 崩溃后所留存下来的分析追踪轨迹日志
    │
    ├── obj/                        # Visual Studio / MSBuild 在生成 C# Assembly 时产生的临时预备汇编文件，无实际意义需被过滤
    │   └── Debug/
    │
    └── UserSettings/               # 当前物理电脑开发机内当前用户独特的如窗口布局排版 (Layout) 、测试面板悬停位置等自定义喜好设定档
```

### 游戏试玩画面
<img width="700" height="600" alt="gaming" src="https://github.com/user-attachments/assets/5825f686-8336-4ccf-9185-1adf32efddbb" />

