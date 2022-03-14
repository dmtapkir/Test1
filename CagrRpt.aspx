<%@ Page Language="VB" MasterPageFile="~/Masters/Market1.master" AutoEventWireup="false" CodeFile="CagrRpt.aspx.vb" Inherits="Pages_Market1_CAGR_CagrRpt" title="CAGR Report" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Market1ContentPlaceHolder" Runat="Server">
    <script type="text/JavaScript" src="../../../JavaScripts/collapseableDIV.js"></script><script type="text/JavaScript" src="../../../JavaScripts/wz_tooltip.js"></script><script type="text/JavaScript" src="../../../JavaScripts/tip_balloon.js"></script>
    <script type="text/javascript">
        function ShowPopWindow(Page, HidID) {
            //window.open('ItemSearch.aspx', 'ItemSearch', 'status=0,toolbar=0,location=0,menubar=0,directories=0,resizable=0,scrollbars=0,height=400,width=600');  

            var width = 500;
            var height = 180;
            var left = (screen.width - width) / 2;
            var top = (screen.height - height) / 2;
            var Hid = document.getElementById(HidID).value
            var params = 'width=' + width + ',height=' + height; params += ',top=' + top + ', left=' + left; params += ', directories=no';
            params += ', location=no';
            params += ', menubar=no';
            params += ', resizable=no';
            params += ', scrollbars=no';
            params += ', status=yes';
            params += ', toolbar=no';
            Page = Page + '&hidValue=' + Hid
            //            alert(Page);
            newwin = window.open(Page, 'PopUp', params);

        }
    </script>
<div id="ContentPagemargin" runat="server">
       <div id="PageSection1" style="text-align:left" >
          <br />
          
            <asp:Table ID="tblCAGR" runat="server" CellPadding="0" CellSpacing="1" ></asp:Table>
          </div>
</div>
</asp:Content>

