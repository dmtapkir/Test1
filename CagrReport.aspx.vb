Imports System.Data
Imports System.Data.OleDb
Imports System
Imports M1SubGetData
Imports M1SubUpInsData
Imports System.Collections
Imports System.IO.StringWriter
Imports System.Math
Imports System.Web.UI.HtmlTextWriter
Partial Class Pages_Market1_CAGR_CagrReport
    Inherits System.Web.UI.Page

    Dim objUpIns As New UpdateInsert()
#Region "Get Set Variables"
    Dim _lErrorLble As Label
    Dim _iUserId As Integer
    Dim _strUserRole As String
    Dim _btnLogOff As ImageButton
    Dim _btnUpdate As ImageButton
    Dim _divMainHeading As HtmlGenericControl
    Dim _ctlContentPlaceHolder As ContentPlaceHolder
    Dim engmetId As String = 1
    Dim _iRepId As Integer

    Public Property REPId() As Integer
        Get
            Return _iRepId
        End Get
        Set(ByVal Value As Integer)
            _iRepId = Value
        End Set
    End Property
    'n

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
        MainHeading.Attributes.Add("onmouseover", "Tip('Market1 Subscription Reports')")
        MainHeading.Attributes.Add("onmouseout", "UnTip()")
        ' MainHeading.InnerHtml = "Market1 Reports"

    End Sub

    Protected Sub GetContentPlaceHolder()
        ctlContentPlaceHolder = Page.Master.FindControl("Market1ContentPlaceHolder")
    End Sub

