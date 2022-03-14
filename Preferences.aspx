<%@ Page Title="Preference" Language="VB" MasterPageFile="~/Masters/Market1.master" AutoEventWireup="false" CodeFile="Preferences.aspx.vb" 
Inherits="Pages_Market1_CAGR_Preferences" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Market1ContentPlaceHolder" Runat="Server">
    <script type="text/JavaScript" src="../../../JavaScripts/collapseableDIV.js"></script>
    <script type="text/JavaScript" src="../../../JavaScripts/wz_tooltip.js"></script>
    <script type="text/JavaScript" src="../../../JavaScripts/tip_balloon.js"></script>
    
    <div id="ContentPagemargin" runat="server">
      <div style="margin: 0px 200px 5px 5px;;text-align:left ">
                                    
                            </div>
       <div id="PageSection1" style="text-align:left" >
             <br />
                  <table width="60%">
                    <tr class="AlterNateColor4">
                        <td class="PageSHeading" style="font-size:14px;" colspan="2">
                            Preferred Units
                        </td>
                    </tr>
                      <tr class="AlterNateColor1">
                        <td colspan="2">
                            <asp:RadioButton ID="rdEnglish" GroupName="Unit" runat="server" Text="English units"  />
                        </td>                        
                     </tr> 
                      <tr class="AlterNateColor1">
                        <td colspan="2">
                            <asp:RadioButton ID="rdMetric" GroupName="Unit" runat="server" Text="Metric units" />
                        </td>                        
                     </tr>                      
                    
                 </table>
             <br />
         
         </div>   
     </div>
</asp:Content>

