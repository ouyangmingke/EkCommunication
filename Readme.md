[使用Sockets](https://docs.microsoft.com/zh-cn/dotnet/framework/network-programming/using-client-sockets)

TCP/IP 使用一个网络地址和一个服务端口号来对唯一标识设备。 

网络地址标识网络上的特定设备；端口号标识该设备要连接到的特定服务。

 网络地址和服务端口的组合称为终结点，它在 .NET Framework 中由 [EndPoint](https://docs.microsoft.com/zh-cn/dotnet/api/system.net.endpoint) 类表示。 

每个受支持的地址都继承 EndPoint ；对于 IP 地址，类为  [IPEndPoint](https://docs.microsoft.com/en-us/dotnet/api/system.net.ipendpoint). 。

[Dns](https://docs.microsoft.com/zh-cn/dotnet/api/system.net.dns) 类向使用 TCP/IP Internet 服务的应用程序提供域名服务。

[Resolve](https://docs.microsoft.com/zh-cn/dotnet/api/system.net.dns.resolve) 方法查询 DNS 服务器以将用户友好的域名（如“host.contoso.com”）映射到数字形式的 Internet 地址（如 192.168.1.1）。

Resolve 返回一个 ，其包含所请求名称的地址和别名的列表。 

在大多数情况下，可以使用 [AddressList](https://docs.microsoft.com/zh-cn/dotnet/api/system.net.iphostentry.addresslist) 数组中返回的第一个地址。 下面的代码获取一个包含服务器 host.contoso.com 的 IP 地址的 [IPAddress](https://docs.microsoft.com/zh-cn/dotnet/api/system.net.ipaddress)。

```c#
IPHostEntry ipHostInfo = Dns.Resolve("host.contoso.com");  
IPAddress ipAddress = ipHostInfo.AddressList[0];
```

