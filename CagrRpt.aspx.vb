Imports System.Data
Imports System.Data.OleDb
Imports System
Imports M1GetData
Imports M1UpInsData
Imports System.Collections
Imports System.IO.StringWriter
Imports System.Math
Imports System.Web.UI.HtmlTextWriter
Partial Class Pages_Market1_CAGR_CagrRpt
    Inherits System.Web.UI.Page
#Region "Get Set Variables"
    Dim _lErrorLble As Label
    Dim _iUserId As Integer
    Dim _strUserRole As String
    Dim _btnLogOff As ImageButton
    Dim _btnUpdate As ImageButton
    Dim _divMainHeading As HtmlGenericControl
    Dim _ctlContentPlaceHolder As ContentPlaceHolder

'n adding comment for Git Trial  

    Public Property ErrorLable() As Label
        Get

            Return _lErrorLble
        End Get
        Set(ByVal Value As Label)
            _lErrorLble = Value
        End Set
    End Property

    Public Property UserId() As Integer
        Get
            Return _iUserId
        End Get
        Set(ByVal Value As Integer)
            _iUserId = Value
        End Set
    End Property

    Public Property UserRole() As String
        Get
            Return _strUserRole
        End Get
        Set(ByVal Value As String)
            _strUserRole = Value
        End Set
    End Property

    Public Property LogOffbtn() As ImageButton
        Get
            Return _btnLogOff
        End Get
        Set(ByVal value As ImageButton)
            _btnLogOff = value
        End Set
    End Property

    Public Property Updatebtn() As ImageButton
        Get
            Return _btnUpdate
        End Get
        Set(ByVal value As ImageButton)
            _btnUpdate = value
        End Set
    End Property

    Public Property MainHeading() As HtmlGenericControl
        Get
            Return _divMainHeading
        End Get
        Set(ByVal value As HtmlGenericControl)
            _divMainHeading = value
        End Set
    End Property

    Public Property ctlContentPlaceHolder() As ContentPlaceHolder
        Get
            Return _ctlContentPlaceHolder
        End Get
        Set(ByVal value As ContentPlaceHolder)
            _ctlContentPlaceHolder = value
        End Set
    End Property



#End Region

#Region "MastePage Content Variables"

    Protected Sub GetMasterPageControls()
        GetErrorLable()
        GetLogOffbtn()
        GetUpdatebtn()
        GetMainHeadingdiv()
        'GetContentPlaceHolder()
    End Sub

    Protected Sub GetErrorLable()
        ErrorLable = Page.Master.FindControl("lblError")
        ErrorLable.Text = String.Empty
    End Sub

    Protected Sub GetLogOffbtn()
        LogOffbtn = Page.Master.FindControl("imgLogoff")
        LogOffbtn.Visible = False
    End Sub

    Protected Sub GetUpdatebtn()
        Updatebtn = Page.Master.FindControl("imgUpdate")
        Updatebtn.Visible = True
        AddHandler Updatebtn.Click, AddressOf Update_Click
    End Sub

    Protected Sub GetMainHeadingdiv()
        MainHeading = Page.Master.FindControl("divMainHeading")
        MainHeading.Attributes.Add("onmouseover", "Tip('CAGR Reports')")
        MainHeading.Attributes.Add("onmouseout", "UnTip()")
        MainHeading.InnerHtml = "CAGR Reports"
    End Sub

    Protected Sub GetContentPlaceHolder()
        ctlContentPlaceHolder = Page.Master.FindControl("Sustain1ContentPlaceHolder")
    End Sub

