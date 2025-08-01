# MCP C# SDK 的 ASP.NET Core 扩展

这是 [模型上下文协议 (Model Context Protocol)](https://modelcontextprotocol.io/) 的官方 C# SDK（net6.0版本），它使 .NET 应用程序、服务和库能够实现 MCP 客户端和服务器并与之交互。请访问 [API 文档 ](https://modelcontextprotocol.github.io/csharp-sdk/api/ModelContextProtocol.html)以获取有关可用功能的更多详情。

## 关于 MCP

模型上下文协议 (MCP) 是一个开放协议，它规范了应用程序向大型语言模型 (LLM) 提供上下文的方式。它能够实现 LLM 与各种数据源和工具之间的安全集成。

有关 MCP 的更多信息：

- [官方文档](https://modelcontextprotocol.io/)
- [协议规范](https://spec.modelcontextprotocol.io/)
- [GitHub 组织](https://github.com/modelcontextprotocol)

## 安装

首先，请从 NuGet 安装该软件包：

```
dotnet add package dotnetmcp
```

## 快速入门

```csharp
// Program.cs
using ModelContextProtocol.Server;
using System.ComponentModel;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();
var app = builder.Build();

app.MapMcp();

app.Run("http://localhost:3001");

[McpServerToolType]
public static class EchoTool
{
    [McpServerTool, Description("将消息回显给客户端。")]
    public static string Echo(string message) => $"你好 {message}";
}
```
