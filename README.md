# TinyBlog

TinyBlog 是使用 C# 语言和 Kestrel + Nancy 框架编写的一款轻博客程序，它不挑操作系统，甚至也不挑运行环境。

[演示地址](http://euch.gotoip1.com/)

Release 说明：

tiny_x.x.x.x.iis.zip IIS 版本

tiny_x.x.x.x.netcore.zip  .NET Core 版本

tiny_x.x.x.x.netfx.zip  .NET Framework 版本

支持如下运行框架环境或应用容器：

☑ .NET Framework >=4.0

☑ .NET Core >=2.0

☑ IIS >=7.0

☑ Git Pages

☑ Jexus

☑ 其它支持静态页面的主机或环境


支持如下操作系统：

☑ 主流和非主流 Linux

☑ Windows Server

☑ Mac OSX

在 Windows 服务器上，可以使用 netfx 版本或 netcore 版本运行，如果安装了 IIS，可以使用 iis 版本运行（参见发行版后缀）。

在 Linux 服务器上，可以安装 .net core 后使用 netcore 版本运行，如果装了 mono 可以使用 netfx 版本，如果安装了 jexus ，可以使用 iis 版本。

在 Git Pages 等只能支持静态页面的服务或者主机上，可以使用 -g 参数生成静态页面。
