# zlp-demo-csharp

Demo các phương thức tích hợp cho CSharp (ASP.NET MVC)

**Phiên bản:** 1.0

## Cài đặt

1. SQLServer / MySQL
2. Thay đổi `AppSettings` trong **Web.config**

```xml
<appSettings>
  <!-- ... -->
  
  <!-- ZaloPay Config -->
  <add key="Appid" value="553" />
  <add key="Key1" value="9phuAOYhan4urywHTh0ndEXiV3pKHr5Q" />
  <add key="Key2" value="Iyz2habzyr7AG8SgvoBCbKwKi3UzlLi3" />
  <add key="RSAPublicKey" value="MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBAOfB6/x0b5UiLkU3pOdcnXIkuCSzmvlVhDJKv1j3yBCyvsgAHacVXd+7WDPcCJmjSEKlRV6bBJWYam5vo7RB740CAwEAAQ==" />
  
  <!-- ... -->
</appSettings>
```

3. Thay đổi `ConnectionString`

```xml
<connectionStrings>
  <!-- SQL Server -->
  <add name="ZaloPayDemoSqlServer" providerName="System.Data.SqlClient" connectionString="Server=.\SQLEXPRESS;Database=zalopay-demo;User Id=zlpdemo;Password=123456;" />
  <!-- MySQL -->
  <add name="ZaloPayDemoMysql" providerName="Mysql.Data.MysqlClient" connectionString="server=127.0.0.1;database=zalopay-demo;uid=root;charset=utf8;" />
</connectionStrings>
```

## Các API tích hợp trong Demo

* Xử lý callback
* Xử lý Redirect
* Thanh toán QR
* Cổng ZaloPay
* QuickPay
* Mobile Web to App
* Hoàn tiền
* Lấy trạng thái đơn hàng
* Lấy trạng thái hoàn tiền
* Lấy danh sách ngân hàng

## Nhận callback ở môi trường local thông qua Ngrok

* Xem thêm [ở đây](https://github.com/tiendung1510/zlp-forward-callback-proxy)