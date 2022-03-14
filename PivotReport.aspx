<%@ Page Title="Market Report" Language="VB" MasterPageFile="~/Masters/Market1.master"
    AutoEventWireup="false" CodeFile="PivotReport.aspx.vb" Inherits="Pages_Market1_CAGR_PivotReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Market1ContentPlaceHolder" runat="Server">
    <script type="text/JavaScript" src="../../../JavaScripts/collapseableDIV.js"></script>
    <script type="text/JavaScript" src="../../../JavaScripts/wz_tooltip.js"></script>
    <script type="text/JavaScript" src="../../../JavaScripts/tip_balloon.js"></script>
     <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.10.1/jquery.min.js"></script>
    <script type="text/javascript">
    function PopupPref() {  
    var Page="Preferences.aspx?RepID="+document.getElementById("<%=hidReportVal.ClientID%>").value;            
  newwin = window.open(Page, '_blank');
}

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
        function checkUint(colSeq) {
            // alert(colSeq);
            if (document.getElementById("ctl00_Market1ContentPlaceHolder_hidReportType").value == 'UNIFORM') {
               //alert(colSeq);
               // alert("ctl00_Market1ContentPlaceHolder_Column_" + (colSeq-1));

                                var str = document.getElementById("ctl00_Market1ContentPlaceHolder_Column_" + (colSeq-1)).innerText;
                if (str.indexOf('CAGR') != -1) {
                                    document.getElementById("ctl00_Market1ContentPlaceHolder_lbl_" + (colSeq)).innerText = '(%)';
                }
                else {
                    var txt = document.getElementById("ctl00_Market1ContentPlaceHolder_hidUnitShort").value;
                    //var txt = e.options[e.selectedIndex].value; 
                                    document.getElementById("ctl00_Market1ContentPlaceHolder_lbl_" + (colSeq)).innerText = txt;
                }

            }

        }

    </script>
    <div id="ContentPagemargin" runat="server">
            <div id="PageHeader" class="PageHeading">
            <asp:Label ID="lblheading" runat="Server"></asp:Label>
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
        </div>
        <div id="PageSection1" style="text-align: left">
            <br />
            <table>
                  <tr>
                     <td style="width: 169px">
                        <b>
                        <%--<asp:LinkButton ID="lnlPref" runat="server" PostBackUrl="javascript:PopupPref()" CssClass="Link"
                        Text="Preferences"></asp:LinkButton>--%>
                        <a href="#" onclick="PopupPref();return false;" class="Link">Preferences</a>
                        </b>
                     </td>
                 </tr>
                  <tr>
                    <td align="left" style="width: 260px;display:none;" >                    
                        <asp:RadioButton ID="rdbActual" GroupName="NumFormat" Text="Actual" runat="server" AutoPostBack="true" />
                        <asp:RadioButton ID="rdbThou" GroupName="NumFormat" Text="Thousands" runat="server" AutoPostBack="true" />
                        <asp:RadioButton ID="rdbMil" GroupName="NumFormat" Text="Millions" runat="server" AutoPostBack="true" />
                        <asp:RadioButton ID="rdbBil" GroupName="NumFormat" Text="Billions" runat="server" AutoPostBack="true" />
                    </td>
                </tr>  
            </table>
            <asp:Table ID="tblCAGR" runat="server" CellPadding="0" CellSpacing="2">
            </asp:Table>
        </div>
    </div>
    <asp:HiddenField ID="hidReportType" runat="server" />
    <asp:HiddenField ID="hidUnitShort" runat="server" />
    
     <asp:HiddenField ID="hidReport" runat="server" />
      <asp:HiddenField ID="hidReportData" runat="server" />
 <asp:HiddenField ID="hidReportIDD" runat="server" />
 
  <asp:HiddenField ID="hidReportVal" runat="server" />
  <script type="text/javascript">

      $(document).ready(function () {
          $('#ctl00_Market1ContentPlaceHolder_tblCAGR').on('click', '.header', function (e) {
              e.preventDefault();
              $(this).find('span.icon').text(function (_, value) { return value == '+' ? '-' : '+' });
              $(this).nextUntil('tr.header').toggle();
          });
      });

      function ShowLines(Id, Val) {
          var x = document.getElementsByClassName("Bhavesh");
          if (Val == '-') {
              for (i = 0; i < x.length; i++) {
                  if (x[i].id.search("ctl00_Market1ContentPlaceHolder_" + Id + "_") != -1) {
                      document.getElementById(x[i].id).style.display = 'none';
                      if (x[i].id.search("Data") == -1) {
                          var b = x[i].id.split("ctl00_Market1ContentPlaceHolder_")
                          document.getElementById("ctl00_Market1ContentPlaceHolder_Up_" + b[1]).style.display = "none";
                          document.getElementById("ctl00_Market1ContentPlaceHolder_Dn_" + b[1]).style.display = "inline";
                      }
                  }
              }
              document.getElementById("ctl00_Market1ContentPlaceHolder_Up_" + Id).style.display = "none";
              document.getElementById("ctl00_Market1ContentPlaceHolder_Dn_" + Id).style.display = "inline";
          }
          else {
              for (i = 0; i < x.length; i++) {
                  if (x[i].id.search("ctl00_Market1ContentPlaceHolder_" + Id + "_") != -1) {
                      document.getElementById(x[i].id).style.display = 'table-row';
                      if (x[i].id.search("Data") == -1) {
                          var b = x[i].id.split("ctl00_Market1ContentPlaceHolder_")
                          document.getElementById("ctl00_Market1ContentPlaceHolder_Up_" + b[1]).style.display = "inline";
                          document.getElementById("ctl00_Market1ContentPlaceHolder_Dn_" + b[1]).style.display = "none";
                      }
                  }
              }
              document.getElementById("ctl00_Market1ContentPlaceHolder_Up_" + Id).style.display = "inline";
              document.getElementById("ctl00_Market1ContentPlaceHolder_Dn_" + Id).style.display = "none";
          }
          return false;
      }
  </script>
    <style type="text/css">
        tr.header {
            cursor: pointer;
          
        }

        tr.com {
            display: none;
        }

        tr.com1 {
            display: none;
        }
    </style>
</asp:Content>
