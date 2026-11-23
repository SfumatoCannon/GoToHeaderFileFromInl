# Go To Header / Inline File

[English](README.en-us.md) | 简体中文

一个 Visual Studio 扩展：在同名的 `.h` 与 `.inl` 文件之间一键互相跳转。默认快捷键 `Ctrl+K, Ctrl+0`，可按需修改。

## 功能

- 当前活动文档是 `.h` 时，打开同目录下同名的 `.inl` 文件。
- 当前活动文档是 `.inl` 时，打开同目录下同名的 `.h` 文件。
- 目标文件已经打开时，直接切换到该窗口，不会重复打开。
- 扩展名不区分大小写（`.H` / `.INL` 同样支持）。
- 命令同时出现在「工具」菜单中：`Go to Header / Inline File (.h / .inl)`。

以下情况只弹出提示，不会创建文件：

- 当前没有打开的文档；
- 当前文件不是 `.h` 或 `.inl`；
- 当前文档尚未保存到磁盘；
- 同目录下找不到同名目标文件。

## 环境要求

- Visual Studio 2022（17.0 及以上）或 Visual Studio 2026，amd64；
- 已安装「Visual Studio 核心编辑器」组件（CoreEditor）。

## 安装

1. 关闭所有 Visual Studio 实例。
2. 双击 `GoToHeaderFileFromInl.vsix`，或在「打开方式」中选择 Visual Studio Version Selector / VSIX Installer。
3. 在安装程序中选择目标 Visual Studio 版本，点击「安装」。
4. 启动 Visual Studio。

卸载：扩展 → 管理扩展（Manage Extensions）→ 已安装 → 找到 `Go To Header/Inline File` → 卸载。

## 使用

| 操作 | 结果 |
| --- | --- |
| 在 `Foo.h` 中按 `Ctrl+K, Ctrl+0` | 打开并跳转到 `Foo.inl` |
| 在 `Foo.inl` 中按 `Ctrl+K, Ctrl+0` | 打开并跳转到 `Foo.h` |

也可以通过「工具」菜单执行同一命令。

## 修改快捷键

1. 打开「工具 → 选项 → 环境 → 键盘」。
2. 在「显示命令包含」中输入 `Go to Header`（或直接按下想用的组合键，查看其当前绑定的命令）。
3. 选中 `Go to Header / Inline File (.h / .inl)`，在「按快捷键」中输入新的组合键，点击「分配」。
4. 点击「确定」保存。

## 从源码构建

需要 Visual Studio（含「Visual Studio 扩展开发」工作负载）与 .NET Framework 4.7.2 目标包。首次构建会从 NuGet 还原依赖（`Microsoft.VisualStudio.SDK`、`Microsoft.VSSDK.BuildTools`）。

```powershell
& "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" `
  GoToHeaderFileFromInl.sln -restore -p:Configuration=Release
```

产物：

```
src/GoToHeaderFileFromInl/bin/Release/net472/GoToHeaderFileFromInl.vsix
```

调试：在 Visual Studio 中打开解决方案，让项目以 `devenv.exe /rootsuffix Exp` 启动（实验实例），F5 时扩展会部署到实验实例，不影响日常使用的 Visual Studio。

## 项目结构

| 路径 | 说明 |
| --- | --- |
| `GoToHeaderFileFromInl.sln` | 解决方案 |
| `src/GoToHeaderFileFromInl/GoToHeaderFileFromInl.csproj` | VSIX 项目（net472，VSSDK 构建工具） |
| `src/GoToHeaderFileFromInl/ToggleHeaderInlPackage.cs` | 包注册与初始化，创建命令 |
| `src/GoToHeaderFileFromInl/ToggleHeaderInlCommand.cs` | 命令逻辑：查找同名文件并打开 |
| `src/GoToHeaderFileFromInl/ToggleHeaderInlCommand.vsct` | 命令定义、菜单挂载、默认快捷键 |
| `src/GoToHeaderFileFromInl/source.extension.vsixmanifest` | VSIX 清单（安装目标版本、资产） |

## 实现说明

- 通过 DTE（`EnvDTE80.DTE2`）获取活动文档路径，使用 `ItemOperations.OpenFile` 以文本视图打开目标文件。
- 命令定义与快捷键写在 `.vsct` 中，以全局作用域注册，因此在任意窗口按下快捷键都可用（无活动文档时会提示）。
- 扩展只做同目录同名匹配，不搜索整个解决方案。
