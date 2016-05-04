<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Import.aspx.cs" Inherits="BAPT.Import" %>
  
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">  
<html xmlns="http://www.w3.org/1999/xhtml">  
<head id="Head1" runat="server">  
    <title>Import Broadband Pricing</title>  
</head>  
<body >  
    <form id="form1" runat="server">  
     <div>
         <asp:Label ID="lblTest" runat="server"></asp:Label>
     </div>
    <div>  
        <h4>  
          Import Broadband Pricing
        </h4>  
        <table>  
            <tr>  
                <td>  
                    Select File  
                </td>  
                <td>  
                    <asp:FileUpload ID="FileUpload1" runat="server" />  
                </td>  
                <td>  
                </td>  
                <td>  
                    <asp:Button ID="Button1" runat="server" Text="Upload" OnClick="Button1_Click" />  
                </td>  
            </tr>  
        </table>  
    </div>  
    </form>  
</body>  
</html>  
