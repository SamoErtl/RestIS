﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ena.aspx.cs" Inherits="RESTService.ena" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:DBZdravilnicaConnectionString %>" SelectCommand="SELECT * FROM [Address]"></asp:SqlDataSource>
        </div>
    </form>
</body>
</html>
