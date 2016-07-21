# PogoFrame - Unity下的游戏开发资源整合框架

----------


需求/进展: [Trello](https://trello.com/b/H9kJLqPU/pogoframe-roadmap)
GitHub: [GitHub](https://github.com/wang-yichun/PogoFrame)
带图Readme: [Leanote](http://leanote.com/blog/post/5790796bab644133ed01bbc8)

## 多工程结构

 1. 本工程为主工程,其它工程(如美术等)将其资源文件夹做软链接
![](leanote://file/getImage?fileId=57907fe077bbd67630000008)
 2. 美术工程如果需要基础功能插件,或需要与主工程的通过AssetBundle绑定的包括代码在内的资源做软链接
![](leanote://file/getImage?fileId=5790809977bbd67630000009)

> 这样以来,主工程不包含美术资源的任何工程目录,但可在项目中使用或修改美术资源,并通过 git 同步修改到美术资源库.
美术资源中的辅助代码(的引用)也可包含进发布的 AssetBundle 中,主工程在需要时会自动挂载.
MonoBehavior 和 ScriptableObject 都可以.

## 核心资源

 1. uFrame MVVM 1.6.2 ,修复了一些针对 Unity 最新版本的问题;
 2. 改进的AssetBundleManager, 负责多 AssetBundle 的发布与获取,可以进行灵活配置
![](leanote://file/getImage?fileId=57907db477bbd67630000007)
 3. DetachableAssetsManager, 将资源进行可拆卸化,便于Bug处理和资源重复利用.
![](leanote://file/getImage?fileId=57907d0977bbd67630000004)

## 第三方资源包
![](leanote://file/getImage?fileId=5790820377bbd6763000000a)

> 绝大多数资源是从之前AEX项目沿用下来,可以在 Stanealone/iOS/Android 平台中测试运行.

 - EasyTouchBundle, 控制触摸屏与用户交互的 API;
 - FTPClient, 可在编辑器下对文件/目录进行 FTP 基本操作,为 AssetBundleManager 提供基础服务;
 - JsonDotNet, 提供对 Json 的序列化/反序列化支持;
 - LINQtoGameObject, 可对 GameObject 在场景层级中进行定位和一些基本操作;
 - LOOM Framework, 提供简易的实现多线程任务处理的支持;
 - NCalc, 提供对数学表达式的解析支持,用于数据策划中的动态公式;
 - PogoAdsX, 自研的用来协调多广告发布平台进行发布视频广告,以提高广告填充率的插件;
 - PrefabEvolution, 提供对基础预制体向多持有者进行更新提交的插件;
 - protobuf-net, ProtocolBuffer是用于结构化数据串行化的灵活、高效、自动的方法;
 - RageSpline, 提供在 Unity 编辑器中对 2D贝塞尔曲边多边形进行快速编辑的工具;
 - ReorderableList, 编辑器插件,将 List 结构自动显示为可拖拽排序的列表(可多选排序);
 - StompyRobot, 将调试信息显示在场景中,并能进行自定义命令的调用,便于测试代码;
 - ZipX, 提供多重数据压缩方式;