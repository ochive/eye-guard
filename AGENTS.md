# AGENTS.md

.NET 8 WPF 桌面护眼软件（仿 EyeFoo / 眼睛护士）。极简单应用，无测试、无 CI、无单元测试框架。

## 解决方案结构
- `eye-guard/` — 唯一的应用工程（WPF）。入口为 `App.xaml.cs`，主控在 `MainWindow.xaml.cs`。
  - `Core/` — `TimerManager`（计时/提醒）、`ScreenController`（全屏黑屏，多显示器支持）。
  - `UI/` — `TrayIcon`（系统托盘）。
- `EyeGuard.Package/` — MSIX 打包工程（wapproj），打包方式见根目录 `打包.md`。
- 打包/发布指引在根目录 `打包.md`，整体设计在 `eye-guard/docs/总体设计.md`。

## 关键技术点
- **框架混合**：`eye-guard.csproj` 同时启用 `UseWPF` 和 `UseWindowsForms`。WPF 用于窗口，WinForms 用于系统托盘 `NotifyIcon`。两套 UI 命名空间并存（`System.Windows.*` 与 `System.Windows.Forms.*`）。
- **命名空间**：`RootNamespace` 为 `EyeGuard`（PascalCase），与目录名 `eye-guard` 不同。C# 命名空间全部用 `EyeGuard` / `EyeGuard.Core` / `EyeGuard.UI`。
- **RootNamespace/ImplicitUsings/Nullable** 均启用；无 MVVM 框架、无第三方 UI 依赖，ViewModel 即 `MainWindow.xaml.cs` 的 code-behind。
- **计时线程**：`TimerManager` 用 `System.Timers.Timer`（后台线程），`ScreenController` 用 `DispatcherTimer`（UI 线程）。从 `TimerManager` 事件回调更新 UI 必须经 `Dispatcher.Invoke(...)`。
- 配置常量集中在 `Core/AppSettings.cs`：黑屏时长 60 秒、提醒间隔 60 分钟、提前警告 1 分钟。无设置持久化，改参数直接改这些常量。
- 主窗口默认隐藏、只驻留托盘：构造函数中 `Visibility = Visibility.Hidden`；`OnClosing` 取消关闭并 `Hide()`。

## 常用命令（在仓库根执行）
- 构建：`dotnet build eye-guard/eye-guard.csproj`
- 发布（单文件 win-x64，见 `Properties/PublishProfiles/FolderProfile.pubxml`）：`dotnet publish eye-guard/eye-guard.csproj -p:PublishProfile=FolderProfile`
- 无测试/lint/格式化脚本。改动后用 `dotnet build` 验证即可。
- MSIX 打包需在 Visual Studio 中通过打包项目“创建应用包”（见 `打包.md`），不通过 dotnet CLI。

## 约定
- 所有代码注释与 UI 文案为中文；专业术语/变量名保持英文。
- 依赖优先通过 NuGet 管理。
- Win32 API 调用优先使用 `Vanara.PInvoke` 库，次选手写 P/Invoke。
- 无 MVVM 框架，ViewModel 即 code-behind；优先使用双向数据绑定。
