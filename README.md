# TinyBlog

TinyBlog 是使用 C# 语言和 Kestrel + Nancy 框架编写的一款轻博客程序，它不挑操作系统，甚至也不挑运行环境。

👉 [演示地址](http://euch.gotoip1.com/) 👈

Release 说明：

tiny_x.x.x.x.iis.zip IIS 版本

tiny_x.x.x.x.netcore.zip  .NET Core 版本

tiny_x.x.x.x.netfx.zip  .NET Framework 版本

支持如下运行框架环境或应用容器：

✅ .NET Framework >=4.0

✅ .NET Core >=2.0

✅ IIS >=7.0

✅ Git Pages

✅ Jexus

✅ 其它支持静态页面的主机或环境


支持如下操作系统：

✅ 主流和非主流 Linux

✅ Windows Server

✅ Mac OSX

在 Windows 服务器上，可以使用 netfx 版本或 netcore 版本运行，如果安装了 IIS，可以使用 iis 版本运行（参见发行版后缀）。

在 Linux 服务器上，可以安装 .net core 后使用 netcore 版本运行，如果装了 mono 可以使用 netfx 版本，如果安装了 jexus ，可以使用 iis 版本。

在 Git Pages 等只能支持静态页面的服务或者主机上，可以使用 -g 参数生成静态页面。

## 如何使用

Tinyblog 使用端口为 6600,启动程序后，在浏览器输入 http://localhost:6600 即可看到网站运行效果，后台管理页面地址为 http://localhost:6600/admin ，默认用户名和密码均为 admin ，配置文件为 Tiny.config。

在配置文件中，字段含义如下：

```
<?xml version="1.0"?>
<TinyConfiguration xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Port>6600</Port>
  <SiteName>站点标题</SiteName>
  <IsSitePublic>true</IsSitePublic><!--是否公开访问-->
  <PageSize>5</PageSize><!--分页大小-->
  <NavigationLinks><!--导航栏链接（非必须）-->
    <ArrayOfString>
      <string>/</string>
      <string>HOME</string>
    </ArrayOfString>
    <ArrayOfString>
      <string>https://estermont.wordpress.com/</string>
      <string>DOWNLOAD</string>
    </ArrayOfString>
    <ArrayOfString>
      <string>https://github.com/yahch</string>
      <string>GITHUB</string>
    </ArrayOfString>
  </NavigationLinks>
  <SiteBuildDate>2018-03-15T00:00:00</SiteBuildDate><!--站点创建日期-->
  <Username>admin</Username><!--用户名-->
  <Password>Buc6XoYh9TGyF68OTjoMBOMTlgvxF6OWAUApkG06jCY=</Password><!--用户密码（加密后）-->
  <AuthExpireSeconds>28800</AuthExpireSeconds><!--登录有效时间-->
  <Encryption>false</Encryption><!--是否对数据加密-->
  <!--工作目录完整路径-->
  <DataDirectory>D:\VS_PROJECTS\TinyFx\src\Tiny.ConsoleApplication\bin\Release\</DataDirectory>
  <LoginRetryTimeSpanSeconds>30</LoginRetryTimeSpanSeconds><!--最大重试登录失败间隔时间-->
</TinyConfiguration>
```

以上，请 happy 使用 😾