#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            GetMasterPageControls()
            REPId = Request.QueryString("RepId").ToString()
            hidReportVal.Value = REPId
            If Not IsPostBack Then
                Session("dsPref" + REPId.ToString()) = ""
                Session("dsRows" + REPId.ToString()) = ""
                Session("dsColumns" + REPId.ToString()) = ""
                Session("dsFilters" + REPId.ToString()) = ""
                Session("dsData" + REPId.ToString()) = ""
                Session("PrefChange" + REPId.ToString()) = "0"
                hidReport.Value = "0"
                hidReportData.Value = "0"
                GetReportDetails()
                GetPageDetails()
                rdbActual.Checked = True
                hidReportIDD.Value ="0"
            Else
                If Session("PrefChange" + REPId.ToString()) = "1" Then
                    hidReport.Value = "0"
                    hidReportData.Value = "0"
                    Session("PrefChange" + REPId.ToString()) = "0"
                End If
                If hidReportIDD.Value <> "0" Then
                    Session("dsPref" + REPId.ToString()) = ""
                    Session("dsRows" + REPId.ToString()) = ""
                    Session("dsColumns" + REPId.ToString()) = ""
                    Session("dsFilters" + REPId.ToString()) = ""
                    Session("dsData" + REPId.ToString()) = ""
                    hidReport.Value = "0"
                    hidReportData.Value = "0"
                    hidReportIDD.Value ="0"
                End If
                End If
        Catch ex As Exception
        End Try
    End Sub
    Protected Sub GetReportDetails()
        Dim objGetData As New M1GetData.Selectdata()
        Dim ds As New DataSet
        Try
            If Request.QueryString("Type").ToString() = "Base" Then
                'ds = objGetData.GetBaseReportsByRptId(REPId.ToString().ToString())
                ds = objGetData.GetUserCustomReportsByRptId(REPId.ToString())
                lblheading.Text = "Base Report Results"
            Else
                ds = objGetData.GetUserCustomReportsByRptId(REPId.ToString())
                lblheading.Text = "Proprietary Report Results"
            End If
            If ds.Tables(0).Rows.Count > 0 Then
                hidReportType.Value = ds.Tables(0).Rows(0).Item("RPTTYPE").ToString()
                lblReportID.Text = ds.Tables(0).Rows(0).Item("REPORTID").ToString()
                lblReportType.Text = ds.Tables(0).Rows(0).Item("RPTTYPE").ToString() + " (" + ds.Tables(0).Rows(0).Item("RPTTYPEDES").ToString() + ")"
                lblReportDe2.Text = ds.Tables(0).Rows(0).Item("REPORTNAME").ToString()
            End If
        Catch ex As Exception
            ErrorLable.Text = "Error:GetReportDetails:" + ex.Message.ToString()
        End Try
    End Sub
    Protected Sub GetPageDetails()
        Dim objGetData As New Selectdata()
        Dim dsRpt As New DataSet()
        Dim rptType As String = String.Empty
        Try
            rptType = Request.QueryString("Type").ToString()
            If rptType = "Base" Then
                dsRpt = objGetData.GetBaseReportsByRptId(REPId.ToString())
            Else
                dsRpt = objGetData.GetUserCustomReportsByRptId(REPId.ToString())
            End If

            If dsRpt.Tables(0).Rows(0)("RPTTYPE").ToString() = "UNIFORM" Then
                Session("RPTTYPE") = "UNIFORM"
                objUpIns.InsertLog(Session("UserId"), Session("MLogInLog"), "3", REPId.ToString(), "Uniform", dsRpt.Tables(0).Rows(0)("RPTTYPEDES").ToString(), "2", REPId.ToString(), Session.SessionID)

                SetUniformReport(dsRpt.Tables(0).Rows(0)("RPTTYPEDES").ToString(), rptType, dsRpt)
            ElseIf dsRpt.Tables(0).Rows(0)("RPTTYPE").ToString() = "MIXED" Then
                objUpIns.InsertLog(Session("UserId"), Session("MLogInLog"), "3", REPId.ToString(), "Mixed", dsRpt.Tables(0).Rows(0)("RPTTYPEDES").ToString(), "2", REPId.ToString(), Session.SessionID)

                Session("RPTTYPE") = "MIXED"
                SetMixedReport_New(dsRpt.Tables(0).Rows(0)("RPTTYPEDES").ToString(), rptType)
            End If
        Catch ex As Exception
            ErrorLable.Text = "Error:GetPageDetails:" + ex.Message.ToString() + ""
        End Try
    End Sub

    Private Sub SetUniformReport(ByVal reportDes As String, ByVal rptType As String, ByVal dsRPTDET As DataSet)
        Dim objGetData As New Selectdata()
        Dim dsRptRws As New DataSet()
        Dim dsRptCols As New DataSet()
        Dim dsRptFilter As New DataSet()
        Dim dsRptAct As New DataSet()
        Dim dsRptFilterValue As New DataSet()
        Dim dsUnitPref As New DataSet
        Dim ColCnt As New Integer
        Dim RowCnt As New Integer
        Dim FiltCnt As New Integer
        Dim groupId As String = String.Empty
        Dim Dr() As DataRow
        Dim i As New Integer
        Dim j As New Integer
        Dim l As New Integer
        Dim HeaderTr As New TableRow()
        Dim HeaderTd As New TableCell()
        Dim Tr As New TableRow()
        Dim Td As New TableCell()
        Dim lbl As New Label
        Dim Fact As New Decimal
        Dim Link As New HyperLink
        Dim hyd As New HiddenField
        Dim CountryFilter As Boolean
        Dim k1 As Integer = 0

        Dim dValue As Integer = 1

        If rdbActual.Checked = True Then
            dValue = 1
        ElseIf rdbMil.Checked = True Then
            dValue = 1000000
        ElseIf rdbBil.Checked = True Then
            dValue = 1000000000
        ElseIf rdbThou.Checked = True Then
            dValue = 1000
        End If
        'Changes started 27Feb2020
        If dsRPTDET.Tables(0).Rows(0).Item("RPTTYPEDES").ToString() = "GROUP" Then
            Dim Regionset = dsRPTDET.Tables(0).Rows(0).Item("REGIONSETID").ToString()
            UpdateReportRows(dsRPTDET.Tables(0).Rows(0).Item("RPTTYPEDES").ToString(), Regionset, dsRPTDET)
        Else
            UpdateReportRows(dsRPTDET.Tables(0).Rows(0).Item("RPTTYPEDES").ToString(), "", dsRPTDET)
        End If
        'Changes ended 27Feb2020

        If hidReport.Value <> "0" Then
            dsUnitPref = Session("dsPref" + REPId.ToString())
            dsRptRws = Session("dsRows" + REPId.ToString())
            dsRptCols = Session("dsColumns" + REPId.ToString())
            dsRptFilter = Session("dsFilters" + REPId.ToString())
        Else
            dsUnitPref = objGetData.GetPref(REPId.ToString())
            dsRptRws = objGetData.GetUsersDynamicReportRows(REPId.ToString())
            dsRptCols = objGetData.GetUsersDynamicReportCols(REPId.ToString())
            dsRptFilter = objGetData.GetUsersReportFilters(REPId.ToString())

            Session("dsPref" + REPId.ToString()) = dsUnitPref
            Session("dsRows" + REPId.ToString()) = dsRptRws
            Session("dsColumns" + REPId.ToString()) = dsRptCols
            Session("dsFilters" + REPId.ToString()) = dsRptFilter

            hidReport.Value = "1"
        End If



        groupId = Session("M1SubGroupId").ToString()

        If dsRptFilter.Tables(0).Rows.Count > 0 Then
            For k1 = 0 To dsRptFilter.Tables(0).Rows.Count - 1
                If dsRptFilter.Tables(0).Rows(k1).Item("FILTERTYPE").ToString().ToUpper() = "COUNTRY" Then
                    CountryFilter = True
                End If
            Next
        End If

        Try
            If hidReportData.Value <> "0" Then
                dsRptAct = Session("dsData" + REPId.ToString())
            Else
                If REPId.ToString() = "16" Or REPId.ToString() = "17" Then
                    dsRptAct = GetReportData(REPId.ToString())
                    If dsRptAct.Tables(0).Rows.Count > 0 Then
                        Session("dsData" + REPId.ToString()) = dsRptAct
                        Dim dsT As New DataSet
                        dsT = objGetData.GetUsersDynamicUniformReportData(13, reportDes, CountryFilter, groupId)
                    Else
                        dsRptAct = objGetData.GetUsersDynamicUniformReportData(REPId.ToString(), reportDes, CountryFilter, groupId)
                        Session("dsData" + REPId.ToString()) = dsRptAct
                        hidReportData.Value = "1"
                        UpdateTempReportData(REPId.ToString(), dsRptAct)

                    End If
                Else
                    ' If rptType = "group" Then
                    'Dim regionset = 300 ' dsrep.tables(0).rows(0).item("regionsetid").tostring()
                    'UpdateReportRows("GROUP", regionset)
                    ' Else
                    ' UpdateReportRows(rptType, "")
                    ' End If

                    dsRptAct = objGetData.GetUsersDynamicUniformReportData(REPId.ToString(), reportDes, CountryFilter, groupId)
                    Session("dsData" + REPId.ToString()) = dsRptAct
                    hidReportData.Value = "1"
                End If
            End If


        Catch ex As Exception

        End Try


        If dsRptRws.Tables.Count > 0 Then
            RowCnt = dsRptRws.Tables(0).Rows.Count - 1
        End If
        If dsRptCols.Tables.Count > 0 Then
            ColCnt = dsRptCols.Tables(0).Rows.Count
        End If
        If dsRptFilter.Tables.Count > 0 Then
            FiltCnt = dsRptFilter.Tables(0).Rows.Count
        End If

        tblCAGR.Rows.Clear()
        'filter row
        HeaderTr = New TableRow
        For j = 0 To ColCnt
            HeaderTd = New TableCell()
            If j = 0 Then
                For l = 0 To FiltCnt - 1
                    Link = New HyperLink
                    hyd = New HiddenField
                    lbl = New Label
                    Link.Text = "Filter " + (l + 1).ToString() + ":" + dsRptFilter.Tables(0).Rows(l).Item("FILTERVALUE") + "<br/> "
                    lbl.Text = "Filter " + (l + 1).ToString() + ":" + dsRptFilter.Tables(0).Rows(l).Item("FILTERVALUE") + "<br/> "
                    hyd.Value = dsRptFilter.Tables(0).Rows(l).Item("USERREPORTFILTERID").ToString()
                    Link.ID = "Filter_" + (l + 1).ToString()
                    hyd.ID = "Filter_ID_" + (l + 1).ToString()
                    Link.CssClass = "LinkM"

                    GetFilterLink(dsRptFilter.Tables(0).Rows(l).Item("FILTERSEQUENCE").ToString(), Link, dsRptFilter.Tables(0).Rows(l).Item("USERREPORTID").ToString(), hyd.ClientID, hyd.Value)
                    If rptType = "Base" Then
                        HeaderTd.Controls.Add(lbl)
                    Else
                        HeaderTd.Controls.Add(Link)
                    End If
                    HeaderTd.Controls.Add(hyd)
                Next
                HeaderTdSetting(HeaderTd, "150px", "", "1")
            Else
                HeaderTdSetting(HeaderTd, "100px", "", "1")
            End If
            HeaderTd.Font.Size = 8.5
            HeaderTd.Font.Name = "Verdana"
            HeaderTd.Font.Bold = False
            HeaderTr.Controls.Add(HeaderTd)
        Next
        tblCAGR.Controls.Add(HeaderTr)


        'Hedaer Row
        HeaderTr = New TableRow
        For j = 0 To ColCnt
            HeaderTd = New TableCell()
            If j = 0 Then
                lbl = New Label
                If reportDes = "REGION" Then
                    lbl.Text = "GEOGRAPHIC REGIONS"
                ElseIf reportDes = "PACKGRP" Then
                    lbl.Text = "PACKAGES"
                ElseIf reportDes = "PROD" Then
                    lbl.Text = "PRODUCTS"
                ElseIf reportDes = "MAT" Then
                    lbl.Text = "MATERIALS"
                ElseIf reportDes = "PACK" Then
                    lbl.Text = "PACKAGES"
                ElseIf reportDes = "GROUP" Then
                    lbl.Text = "GROUP"
                    'lbl.Text = "MOD"
                ElseIf reportDes = "CNTRY" Then
                    lbl.Text = "COUNTRIES"
                End If
                HeaderTd.Controls.Add(lbl)
                HeaderTdSetting(HeaderTd, "150px", "", "1")
            Else
                hyd = New HiddenField
                Link = New HyperLink
                lbl = New Label
                Dim k As New Integer
                k = j - 1
                If dsRptCols.Tables(0).Rows(k).Item("COLUMNVALUETYPE").ToString() = "Formula" Then
                    Link.Text = dsRptCols.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + "(" + dsRptCols.Tables(0).Rows(k).Item("INPUTVALUE1").ToString() + "/" + dsRptCols.Tables(0).Rows(k).Item("INPUTVALUE2").ToString() + ")"
                Else
                    Link.Text = dsRptCols.Tables(0).Rows(k).Item("COLUMNVALUE").ToString()
                End If
                lbl.Text = Link.Text
                Link.ID = "Column_" + k.ToString()
                hyd.ID = "Column_ID_" + k.ToString()
                hyd.Value = dsRptCols.Tables(0).Rows(k).Item("USERREPORTCOLUMNID").ToString()
                Link.CssClass = "LinkM"
                GetColLink(dsRptCols.Tables(0).Rows(k).Item("COLUMNSEQUENCE").ToString(), Link, dsRptCols.Tables(0).Rows(k).Item("USERREPORTID").ToString(), hyd.ClientID, hyd.Value)
                If rptType = "Base" Then
                    HeaderTd.Controls.Add(lbl)
                Else
                    HeaderTd.Controls.Add(Link)
                End If
                HeaderTd.Controls.Add(hyd)
                HeaderTdSetting(HeaderTd, "100px", "", "1")

            End If
            HeaderTd.Font.Size = 8.5
            HeaderTd.Font.Name = "Verdana"
            HeaderTd.Font.Bold = False
            HeaderTr.Controls.Add(HeaderTd)
        Next
        tblCAGR.Controls.Add(HeaderTr)


        'SET THE UNIT & SUMMERY PART
        For i = 0 To 0
            Tr = New TableRow()
            For j = 0 To ColCnt
                Td = New TableCell()
                lbl = New Label
                If j = 0 Then
                    HeaderTdSetting(Td, "150px", "", "1")
                    If i = 0 Then
                        lbl.Text = "UNIT"
                    End If
                Else
                    HeaderTdSetting(Td, "100px", "", "1")
                    If i = 0 Then
                        lbl.ID = "lbl_" + j.ToString()
                        If dsRptCols.Tables(0).Rows(j - 1).Item("COLUMNVALUETYPE").ToString() = "Formula" Then
                            lbl.Text = "(%)"
                        Else
                            If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                                lbl.Text = "(" + dsRptRws.Tables(0).Rows(0).Item("UNITSHRT").ToString() + ")"
                            Else
                                lbl.Text = "(" + dsRptRws.Tables(0).Rows(0).Item("METRICUNIT").ToString() + ")"
                            End If

                            hidUnitShort.Value = lbl.Text
                        End If
                        Td.Style.Add("text-align", "Center")
                    End If
                End If

                Td.Font.Size = 8
                Td.Font.Name = "Verdana"
                Td.Font.Bold = False
                Td.Controls.Add(lbl)
                Tr.Controls.Add(Td)
            Next
            tblCAGR.Controls.Add(Tr)
        Next
        'Inner Row
        For i = 0 To RowCnt
            Tr = New TableRow()
            Dim flag As Boolean = False
            For j = 0 To ColCnt
                Td = New TableCell()
                If j = 0 Then
                    HeaderTdSetting(Td, "150px", "", "1")
                    lbl = New Label
                    lbl.Text = dsRptRws.Tables(0).Rows(i).Item("ROWVALUE").ToString() '+ "(" + dsRptRws.Tables(0).Rows(i).Item("UNITSHRT").ToString() + ")"

                    Td.Style.Add("text-align", "Left")
                    Td.Font.Size = 10
                    Td.Font.Name = "Verdana"
                    Td.Font.Bold = True
                    Td.Style.Add("padding-left", "5px")
                    Td.Controls.Add(lbl)
                Else
                    Dim k As New Integer
                    k = j - 1
                    Dim ColType As String = dsRptCols.Tables(0).Rows(k).Item("COLUMNVALUETYPE").ToString()
                    Dim RowId As String = String.Empty
                    Dim ColId As String = String.Empty
                    Dim RowValue As String = String.Empty
                    Dim ColValue As String = String.Empty
                    Dim UnitId As String = String.Empty
                    Dim UnitValue As String = String.Empty
                    RowId = dsRptRws.Tables(0).Rows(i).Item("ROWCOLUMNID").ToString()
                    RowValue = dsRptRws.Tables(0).Rows(i).Item("ROWVALUEID").ToString()
                    ColId = dsRptCols.Tables(0).Rows(k).Item("COLCOLUMNID").ToString()
                    ColValue = dsRptCols.Tables(0).Rows(k).Item("COLUMNVALUEID").ToString()
                    UnitId = "UnitId"
                    UnitValue = dsRptRws.Tables(0).Rows(i).Item("UNITID").ToString()

                    If ColType = "Year" Then
                        If CDbl(GetFactValue(dsRptAct, RowId, ColId, RowValue, ColValue, UnitId, UnitValue, dsUnitPref)) <> 0.0 Then
                            'Td.Text = FormatNumber(CDbl(GetFactValue(dsRptAct, RowId, ColId, RowValue, ColValue, UnitId, UnitValue, dsUnitPref)), 0)
                            If rdbMil.Checked = True Or rdbBil.Checked = True Then
                                Td.Text = FormatNumber(CDbl(GetFactValue(dsRptAct, RowId, ColId, RowValue, ColValue, UnitId, UnitValue, dsUnitPref) / dValue), 3)
                            Else
                                Td.Text = FormatNumber(CDbl(GetFactValue(dsRptAct, RowId, ColId, RowValue, ColValue, UnitId, UnitValue, dsUnitPref) / dValue), 0)
                            End If
                            flag = True
                        End If

                    Else
                        Td.Text = "0.00"
                        Dim BeginYearId As String = String.Empty
                        Dim EndYearId As String = String.Empty
                        Dr = dsRptCols.Tables(0).Select("USERREPORTCOLUMNID=" + dsRptCols.Tables(0).Rows(k).Item("INPUTVALUETYPE1").ToString() + "")

                        BeginYearId = Dr(0).Item("COLUMNVALUEID").ToString() 'dsRptCols.Tables(0).Rows(k).Item("INPUTVALUETYPE1").ToString()
                        Dr = dsRptCols.Tables(0).Select("USERREPORTCOLUMNID=" + dsRptCols.Tables(0).Rows(k).Item("INPUTVALUETYPE2").ToString() + "")
                        EndYearId = Dr(0).Item("COLUMNVALUEID").ToString() 'dsRptCols.Tables(0).Rows(k).Item("INPUTVALUETYPE2").ToString()
                        Td.Text = FormatNumber(GetCAGR(dsRptAct, RowId, ColId, RowValue, UnitId, UnitValue, BeginYearId, EndYearId), 4)
                    End If

                    InnerTdSetting(Td, "", "Right")
                    Td.CssClass = "AlterNateColor2"
                End If
                Td.Font.Size = 8.5
                Td.Font.Name = "Verdana"
                Td.Font.Bold = False
                Tr.Controls.Add(Td)
            Next
            'tblCAGR.Controls.Add(Tr)
            If flag = True Then
                tblCAGR.Controls.Add(Tr)
            End If
        Next

        For i = 0 To 0
            Tr = New TableRow()
            For j = 0 To ColCnt
                Td = New TableCell()
                lbl = New Label
                If j = 0 Then
                    HeaderTdSetting(Td, "150px", "", "1")
                    If i = 0 Then
                        lbl.Text = "GRAND TOTAL"
                    End If
                Else
                    HeaderTdSetting(Td, "100px", "", "1")
                    If i = 0 Then
                        lbl.ID = "lblb_" + j.ToString()
                        If dsRptCols.Tables(0).Rows(j - 1).Item("COLUMNVALUETYPE").ToString() = "Formula" Then
                            Dim BeginYearId As String = String.Empty
                            Dim EndYearId As String = String.Empty
                            BeginYearId = dsRptCols.Tables(0).Rows(j - 1).Item("INPUTVALUETYPE1").ToString()
                            EndYearId = dsRptCols.Tables(0).Rows(j - 1).Item("INPUTVALUETYPE2").ToString()
                            lbl.Text = FormatNumber(GetCAGRValueTotal(dsRptAct, dsRptCols.Tables(0).Rows(j - 1).Item("COLCOLUMNID").ToString(), BeginYearId, EndYearId, dsRptCols), 4)
                        Else
                            If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                                If dsRptRws.Tables(0).Rows(0).Item("UNITID").ToString() = "1" Then
                                    lbl.Text = GetFactValueTotalCountry(dsRptAct, dsRptCols.Tables(0).Rows(j - 1).Item("COLCOLUMNID").ToString(), dsRptCols.Tables(0).Rows(j - 1).Item("COLUMNVALUEID").ToString(), dsRptRws)
                                ElseIf dsRptRws.Tables(0).Rows(0).Item("UNITID").ToString() = "2" Then
                                    lbl.Text = GetFactValueTotalCountry(dsRptAct, dsRptCols.Tables(0).Rows(j - 1).Item("COLCOLUMNID").ToString(), dsRptCols.Tables(0).Rows(j - 1).Item("COLUMNVALUEID").ToString(), dsRptRws)
                                ElseIf dsRptRws.Tables(0).Rows(0).Item("UNITID").ToString() = "4" Then
                                    lbl.Text = GetFactValueTotalCountry(dsRptAct, dsRptCols.Tables(0).Rows(j - 1).Item("COLCOLUMNID").ToString(), dsRptCols.Tables(0).Rows(j - 1).Item("COLUMNVALUEID").ToString(), dsRptRws)
                                Else
                                    lbl.Text = GetFactValueTotalCountry(dsRptAct, dsRptCols.Tables(0).Rows(j - 1).Item("COLCOLUMNID").ToString(), dsRptCols.Tables(0).Rows(j - 1).Item("COLUMNVALUEID").ToString(), dsRptRws)
                                End If
                            ElseIf dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 1 Then
                                If dsRptRws.Tables(0).Rows(0).Item("UNITID").ToString() = "1" Then
                                    lbl.Text = (GetFactValueTotalCountry(dsRptAct, dsRptCols.Tables(0).Rows(j - 1).Item("COLCOLUMNID").ToString(), dsRptCols.Tables(0).Rows(j - 1).Item("COLUMNVALUEID").ToString(), dsRptRws) * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT")).ToString()
                                ElseIf dsRptRws.Tables(0).Rows(0).Item("UNITID").ToString() = "2" Then
                                    lbl.Text = (GetFactValueTotalCountry(dsRptAct, dsRptCols.Tables(0).Rows(j - 1).Item("COLCOLUMNID").ToString(), dsRptCols.Tables(0).Rows(j - 1).Item("COLUMNVALUEID").ToString(), dsRptRws) * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL").ToString())
                                ElseIf dsRptRws.Tables(0).Rows(0).Item("UNITID").ToString() = "3" Then
                                    lbl.Text = (GetFactValueTotalCountry(dsRptAct, dsRptCols.Tables(0).Rows(j - 1).Item("COLCOLUMNID").ToString(), dsRptCols.Tables(0).Rows(j - 1).Item("COLUMNVALUEID").ToString(), dsRptRws) * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA").ToString())
                                ElseIf dsRptRws.Tables(0).Rows(0).Item("UNITID").ToString() = "4" Then
                                    lbl.Text = GetFactValueTotalCountry(dsRptAct, dsRptCols.Tables(0).Rows(j - 1).Item("COLCOLUMNID").ToString(), dsRptCols.Tables(0).Rows(j - 1).Item("COLUMNVALUEID").ToString(), dsRptRws)
                                Else
                                    lbl.Text = GetFactValueTotalCountry(dsRptAct, dsRptCols.Tables(0).Rows(j - 1).Item("COLCOLUMNID").ToString(), dsRptCols.Tables(0).Rows(j - 1).Item("COLUMNVALUEID").ToString(), dsRptRws)
                                End If
                            End If
                            'lbl.Text = FormatNumber(lbl.Text, 0)
                            If rdbMil.Checked = True Or rdbBil.Checked = True Then
                                lbl.Text = FormatNumber((lbl.Text / dValue), 3)
                            Else
                                lbl.Text = FormatNumber((lbl.Text / dValue), 0)
                            End If
                        End If
                        Td.Style.Add("text-align", "Right")
                        Td.Style.Add("padding-right", "5px")
                        Td.CssClass = "AlterNateColor2"
                    End If

                End If

                Td.Font.Size = 8
                Td.Font.Name = "Verdana"
                Td.Font.Bold = False
                Td.Controls.Add(lbl)
                Tr.Controls.Add(Td)
            Next
            tblCAGR.Controls.Add(Tr)
        Next
        tblCAGR.Visible = True
    End Sub

    Private Sub SetMixedReport(ByVal reportDes As String, ByVal rptType As String)
        Dim objGetData As New Selectdata()
        Dim dsRptRws As New DataSet()
        Dim dsRptCols As New DataSet()
        Dim dsRptFilter As New DataSet()
        Dim dsRptAct As New DataSet()
        Dim dsUnitPref As New DataSet
        Dim ColCnt As New Integer
        Dim RowCnt As New Integer
        Dim FiltCnt As New Integer
        Dim Dr() As DataRow
        Dim i As New Integer
        Dim j As New Integer
        Dim l As New Integer
        Dim HeaderTr As New TableRow()
        Dim HeaderTd As New TableCell()
        Dim Tr As New TableRow()
        Dim Td As New TableCell()
        Dim lbl As New Label
        Dim Link As New HyperLink
        Dim hyd As New HiddenField
        Dim groupId As String = String.Empty
        Dim dValue As Integer = 1

        If rdbActual.Checked = True Then
            dValue = 1
        ElseIf rdbMil.Checked = True Then
            dValue = 1000000
        ElseIf rdbBil.Checked = True Then
            dValue = 1000000000
        ElseIf rdbThou.Checked = True Then
            dValue = 1000
        End If

        If hidReport.Value <> "0" Then
            dsUnitPref = Session("dsPref" + REPId.ToString())
            dsRptRws = Session("dsRows" + REPId.ToString())
            dsRptCols = Session("dsColumns" + REPId.ToString())
            dsRptFilter = Session("dsFilters" + REPId.ToString())
        Else
            dsUnitPref = objGetData.GetPref(REPId.ToString())
            dsRptRws = objGetData.GetUsersDynamicReportRows(REPId.ToString())
            dsRptCols = objGetData.GetUsersDynamicReportCols(REPId.ToString())
            dsRptFilter = objGetData.GetUsersReportFilters(REPId.ToString())

            Session("dsPref" + REPId.ToString()) = dsUnitPref
            Session("dsRows" + REPId.ToString()) = dsRptRws
            Session("dsColumns" + REPId.ToString()) = dsRptCols
            Session("dsFilters" + REPId.ToString()) = dsRptFilter

            hidReport.Value = "1"
        End If


        'changes Start by Bhavesh
        groupId = Session("M1SubGroupId").ToString()
        Try
            If hidReportData.Value <> "0" Then
                dsRptAct = Session("dsData")
            Else
                dsRptAct = objGetData.GetUsersDynamicReportData(Session("M1RptId"), reportDes, groupId)

                Session("dsData") = dsRptAct
                hidReportData.Value = "1"
            End If


        Catch ex As Exception

        End Try
        'End changes by Bhavesh



        If dsRptRws.Tables.Count > 0 Then
            RowCnt = dsRptRws.Tables(0).Rows.Count - 1
        End If

        If dsRptCols.Tables.Count > 0 Then
            ColCnt = dsRptCols.Tables(0).Rows.Count
        End If

        If dsRptFilter.Tables.Count > 0 Then
            FiltCnt = dsRptFilter.Tables(0).Rows.Count
        End If

        tblCAGR.Rows.Clear()
        'filter row
        HeaderTr = New TableRow
        For j = 0 To ColCnt
            HeaderTd = New TableCell()
            If j = 0 Then
                For l = 0 To FiltCnt - 1
                    Link = New HyperLink
                    hyd = New HiddenField
                    lbl = New Label
                    Link.Text = "Filter " + (l + 1).ToString() + ":" + dsRptFilter.Tables(0).Rows(l).Item("FILTERVALUE") + "<br/> "
                    lbl.Text = "Filter " + (l + 1).ToString() + ":" + dsRptFilter.Tables(0).Rows(l).Item("FILTERVALUE") + "<br/> "
                    hyd.Value = dsRptFilter.Tables(0).Rows(l).Item("USERREPORTFILTERID").ToString()
                    Link.ID = "Column_Fil_" + l.ToString()
                    hyd.ID = "Column_ID_Fil_" + l.ToString()
                    Link.CssClass = "LinkM"

                    GetFilterLink(dsRptFilter.Tables(0).Rows(l).Item("FILTERSEQUENCE").ToString(), Link, dsRptFilter.Tables(0).Rows(l).Item("USERREPORTID").ToString(), hyd.ClientID, hyd.Value)
                    If rptType = "Base" Then
                        HeaderTd.Controls.Add(lbl)
                    Else
                        HeaderTd.Controls.Add(Link)
                    End If
                    HeaderTd.Controls.Add(hyd)
                Next
                HeaderTdSetting(HeaderTd, "150px", "", "1")
            Else
                HeaderTdSetting(HeaderTd, "100px", "", "1")
            End If
            HeaderTd.Font.Size = 8.5
            HeaderTd.Font.Name = "Verdana"
            HeaderTd.Font.Bold = False
            HeaderTr.Controls.Add(HeaderTd)
        Next
        tblCAGR.Controls.Add(HeaderTr)

        'Hedaer Row
        HeaderTr = New TableRow
        For j = 0 To ColCnt
            HeaderTd = New TableCell()
            If j = 0 Then
                'lbl = New Label

                ' HeaderTd.Controls.Add(lbl)
                HeaderTdSetting(HeaderTd, "150px", "", "1")
            Else
                hyd = New HiddenField
                Link = New HyperLink
                lbl = New Label
                Dim k As New Integer
                k = j - 1
                If dsRptCols.Tables(0).Rows(k).Item("COLUMNVALUETYPE").ToString() = "Formula" Then
                    Link.Text = dsRptCols.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + "(" + dsRptCols.Tables(0).Rows(k).Item("INPUTVALUE1").ToString() + "/" + dsRptCols.Tables(0).Rows(k).Item("INPUTVALUE2").ToString() + ")"
                Else
                    Link.Text = dsRptCols.Tables(0).Rows(k).Item("COLUMNVALUE").ToString()
                End If
                lbl.Text = Link.Text
                Link.ID = "Column_" + k.ToString()
                hyd.ID = "Column_ID_" + k.ToString()
                hyd.Value = dsRptCols.Tables(0).Rows(k).Item("USERREPORTCOLUMNID").ToString()
                Link.CssClass = "LinkM"
                GetColLink(dsRptCols.Tables(0).Rows(k).Item("COLUMNSEQUENCE").ToString(), Link, dsRptCols.Tables(0).Rows(k).Item("USERREPORTID").ToString(), hyd.ClientID, hyd.Value)
                If rptType = "Base" Then
                    HeaderTd.Controls.Add(lbl)
                Else
                    HeaderTd.Controls.Add(Link)
                End If
                HeaderTd.Controls.Add(hyd)
                HeaderTdSetting(HeaderTd, "100px", "", "1")

            End If
            HeaderTd.Font.Size = 8.5
            HeaderTd.Font.Name = "Verdana"
            HeaderTd.Font.Bold = False
            HeaderTr.Controls.Add(HeaderTd)
        Next
        tblCAGR.Controls.Add(HeaderTr)


        'Inner Row
        For i = 0 To RowCnt
            Tr = New TableRow()
            For j = 0 To ColCnt
                Td = New TableCell()
                If j = 0 Then
                    HeaderTdSetting(Td, "150px", "", "1")
                    hyd = New HiddenField
                    Link = New HyperLink
                    lbl = New Label
                    Link.ID = "Row_" + i.ToString()
                    hyd.ID = "Row_ID_" + i.ToString()
                    hyd.Value = dsRptRws.Tables(0).Rows(i).Item("USERREPORTROWID").ToString()
                    Link.CssClass = "LinkM"
                    If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                        Link.Text = dsRptRws.Tables(0).Rows(i).Item("ROWVALUE").ToString() + " (" + dsRptRws.Tables(0).Rows(i).Item("UNITSHRT").ToString() + ")"
                    Else
                        Link.Text = dsRptRws.Tables(0).Rows(i).Item("ROWVALUE").ToString() + " (" + dsRptRws.Tables(0).Rows(i).Item("METRICUNIT").ToString() + ")"
                    End If
                    lbl.Text = Link.Text
                    GetRowLink(dsRptRws.Tables(0).Rows(i).Item("ROWSEQUENCE").ToString(), Link, dsRptRws.Tables(0).Rows(i).Item("USERREPORTID").ToString(), hyd.ClientID, hyd.Value)
                    Td.Style.Add("text-align", "Left")
                    Td.Style.Add("padding-left", "5px")
                    If rptType = "Base" Then
                        Td.Controls.Add(lbl)
                    Else
                        Td.Controls.Add(Link)
                    End If
                    Td.Controls.Add(hyd)

                Else
                    Dim k As New Integer
                    k = j - 1
                    Dim ColType As String = dsRptCols.Tables(0).Rows(k).Item("COLUMNVALUETYPE").ToString()
                    Dim RowType As String = dsRptRws.Tables(0).Rows(i).Item("ROWVALUETYPE").ToString()
                    Dim RowId As String = String.Empty
                    Dim ColId As String = String.Empty
                    Dim RowValue As String = String.Empty
                    Dim ColValue As String = String.Empty
                    Dim UnitId As String = String.Empty
                    Dim UnitValue As String = String.Empty
                    RowId = dsRptRws.Tables(0).Rows(i).Item("ROWCOLUMNID").ToString()
                    RowValue = dsRptRws.Tables(0).Rows(i).Item("ROWVALUEID").ToString()
                    ColId = dsRptCols.Tables(0).Rows(k).Item("COLCOLUMNID").ToString()
                    ColValue = dsRptCols.Tables(0).Rows(k).Item("COLUMNVALUEID").ToString()
                    UnitId = "UnitId"
                    UnitValue = dsRptRws.Tables(0).Rows(i).Item("UNITID").ToString()

                    If ColType = "Year" Then
                        'If RowType = "Category" Then
                        '    RowValue = "9999" + RowValue
                        'End If
                        If GetFactValueM(dsRptAct, RowId, ColId, RowValue, ColValue, UnitId, UnitValue, dsUnitPref).ToString() <> String.Empty Then
                            Td.Text = FormatNumber(GetFactValueM(dsRptAct, RowId, ColId, RowValue, ColValue, UnitId, UnitValue, dsUnitPref).ToString(), 0)
                            If rdbMil.Checked = True Or rdbBil.Checked = True Then
                                Td.Text = FormatNumber((Td.Text / dValue), 3)
                            Else
                                Td.Text = FormatNumber((Td.Text / dValue), 0)
                            End If
                        Else
                            Td.Text = ""
                        End If

                    Else
                        Td.Text = "0.00"
                        Dim BeginYearId As String = String.Empty
                        Dim EndYearId As String = String.Empty

                        Dr = dsRptCols.Tables(0).Select("USERREPORTCOLUMNID=" + dsRptCols.Tables(0).Rows(k).Item("INPUTVALUETYPE1").ToString() + "")

                        BeginYearId = Dr(0).Item("COLUMNVALUEID").ToString() 'dsRptCols.Tables(0).Rows(k).Item("INPUTVALUETYPE1").ToString()
                        Dr = dsRptCols.Tables(0).Select("USERREPORTCOLUMNID=" + dsRptCols.Tables(0).Rows(k).Item("INPUTVALUETYPE2").ToString() + "")
                        EndYearId = Dr(0).Item("COLUMNVALUEID").ToString() 'dsRptCols.Tables(0).Rows(k).Item("INPUTVALUETYPE2").ToString()
                        'If RowType = "Category" Then
                        '    RowValue = "9999" + RowValue
                        'End If
                        Td.Text = FormatNumber(GetCAGR(dsRptAct, RowId, ColId, RowValue, UnitId, UnitValue, BeginYearId, EndYearId), 4)
                        Td.Text = FormatNumber(Td.Text, 4)
                    End If
                    InnerTdSetting(Td, "", "Right")
                    Td.CssClass = "AlterNateColor2"
                End If
                Td.Font.Size = 8.5
                Td.Font.Name = "Verdana"
                Td.Font.Bold = False
                Tr.Controls.Add(Td)
            Next
            tblCAGR.Controls.Add(Tr)
        Next
        tblCAGR.Visible = True
    End Sub

    Private Sub SetMixedReport_New(ByVal reportDes As String, ByVal rptType As String)
        Dim objGetData As New Selectdata()
        Dim dsRptRws As New DataSet()
        Dim dsRptCols As New DataSet()
        Dim dsRptFilter As New DataSet()
        Dim dsRptAct As New DataSet()
        Dim dsUnitPref As New DataSet
        Dim ColCnt As New Integer
        Dim RowCnt As New Integer
        Dim FiltCnt As New Integer
        Dim Dr() As DataRow
        Dim i As New Integer
        Dim j As New Integer
        Dim l As New Integer
        Dim HeaderTr As New TableRow()
        Dim HeaderTd As New TableCell()
        Dim Tr As New TableRow()
        Dim Td As New TableCell()
        Dim lbl As New Label
        Dim Link As New HyperLink
        Dim hyd As New HiddenField
        Dim groupId As String = String.Empty
        Dim dValue As Integer = 1

        If rdbActual.Checked = True Then
            dValue = 1
        ElseIf rdbMil.Checked = True Then
            dValue = 1000000
        ElseIf rdbBil.Checked = True Then
            dValue = 1000000000
        ElseIf rdbThou.Checked = True Then
            dValue = 1000
        End If

        If hidReport.Value <> "0" Then
            dsUnitPref = Session("dsPref" + REPId.ToString())
            dsRptRws = Session("dsRows" + REPId.ToString())
            dsRptCols = Session("dsColumns" + REPId.ToString())
            dsRptFilter = Session("dsFilters" + REPId.ToString())
        Else
            dsUnitPref = objGetData.GetPref(REPId.ToString())
            dsRptRws = objGetData.GetUsersDynamicReportRows(REPId.ToString())
            dsRptCols = objGetData.GetUsersDynamicReportCols(REPId.ToString())
            dsRptFilter = objGetData.GetUsersReportFilters(REPId.ToString())

            Session("dsPref" + REPId.ToString()) = dsUnitPref
            Session("dsRows" + REPId.ToString()) = dsRptRws
            Session("dsColumns" + REPId.ToString()) = dsRptCols
            Session("dsFilters" + REPId.ToString()) = dsRptFilter

            hidReport.Value = "1"
        End If

        groupId = Session("M1SubGroupId").ToString()

        If dsRptRws.Tables.Count > 0 Then
            RowCnt = dsRptRws.Tables(0).Rows.Count - 1
        End If

        If dsRptCols.Tables.Count > 0 Then
            ColCnt = dsRptCols.Tables(0).Rows.Count
        End If

        If dsRptFilter.Tables.Count > 0 Then
            FiltCnt = dsRptFilter.Tables(0).Rows.Count
        End If

        Dim dsTables As New DataSet
        Dim dvTables As New DataView
        Dim dtTables As New DataTable
        'Dim ProdTbl(11) As String
        Dim arrRfilt(FiltCnt) As String
        Dim arrRrowId(RowCnt) As String
        Dim arrRtype(RowCnt) As String
        Dim arrRrow(RowCnt) As String
        Dim arrRunit(RowCnt) As String
        Dim whereCon As String = ""
        Dim pgCon As String = ""
        Dim ProdId As String = ""
        Dim filtProdId As String = ""
        Dim PackId As String = ""
        Dim MatId As String = ""
        Dim GrpId As String = ""
        Dim RegId As String = ""
        Dim CntryId As String = ""
        Dim RowCount As New Integer
        Dim IsMat As Boolean = False

        dsTables = objGetData.GetProductDetails(Session("M1SubGroupId"))
        dvTables = dsTables.Tables(0).DefaultView
        Dim ProdTbl(dsTables.Tables(0).Rows.Count - 1) As String

        For a = 0 To dsRptFilter.Tables(0).Rows.Count - 1
            If dsRptFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PRODUCT" Then
                arrRfilt(a) = "PRODUCT"
                If dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    For b = 0 To dsTables.Tables(0).Rows.Count - 1
                        ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                        ProdId = ProdId + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                        RowCount += 1
                    Next
                    dtTables = dsTables.Tables(0)
                    ProdId = ProdId.Remove(ProdId.Length - 1)
                    filtProdId = "0"
                Else
                    ProdId = dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    filtProdId = ProdId
                    dvTables.RowFilter = "ACTUALPRODUCTID=" + ProdId
                    dtTables = dvTables.ToTable()
                    ProdTbl(0) = dtTables.Rows(0).Item("TABLENAME").ToString()
                    RowCount += 1
                End If
            ElseIf dsRptFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PACKAGE" Then
                arrRfilt(a) = "PACKAGETYPEID"
                If dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    For b = 0 To dsTables.Tables(0).Rows.Count - 1
                        ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                        ProdId = ProdId + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                    Next
                    dtTables = dsTables.Tables(0)
                    ProdId = ProdId.Remove(ProdId.Length - 1)
                Else
                    PackId = dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                End If
            ElseIf dsRptFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "MATERIAL" Then
                arrRfilt(a) = "MATERIALID"
                MatId = dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                IsMat = True
            ElseIf dsRptFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "COUNTRY" Then
                arrRfilt(a) = "COUNTRY"
                CntryId = dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
            ElseIf dsRptFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "REGION" Then
                arrRfilt(a) = "REGION"
                RegId = dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
            ElseIf dsRptFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "GROUP" Then
                arrRfilt(a) = "GROUPID"
                GrpId = dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
            End If
        Next

        If ProdId <> "" Then
            If whereCon <> "" Then
                whereCon = whereCon + "AND PRODUCTID IN (" + filtProdId + ") "
            Else
                whereCon = whereCon + "PRODUCTID IN (" + filtProdId + ") "
            End If
        End If
        If PackId <> "" Then
            If whereCon <> "" Then
                whereCon = whereCon + "AND PACKAGETYPEID IN (" + PackId + ") "
            Else
                whereCon = whereCon + "PACKAGETYPEID IN (" + PackId + ") "
            End If
        End If
        If MatId <> "" Then
            If whereCon <> "" Then
                whereCon = whereCon + "AND MATERIALID IN (" + MatId + ") "
            Else
                whereCon = whereCon + "MATERIALID IN (" + MatId + ") "
            End If
        End If
        If CntryId <> "" Then
            If whereCon <> "" Then
                whereCon = whereCon + "AND COUNTRYID IN (" + CntryId + ") "
            Else
                whereCon = whereCon + "COUNTRYID IN (" + CntryId + ") "
            End If
            If pgCon <> "" Then
                pgCon = pgCon + "AND COUNTRYID IN (" + CntryId + ") "
            Else
                pgCon = pgCon + "COUNTRYID IN (" + CntryId + ") "
            End If
        End If
        If RegId <> "" Then
            If whereCon <> "" Then
                whereCon = whereCon + "AND COUNTRYID IN (SELECT COUNTRYID FROM USERREGIONCOUNTRIES WHERE REGIONID IN (" + RegId + ")) "
            Else
                whereCon = whereCon + "COUNTRYID IN (SELECT COUNTRYID FROM USERREGIONCOUNTRIES WHERE REGIONID IN (" + RegId + ")) "
            End If
            If pgCon <> "" Then
                pgCon = pgCon + "AND COUNTRYID IN (SELECT COUNTRYID FROM USERREGIONCOUNTRIES WHERE REGIONID IN (" + RegId + ")) "
            Else
                pgCon = pgCon + "COUNTRYID IN (SELECT COUNTRYID FROM USERREGIONCOUNTRIES WHERE REGIONID IN (" + RegId + ")) "
            End If
        End If
        If GrpId <> "" Then
            If whereCon <> "" Then
                whereCon = whereCon + "AND SUBGROUPID IN (" + ProdId + ") "
            Else
                whereCon = whereCon + "SUBGROUPID IN (" + ProdId + ") "
            End If
        Else
            If whereCon <> "" Then
                whereCon = whereCon + "AND SUBGROUPID IS NULL "
            Else
                whereCon = whereCon + "SUBGROUPID IS NULL "
            End If
        End If

        If ProdId = "" Then
            For b = 0 To dsTables.Tables(0).Rows.Count - 1
                ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                ProdId = ProdId + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                RowCount += 1
            Next
            dtTables = dsTables.Tables(0)
            ProdId = ProdId.Remove(ProdId.Length - 1)
        End If

        Dim YearId As String = ""
        For a = 0 To dsRptCols.Tables(0).Rows.Count - 1
            If dsRptCols.Tables(0).Rows(a).Item("COLUMNVALUETYPE").ToString() <> "Formula" Then
                YearId = YearId + "" + dsRptCols.Tables(0).Rows(a).Item("COLUMNVALUEID").ToString() + ","
            End If
        Next
        YearId = YearId.Remove(YearId.Length - 1)

        Dim dsData As New DataSet
        Dim dvData As New DataView
        Dim dtData As New DataTable
        For a = 0 To dsRptRws.Tables(0).Rows.Count - 1
            Dim dts As New DataSet()
            arrRrowId(a) = dsRptRws.Tables(0).Rows(a).Item("USERREPORTROWID").ToString()
            arrRrow(a) = dsRptRws.Tables(0).Rows(a).Item("ROWVALUEID").ToString()
            arrRunit(a) = dsRptRws.Tables(0).Rows(a).Item("UNITID").ToString()
            If dsRptRws.Tables(0).Rows(a).Item("ROWVALUETYPE").ToString().ToUpper() = "PRODUCT" Then
                arrRtype(a) = "PRODUCTID"
                Dim ProdTbl1(12) As String
                Dim RowCount1 As Integer = 0
                If dsRptRws.Tables(0).Rows(a).Item("ROWVALUEID").ToString() = "0" Then
                    For b = 0 To dsTables.Tables(0).Rows.Count - 1
                        ProdTbl1(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                        RowCount1 += 1
                    Next
                Else
                    For b = 0 To dsTables.Tables(0).Rows.Count - 1
                        If dsRptRws.Tables(0).Rows(a).Item("ROWVALUEID").ToString() = dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() Then
                            ProdTbl1(0) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                            RowCount1 += 1
                        End If
                    Next
                End If
                dts = objGetData.GetMixedReportData(ProdTbl1, YearId, dsRptRws.Tables(0).Rows(a).Item("UNITID").ToString().ToUpper(), dsRptRws.Tables(0).Rows(a).Item("ROWVALUETYPE").ToString().ToUpper(), dsRptRws.Tables(0).Rows(a).Item("ROWVALUEID").ToString().ToUpper(), whereCon, RowCount1, filtProdId, IsMat)
                dts.Tables(0).TableName = a.ToString()
                dsData.Tables.Add(dts.Tables(a.ToString()).Copy())
            ElseIf dsRptRws.Tables(0).Rows(a).Item("ROWVALUETYPE").ToString().ToUpper() = "METRIC" Then
                arrRtype(a) = "PRODUCTID"
                If dsRptRws.Tables(0).Rows(a).Item("ROWVALUEID").ToString() = "2" Then
                    dts = objGetData.GetMixedReportData_Popl(YearId, dsRptRws.Tables(0).Rows(a).Item("UNITID").ToString().ToUpper(), dsRptRws.Tables(0).Rows(a).Item("ROWVALUETYPE").ToString().ToUpper(), dsRptRws.Tables(0).Rows(a).Item("ROWVALUEID").ToString().ToUpper(), pgCon)
                Else
                    dts = objGetData.GetMixedReportData_Gdp(YearId, dsRptRws.Tables(0).Rows(a).Item("UNITID").ToString().ToUpper(), dsRptRws.Tables(0).Rows(a).Item("ROWVALUETYPE").ToString().ToUpper(), dsRptRws.Tables(0).Rows(a).Item("ROWVALUEID").ToString().ToUpper(), pgCon)
                End If
                dts.Tables(0).TableName = a.ToString()
                dsData.Tables.Add(dts.Tables(a.ToString()).Copy())
            ElseIf dsRptRws.Tables(0).Rows(a).Item("ROWVALUETYPE").ToString().ToUpper() = "FORMULA" Then
                Dim dt1 As New DataTable
                dt1.Columns.Add("FCT")
                dt1.Columns.Add("YEARID")
                dt1.Columns.Add("UNITID")
                dt1.Columns.Add("PRODUCTID")
                dt1.Columns.Add("PACKAGETYPEID")
                dt1.Columns.Add("MATERIALID")
                dt1.Columns.Add("REGIONID")
                dt1.Columns.Add("COUNTRYID")
                dt1.Columns.Add("SUBGROUPID")
                dts.Tables.Add(dt1)
                dts.Tables(0).TableName = a.ToString()
                dsData.Tables.Add(dts.Tables(a.ToString()).Copy())
            Else
                If dsRptRws.Tables(0).Rows(a).Item("ROWVALUETYPE").ToString().ToUpper() = "PACKAGE" Then
                    arrRtype(a) = "PACKAGETYPEID"
                ElseIf dsRptRws.Tables(0).Rows(a).Item("ROWVALUETYPE").ToString().ToUpper() = "MATERIAL" Then
                    arrRtype(a) = "MATERIALID"
                ElseIf dsRptRws.Tables(0).Rows(a).Item("ROWVALUETYPE").ToString().ToUpper() = "COUNTRY" Then
                    arrRtype(a) = "COUNTRYID"
                ElseIf dsRptRws.Tables(0).Rows(a).Item("ROWVALUETYPE").ToString().ToUpper() = "REGION" Then
                    arrRtype(a) = "REGIONID"
                ElseIf dsRptRws.Tables(0).Rows(a).Item("ROWVALUETYPE").ToString().ToUpper() = "GROUP" Then
                    arrRtype(a) = "GROUPID"
                End If
                dts = objGetData.GetMixedReportData(ProdTbl, YearId, dsRptRws.Tables(0).Rows(a).Item("UNITID").ToString().ToUpper(), dsRptRws.Tables(0).Rows(a).Item("ROWVALUETYPE").ToString().ToUpper(), dsRptRws.Tables(0).Rows(a).Item("ROWVALUEID").ToString().ToUpper(), whereCon, RowCount, filtProdId, IsMat)
                dts.Tables(0).TableName = a.ToString()
                dsData.Tables.Add(dts.Tables(a.ToString()).Copy())
            End If
        Next

        dvData = dsData.Tables(0).DefaultView
        tblCAGR.Rows.Clear()
        'filter row
        HeaderTr = New TableRow
        For j = 0 To ColCnt
            HeaderTd = New TableCell()
            If j = 0 Then
                For l = 0 To FiltCnt - 1
                    Link = New HyperLink
                    hyd = New HiddenField
                    lbl = New Label
                    Link.Text = "Filter " + (l + 1).ToString() + ":" + dsRptFilter.Tables(0).Rows(l).Item("FILTERVALUE") + "<br/> "
                    lbl.Text = "Filter " + (l + 1).ToString() + ":" + dsRptFilter.Tables(0).Rows(l).Item("FILTERVALUE") + "<br/> "
                    hyd.Value = dsRptFilter.Tables(0).Rows(l).Item("USERREPORTFILTERID").ToString()
                    Link.ID = "Column_Fil_" + l.ToString()
                    hyd.ID = "Column_ID_Fil_" + l.ToString()
                    Link.CssClass = "LinkM"

                    GetFilterLink(dsRptFilter.Tables(0).Rows(l).Item("FILTERSEQUENCE").ToString(), Link, dsRptFilter.Tables(0).Rows(l).Item("USERREPORTID").ToString(), hyd.ClientID, hyd.Value)
                    If rptType = "Base" Then
                        HeaderTd.Controls.Add(lbl)
                    Else
                        HeaderTd.Controls.Add(Link)
                    End If
                    HeaderTd.Controls.Add(hyd)
                Next
                HeaderTdSetting(HeaderTd, "150px", "", "1")
            Else
                HeaderTdSetting(HeaderTd, "100px", "", "1")
            End If
            HeaderTd.Font.Size = 8.5
            HeaderTd.Font.Name = "Verdana"
            HeaderTd.Font.Bold = False
            HeaderTr.Controls.Add(HeaderTd)
        Next
        tblCAGR.Controls.Add(HeaderTr)

        'Hedaer Row
        HeaderTr = New TableRow
        For j = 0 To ColCnt
            HeaderTd = New TableCell()
            If j = 0 Then
                HeaderTdSetting(HeaderTd, "150px", "", "1")
            Else
                hyd = New HiddenField
                Link = New HyperLink
                lbl = New Label
                Dim k As New Integer
                k = j - 1
                If dsRptCols.Tables(0).Rows(k).Item("COLUMNVALUETYPE").ToString() = "Formula" Then
                    Link.Text = dsRptCols.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + "(" + dsRptCols.Tables(0).Rows(k).Item("INPUTVALUE1").ToString() + "/" + dsRptCols.Tables(0).Rows(k).Item("INPUTVALUE2").ToString() + ")"
                Else
                    Link.Text = dsRptCols.Tables(0).Rows(k).Item("COLUMNVALUE").ToString()
                End If
                lbl.Text = Link.Text
                Link.ID = "Column_" + k.ToString()
                hyd.ID = "Column_ID_" + k.ToString()
                hyd.Value = dsRptCols.Tables(0).Rows(k).Item("USERREPORTCOLUMNID").ToString()
                Link.CssClass = "LinkM"
                GetColLink(dsRptCols.Tables(0).Rows(k).Item("COLUMNSEQUENCE").ToString(), Link, dsRptCols.Tables(0).Rows(k).Item("USERREPORTID").ToString(), hyd.ClientID, hyd.Value)
                If rptType = "Base" Then
                    HeaderTd.Controls.Add(lbl)
                Else
                    HeaderTd.Controls.Add(Link)
                End If
                HeaderTd.Controls.Add(hyd)
                HeaderTdSetting(HeaderTd, "100px", "", "1")

            End If
            HeaderTd.Font.Size = 8.5
            HeaderTd.Font.Name = "Verdana"
            HeaderTd.Font.Bold = False
            HeaderTr.Controls.Add(HeaderTd)
        Next
        tblCAGR.Controls.Add(HeaderTr)


        'Inner Row
        For i = 0 To RowCnt
            Tr = New TableRow()
            For j = 0 To ColCnt
                Td = New TableCell()
                If j = 0 Then
                    HeaderTdSetting(Td, "150px", "", "1")
                    hyd = New HiddenField
                    Link = New HyperLink
                    lbl = New Label
                    Link.ID = "Row_" + i.ToString()
                    hyd.ID = "Row_ID_" + i.ToString()
                    hyd.Value = dsRptRws.Tables(0).Rows(i).Item("USERREPORTROWID").ToString()
                    Link.CssClass = "LinkM"
                    If dsRptRws.Tables(0).Rows(i).Item("ROWVALUETYPE").ToString().ToUpper() = "FORMULA" Then
                        If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                            Link.Text = dsRptRws.Tables(0).Rows(i).Item("ROWVAL1").ToString() + " (" + dsRptRws.Tables(0).Rows(i).Item("UNITSHRT1").ToString() + ")/" + dsRptRws.Tables(0).Rows(i).Item("ROWVAL2").ToString() + " (" + dsRptRws.Tables(0).Rows(i).Item("UNITSHRT2").ToString() + ")"
                        Else
                            Link.Text = dsRptRws.Tables(0).Rows(i).Item("ROWVAL1").ToString() + " (" + dsRptRws.Tables(0).Rows(i).Item("METRICUNIT1").ToString() + ")/" + dsRptRws.Tables(0).Rows(i).Item("ROWVAL2").ToString() + " (" + dsRptRws.Tables(0).Rows(i).Item("METRICUNIT2").ToString() + ")"
                        End If
                    Else
                        If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                            Link.Text = dsRptRws.Tables(0).Rows(i).Item("ROWVALUE").ToString() + " (" + dsRptRws.Tables(0).Rows(i).Item("UNITSHRT").ToString() + ")"
                        Else
                            Link.Text = dsRptRws.Tables(0).Rows(i).Item("ROWVALUE").ToString() + " (" + dsRptRws.Tables(0).Rows(i).Item("METRICUNIT").ToString() + ")"
                        End If
                    End If
                    lbl.Text = Link.Text
                    GetRowLink(dsRptRws.Tables(0).Rows(i).Item("ROWSEQUENCE").ToString(), Link, dsRptRws.Tables(0).Rows(i).Item("USERREPORTID").ToString(), hyd.ClientID, hyd.Value)
                    Td.Style.Add("text-align", "Left")
                    Td.Style.Add("padding-left", "5px")
                    If rptType = "Base" Then
                        Td.Controls.Add(lbl)
                    Else
                        Td.Controls.Add(Link)
                    End If
                    Td.Controls.Add(hyd)

                Else
                    Dim k As New Integer
                    k = j - 1
                    Dim ColType As String = dsRptCols.Tables(0).Rows(k).Item("COLUMNVALUETYPE").ToString()
                    Dim RowType As String = dsRptRws.Tables(0).Rows(i).Item("ROWVALUETYPE").ToString()
                    Dim RowId As String = String.Empty
                    Dim ColId As String = String.Empty
                    Dim RowValue As String = String.Empty
                    Dim ColValue As String = String.Empty
                    Dim UnitId As String = String.Empty
                    Dim UnitValue As String = String.Empty
                    Dim dvData1 As New DataView
                    Dim dvData2 As New DataView
                    Dim dtData1 As New DataTable
                    Dim dtData2 As New DataTable
                    Dim Val1 As New Double
                    Dim Val2 As New Double
                    RowId = dsRptRws.Tables(0).Rows(i).Item("ROWCOLUMNID").ToString()
                    RowValue = dsRptRws.Tables(0).Rows(i).Item("ROWVALUEID").ToString()
                    ColId = dsRptCols.Tables(0).Rows(k).Item("COLCOLUMNID").ToString()
                    ColValue = dsRptCols.Tables(0).Rows(k).Item("COLUMNVALUEID").ToString()
                    UnitId = "UnitId"
                    UnitValue = dsRptRws.Tables(0).Rows(i).Item("UNITID").ToString()
                    If RowType = "Formula" Then
                        If ColType = "Year" Then
                            For b = 0 To RowCnt - 1
                                dvData1 = dsData.Tables(b).DefaultView
                                If dsRptRws.Tables(0).Rows(i).Item("ROWVALID1").ToString() = arrRrowId(b) Then
                                    dvData1.RowFilter = arrRtype(b) + "=" + arrRrow(b) + " AND UNITID=" + arrRunit(b) + " AND YEARID=" + dsRptCols.Tables(0).Rows(k).Item("COLUMNVALUEID").ToString()
                                    dtData1 = dvData1.ToTable()
                                    If dtData1.Rows.Count > 0 Then
                                        For a = 0 To dtData1.Rows.Count - 1
                                            If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                                                If arrRunit(b) = 1 Then
                                                    Val1 = Val1 + CDbl(dtData1.Rows(a).Item("FCT").ToString())
                                                ElseIf arrRunit(b) = 2 Then
                                                    Val1 = Val1 + CDbl(dtData1.Rows(a).Item("FCT").ToString())
                                                ElseIf arrRunit(b) = 4 Then
                                                    Val1 = Val1 + CDbl(dtData1.Rows(a).Item("FCT").ToString())
                                                Else
                                                    Val1 = Val1 + CDbl(dtData1.Rows(a).Item("FCT").ToString())
                                                End If
                                            Else
                                                If arrRunit(b) = 1 Then
                                                    Val1 = Val1 + CDbl(dtData1.Rows(a).Item("FCT").ToString() * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT"))
                                                ElseIf arrRunit(b) = 2 Then
                                                    Val1 = Val1 + CDbl(dtData1.Rows(a).Item("FCT").ToString() * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL"))
                                                ElseIf arrRunit(b) = 3 Then
                                                    Val1 = Val1 + CDbl(dtData1.Rows(a).Item("FCT").ToString() * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA"))
                                                ElseIf arrRunit(b) = 4 Then
                                                    Val1 = Val1 + CDbl(dtData1.Rows(a).Item("FCT").ToString())
                                                Else
                                                    Val1 = Val1 + CDbl(dtData1.Rows(a).Item("FCT").ToString())
                                                End If
                                            End If
                                        Next
                                    End If
                                End If
                                dvData2 = dsData.Tables(b).DefaultView
                                If dsRptRws.Tables(0).Rows(i).Item("ROWVALID2").ToString() = arrRrowId(b) Then
                                    dvData2.RowFilter = arrRtype(b) + "=" + arrRrow(b) + " AND UNITID=" + arrRunit(b) + " AND YEARID=" + dsRptCols.Tables(0).Rows(k).Item("COLUMNVALUEID").ToString()
                                    dtData2 = dvData2.ToTable()
                                    If dtData2.Rows.Count > 0 Then
                                        For a = 0 To dtData2.Rows.Count - 1
                                            If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                                                If arrRunit(b) = 1 Then
                                                    Val2 = Val2 + CDbl(dtData2.Rows(a).Item("FCT").ToString())
                                                ElseIf arrRunit(b) = 2 Then
                                                    Val2 = Val2 + CDbl(dtData2.Rows(a).Item("FCT").ToString())
                                                ElseIf arrRunit(b) = 4 Then
                                                    Val2 = Val2 + CDbl(dtData2.Rows(a).Item("FCT").ToString())
                                                Else
                                                    Val2 = Val2 + CDbl(dtData2.Rows(a).Item("FCT").ToString())
                                                End If
                                            Else
                                                If arrRunit(b) = 1 Then
                                                    Val2 = Val2 + CDbl(dtData2.Rows(a).Item("FCT").ToString() * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT"))
                                                ElseIf arrRunit(b) = 2 Then
                                                    Val2 = Val2 + CDbl(dtData2.Rows(a).Item("FCT").ToString() * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL"))
                                                ElseIf arrRunit(b) = 3 Then
                                                    Val2 = Val2 + CDbl(dtData2.Rows(a).Item("FCT").ToString() * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA"))
                                                ElseIf arrRunit(b) = 4 Then
                                                    Val2 = Val2 + CDbl(dtData2.Rows(a).Item("FCT").ToString())
                                                Else
                                                    Val2 = Val2 + CDbl(dtData2.Rows(a).Item("FCT").ToString())
                                                End If
                                            End If
                                        Next
                                    End If
                                End If
                            Next
                            Dim CapVal As Double = Val1 / Val2
                            If rdbMil.Checked = True Or rdbBil.Checked = True Then
                                Td.Text = FormatNumber((CapVal / dValue), 3)
                            Else
                                Td.Text = FormatNumber((CapVal / dValue), 6)
                            End If
                        Else
                            Dim BeginYearId As String = String.Empty
                            Dim EndYearId As String = String.Empty
                            Dim dvN1 As New DataView
                            Dim dvN2 As New DataView
                            Dim dvD1 As New DataView
                            Dim dvD2 As New DataView
                            Dim dtN1 As New DataTable
                            Dim dtN2 As New DataTable
                            Dim dtD1 As New DataTable
                            Dim dtD2 As New DataTable
                            Dim ValN1 As New Double
                            Dim ValN2 As New Double
                            Dim ValD1 As New Double
                            Dim ValD2 As New Double
                            Dr = dsRptCols.Tables(0).Select("USERREPORTCOLUMNID=" + dsRptCols.Tables(0).Rows(k).Item("INPUTVALUETYPE1").ToString() + "")

                            BeginYearId = Dr(0).Item("COLUMNVALUEID").ToString()
                            Dr = dsRptCols.Tables(0).Select("USERREPORTCOLUMNID=" + dsRptCols.Tables(0).Rows(k).Item("INPUTVALUETYPE2").ToString() + "")
                            EndYearId = Dr(0).Item("COLUMNVALUEID").ToString()

                            For b = 0 To RowCnt - 1
                                dvN1 = dsData.Tables(b).DefaultView
                                dvN2 = dsData.Tables(b).DefaultView
                                If dsRptRws.Tables(0).Rows(i).Item("ROWVALID1").ToString() = arrRrowId(b) Then
                                    dvN1.RowFilter = arrRtype(b) + "=" + arrRrow(b) + " AND UNITID=" + arrRunit(b) + " AND YEARID=" + BeginYearId ' dsRptCols.Tables(0).Rows(k).Item("COLUMNVALUEID").ToString()
                                    dtN1 = dvN1.ToTable()
                                    If dtN1.Rows.Count > 0 Then
                                        For a = 0 To dtN1.Rows.Count - 1
                                            If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                                                If arrRunit(b) = 1 Then
                                                    ValN1 = ValN1 + CDbl(dtN1.Rows(a).Item("FCT").ToString())
                                                ElseIf arrRunit(b) = 2 Then
                                                    ValN1 = ValN1 + CDbl(dtN1.Rows(a).Item("FCT").ToString())
                                                ElseIf arrRunit(b) = 4 Then
                                                    ValN1 = ValN1 + CDbl(dtN1.Rows(a).Item("FCT").ToString())
                                                Else
                                                    ValN1 = ValN1 + CDbl(dtN1.Rows(a).Item("FCT").ToString())
                                                End If
                                            Else
                                                If arrRunit(b) = 1 Then
                                                    ValN1 = ValN1 + CDbl(dtN1.Rows(a).Item("FCT").ToString() * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT"))
                                                ElseIf arrRunit(b) = 2 Then
                                                    ValN1 = ValN1 + CDbl(dtN1.Rows(a).Item("FCT").ToString() * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL"))
                                                ElseIf arrRunit(b) = 3 Then
                                                    ValN1 = ValN1 + CDbl(dtN1.Rows(a).Item("FCT").ToString() * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA"))
                                                ElseIf arrRunit(b) = 4 Then
                                                    ValN1 = ValN1 + CDbl(dtN1.Rows(a).Item("FCT").ToString())
                                                Else
                                                    ValN1 = ValN1 + CDbl(dtN1.Rows(a).Item("FCT").ToString())
                                                End If
                                            End If
                                        Next
                                    End If

                                    dvN2.RowFilter = arrRtype(b) + "=" + arrRrow(b) + " AND UNITID=" + arrRunit(b) + " AND YEARID=" + EndYearId  ' dsRptCols.Tables(0).Rows(k).Item("COLUMNVALUEID").ToString()
                                    dtN2 = dvN2.ToTable()
                                    If dtN2.Rows.Count > 0 Then
                                        For a = 0 To dtN2.Rows.Count - 1
                                            If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                                                If arrRunit(b) = 1 Then
                                                    ValN2 = ValN2 + CDbl(dtN2.Rows(a).Item("FCT").ToString())
                                                ElseIf arrRunit(b) = 2 Then
                                                    ValN2 = ValN2 + CDbl(dtN2.Rows(a).Item("FCT").ToString())
                                                ElseIf arrRunit(b) = 4 Then
                                                    ValN2 = ValN2 + CDbl(dtN2.Rows(a).Item("FCT").ToString())
                                                Else
                                                    ValN2 = ValN2 + CDbl(dtN2.Rows(a).Item("FCT").ToString())
                                                End If
                                            Else
                                                If arrRunit(b) = 1 Then
                                                    ValN2 = ValN2 + CDbl(dtN2.Rows(a).Item("FCT").ToString() * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT"))
                                                ElseIf arrRunit(b) = 2 Then
                                                    ValN2 = ValN2 + CDbl(dtN2.Rows(a).Item("FCT").ToString() * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL"))
                                                ElseIf arrRunit(b) = 3 Then
                                                    ValN2 = ValN2 + CDbl(dtN2.Rows(a).Item("FCT").ToString() * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA"))
                                                ElseIf arrRunit(b) = 4 Then
                                                    ValN2 = ValN2 + CDbl(dtN2.Rows(a).Item("FCT").ToString())
                                                Else
                                                    ValN2 = ValN2 + CDbl(dtN2.Rows(a).Item("FCT").ToString())
                                                End If
                                            End If
                                        Next
                                    End If
                                End If

                                dvD1 = dsData.Tables(b).DefaultView
                                dvD2 = dsData.Tables(b).DefaultView
                                If dsRptRws.Tables(0).Rows(i).Item("ROWVALID2").ToString() = arrRrowId(b) Then
                                    dvD1.RowFilter = arrRtype(b) + "=" + arrRrow(b) + " AND UNITID=" + arrRunit(b) + " AND YEARID=" + BeginYearId ' dsRptCols.Tables(0).Rows(k).Item("COLUMNVALUEID").ToString()
                                    dtD1 = dvD1.ToTable()
                                    If dtD1.Rows.Count > 0 Then
                                        For a = 0 To dtD1.Rows.Count - 1
                                            If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                                                If arrRunit(b) = 1 Then
                                                    ValD1 = ValD1 + CDbl(dtD1.Rows(a).Item("FCT").ToString())
                                                ElseIf arrRunit(b) = 2 Then
                                                    ValD1 = Val2 + CDbl(dtD1.Rows(a).Item("FCT").ToString())
                                                ElseIf arrRunit(b) = 4 Then
                                                    ValD1 = ValD1 + CDbl(dtD1.Rows(a).Item("FCT").ToString())
                                                Else
                                                    ValD1 = ValD1 + CDbl(dtD1.Rows(a).Item("FCT").ToString())
                                                End If
                                            Else
                                                If arrRunit(b) = 1 Then
                                                    ValD1 = ValD1 + CDbl(dtD1.Rows(a).Item("FCT").ToString() * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT"))
                                                ElseIf arrRunit(b) = 2 Then
                                                    ValD1 = ValD1 + CDbl(dtD1.Rows(a).Item("FCT").ToString() * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL"))
                                                ElseIf arrRunit(b) = 3 Then
                                                    ValD1 = ValD1 + CDbl(dtD1.Rows(a).Item("FCT").ToString() * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA"))
                                                ElseIf arrRunit(b) = 4 Then
                                                    ValD1 = ValD1 + CDbl(dtD1.Rows(a).Item("FCT").ToString())
                                                Else
                                                    ValD1 = ValD1 + CDbl(dtD1.Rows(a).Item("FCT").ToString())
                                                End If
                                            End If
                                        Next
                                    End If

                                    dvD2.RowFilter = arrRtype(b) + "=" + arrRrow(b) + " AND UNITID=" + arrRunit(b) + " AND YEARID=" + EndYearId ' dsRptCols.Tables(0).Rows(k).Item("COLUMNVALUEID").ToString()
                                    dtD2 = dvD2.ToTable()
                                    If dtD2.Rows.Count > 0 Then
                                        For a = 0 To dtD1.Rows.Count - 1
                                            If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                                                If arrRunit(b) = 1 Then
                                                    ValD2 = ValD2 + CDbl(dtD2.Rows(a).Item("FCT").ToString())
                                                ElseIf arrRunit(b) = 2 Then
                                                    ValD2 = ValD2 + CDbl(dtD2.Rows(a).Item("FCT").ToString())
                                                ElseIf arrRunit(b) = 4 Then
                                                    ValD2 = ValD2 + CDbl(dtD2.Rows(a).Item("FCT").ToString())
                                                Else
                                                    ValD2 = ValD2 + CDbl(dtD2.Rows(a).Item("FCT").ToString())
                                                End If
                                            Else
                                                If arrRunit(b) = 1 Then
                                                    ValD2 = ValD2 + CDbl(dtD2.Rows(a).Item("FCT").ToString() * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT"))
                                                ElseIf arrRunit(b) = 2 Then
                                                    ValD2 = ValD2 + CDbl(dtD2.Rows(a).Item("FCT").ToString() * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL"))
                                                ElseIf arrRunit(b) = 3 Then
                                                    ValD2 = ValD2 + CDbl(dtD2.Rows(a).Item("FCT").ToString() * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA"))
                                                ElseIf arrRunit(b) = 4 Then
                                                    ValD2 = ValD2 + CDbl(dtD2.Rows(a).Item("FCT").ToString())
                                                Else
                                                    ValD2 = ValD2 + CDbl(dtD2.Rows(a).Item("FCT").ToString())
                                                End If
                                            End If
                                        Next
                                    End If
                                End If
                            Next
                            Dim diff As Integer = EndYearId - BeginYearId
                            Val1 = ValN1 / ValD1
                            Val2 = ValN2 / ValD2

                            If Val1 <> 0 And Val2 <> 0 Then
                                Td.Text = FormatNumber((((Val2 / Val1) ^ (1 / diff)) - 1) * 100, 4)
                            Else
                                Td.Text = FormatNumber(0, 4)
                            End If

                        End If
                    Else
                        dvData = dsData.Tables(i).DefaultView
                        If ColType = "Year" Then
                            dvData.RowFilter = arrRtype(i) + "=" + arrRrow(i) + " AND UNITID=" + arrRunit(i) + " AND YEARID=" + dsRptCols.Tables(0).Rows(k).Item("COLUMNVALUEID").ToString()
                            dtData = dvData.ToTable()
                            Dim prodCount As New Double
                            If dtData.Rows.Count > 0 Then
                                For k = 0 To dtData.Rows.Count - 1
                                    If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                                        If UnitValue = 1 Then
                                            prodCount = prodCount + CDbl(dtData.Rows(k).Item("FCT").ToString())
                                        ElseIf UnitValue = 2 Then
                                            prodCount = prodCount + CDbl(dtData.Rows(k).Item("FCT").ToString())
                                        ElseIf UnitValue = 4 Then
                                            prodCount = prodCount + CDbl(dtData.Rows(k).Item("FCT").ToString())
                                        Else
                                            prodCount = prodCount + CDbl(dtData.Rows(k).Item("FCT").ToString())
                                        End If
                                    Else
                                        If UnitValue = 1 Then
                                            prodCount = prodCount + CDbl(dtData.Rows(k).Item("FCT").ToString() * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT"))
                                        ElseIf UnitValue = 2 Then
                                            prodCount = prodCount + CDbl(dtData.Rows(k).Item("FCT").ToString() * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL"))
                                        ElseIf UnitValue = 3 Then
                                            prodCount = prodCount + CDbl(dtData.Rows(k).Item("FCT").ToString() * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA"))
                                        ElseIf UnitValue = 4 Then
                                            prodCount = prodCount + CDbl(dtData.Rows(k).Item("FCT").ToString())
                                        Else
                                            prodCount = prodCount + CDbl(dtData.Rows(k).Item("FCT").ToString())
                                        End If
                                    End If
                                Next
                                If rdbMil.Checked = True Or rdbBil.Checked = True Then
                                    Td.Text = FormatNumber((prodCount / dValue), 3)
                                Else
                                    Td.Text = FormatNumber((prodCount / dValue), 0)
                                End If
                                'Td.Text = FormatNumber(prodCount, 0)
                            End If
                        Else
                            Td.Text = "0.00"
                            Dim BeginYearId As String = String.Empty
                            Dim EndYearId As String = String.Empty
                            Dim dv1 As New DataView
                            Dim dv2 As New DataView
                            Dim dt1 As New DataTable
                            Dim dt2 As New DataTable
                            Dr = dsRptCols.Tables(0).Select("USERREPORTCOLUMNID=" + dsRptCols.Tables(0).Rows(k).Item("INPUTVALUETYPE1").ToString() + "")

                            BeginYearId = Dr(0).Item("COLUMNVALUEID").ToString()
                            Dr = dsRptCols.Tables(0).Select("USERREPORTCOLUMNID=" + dsRptCols.Tables(0).Rows(k).Item("INPUTVALUETYPE2").ToString() + "")
                            EndYearId = Dr(0).Item("COLUMNVALUEID").ToString()

                            dv1 = dsData.Tables(i).DefaultView
                            dv2 = dsData.Tables(i).DefaultView
                            dv1.RowFilter = arrRtype(i) + "=" + arrRrow(i) + " AND UNITID=" + arrRunit(i) + " AND YEARID=" + BeginYearId 'dsRptCols.Tables(0).Rows(k).Item("COLUMNVALUEID").ToString()
                            dt1 = dv1.ToTable()
                            Dim prodCount1 As New Double
                            If dt1.Rows.Count > 0 Then
                                For a = 0 To dt1.Rows.Count - 1
                                    If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                                        If UnitValue = 1 Then
                                            prodCount1 = prodCount1 + CDbl(dt1.Rows(a).Item("FCT").ToString())
                                        ElseIf UnitValue = 2 Then
                                            prodCount1 = prodCount1 + CDbl(dt1.Rows(a).Item("FCT").ToString())
                                        ElseIf UnitValue = 4 Then
                                            prodCount1 = prodCount1 + CDbl(dt1.Rows(a).Item("FCT").ToString())
                                        Else
                                            prodCount1 = prodCount1 + CDbl(dt1.Rows(a).Item("FCT").ToString())
                                        End If
                                    Else
                                        If UnitValue = 1 Then
                                            prodCount1 = prodCount1 + CDbl(dt1.Rows(a).Item("FCT").ToString() * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT"))
                                        ElseIf UnitValue = 2 Then
                                            prodCount1 = prodCount1 + CDbl(dt1.Rows(a).Item("FCT").ToString() * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL"))
                                        ElseIf UnitValue = 3 Then
                                            prodCount1 = prodCount1 + CDbl(dt1.Rows(a).Item("FCT").ToString() * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA"))
                                        ElseIf UnitValue = 4 Then
                                            prodCount1 = prodCount1 + CDbl(dt1.Rows(a).Item("FCT").ToString())
                                        Else
                                            prodCount1 = prodCount1 + CDbl(dt1.Rows(a).Item("FCT").ToString())
                                        End If
                                    End If
                                Next
                            End If
                            dv2.RowFilter = arrRtype(i) + "=" + arrRrow(i) + " AND UNITID=" + arrRunit(i) + " AND YEARID=" + EndYearId 'dsRptCols.Tables(0).Rows(k).Item("COLUMNVALUEID").ToString()
                            dt2 = dv2.ToTable()
                            Dim prodCount2 As New Double
                            If dt2.Rows.Count > 0 Then
                                For a = 0 To dt2.Rows.Count - 1
                                    If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                                        If UnitValue = 1 Then
                                            prodCount2 = prodCount2 + CDbl(dt2.Rows(a).Item("FCT").ToString())
                                        ElseIf UnitValue = 2 Then
                                            prodCount2 = prodCount2 + CDbl(dt2.Rows(a).Item("FCT").ToString())
                                        ElseIf UnitValue = 4 Then
                                            prodCount2 = prodCount2 + CDbl(dt2.Rows(a).Item("FCT").ToString())
                                        Else
                                            prodCount2 = prodCount2 + CDbl(dt2.Rows(a).Item("FCT").ToString())
                                        End If
                                    Else
                                        If UnitValue = 1 Then
                                            prodCount2 = prodCount2 + CDbl(dt2.Rows(a).Item("FCT").ToString() * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT"))
                                        ElseIf UnitValue = 2 Then
                                            prodCount2 = prodCount2 + CDbl(dt2.Rows(a).Item("FCT").ToString() * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL"))
                                        ElseIf UnitValue = 3 Then
                                            prodCount2 = prodCount2 + CDbl(dt2.Rows(a).Item("FCT").ToString() * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA"))
                                        ElseIf UnitValue = 4 Then
                                            prodCount2 = prodCount2 + CDbl(dt2.Rows(a).Item("FCT").ToString())
                                        Else
                                            prodCount2 = prodCount2 + CDbl(dt2.Rows(a).Item("FCT").ToString())
                                        End If
                                    End If
                                Next
                            End If
                            Dim diff As Integer = EndYearId - BeginYearId
                            If prodCount1 <> 0 And prodCount2 <> 0 Then
                                Td.Text = FormatNumber((((prodCount2 / prodCount1) ^ (1 / diff)) - 1) * 100, 4)
                            Else
                                Td.Text = FormatNumber(0, 4)
                            End If
                            'Td.Text = FormatNumber(GetCAGR(dsRptAct, RowId, ColId, RowValue, UnitId, UnitValue, BeginYearId, EndYearId), 4)
                            'Td.Text = FormatNumber(Td.Text, 4)
                        End If
                    End If

                    InnerTdSetting(Td, "", "Right")
                    Td.CssClass = "AlterNateColor2"
                End If
                Td.Font.Size = 8.5
                Td.Font.Name = "Verdana"
                Td.Font.Bold = False
                Tr.Controls.Add(Td)
            Next
            tblCAGR.Controls.Add(Tr)
        Next
        tblCAGR.Visible = True
    End Sub

    Protected Function GetFactValueM(ByVal ds As DataSet, ByVal RowId As String, ByVal ColId As String, ByVal RowValue As String, ByVal ColValue As String, ByVal UnitId As String, ByVal UnitValue As String, ByVal dsUnitPref As DataSet) As Double
        'Dim Fact As New Decimal
        Dim fact As Double
        Dim factTotal As Double = 0.0
        Dim factVal As Double = 0.0
        Dim dsMaterials As DataSet
        Dim objGetData As New Selectdata()
        Dim dr() As DataRow
        Try
            If ds.Tables(0).Columns.Item(0).ColumnName = "SUBGROUPID" Then
                RowId = "SUBGROUPID"
            End If
            If RowId <> "MatGroupId" And RowId <> "MaterialId" Then
                Try
                    dr = ds.Tables(0).Select("" + RowId + "=" + RowValue + " And " + ColId + "=" + ColValue + "And " + UnitId + "=" + UnitValue + " AND MATERIALID=-1")
                Catch ex As Exception
                    dr = ds.Tables(0).Select("" + RowId + "=" + RowValue + " And " + ColId + "=" + ColValue + "And " + UnitId + "=" + UnitValue + "")
                End Try
             If dr.Length > 0 Then
                    If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                        If UnitValue = 1 Then
                            fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                        ElseIf UnitValue = 2 Then
                            fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                        ElseIf UnitValue = 4 Then
                            fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                        Else
                            fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                        End If
                    Else
                        If UnitValue = 1 Then
                            fact = Convert.ToDecimal(dr(0).Item("FACTRES") * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT").ToString())
                        ElseIf UnitValue = 2 Then
                            fact = Convert.ToDecimal(dr(0).Item("FACTRES") * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL").ToString())
                        ElseIf UnitValue = 3 Then
                            fact = Convert.ToDecimal(dr(0).Item("FACTRES") * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA").ToString())
                        ElseIf UnitValue = 4 Then
                            fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                        Else
                            fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                        End If
                    End If

                Else
                    fact = ""
                End If
                factVal = fact
            ElseIf RowId = "MaterialId" Then
                dr = ds.Tables(0).Select("" + RowId + "=" + RowValue + " And " + ColId + "=" + ColValue + "And " + UnitId + "=" + UnitValue + "")
                If dr.Length > 0 Then
                    If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                        If UnitValue = 1 Then
                            fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                        ElseIf UnitValue = 2 Then
                            fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                        ElseIf UnitValue = 4 Then
                            fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                        Else
                            fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                        End If
                    Else
                        If UnitValue = 1 Then
                            fact = Convert.ToDecimal(dr(0).Item("FACTRES") * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT").ToString())
                        ElseIf UnitValue = 2 Then
                            fact = Convert.ToDecimal(dr(0).Item("FACTRES") * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL").ToString())
                        ElseIf UnitValue = 3 Then
                            fact = Convert.ToDecimal(dr(0).Item("FACTRES") * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA").ToString())
                        ElseIf UnitValue = 4 Then
                            fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                        Else
                            fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                        End If
                    End If

                Else
                    fact = ""
                End If
                factVal = fact
            Else
                dsMaterials = objGetData.GetMaterials(RowValue)
                For i = 0 To dsMaterials.Tables(0).Rows.Count - 1
                    dr = ds.Tables(0).Select("MATERIALID = " + dsMaterials.Tables(0).Rows(i).Item("MATERIALID").ToString() + " And " + ColId + " = " + ColValue + " And " + UnitId + " = " + UnitValue + "")
                    If dr.Length > 0 Then
                        If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                            If UnitValue = 1 Then
                                fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                            ElseIf UnitValue = 2 Then
                                fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                            ElseIf UnitValue = 4 Then
                                fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                            Else
                                fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                            End If
                        Else
                            If UnitValue = 1 Then
                                fact = Convert.ToDecimal(dr(0).Item("FACTRES") * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT").ToString())
                            ElseIf UnitValue = 2 Then
                                fact = Convert.ToDecimal(dr(0).Item("FACTRES") * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL").ToString())
                            ElseIf UnitValue = 3 Then
                                fact = Convert.ToDecimal(dr(0).Item("FACTRES") * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA").ToString())
                            ElseIf UnitValue = 4 Then
                                fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                            Else
                                fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                            End If
                        End If
                    Else
                        fact = "0"
                    End If
                    factTotal = fact + factTotal
                Next
                factVal = factTotal
            End If

            '* Convert.ToDecimal(dr(0).Item("CURR"))
        Catch ex As Exception

        End Try
        Return factVal

    End Function

    Protected Function GetFactValueTotal(ByVal ds As DataSet, ByVal colId As String, ByVal colValue As String) As Decimal
        ' Dim Fact As New Decimal
        Dim dr() As DataRow
        Dim i As Integer
        Dim total As Decimal
        Dim dsCol As DataSet
        Dim objGetData As New M1SubGetData.Selectdata
        Try
            dsCol = objGetData.GetCAGRReportColumn(colValue)
            colValue = dsCol.Tables(0).Rows(0).Item("COLUMNVALUEID")
            dr = ds.Tables(0).Select("" + colId + "=" + colValue + " ")
            total = 0
            For i = 0 To dr.Length - 1 'ds.Tables(0).Rows.Count - 1
                'If dr(i).Item("COUNTRYID").ToString() <> "" Then
                total = Convert.ToDecimal(dr(i).Item("FACTRES")) + total
                'End If

            Next
        Catch ex As Exception
            'ErrorLable.Text = "Error:GetFactValue:" + ex.Message.ToString() + ""
        End Try
        Return total
    End Function

    Protected Function GetFactValueTotalCountry_ORG(ByVal ds As DataSet, ByVal colId As String, ByVal colValue As String, ByVal dsRow As DataSet) As Decimal
        ' Dim Fact As New Decimal
        Dim dr() As DataRow
        Dim i As Integer
        Dim total As Decimal
        Dim RowId As String = String.Empty
        Dim RowValue As String = String.Empty
        Try
            total = 0
            For j = 0 To dsRow.Tables(0).Rows.Count - 1
                RowId = dsRow.Tables(0).Rows(j).Item("ROWCOLUMNID").ToString()
                RowValue = dsRow.Tables(0).Rows(j).Item("ROWVALUEID").ToString()
                dr = ds.Tables(0).Select("" + colId + "=" + colValue + " And " + RowId + "=" + RowValue + "")
                For i = 0 To dr.Length - 1 'ds.Tables(0).Rows.Count - 1
                    total = Convert.ToDecimal(dr(i).Item("FACTRES")) + total
                Next
            Next
        Catch ex As Exception
            'ErrorLable.Text = "Error:GetFactValue:" + ex.Message.ToString() + ""
        End Try
        Return total
    End Function
    Protected Function GetFactValueTotalCountry(ByVal ds As DataSet, ByVal colId As String, ByVal colValue As String, ByVal dsRow As DataSet) As Decimal
        ' Dim Fact As New Decimal
        Dim dr() As DataRow
        Dim i As Integer
        Dim total As Decimal
        Dim RowId As String = String.Empty
        Dim RowValue As String = String.Empty
        Dim dsMaterials As DataSet
        Dim objGetData As New Selectdata()
        Try
            total = 0
            For j = 0 To dsRow.Tables(0).Rows.Count - 1
                RowId = dsRow.Tables(0).Rows(j).Item("ROWCOLUMNID").ToString()
                RowValue = dsRow.Tables(0).Rows(j).Item("ROWVALUEID").ToString()
                If RowId <> "MatGroupId" Then
                    dr = ds.Tables(0).Select("" + colId + "=" + colValue + " And " + RowId + "=" + RowValue + "")
                    For i = 0 To dr.Length - 1 'ds.Tables(0).Rows.Count - 1
                        total = Convert.ToDecimal(dr(i).Item("FACTRES")) + total
                    Next
                Else
                    dsMaterials = objGetData.GetMaterials(RowValue)
                    For i = 0 To dsMaterials.Tables(0).Rows.Count - 1
                        dr = ds.Tables(0).Select("MATERIALID = " + dsMaterials.Tables(0).Rows(i).Item("MATERIALID").ToString() + " And " + colId + " = " + colValue + "")
                        If dr.Length > 0 Then
                            total = Convert.ToDecimal(dr(0).Item("FACTRES")) + total
                        End If
                    Next

                End If

            Next
        Catch ex As Exception
            'ErrorLable.Text = "Error:GetFactValue:" + ex.Message.ToString() + ""
        End Try
        Return total
    End Function

    Protected Function GetCAGRValueTotal(ByVal ds As DataSet, ByVal colId As String, ByVal BeginYear As String, ByVal EndYear As String, ByVal dsCols As DataSet) As Decimal
        Dim CAGR As New Decimal
        Dim BeginYearFct As New Decimal
        Dim EndYearFct As New Decimal
        Dim YearDiff As New Decimal
        Dim dr1() As DataRow
        Dim dr2() As DataRow
        Try
            BeginYearFct = GetFactValueTotal(ds, colId, BeginYear)
            EndYearFct = GetFactValueTotal(ds, colId, EndYear)
            dr1 = dsCols.Tables(0).Select("USERREPORTCOLUMNID=" + BeginYear + " ")
            BeginYear = Convert.ToDecimal(dr1(0).Item("COLUMNVALUEID"))
            dr2 = dsCols.Tables(0).Select("USERREPORTCOLUMNID=" + EndYear + " ")
            EndYear = Convert.ToDecimal(dr2(0).Item("COLUMNVALUEID"))
            YearDiff = EndYear - BeginYear
            CAGR = (((EndYearFct / BeginYearFct) ^ (1 / YearDiff)) - 1) * 100
            Return CAGR
        Catch ex As Exception

        End Try
    End Function

    Protected Function GetFactValue(ByVal ds As DataSet, ByVal RowId As String, ByVal ColId As String, ByVal RowValue As String, ByVal ColValue As String, ByVal UnitId As String, ByVal UnitValue As String, ByVal dsUnitPref As DataSet) As Double
        'Dim Fact As New Decimal
        Dim fact As Double
        Dim factTotal As Double = 0.0
        Dim factVal As Double = 0.0
        Dim dsMaterials As DataSet
        Dim objGetData As New Selectdata()
        Dim dr() As DataRow
        Try
            If ds.Tables(0).Columns.Item(0).ColumnName = "SUBGROUPID" Then
                RowId = "SUBGROUPID"
            End If
            If RowId <> "MatGroupId" Then
                dr = ds.Tables(0).Select("" + RowId + "=" + RowValue + " And " + ColId + "=" + ColValue + "And " + UnitId + "=" + UnitValue + "")
                If dr.Length > 0 Then
                    If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                        If UnitValue = 1 Then
                            fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                        ElseIf UnitValue = 2 Then
                            fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                        ElseIf UnitValue = 4 Then
                            fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                        Else
                            fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                        End If
                    Else
                        If UnitValue = 1 Then
                            fact = Convert.ToDecimal(dr(0).Item("FACTRES") * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT").ToString())
                        ElseIf UnitValue = 2 Then
                            fact = Convert.ToDecimal(dr(0).Item("FACTRES") * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL").ToString())
                        ElseIf UnitValue = 3 Then
                            fact = Convert.ToDecimal(dr(0).Item("FACTRES") * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA").ToString())
                        ElseIf UnitValue = 4 Then
                            fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                        Else
                            fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                        End If
                    End If

                Else
                    fact = ""
                End If
                factVal = fact
            Else
                dsMaterials = objGetData.GetMaterials(RowValue)
                For i = 0 To dsMaterials.Tables(0).Rows.Count - 1
                    dr = ds.Tables(0).Select("MATERIALID = " + dsMaterials.Tables(0).Rows(i).Item("MATERIALID").ToString() + " And " + ColId + " = " + ColValue + " And " + UnitId + " = " + UnitValue + "")
                    If dr.Length > 0 Then
                        If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                            If UnitValue = 1 Then
                                fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                            ElseIf UnitValue = 2 Then
                                fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                            ElseIf UnitValue = 4 Then
                                fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                            Else
                                fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                            End If
                        Else
                            If UnitValue = 1 Then
                                fact = Convert.ToDecimal(dr(0).Item("FACTRES") * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT").ToString())
                            ElseIf UnitValue = 2 Then
                                fact = Convert.ToDecimal(dr(0).Item("FACTRES") * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL").ToString())
                            ElseIf UnitValue = 3 Then
                                fact = Convert.ToDecimal(dr(0).Item("FACTRES") * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA").ToString())
                            ElseIf UnitValue = 4 Then
                                fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                            Else
                                fact = Convert.ToDecimal(dr(0).Item("FACTRES"))
                            End If
                        End If
                    Else
                        fact = "0"
                    End If
                    factTotal = fact + factTotal
                Next
                factVal = factTotal
            End If
           
            '* Convert.ToDecimal(dr(0).Item("CURR"))
        Catch ex As Exception

        End Try
        Return factVal

    End Function

    Protected Function GetCAGR_ORG(ByVal ds As DataSet, ByVal RowId As String, ByVal ColId As String, ByVal RowValue As String, ByVal UnitId As String, ByVal UnitValue As String, ByVal BeginYear As String, ByVal EndYear As String) As Decimal
        Dim CAGR As New Decimal
        Dim dsUnitPref As New DataSet
        Dim objGetData As New Selectdata()
        Dim BeginYearFct As New Decimal
        Dim EndYearFct As New Decimal
        Dim YearDiff As New Decimal
        Try
            dsUnitPref = objGetData.GetPref(REPId.ToString())
            If (GetFactValue(ds, RowId, ColId, RowValue, BeginYear, UnitId, UnitValue, dsUnitPref)) <> String.Empty Then
                BeginYearFct = GetFactValue(ds, RowId, ColId, RowValue, BeginYear, UnitId, UnitValue, dsUnitPref)
            End If
            If (GetFactValue(ds, RowId, ColId, RowValue, EndYear, UnitId, UnitValue, dsUnitPref)) <> String.Empty Then
                EndYearFct = GetFactValue(ds, RowId, ColId, RowValue, EndYear, UnitId, UnitValue, dsUnitPref)
            End If
            YearDiff = EndYear - BeginYear
            CAGR = (((EndYearFct / BeginYearFct) ^ (1 / YearDiff)) - 1) * 100
            Return CAGR
        Catch ex As Exception

        End Try
    End Function

    Protected Function GetCAGR(ByVal ds As DataSet, ByVal RowId As String, ByVal ColId As String, ByVal RowValue As String, ByVal UnitId As String, ByVal UnitValue As String, ByVal BeginYear As String, ByVal EndYear As String) As Decimal
        Dim CAGR As New Decimal
        Dim dsUnitPref As New DataSet
        Dim objGetData As New Selectdata()
        Dim BeginYearFct As New Decimal
        Dim EndYearFct As New Decimal
        Dim YearDiff As New Decimal
        Try
            dsUnitPref = objGetData.GetPref(REPId.ToString())
            If (GetFactValue(ds, RowId, ColId, RowValue, BeginYear, UnitId, UnitValue, dsUnitPref)).ToString() <> String.Empty Then
                BeginYearFct = (GetFactValue(ds, RowId, ColId, RowValue, BeginYear, UnitId, UnitValue, dsUnitPref)).ToString()
            End If
            If (GetFactValue(ds, RowId, ColId, RowValue, EndYear, UnitId, UnitValue, dsUnitPref)).ToString() <> String.Empty Then
                EndYearFct = (GetFactValue(ds, RowId, ColId, RowValue, EndYear, UnitId, UnitValue, dsUnitPref)).ToString()
            End If
            YearDiff = EndYear - BeginYear
            CAGR = (((EndYearFct / BeginYearFct) ^ (1 / YearDiff)) - 1) * 100
            Return CAGR
        Catch ex As Exception

        End Try
    End Function

    Protected Sub GetColLink(ByVal Seq As String, ByVal Link As HyperLink, ByVal RptId As String, ByVal hidId As String, ByVal hidValue As String)
        Dim Path As String
        Try
            Path = "../PopUp/ColSelector.aspx?RptId=" + RptId + "&Seq=" + Seq.ToString() + "&Id=ctl00_Market1ContentPlaceHolder_" + Link.ClientID + "&hidId=ctl00_Market1ContentPlaceHolder_" + hidId + "&isTemp=N"
            Link.NavigateUrl = "javascript:ShowPopWindow('" + Path + "','ctl00_Market1ContentPlaceHolder_" + hidId + "')"
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub GetRowLink(ByVal Seq As String, ByVal Link As HyperLink, ByVal RptId As String, ByVal hidId As String, ByVal hidValue As String)
        Dim Path As String

        Dim RowVal2D As String = String.Empty
        Dim RegionSetId As String = String.Empty
        Dim Curr As String = String.Empty
        Try

            Path = "../PopUp/RowSelector.aspx?RptId=" + RptId + "&Seq=" + Seq.ToString() + "&Id=ctl00_Market1ContentPlaceHolder_" + Link.ClientID + "&hidId=ctl00_Market1ContentPlaceHolder_" + hidId + "&isTemp=N"
            Link.NavigateUrl = "javascript:ShowPopWindow('" + Path + "','ctl00_Market1ContentPlaceHolder_" + hidId + "')"
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub GetFilterLink(ByVal Seq As String, ByVal Link As HyperLink, ByVal RptId As String, ByVal hidId As String, ByVal hidValue As String)
        Dim Path As String
        Try
            Path = "../PopUp/FilterSelectorRep.aspx?RptId=" + RptId + "&Seq=" + Seq.ToString() + "&Id=ctl00_Market1ContentPlaceHolder_" + Link.ClientID + "&hidId=ctl00_Market1ContentPlaceHolder_" + hidId + "&isTemp=N"
            Link.NavigateUrl = "javascript:ShowPopWindow('" + Path + "','ctl00_Market1ContentPlaceHolder_" + hidId + "')"
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub HeaderTdSetting(ByVal Td As TableCell, ByVal Width As String, ByVal HeaderText As String, ByVal ColSpan As String)
        Try
            'Td.Text = HeaderText
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

            Dim dsRp As New DataSet
            Dim objUpInsdata As New M1SubUpInsData.UpdateInsert()
            Dim objGetData As New Selectdata()
            dsRp = objGetData.GetReportDetails(REPId)
            If dsRp.Tables(0).Rows.Count > 0 Then
                objUpIns.InsertLog(Session("UserId"), Session("MLogInLog"), "3", REPId.ToString(), dsRp.Tables(0).Rows(0)("RPTTYPE").ToString(), dsRp.Tables(0).Rows(0).Item("RPTTYPEDES").ToString(), "13", REPId.ToString(), Session.SessionID)
            End If

            GetReportDetails()
        Catch ex As Exception
            _lErrorLble.Text = "Error:HeaderTdSetting:" + ex.Message.ToString()
        End Try
    End Sub
    Protected Sub rdbActual_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rdbActual.CheckedChanged
        GetPageDetails()
    End Sub

    Protected Sub rdbThou_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rdbThou.CheckedChanged
        GetPageDetails()
    End Sub

    Protected Sub rdbMil_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rdbMil.CheckedChanged
        GetPageDetails()
    End Sub

    Protected Sub rdbBil_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rdbBil.CheckedChanged
        GetPageDetails()
    End Sub

Public Function GetReportData(ByVal ReportId As String) As DataSet
        Dim Dts As New DataSet()
        Dim odbUtil As New DBUtil()
        Dim StrSql As String = String.Empty
        Dim Market1Connection As String = System.Configuration.ConfigurationManager.AppSettings("Market1ConnectionString")
        Try
            StrSql = "SELECT REGIONID, UNITID, YEARID, FACTRES "
            StrSql = StrSql + "FROM TEMPREPDATA "
            StrSql = StrSql + " WHERE REPORTID=" + ReportId

            Dts = odbUtil.FillDataSet(StrSql, Market1Connection)
            Return Dts
        Catch ex As Exception
            Throw New Exception("GetReportData:" + ex.Message.ToString())
            Return Dts
        End Try
    End Function

    Public Sub UpdateTempReportData(ByVal ReportId As String, ByVal ds As DataSet)
        Dim Dts As New DataSet()
        Dim odbUtil As New DBUtil()
        Dim StrSql As String = String.Empty
        Dim Market1Connection As String = System.Configuration.ConfigurationManager.AppSettings("Market1ConnectionString")
        Try
            For i = 0 To ds.Tables(0).Rows.Count - 1
                StrSql = "INSERT INTO TEMPREPDATA(REPORTID,REGIONID,UNITID,YEARID,FACTRES) VALUES "
                StrSql = StrSql + "(" + ReportId + "," + ds.Tables(0).Rows(i).Item("REGIONID").ToString() + "," + ds.Tables(0).Rows(i).Item("UNITID").ToString() + ", "
                StrSql = StrSql + " " + ds.Tables(0).Rows(i).Item("YEARID").ToString() + "," + ds.Tables(0).Rows(i).Item("FACTRES").ToString() + ")  "
                odbUtil.UpIns(StrSql, Market1Connection)
            Next


        Catch ex As Exception
            Throw New Exception("M1SubGetData:GetRowSQL:" + ex.Message.ToString())

        End Try
    End Sub

    Protected Sub UpdateReportRows(ByVal rptRepType As String, ByVal regionset As String, ByVal dsRPTDET As DataSet)
        Try
            Dim RptID As String = dsRPTDET.Tables(0).Rows(0).Item("REPORTID").ToString()
            Dim objUpIns As New UpdateInsert()
            Dim objGetData As New Selectdata()
            Dim RowCnt As String = String.Empty
            Dim FactId As String = String.Empty
            Dim dsRegions As New DataSet
            Dim dsPackTypeGroup As New DataSet
            Dim dsMaterials As New DataSet
            Dim dsPackType As New DataSet
            Dim dsProdGroup As New DataSet
            Dim dsProducts As New DataSet
            Dim dsFilters As New DataSet
            Dim dsRowSelector As New DataSet
            Dim dsRows As New DataSet
            Dim dsFact As New DataSet
            Dim arrFact() As String
            Dim listFilterType As New ArrayList
            Dim listFilterValue As New ArrayList
            Dim listFilterType1 As New ArrayList
            Dim listFilterValue1 As New ArrayList
            Dim RowVal1 As String = String.Empty
            Dim RowVal2 As String = String.Empty
            Dim packId As String = String.Empty
            Dim matId As String = String.Empty
            Dim i As Integer
            Dim RegionId As String = String.Empty
            Dim UnitID As String = ""

            'Change started for Edit
            If rptRepType = "PACKGRP" Then
                'RowCnt = objGetData.GetReportPackCount(ddlPackageGrp.SelectedValue.ToString())
                'dsPackTypeGroup = objGetData.GetReportPackagesByGroup(ddlPackageGrp.SelectedValue.ToString())
                'dsRowSelector = objGetData.GetRowsSelectorByCode("PACK")
                'RegionId = "null"
            ElseIf rptRepType = "PROD" Then
                dsFilters = objGetData.GetReportFiltersByRepId(RptID)
                For i = 0 To dsFilters.Tables(0).Rows.Count - 1
                    If dsFilters.Tables(0).Rows(i).Item("FILTERVALUEID").ToString() <> "" Then
                        listFilterType.Add(dsFilters.Tables(0).Rows(i).Item("FILTERTYPE").ToString())
                        listFilterValue.Add(dsFilters.Tables(0).Rows(i).Item("FILTERVALUEID").ToString())
                        If i = dsFilters.Tables(0).Rows.Count - 1 Then
                            dsProducts = objGetData.GetReportProductByPackMat(listFilterType, listFilterValue, Session("M1SubGroupId"))
                        End If
                    End If
                Next
                RegionId = "null"
                If dsProducts.Tables(0).Rows.Count <> 0 Then
                    RowCnt = dsProducts.Tables(0).Rows.Count
                    dsRowSelector = objGetData.GetRowsSelectorByCode("PROD")
                    If RptID <> "0" Then
                        UnitID = objUpIns.EditUSERReportRowDetail(RptID, RowCnt)
                    End If
                    dsRows = objGetData.GetUsersReportRowsRep(RptID)
                    For i = 0 To RowCnt - 1
                        objUpIns.UpdateRowDetailsRep(dsRowSelector.Tables(0).Rows(0)("ROWDES").ToString(), dsProducts.Tables(0).Rows(i)("NAME").ToString().Replace("'", "''"), dsRows.Tables(0).Rows(i)("USERREPORTROWID").ToString, dsRowSelector.Tables(0).Rows(0)("ROWDES").ToString(), "0", RowVal1, RowVal2, dsRowSelector.Tables(0).Rows(0)("ROWTYPEID").ToString(), dsProducts.Tables(0).Rows(i)("ID").ToString(), UnitID)
                    Next
                Else
                    RowCnt = "1"
                    dsRowSelector = objGetData.GetRowsSelectorByCode("PROD")
                    dsProducts = objGetData.GetReportDummy("PROD")
                End If

            ElseIf rptRepType = "PACK" Then
                dsFilters = objGetData.GetReportFiltersByRepId(RptID)
                For i = 0 To dsFilters.Tables(0).Rows.Count - 1
                    If dsFilters.Tables(0).Rows(i).Item("FILTERVALUEID").ToString() <> "" Then
                        If dsFilters.Tables(0).Rows(i).Item("FILTERVALUE").ToString() = "All Products" Then
                            arrFact = Regex.Split(Session("M1SubGroupId"), ",")
                            For j = 0 To arrFact.Length - 1
                                dsFact = objGetData.GetSubGroupDetails(arrFact(j))

                                If dsFact.Tables(0).Rows(0).Item("CATID").ToString() <> "" Then
                                    If j = 0 Then
                                        FactId = dsFact.Tables(0).Rows(0).Item("CATID").ToString() + ","
                                    Else
                                        FactId = FactId + dsFact.Tables(0).Rows(0).Item("CATID").ToString() + ","
                                    End If

                                End If
                            Next
                            FactId = FactId.Remove(FactId.Length - 1)

                            dsFact = objGetData.GetSubFactGroupDetails(FactId)
                            FactId = String.Empty
                            For k = 0 To dsFact.Tables(0).Rows.Count - 1
                                If dsFact.Tables(0).Rows(0).Item("ID").ToString() <> "" Then
                                    If k = 0 Then
                                        FactId = dsFact.Tables(0).Rows(k).Item("ID").ToString()
                                    Else
                                        FactId = FactId + "," + dsFact.Tables(0).Rows(k).Item("ID").ToString()
                                    End If
                                End If
                            Next

                        End If
                        listFilterType1.Add(dsFilters.Tables(0).Rows(i).Item("FILTERTYPE").ToString())
                        listFilterValue1.Add(dsFilters.Tables(0).Rows(i).Item("FILTERVALUEID").ToString())
                        If i = dsFilters.Tables(0).Rows.Count - 1 Then
                            dsPackType = objGetData.GetReportPackageByProdMat(listFilterType1, listFilterValue1, FactId)
                        End If
                    End If
                Next
                RegionId = "null"
                If dsPackType.Tables(0).Rows.Count <> 0 Then
                    RowCnt = dsPackType.Tables(0).Rows.Count
                    dsRowSelector = objGetData.GetRowsSelectorByCode("PACK")
                    If RptID <> "0" Then
                        UnitID = objUpIns.EditUSERReportRowDetail(RptID, RowCnt)
                    End If
                    dsRows = objGetData.GetUsersReportRowsRep(RptID)
                    For i = 0 To RowCnt - 1
                        objUpIns.UpdateRowDetailsRep(dsRowSelector.Tables(0).Rows(0)("ROWDES").ToString(), dsPackType.Tables(0).Rows(i)("NAME").ToString().Replace("'", "''"), dsRows.Tables(0).Rows(i)("USERREPORTROWID").ToString, dsRowSelector.Tables(0).Rows(0)("ROWDES").ToString(), "0", RowVal1, RowVal2, dsRowSelector.Tables(0).Rows(0)("ROWTYPEID").ToString(), dsPackType.Tables(0).Rows(i)("ID").ToString(), UnitID)
                    Next
                Else
                    RowCnt = "1"
                    dsRowSelector = objGetData.GetRowsSelectorByCode("PACK")
                    dsPackType = objGetData.GetReportDummy("PACK")
                End If


            ElseIf rptRepType = "MAT" Then
                dsFilters = objGetData.GetReportFiltersByRepId(RptID)
                For i = 0 To dsFilters.Tables(0).Rows.Count - 1
                    If dsFilters.Tables(0).Rows(i).Item("FILTERVALUEID").ToString() <> "" Then
                        If dsFilters.Tables(0).Rows(i).Item("FILTERVALUE").ToString() = "All Products" Then
                            arrFact = Regex.Split(Session("M1SubGroupId"), ",")
                            For j = 0 To arrFact.Length - 1
                                dsFact = objGetData.GetSubGroupDetails(arrFact(j))

                                If dsFact.Tables(0).Rows(0).Item("CATID").ToString() <> "" Then
                                    If j = 0 Then
                                        FactId = dsFact.Tables(0).Rows(0).Item("CATID").ToString() + ","
                                    Else
                                        FactId = FactId + dsFact.Tables(0).Rows(0).Item("CATID").ToString() + ","
                                    End If

                                End If
                            Next
                            FactId = FactId.Remove(FactId.Length - 1)

                            dsFact = objGetData.GetSubFactGroupDetails(FactId)
                            FactId = String.Empty
                            For k = 0 To dsFact.Tables(0).Rows.Count - 1
                                If dsFact.Tables(0).Rows(0).Item("ID").ToString() <> "" Then
                                    If k = 0 Then
                                        FactId = dsFact.Tables(0).Rows(k).Item("ID").ToString()
                                    Else
                                        FactId = FactId + "," + dsFact.Tables(0).Rows(k).Item("ID").ToString()
                                    End If
                                End If
                            Next
                            'FactId = FactId.Remove(FactId.Length - 1)
                        End If
                        listFilterType1.Add(dsFilters.Tables(0).Rows(i).Item("FILTERTYPE").ToString())
                        listFilterValue1.Add(dsFilters.Tables(0).Rows(i).Item("FILTERVALUEID").ToString())
                        If i = dsFilters.Tables(0).Rows.Count - 1 Then
                            dsMaterials = objGetData.GetReportMaterialByProdPack(listFilterType1, listFilterValue1, FactId)
                        End If
                    End If
                Next
                RegionId = "null"
                If dsMaterials.Tables(0).Rows.Count <> 0 Then
                    RowCnt = dsMaterials.Tables(0).Rows.Count
                    dsRowSelector = objGetData.GetRowsSelectorByCode("MAT")
                    If RptID <> "0" Then
                        UnitID = objUpIns.EditUSERReportRowDetail(RptID, RowCnt)
                    End If
                    dsRows = objGetData.GetUsersReportRowsRep(RptID)
                    For i = 0 To RowCnt - 1
                        objUpIns.UpdateRowDetailsRep(dsRowSelector.Tables(0).Rows(0)("ROWDES").ToString(), dsMaterials.Tables(0).Rows(i)("NAME").ToString().Replace("'", "''"), dsRows.Tables(0).Rows(i)("USERREPORTROWID").ToString, dsRowSelector.Tables(0).Rows(0)("ROWDES").ToString(), "0", RowVal1, RowVal2, dsRowSelector.Tables(0).Rows(0)("ROWTYPEID").ToString(), dsMaterials.Tables(0).Rows(i)("ID").ToString(), UnitID)
                    Next
                Else
                    RowCnt = "1"
                    dsRowSelector = objGetData.GetRowsSelectorByCode("MAT")
                    dsMaterials = objGetData.GetReportDummy("MAT")
                End If

           ElseIf rptRepType = "GROUP" Then
                dsFilters = objGetData.GetReportFiltersByRepId(RptID)
                For i = 0 To dsFilters.Tables(0).Rows.Count - 1
                    If dsFilters.Tables(0).Rows(i).Item("FILTERVALUEID").ToString() <> "" Then
                        arrFact = Regex.Split(Session("M1SubGroupId"), ",")
                        For j = 0 To arrFact.Length - 1
                            dsFact = objGetData.GetSubGroupDetails(arrFact(j))

                            If dsFact.Tables(0).Rows(0).Item("CATID").ToString() <> "" Then
                                If j = 0 Then
                                    FactId = dsFact.Tables(0).Rows(0).Item("CATID").ToString() + ","
                                Else
                                    FactId = FactId + dsFact.Tables(0).Rows(0).Item("CATID").ToString() + ","
                                End If

                            End If
                        Next
                        FactId = FactId.Remove(FactId.Length - 1)

                        dsFact = objGetData.GetSubFactGroupDetails(FactId)
                        FactId = String.Empty
                        If dsFact.Tables(0).Rows.Count > 0 Then
                            For k = 0 To dsFact.Tables(0).Rows.Count - 1
                                If dsFact.Tables(0).Rows(0).Item("ID").ToString() <> "" Then
                                    If k = 0 Then
                                        FactId = dsFact.Tables(0).Rows(k).Item("ID").ToString()
                                    Else
                                        FactId = FactId + "," + dsFact.Tables(0).Rows(k).Item("ID").ToString()
                                    End If
                                End If
                            Next
                        End If
                        
                        listFilterType.Add(dsFilters.Tables(0).Rows(i).Item("FILTERTYPE").ToString())
                        listFilterValue.Add(dsFilters.Tables(0).Rows(i).Item("FILTERVALUEID").ToString())
                        If i = dsFilters.Tables(0).Rows.Count - 1 Then
                            dsProdGroup = objGetData.GetReportGroupByProdCntry(listFilterType, listFilterValue, regionset, FactId)
                        End If
                    End If
                Next
                Dim Market1Connection As String = System.Configuration.ConfigurationManager.AppSettings("Market1ConnectionString")
                Dim odbUtil As New DBUtil()
                If dsProdGroup.Tables(0).Rows.Count <> 0 Then
                    RowCnt = dsProdGroup.Tables(0).Rows.Count
                    dsRowSelector = objGetData.GetRowsSelectorByCode("GRP")
                    If RptID <> "0" Then
                        UnitID = objUpIns.EditUSERReportRowDetailGRP(RptID, RowCnt)
                    End If
                    Dim StrSql As String = "INSERT ALL "
                    dsRows = objGetData.GetUsersReportRowsRep(RptID)
                    For i = 0 To RowCnt - 1
                        StrSql = StrSql + "INTO USERREPORTROWS(USERREPORTID,USERREPORTROWID,ROWDECRIPTION,ROWVALUE,ROWVALUETYPE,CURR,ROWVAL1,ROWVAL2,ROWTYPEID,ROWVALUEID,UNITID,ROWSEQUENCE)  "
                        StrSql = StrSql + "VALUES (" + REPId.ToString() + ", SEQUSERREPORTROWID.NEXTVAL,'" + dsRowSelector.Tables(0).Rows(0)("ROWDES").ToString() + "','" + dsProdGroup.Tables(0).Rows(i)("NAME").ToString().Replace("'", "''") + "','" + dsRowSelector.Tables(0).Rows(0)("ROWDES").ToString() + "',0,'" + RowVal1 + "','" + RowVal2 + "'," + dsRowSelector.Tables(0).Rows(0)("ROWTYPEID").ToString() + "," + dsProdGroup.Tables(0).Rows(i)("ID").ToString() + ","
                        StrSql = StrSql + "" + UnitID.ToString() + "," + (i + 1).ToString() + ") "
                    Next
                    StrSql = StrSql + "SELECT * FROM DUAL"
                    odbUtil.UpIns(StrSql, Market1Connection)
                Else
                    RowCnt = "1"
                    dsRowSelector = objGetData.GetRowsSelectorByCode("GRP")
                    dsProdGroup = objGetData.GetReportDummy("GROUP")
                    If RptID <> "0" Then
                        UnitID = objUpIns.EditUSERReportRowDetailGRP(RptID, RowCnt)
                    End If
                    dsRows = objGetData.GetUsersReportRowsRep(RptID)
                    Dim StrSql As String = "INSERT ALL "
                    Dim b As Integer = 0
                    For i = 0 To RowCnt - 1
                         StrSql = StrSql + "INTO USERREPORTROWS(USERREPORTID,USERREPORTROWID,ROWDECRIPTION,ROWVALUE,ROWVALUETYPE,CURR,ROWVAL1,ROWVAL2,ROWTYPEID,ROWVALUEID,UNITID,ROWSEQUENCE)  "
                        StrSql = StrSql + "VALUES (" + REPId.ToString() + ", SEQUSERREPORTROWID.NEXTVAL,'" + dsRowSelector.Tables(0).Rows(0)("ROWDES").ToString() + "','" + dsProdGroup.Tables(0).Rows(i)("NAME").ToString().Replace("'", "''") + "','" + dsRowSelector.Tables(0).Rows(0)("ROWDES").ToString() + "',0,'" + RowVal1 + "','" + RowVal2 + "'," + dsRowSelector.Tables(0).Rows(0)("ROWTYPEID").ToString() + "," + dsProdGroup.Tables(0).Rows(i)("ID").ToString() + ","
                        StrSql = StrSql + "" + UnitID.ToString() + "," + (i + 1).ToString() + ") "    
		    Next
                    StrSql = StrSql + "SELECT * FROM DUAL"
                    odbUtil.UpIns(StrSql, Market1Connection)
                End If
                RegionId = "null"

            End If
        Catch ex As Exception

        End Try
    End Sub
End Class
