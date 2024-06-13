# 满天星

#### 介绍
本项目为张馨木（22920202204705）毕设项目仓库，地址 https://gitee.com/zhang-xinmu/sketch ，主要包含3D横版解谜游戏《满天星》得Unity客户端源码及美术资源等文件。希望以此作品让大家更加了解那些来自星星的孩子们。

游戏共有三个关卡及内部的十余个小关卡。本仓库仅包含客户端项目源码，服务端源码见 https://gitee.com/zhang-xinmu/doodle-animation 。

#### 软件架构
软件架构说明

本项目主要资源存入Assets文件夹中，包含Animation、AudioClip、External、Font等子文件夹，下面将简述文件夹的存放内容。

1. Animation：角色及关卡的3D动画的Animation Clip片段
2. AudioClips：关卡音效及背景音乐 
3. External：外部插件及资源包，主要包含镜面反射插件以及多个3D场景模型的资源包
4. Font：字体
5. Materials：场景所使用的材质
6. Prefabs：场景关卡及道具等元素的预制体
7. Scenes：游戏主要场景
8. Scripts：游戏主要脚本
9. Shaders：游戏中材质所使用的着色器
10. StreamingAssets：系统存储路径
11. Textures：材质用到的贴图等图片


#### 安装教程

1.  打开UnityHub，选择下载Unity版本为Unity2021.3.4f1c1
2.  Unity内打开项目
3.  选择场景Scene0运行项目

#### 使用说明

1.  此仓库为项目的客户端部分的源码仓库，无服务器端单独时无法运行使用涂鸦绘画功能
2.  若要运行完整功能，需在Python服务端(https://gitee.com/zhang-xinmu/doodle-animation)找到并Server.py后再运行本项目

#### 未来可能的改进方向
1. 服务端与客户端连接优化，更换掉现有的tcp连接交互方式
2. 优化资源管理，减少资源冗余
3. 优化渲染，减少项目setpass call
4. 项目池管理场景资源，减少加载负荷
5. 优化存档系统