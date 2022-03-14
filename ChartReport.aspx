<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ChartReport.aspx.vb" Inherits="Pages_Market1Sub_CAGR_ChartReport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Pivot Report Chart</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio 7.0" />
    <meta name="CODE_LANGUAGE" content="Visual Basic 7.0" />
    <meta name="vs_defaultClientScript" content="JavaScript" />
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />   
   <link rel="stylesheet" href="../../../App_Themes/SkinFile/AlliedNew.css" />

      <script type="text/JavaScript" src="../../../JavaScripts/collapseableDIV.js"></script>
    <style type="text/css">
        .style1
        {
            width: 100%;
        }
    </style>
   </head> 
   
<body>
    <form id="form1" runat="server">
       <div id="ContentPage" runat="server">
           <div id="PageSection1" >
           
          <h3>  <asp:Label ID="lblheading" runat="Server"></asp:Label></h3>
            <table border="0" id="tblCaseDes" runat="server" cellpadding="0" cellspacing="0">
                <tr style="height: 20px">
                    <td style="width: 350px;">
                        <span style="font-size: 12px;text-align: right;"><b>Report Id:</b></span>
                    <asp:Label ID="lblReportID" runat="server" CssClass="NormalLable" style="text-align: left;"></asp:Label>
                    </td>
                    <td style="width:70px; text-align: right;">
                        <span style="font-size: 12px;"><b>Report Type:</b></span>
                    </td>
                    <td style="width:200px;text-align:left;">
                        <asp:Label ID="lblReportType" runat="server" CssClass="NormalLable"></asp:Label>
                    </td>
                </tr>
                <tr style="height: 20px">
                    <td colspan="2">
                        <span id="caseDe3" runat="server" style="font-size: 12px;"><b>Report Brief:</b></span>
                        <asp:Label ID="lblReportDe2" runat="server" CssClass="NormalLable"></asp:Label>
                    </td>
                </tr>
            </table>    
            <br />

               <table border="0" id="tblddlset" runat="server" cellpadding="0" cellspacing="0">
                   <tr>
                       <td>
                           <asp:Table ID="tblFil" runat="server" CellPadding="2" CellSpacing="2">
            </asp:Table>
                           </td>
                       
                   </tr>
                   <tr>
                   <td>
                          <asp:Button ID="btnSumit" runat="server" Text="Submit" CssClass="ButtonWMarigin" OnClientClick="return Validation();" />
              
                           </td>
                   </tr>
               </table>

             
           
           <br />
                <table>
                    <tr>
                     <td >
                        <asp:Label ID="lblNOG" runat="server" CssClass="NormalLable" Visible ="false" Font-Bold ="true" ForeColor ="Red" ></asp:Label>
                    </td>
                        <td>
                         <div style="width: 700px; ">
                            <div id="MaterialPrice" runat="server">
                            </div>
                            </div> 
                         </td>
                      </tr>                                                                 
                 </table>
                 <br />

                
           </div>
       </div>                                                                      
      <asp:HiddenField ID="hidReportType" runat="server" />
      <asp:HiddenField ID="hidUnitShort" runat="server" />    
      <asp:HiddenField ID="hidReport" runat="server" />
      <asp:HiddenField ID="hidReportData" runat="server" />
      <asp:HiddenField ID="hidReportIDD" runat="server" /> 
      <asp:HiddenField ID="hidReportVal" runat="server" />
      <asp:HiddenField  ID="hidFilterDes" runat="server" />
      <asp:HiddenField  ID="hidFilterId" runat="server" />
      <asp:HiddenField  ID="hidCatID" runat="server" />
      <asp:HiddenField  ID="hidCatDes" runat="server" />
      <asp:HiddenField  ID="hidCatDes1" runat="server" />
      <asp:HiddenField  ID="hidfil" runat="server" />
    </form>

</body>
</html>
