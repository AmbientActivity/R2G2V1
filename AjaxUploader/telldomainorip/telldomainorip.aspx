<%@ Page Language="C#" %>

<html>
<head>

    <script runat="server">
        string name = HttpContext.Current.Request.Url.Host.ToLower();
        string IP = HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"];
    </script><style type="text/css">
      body {
        font: 11pt verdana,arial,sans-serif;
      }
	</style>

    <title>CuteSoft.NET Components License Generation Helper</title>
</head>
<body>
    <h1 align="center">
        License Generation Helper</h1>
    <table border="3" width="70%" cellspacing="5" cellpadding="10" align="center">
        <tr>
            <td align="right" width="30%">
                <b>Current URL</b></td>
            <td align="left">

                <script language="javascript"> document.write(location.href) </script>

            </td>
        </tr>
        <tr>
            <td align="right">
                <b>Domain/Subdomain name</b></td>
            <td align="left">
                <%=name%>
            </td>
        </tr>
        <tr>
            <td align="right" width="30%">
                <b>IP@ Detected</b></td>
            <td align="left">
                <%=IP%>
            </td>
        </tr>
    </table>
    <br>
</body>
</html>