#End Region


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            GetMasterPageControls()
            If Not IsPostBack Then
                GetPageDetails()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub GetPageDetails()
        Dim objGetData As New Selectdata()
        Dim dsRpt As New DataSet()
        Dim dsRptRws As New DataSet()
        Dim dsRptCols As New DataSet()
        Dim dsRptFilter As New DataSet()
        Dim dsRptAct As New DataSet()
        Dim dsRptFilterValue As New DataSet()
        Dim ColCnt As New Integer
        Dim Dr() As DataRow
        Dim Years As String = String.Empty
        Dim Cntry As String = String.Empty
        Dim i As New Integer
        Dim j As New Integer

        Dim HeaderTr As New TableRow()
        Dim HeaderTd As New TableCell()

        Dim Tr As New TableRow()
        Dim Td As New TableCell()

        Dim lbl As New Label
        Dim str As String = String.Empty
        Dim ColHeader As String = String.Empty
        Dim Fact As New Decimal
        Dim FilterCode As String = String.Empty
        Dim Link As New HyperLink
        Dim hyd As New HiddenField
        Try
            tblCAGR.Rows.Clear()
            dsRpt = objGetData.GetUserCustomReportsByRptId(Session("M1RptId"))
            dsRptRws = objGetData.GetUserReportsRows(Session("M1RptId"))
            dsRptCols = objGetData.GetUserReportsCols(Session("M1RptId"))
            dsRptFilter = objGetData.GetUserReportsFilter(Session("M1RptId"))

            Dr = dsRptCols.Tables(0).Select("COLUMNVALUETYPE='Year'")
            ColCnt = dsRptCols.Tables(0).Rows.Count

            For i = 0 To Dr.Length - 1
                Years = Years + Dr(i).Item("COLUMNVALUE").ToString() + ","
            Next
            Years = Years.Remove(Years.Length - 1, 1)

            Try
                dsRptFilterValue = objGetData.GetCountryIds(dsRptFilter.Tables(0).Rows(0).Item("VALUE").ToString(), dsRptFilter.Tables(0).Rows(0).Item("FILTERTYPE").ToString())
                Cntry = dsRptFilterValue.Tables(0).Rows(0).Item("COUNTRYID").ToString()
            Catch ex As Exception

            End Try




            HeaderTr = New TableRow()
            HeaderTd = New TableCell()
            HeaderTr.Height = 30
            str = "&nbsp;&nbsp;Filter Type:" + dsRptFilter.Tables(0).Rows(0).Item("FILTERNAME").ToString() + "&nbsp;&nbsp;" + "Filter Value:" + dsRptFilter.Tables(0).Rows(0).Item("FILTERVALUE").ToString()
            HeaderTdSetting(HeaderTd, "", str, ColCnt + 1)
            HeaderTd.HorizontalAlign = HorizontalAlign.Left
            HeaderTr.Controls.Add(HeaderTd)
            HeaderTd.CssClass = "AlterNateColor5"
            tblCAGR.Controls.Add(HeaderTr)

            HeaderTr = New TableRow()
            For i = -1 To ColCnt - 1
                HeaderTd = New TableCell()
                hyd = New HiddenField
                Link = New HyperLink
                If i = -1 Then
                    ColHeader = ""
                Else
                    ColHeader = dsRptCols.Tables(0).Rows(i).Item("COLUMNVALUE").ToString()
                    If dsRptCols.Tables(0).Rows(i).Item("COLUMNVALUETYPE").ToString() = "Formula" Then
                        ColHeader = ColHeader + "(" + dsRptCols.Tables(0).Rows(i).Item("INPUTVALUE1").ToString() + "/" + dsRptCols.Tables(0).Rows(i).Item("INPUTVALUE2").ToString() + ")"
                    End If
                    'If 
                    '    
                    Link.ID = "Column_" + i.ToString()
                    hyd.ID = "Column_ID_" + i.ToString()
                    Link.Text = ColHeader
                    hyd.Value = dsRptCols.Tables(0).Rows(i).Item("USERREPORTCOLUMNID").ToString()
                    GetColLink(j, Link, Session("M1RptId"), hyd.ID, hyd.Value)
                    Link.CssClass = "LinkM"
                End If

                'HeaderTdSetting(HeaderTd, "15%", ColHeader, 1)


                HeaderTd.Controls.Add(Link)
                HeaderTd.Controls.Add(hyd)
                HeaderTdWLinkSetting(HeaderTd, "200px", "", "1")
                HeaderTr.Controls.Add(HeaderTd)
            Next
            tblCAGR.Controls.Add(HeaderTr)


            If dsRpt.Tables(0).Rows(0).Item("RPTTYPE") = "2D" Then
                HeaderTr = New TableRow()
                For i = -1 To ColCnt - 1
                    HeaderTd = New TableCell()
                    If i = -1 Then
                        ColHeader = ""
                    Else
                        If dsRptCols.Tables(0).Rows(i).Item("COLUMNVALUETYPE").ToString() <> "Formula" Then
                            ColHeader = dsRptRws.Tables(0).Rows(i).Item("TITLE")
                            HeaderTd.Text = "(" + ColHeader + ")"
                        Else
                            HeaderTd.Text = "(unitless)"
                        End If
                    End If
                    HeaderTdWLinkSetting(HeaderTd, "200px", "", "1")
                    HeaderTr.Controls.Add(HeaderTd)
                Next
                tblCAGR.Controls.Add(HeaderTr)
            End If


            For i = 0 To dsRptRws.Tables(0).Rows.Count - 1
                Dim RowValueType As String = Convert.ToString(dsRptRws.Tables(0).Rows(i).Item("ROWVALUETYPE"))
                If RowValueType = "Product" Or RowValueType = "Package" Then
                    dsRptAct = objGetData.GetCAGRProductReports(dsRptRws.Tables(0).Rows(i).Item("ROWVALUE"), Cntry, Years, dsRptRws.Tables(0).Rows(i).Item("CURR"), dsRptRws.Tables(0).Rows(i).Item("ROWVAL2"), dsRptRws.Tables(0).Rows(i).Item("ROWVAL1"))
                ElseIf RowValueType = "Regions" Then
                    dsRptAct = objGetData.GetCAGRRegReports(dsRptRws.Tables(0).Rows(i).Item("ROWVALUE"), dsRptRws.Tables(0).Rows(i).Item("ROWVAL1"), Years, dsRptRws.Tables(0).Rows(i).Item("CURR"))
                Else
                    dsRptAct = objGetData.GetCAGRReports(dsRptRws.Tables(0).Rows(i).Item("ROWVALUE"), Cntry, Years, dsRptRws.Tables(0).Rows(i).Item("CURR"))
                End If
                Tr = New TableRow()
                For j = -1 To ColCnt - 1
                    Td = New TableCell()
                    Dim Title As String = String.Empty
                    If j = -1 Then
                        If dsRptAct.Tables(0).Rows(j + 1).Item("TITLE").ToString() <> "" Then
                            Title = "(" + dsRptAct.Tables(0).Rows(j + 1).Item("TITLE") + ")"
                        Else
                            Title = ""
                        End If
                        If dsRptRws.Tables(0).Rows(i).Item("ROWVALUE") = "VW_PACKAGESPCAPITA_DATA" Or dsRptRws.Tables(0).Rows(i).Item("ROWVALUE") = "VW_PRODUCTSPCAPITA" Then
                            ColHeader = dsRptRws.Tables(0).Rows(i).Item("ROWDECRIPTION") + "/Capita" + Title
                        Else
                            ColHeader = dsRptRws.Tables(0).Rows(i).Item("ROWDECRIPTION") + Title
                        End If

                        hyd = New HiddenField
                        Link = New HyperLink

                        Link.ID = "Row_" + i.ToString()
                        hyd.ID = "Row_ID_" + i.ToString()
                        Link.Text = ColHeader
                        Link.CssClass = "LinkM"
                        hyd.Value = dsRptRws.Tables(0).Rows(i).Item("USERREPORTROWID").ToString()
                        If dsRpt.Tables(0).Rows(0).Item("RPTTYPE") = "2D" Then
                            GetRowLink(i, Link, Session("M1RptId"), hyd.ID, hyd.Value, dsRpt.Tables(0).Rows(0).Item("RPTTYPE"), dsRptRws.Tables(0).Rows(i).Item("ROWVALUE"), dsRptFilter.Tables(0).Rows(0).Item("VALUE").ToString(), dsRptRws.Tables(0).Rows(i).Item("CURR"))
                        Else
                            GetRowLink(i, Link, Session("M1RptId"), hyd.ID, hyd.Value, dsRpt.Tables(0).Rows(0).Item("RPTTYPE"), "", "", "")
                        End If
                        'InnerTdSetting(Td, "", "Left")
                        Td.Controls.Add(Link)
                        Td.Controls.Add(hyd)

                        HeaderTdWLinkSetting(Td, "150px", "", "1")
                        Td.HorizontalAlign = HorizontalAlign.Left
                        Td.Style.Add("padding-left", "5px")

                    Else
                        If dsRptCols.Tables(0).Rows(j).Item("COLUMNVALUETYPE").ToString() = "Year" Then
                            Fact = GetFactValue(dsRptAct, dsRptCols.Tables(0).Rows(j).Item("COLUMNVALUE").ToString())
                            If dsRptRws.Tables(0).Rows(i).Item("ROWVALUE") = "VW_GDPPCAPITA" Or dsRptRws.Tables(0).Rows(i).Item("ROWVALUE") = "VW_PACKAGESPCAPITA_DATA" Or dsRptRws.Tables(0).Rows(i).Item("ROWVALUE") = "VW_PRODUCTSPCAPITA" Then
                                ColHeader = FormatNumber(Fact, 2)
                            Else
                                ColHeader = FormatNumber(Fact, 0)
                            End If

                            InnerTdSetting(Td, "", "Right")
                        ElseIf dsRptCols.Tables(0).Rows(j).Item("COLUMNVALUE").ToString() = "CAGR" Then
                            Fact = GetCAGR(dsRptAct, dsRptCols.Tables(0).Rows(j).Item("INPUTVALUE1").ToString(), dsRptCols.Tables(0).Rows(j).Item("INPUTVALUE2").ToString())
                            ColHeader = FormatNumber(Fact, 2)
                            InnerTdSetting(Td, "", "Right")
                        End If
                        Td.Text = ColHeader
                    End If


                    Tr.Controls.Add(Td)
                Next
                tblCAGR.Controls.Add(Tr)
                If (i Mod 2 = 0) Then
                    Tr.CssClass = "AlterNateColor1"
                Else
                    Tr.CssClass = "AlterNateColor2"
                End If
            Next











        Catch ex As Exception
            ErrorLable.Text = "Error:GetPageDetails:" + ex.Message.ToString() + ""
        End Try
    End Sub

    Protected Function GetFactValue(ByVal ds As DataSet, ByVal Year As String) As Decimal
        Dim Fact As New Decimal
        Dim dr() As DataRow
        Try
            dr = ds.Tables(0).Select("Year=" + Year + "")
            Fact = Convert.ToDecimal(dr(0).Item("FACT")) * Convert.ToDecimal(dr(0).Item("CURR"))
            Return Fact
        Catch ex As Exception
            ErrorLable.Text = "Error:GetFactValue:" + ex.Message.ToString() + ""
        End Try
    End Function

    Protected Function GetCAGR(ByVal ds As DataSet, ByVal BeginYear As String, ByVal EndYear As String) As Decimal
        Dim CAGR As New Decimal
        Dim BeginYearFct As New Decimal
        Dim EndYearFct As New Decimal
        Dim YearDiff As New Decimal
        Try
            BeginYearFct = GetFactValue(ds, BeginYear)
            EndYearFct = GetFactValue(ds, EndYear)
            YearDiff = EndYear - BeginYear
            CAGR = (((EndYearFct / BeginYearFct) ^ (1 / YearDiff)) - 1) * 100
            Return CAGR
        Catch ex As Exception

        End Try
    End Function

    Protected Sub GetColLink(ByVal Seq As String, ByVal Link As HyperLink, ByVal RptId As String, ByVal hidId As String, ByVal hidValue As String)
        Dim Path As String
        Try
            Path = "../PopUp/ColSelector.aspx?RptId=" + RptId + "&Seq=" + Seq.ToString() + "&Id=ctl00_Market1ContentPlaceHolder_" + Link.ClientID + "&hidId=ctl00_Market1ContentPlaceHolder_" + hidId + ""
            Link.NavigateUrl = "javascript:ShowPopWindow('" + Path + "','ctl00_Market1ContentPlaceHolder_" + hidId + "')"
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub GetRowLink(ByVal Seq As String, ByVal Link As HyperLink, ByVal RptId As String, ByVal hidId As String, ByVal hidValue As String, ByVal RptType As String, ByVal RowVal2D As String, ByVal RegionSetId As String, ByVal Curr As String)
        Dim Path As String
        Try
            Path = "../PopUp/RowSelector.aspx?RptId=" + RptId + "&Seq=" + Seq.ToString() + "&Id=ctl00_Market1ContentPlaceHolder_" + Link.ClientID + "&hidId=ctl00_Market1ContentPlaceHolder_" + hidId + "&RptType=" + RptType + "&RID=" + RegionSetId + "&ROWVAL2D=" + RowVal2D + "&CURR=" + Curr + ""
            Link.NavigateUrl = "javascript:ShowPopWindow('" + Path + "','ctl00_Market1ContentPlaceHolder_" + hidId + "')"
        Catch ex As Exception

        End Try
    End Sub


    Protected Sub HeaderTdSetting(ByVal Td As TableCell, ByVal Width As String, ByVal HeaderText As String, ByVal ColSpan As String)
        Try
            Td.Text = HeaderText
            Td.ColumnSpan = ColSpan
            If Width <> "" Then
                Td.Style.Add("width", Width)
            End If
            Td.CssClass = "TdHeading"
            Td.Height = 20
            Td.Font.Size = 10
            Td.Font.Bold = True
            Td.HorizontalAlign = HorizontalAlign.Center



        Catch ex As Exception
            _lErrorLble.Text = "Error:HeaderTdSetting:" + ex.Message.ToString()
        End Try
    End Sub

    Protected Sub InnerTdSetting(ByVal Td As TableCell, ByVal Width As String, ByVal Align As String)
        Try

            If Width <> "" Then
                Td.Style.Add("width", Width)
            End If
            Td.Style.Add("text-align", Align)
            If Align = "Left" Then
                Td.Style.Add("padding-left", "5px")
            End If
            If Align = "Right" Then
                Td.Style.Add("padding-right", "5px")
            End If
        Catch ex As Exception
            _lErrorLble.Text = "Error:InnerTdSetting:" + ex.Message.ToString()
        End Try
    End Sub

    Protected Sub HeaderTdWLinkSetting(ByVal Td As TableCell, ByVal Width As String, ByVal HeaderText As String, ByVal ColSpan As String)
        Try
            Td.ColumnSpan = ColSpan
            If Width <> "" Then
                Td.Style.Add("width", Width)
            End If
            Td.CssClass = "TdHeading"
            Td.Height = 20
            Td.Font.Size = 10
            Td.Font.Bold = True
            Td.HorizontalAlign = HorizontalAlign.Center



        Catch ex As Exception
            _lErrorLble.Text = "Error:HeaderTdSetting:" + ex.Message.ToString()
        End Try
    End Sub


    Protected Sub Update_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
        Try
            GetPageDetails()
        Catch ex As Exception
            _lErrorLble.Text = "Error:HeaderTdSetting:" + ex.Message.ToString()
        End Try
    End Sub

End Class
