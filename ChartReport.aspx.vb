Imports System.Data
Imports System.Data.OleDb
Imports System
Imports M1SubGetData
Imports M1SubUpInsData
Imports System.Collections
Imports System.IO.StringWriter
Imports System.Math
Imports System.Web.UI.HtmlTextWriter
Imports Corda
Imports System.Globalization


Public Class Pages_Market1Sub_CAGR_ChartReport
    Inherits System.Web.UI.Page
    Dim Check As String = ""
    Dim Fils(3) As String
    Protected WithEvents Chart1 As System.Web.UI.HtmlControls.HtmlGenericControl

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

    Private Shared Property InnerHtml As String


    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            REPId = Request.QueryString("RepId").ToString()
            hidReportVal.Value = REPId
            If Not IsPostBack Then
                InitializeComponent()

                Session("dsPref" + REPId.ToString()) = ""
                Session("dsRows" + REPId.ToString()) = ""
                Session("dsColumns" + REPId.ToString()) = ""
                Session("dsFilters" + REPId.ToString()) = ""
                Session("dsData" + REPId.ToString()) = ""
                Session("PrefChange" + REPId.ToString()) = "0"
                hidReport.Value = "0"
                hidReportData.Value = "0"
                hidfil.Value = "0"
                GetReportDetails()
                GetFiltersDropDown()
                GetPageDetails()
                ' Chartreporetdetail()
            End If

        Catch ex As Exception
        End Try
    End Sub

    Protected Sub GetFiltersDropDown()
        Dim objGetData As New Selectdata()
        Dim dsFilter As New DataSet()
        Dim dsProd As New DataSet()
        Dim dsPack As New DataSet()
        Dim dsComp As New DataSet()

        Dim dsTables As New DataSet()
        Dim dvTables As New DataView
        Dim dtTables As New DataTable
        Dim RowCnt As New Integer
        Dim dvProd As New DataView
        Dim dtProd As New DataTable
        Dim dvPack As New DataView
        Dim dtPack As New DataTable
        Dim dsOrder As New DataSet()
        Dim ProdId As String = ""
        Dim PackId As String = ""
        Dim MatId As String = ""
        Dim CompId As String = ""
        Dim FactId As String = ""
        Dim dsMat As New DataSet
        Dim GrpId As String = ""
        Dim dsGrp As New DataSet
        Dim dsProdMat As New DataSet
        Dim CountryId As String = ""
        Dim RegionId As String = ""
        dsFilter = objGetData.GetUsersReportFilters(REPId.ToString())

        tblFil.Controls.Clear()
        Dim tr As TableRow
        Dim td As TableCell
        Dim ddlfil As DropDownList
        Dim lblfil As Label
        dsTables = objGetData.GetProductDetails(Session("M1SubGroupId"))
        dvTables = dsTables.Tables(0).DefaultView
        Dim ProdTbl(dsTables.Tables(0).Rows.Count) As String

        For a = 0 To dsFilter.Tables(0).Rows.Count - 1
            ddlfil = New DropDownList
            lblfil = New Label
            tr = New TableRow
            ddlfil.ID = "ddlfil_" + a.ToString()
            ddlfil.Width = 150
            If hidfil.Value = "0" Then
                Fils(a) = ""
            End If
            If dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PRODUCT" Then
                td = New TableCell
                lblfil.Text = "<b>Select Product: </b>"
                td.Controls.Add(lblfil)
                tr.Controls.Add(td)

                td = New TableCell

                If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    Dim lst As ListItem = New ListItem("All Product", "0")
                    ddlfil.Items.Add(lst)
                    ddlfil.AppendDataBoundItems = True
                    For b = 0 To dsTables.Tables(0).Rows.Count - 1
                        ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                        ProdId = ProdId + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                        RowCnt += 1
                    Next
                    dtTables = dsTables.Tables(0)
                    ProdId = ProdId.Remove(ProdId.Length - 1)
                    If MatId <> "" And PackId <> "" Then
                        dsProd = objGetData.GetPivotPackMatProductsDistinct(ProdId, PackId, MatId)
                    ElseIf CompId <> "" And PackId <> "" Then
                        dsProd = objGetData.GetPivotPackCompProductsDistinct(ProdId, PackId, CompId)
                    ElseIf MatId <> "" Then
                        dsProd = objGetData.GetPivotMatProductsDistinct(MatId, ProdId)
                    ElseIf CompId <> "" Then
                        dsProd = objGetData.GetPivotCompProductsDistinct(CompId, ProdId)
                    ElseIf PackId <> "" Then
                        dsProd = objGetData.GetPivotPackProductsDistinct(ProdId, PackId)
                    Else
                        dsProd = objGetData.GetPivotProductDescription(ProdId)
                    End If
                    ProdId = ""
                    For b = 0 To dsProd.Tables(0).Rows.Count - 1
                        ProdId = ProdId + "" + dsProd.Tables(0).Rows(b).Item("ID").ToString() + ","
                    Next
                    ProdId = ProdId.Remove(ProdId.Length - 1)
                Else
                    ProdId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    dvTables.RowFilter = "ACTUALPRODUCTID=" + ProdId
                    dtTables = dvTables.ToTable()
                    ProdTbl(0) = dtTables.Rows(0).Item("TABLENAME").ToString()
                    RowCnt += 1
                    If MatId <> "" And PackId <> "" Then
                        dsProd = objGetData.GetPivotPackMatProductsDistinct(ProdId, PackId, MatId)
                    ElseIf CompId <> "" And PackId <> "" Then
                        dsProd = objGetData.GetPivotPackCompProductsDistinct(ProdId, PackId, CompId)
                    ElseIf MatId <> "" Then
                        dsProd = objGetData.GetPivotMatProductsDistinct(MatId, ProdId)
                    ElseIf CompId <> "" Then
                        dsProd = objGetData.GetPivotCompProductsDistinct(CompId, ProdId)
                    ElseIf PackId <> "" Then
                        dsProd = objGetData.GetPivotPackProductsDistinct(ProdId, PackId)
                    Else
                        dsProd = objGetData.GetPivotProductDescription(ProdId)
                    End If
                End If
                ' dsProd.Tables(0).TableName = (a + 1).ToString()
                '  dsOrder.Tables.Add(dsProd.Tables((a + 1).ToString()).Copy())


                With ddlfil
                    .DataSource = dsProd
                    .DataTextField = "VALUE"
                    .DataValueField = "ID"
                    .DataBind()
                End With
                If Fils(a).ToString() <> "" Then
                    ddlfil.SelectedValue = Fils(a).ToString()
                Else
                    If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID") = "0" Then
                        ddlfil.SelectedValue = 0 'dsProd.Tables(0).Rows(0).Item("ID").ToString()
                    Else
                        ddlfil.SelectedValue = dsProd.Tables(0).Rows(0).Item("ID").ToString()
                    End If
                End If
                td = New TableCell
                td.Controls.Add(ddlfil)
                tr.Controls.Add(td)
            ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PACKAGE" Then
                td = New TableCell
                lblfil.Text = "<b>Select Package: </b>"
                td.Controls.Add(lblfil)
                tr.Controls.Add(td)

                td = New TableCell
                If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    If ProdId <> "" And MatId <> "" Then
                        dsPack = objGetData.GetPivotProdMatPackagesDistinct(ProdId, MatId, "")
                    ElseIf ProdId <> "" And CompId <> "" Then
                        dsPack = objGetData.GetProdCompPackagesDistinct(ProdId, CompId, "")
                    ElseIf ProdId <> "" Then
                        dsPack = objGetData.GetPivotProdPackageDistinct(ProdId, "")
                    Else
                        Dim ProdId1 As String = ""
                        For b = 0 To dsTables.Tables(0).Rows.Count - 1
                            ProdId1 = ProdId1 + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                        Next
                        ProdId1 = ProdId1.Remove(ProdId1.Length - 1)
                        If MatId <> "" Then
                            dsPack = objGetData.GetPivotMatAllPackagesDistinct(ProdId1, MatId)
                        ElseIf CompId <> "" Then
                            dsPack = objGetData.GetPivotCompAllPackagesDistinct(ProdId1, CompId)
                        Else
                            dsPack = objGetData.GetPackages(ProdId1)
                        End If
                    End If
                    For b = 0 To dsPack.Tables(0).Rows.Count - 1
                        PackId = PackId + "" + dsPack.Tables(0).Rows(b).Item("PACKAGETYPEID").ToString() + ","
                    Next
                    PackId = PackId.Remove(PackId.Length - 1)
                    Dim lst As ListItem = New ListItem("All Package", "0")
                    ddlfil.Items.Add(lst)
                    'ddlfil.Items.Insert(0, "All Package")
                    ddlfil.AppendDataBoundItems = True

                Else
                    PackId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    If ProdId <> "" And MatId <> "" Then
                        dsPack = objGetData.GetPivotProdMatPackagesDistinct(ProdId, MatId, PackId)
                    ElseIf ProdId <> "" And CompId <> "" Then
                        dsPack = objGetData.GetProdCompPackagesDistinct(ProdId, CompId, PackId)
                    ElseIf ProdId <> "" Then
                        dsPack = objGetData.GetPivotProdPackageDistinct(ProdId, PackId)
                    ElseIf MatId <> "" Then
                        dsPack = objGetData.GetPivotMatPackages(MatId, PackId)
                    ElseIf CompId <> "" Then
                        dsPack = objGetData.GetPivotCompPackages(PackId, CompId)
                    Else
                        dsPack = objGetData.GetPivotPackages(PackId)
                    End If
                End If
                'dsPack.Tables(0).TableName = (a + 1).ToString()
                'dsOrder.Tables.Add(dsPack.Tables((a + 1).ToString()).Copy())

                'Changes started
                If dsPack.Tables(0).Rows.Count > 0 Then
                    '
                    With ddlfil
                        .DataSource = dsPack
                        .DataTextField = "VALUE"
                        .DataValueField = "PACKAGETYPEID"
                        .DataBind()
                    End With
                    If Fils(a).ToString() <> "" Then
                        ddlfil.SelectedValue = Fils(a).ToString()
                    Else
                        If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID") = "0" Then
                            ddlfil.SelectedValue = 0
                        Else
                            ddlfil.SelectedValue = dsPack.Tables(0).Rows(0).Item("PACKAGETYPEID").ToString()

                        End If
                    End If


                Else
                    lblNOG.Text = "No Data For this Combination."

                End If
                ' 'Changes end

                td.Controls.Add(ddlfil)
                tr.Controls.Add(td)
            ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "MATERIAL" Then

                MatId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                If ProdId <> "" And PackId <> "" Then
                    dsMat = objGetData.GetPivotPackProdMaterialsDistinct(ProdId, PackId, MatId)
                ElseIf ProdId <> "" Then
                    dsMat = objGetData.GetPivotProdMaterialsDistinct(ProdId, MatId)
                ElseIf PackId <> "" Then
                    Dim ProdId1 As String = ""
                    For b = 0 To dsTables.Tables(0).Rows.Count - 1
                        ProdId1 = ProdId1 + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                    Next
                    ProdId1 = ProdId1.Remove(ProdId1.Length - 1)
                    dsMat = objGetData.GetPivotPackMaterialsDistinct(ProdId1, PackId, MatId)
                Else
                    dsMat = objGetData.GetPivotMaterials(MatId)
                End If
                'dsMat.Tables(0).TableName = (a + 1).ToString()
                'dsOrder.Tables.Add(dsMat.Tables((a + 1).ToString()).Copy())

                'Changes started
                If dsMat.Tables(0).Rows.Count > 0 Then
                    '

                    td = New TableCell
                    lblfil.Text = "<b>Select Material: </b>"
                    td.Controls.Add(lblfil)
                    tr.Controls.Add(td)
                    td = New TableCell
                    With ddlfil
                        .DataSource = dsMat
                        .DataTextField = "VALUE"
                        .DataValueField = "MATERIALID"
                        .DataBind()
                    End With


                    If Fils(a).ToString() <> "" Then
                        ddlfil.SelectedValue = Fils(a).ToString()
                    Else
                        ddlfil.SelectedValue = dsMat.Tables(0).Rows(0).Item("MATERIALID").ToString()
                    End If
                    td.Controls.Add(ddlfil)
                    tr.Controls.Add(td)

                Else
                    With ddlfil
                        .DataSource = dsMat
                        .DataTextField = "VALUE"
                        .DataValueField = "MATERIALID"
                        .DataBind()

                    End With

                    lblNOG.Text = "No Data For this Combination."

                End If

                'Changes end

            ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "COMPONENT" Then
                td = New TableCell
                lblfil.Text = "<b>Select Component: </b>"
                td.Controls.Add(lblfil)
                tr.Controls.Add(td)

                td = New TableCell
                If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    Dim lst As ListItem = New ListItem("All Component", "0")
                    ddlfil.Items.Add(lst)
                    ddlfil.AppendDataBoundItems = True

                    If ProdId <> "" And PackId <> "" Then
                        dsComp = objGetData.GetPivotPackProdComponentsDistinct(ProdId, PackId, "")

                    ElseIf ProdId <> "" Then
                        dsComp = objGetData.GetPivotProdComponentsDistict(ProdId, "")

                    Else
                        Dim ProdId1 As String = ""
                        For b = 0 To dsTables.Tables(0).Rows.Count - 1
                            ProdId1 = ProdId1 + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                        Next
                        ProdId1 = ProdId1.Remove(ProdId1.Length - 1)
                        If PackId <> "" Then
                            dsComp = objGetData.GetPivotPackProdComponentsDistinct(ProdId1, PackId, "")
                        Else
                            dsComp = objGetData.GetPivotAllComponents(ProdId1)
                        End If
                    End If
                    For b = 0 To dsComp.Tables(0).Rows.Count - 1
                        CompId = CompId + "" + dsComp.Tables(0).Rows(b).Item("COMPONENTID").ToString() + ","
                    Next
                    CompId = CompId.Remove(CompId.Length - 1)
                Else
                    CompId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    If ProdId <> "" And PackId <> "" Then
                        dsComp = objGetData.GetPivotPackProdComponents(ProdId, PackId, CompId)

                    ElseIf ProdId <> "" Then
                        dsComp = objGetData.GetPivotProdComponents(ProdId, CompId)

                    ElseIf PackId <> "" Then
                        Dim ProdId1 As String = ""
                        For b = 0 To dsTables.Tables(0).Rows.Count - 1
                            ProdId1 = ProdId1 + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                        Next
                        ProdId1 = ProdId1.Remove(ProdId1.Length - 1)
                        dsComp = objGetData.GetPivotPackComponents(ProdId1, PackId, CompId)

                    Else
                        dsComp = objGetData.GetRepComponent(CompId)
                    End If
                End If

                'dsComp.Tables(0).TableName = (a + 1).ToString()
                'dsOrder.Tables.Add(dsComp.Tables((a + 1).ToString()).Copy())

                With ddlfil
                    .DataSource = dsComp
                    .DataTextField = "VALUE"
                    .DataValueField = "COMPONENTID"
                    .DataBind()
                End With
                If Fils(a).ToString() <> "" Then
                    ddlfil.SelectedValue = Fils(a).ToString()
                Else
                    'ddlfil.SelectedValue = "0" 'dsComp.Tables(0).Rows(0).Item("COMPONENTID").ToString()
                    If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID") = "0" Then
                        ddlfil.SelectedValue = 0
                    Else
                        ddlfil.SelectedValue = dsComp.Tables(0).Rows(0).Item("COMPONENTID").ToString()
                    End If
                End If

                td.Controls.Add(ddlfil)
                tr.Controls.Add(td)


            ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "GROUP" Then
                td = New TableCell
                lblfil.Text = "<b>Select Group: </b>"
                td.Controls.Add(lblfil)
                tr.Controls.Add(td)


                td = New TableCell ''
                GrpId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                dsGrp = objGetData.GetPivotGroups(GrpId)
                'dsGrp.Tables(0).TableName = (a + 1).ToString()
                'dsOrder.Tables.Add(dsGrp.Tables((a + 1).ToString()).Copy())
                With ddlfil
                    .DataSource = dsGrp
                    .DataTextField = "VALUE"
                    .DataValueField = "SUBGROUPID"
                    .DataBind()
                End With
                If Fils(a).ToString() <> "" Then
                    ddlfil.SelectedValue = Fils(a).ToString()
                Else
                    ddlfil.SelectedValue = dsGrp.Tables(0).Rows(0).Item("SUBGROUPID").ToString()

                End If
                td = New TableCell
                td.Controls.Add(ddlfil)
                tr.Controls.Add(td)

            ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "COUNTRY" Then
                td = New TableCell
                lblfil.Text = "<b>Select Country: </b>"
                td.Controls.Add(lblfil)
                tr.Controls.Add(td)
                td = New TableCell
                CountryId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                dsMat = objGetData.GetPivotCountriesByRegion(CountryId)
                'dsMat.Tables(0).TableName = (a + 1).ToString()
                'dsOrder.Tables.Add(dsMat.Tables((a + 1).ToString()).Copy())
                With ddlfil
                    .DataSource = dsMat
                    .DataTextField = "VALUE"
                    .DataValueField = "COUNTRYID"
                    .DataBind()
                End With
                If Fils(a).ToString() <> "" Then
                    ddlfil.SelectedValue = Fils(a).ToString()
                Else
                    ddlfil.SelectedValue = dsMat.Tables(0).Rows(0).Item("COUNTRYID").ToString()

                End If

                td.Controls.Add(ddlfil)
                tr.Controls.Add(td)

            ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "REGION" Then
                td = New TableCell
                lblfil.Text = "<b>Select Region: </b>"
                td.Controls.Add(lblfil)
                tr.Controls.Add(td)
                td = New TableCell
                RegionId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                dsMat = objGetData.GetPivotRegion(RegionId)
                'dsMat.Tables(0).TableName = (a + 1).ToString()
                'dsOrder.Tables.Add(dsMat.Tables((a + 1).ToString()).Copy())
                With ddlfil
                    .DataSource = dsMat
                    .DataTextField = "VALUE"
                    .DataValueField = "REGIONID"
                    .DataBind()
                End With

                If Fils(a).ToString() <> "" Then
                    ddlfil.SelectedValue = Fils(a).ToString()
                Else
                    ddlfil.SelectedValue = dsMat.Tables(0).Rows(0).Item("REGIONID").ToString()
                End If
                td.Controls.Add(ddlfil)
                tr.Controls.Add(td)

            End If
            tblFil.Controls.Add(tr)
        Next
        hidfil.Value = "1"

    End Sub

    Protected Sub GetReportDetails()
        Dim objGetData As New M1GetData.Selectdata()
        Dim ds As New DataSet

        ds = objGetData.GetUserCustomReportsByRptId(REPId.ToString())
        lblheading.Text = "Proprietary Report Chart"

        If ds.Tables(0).Rows.Count > 0 Then
            hidReportType.Value = ds.Tables(0).Rows(0).Item("RPTTYPE").ToString()
            lblReportID.Text = ds.Tables(0).Rows(0).Item("REPORTID").ToString()
            lblReportType.Text = ds.Tables(0).Rows(0).Item("RPTTYPE").ToString() + " (" + ds.Tables(0).Rows(0).Item("RPTTYPEDES").ToString() + ")"
            lblReportDe2.Text = ds.Tables(0).Rows(0).Item("REPORTNAME").ToString()
        End If

        ' ErrorLable.Text = "Error:GetReportDetails:" + ex.Message.ToString()

    End Sub

    Protected Sub GetPageDetails()
        Dim objGetData As New Selectdata()
        Dim dsRpt As New DataSet()
        Dim rptType As String = String.Empty
        dsRpt = objGetData.GetUserCustomReportsByRptId(REPId.ToString())
        If dsRpt.Tables(0).Rows(0)("RPTTYPE").ToString() = "PIVOT" Then
            If REPId = 4021 Or REPId = 4032 Then
                SetUniformReportProdPackMat(dsRpt.Tables(0).Rows(0)("RPTTYPEDES").ToString(), rptType)
            ElseIf REPId = 4044 Or REPId = 4140 Then
                SetUniformReportProdPackMatEMEA(dsRpt.Tables(0).Rows(0)("RPTTYPEDES").ToString(), rptType)
            ElseIf REPId = 4023 Or REPId = 4033 Then
                SetUniformReport_MAT_PACK_PROD(dsRpt.Tables(0).Rows(0)("RPTTYPEDES").ToString(), rptType)
            ElseIf REPId = 4025 Or REPId = 4034 Then
                SetUniformReportPACKPRODMAT_CNTRY(dsRpt.Tables(0).Rows(0)("RPTTYPEDES").ToString(), rptType)
            ElseIf REPId = 4024 Or REPId = 4035 Then
                SetUniformReportPACKPRODMAT_REGION(dsRpt.Tables(0).Rows(0)("RPTTYPEDES").ToString(), rptType)
            ElseIf REPId = 4026 Or REPId = 4036 Then
                SetUniformReportByRegion(dsRpt.Tables(0).Rows(0)("RPTTYPEDES").ToString(), rptType)
            ElseIf REPId = 4027 Or REPId = 4037 Then
                SetUniformReportByCountry(dsRpt.Tables(0).Rows(0)("RPTTYPEDES").ToString(), rptType)
            ElseIf REPId = 4138 Then
                SetUniformReportProdRegBuyers(dsRpt.Tables(0).Rows(0)("RPTTYPEDES").ToString(), rptType)
            ElseIf dsRpt.Tables(0).Rows(0)("RPTTYPEDES").ToString() = "GROUP" Then
                SetPivotGrpReportFrameWorkTemp(REPId.ToString())
            ElseIf dsRpt.Tables(0).Rows(0)("RPTTYPEDES").ToString() = "PROD" Then
                SetPivotProdReportFrameWorkTemp(REPId.ToString())
            ElseIf dsRpt.Tables(0).Rows(0)("RPTTYPEDES").ToString() = "MAT" Then
                SetPivotMatReportFrameWorkTemp(REPId.ToString())
            ElseIf dsRpt.Tables(0).Rows(0)("RPTTYPEDES").ToString() = "COMPONENT" Then
                SetPivotCompReportFrameWorkTemp(REPId.ToString())
            ElseIf dsRpt.Tables(0).Rows(0)("RPTTYPEDES").ToString() = "REGION" Then
                SetPivotRegReportFrameWorkTemp(REPId.ToString())
            ElseIf dsRpt.Tables(0).Rows(0)("RPTTYPEDES").ToString() = "PACK" Then
                SetPivotPackReportFrameWorkTemp(REPId.ToString())
            ElseIf dsRpt.Tables(0).Rows(0)("RPTTYPEDES").ToString() = "CNTRY" Then
                SetPivotCntryReportFrameWorkTemp(REPId.ToString())
            End If
        End If
    End Sub

    Private Sub SetPivotRegReportFrameWorkTemp(ByVal p1 As String)
        Dim objGetData As New Selectdata()
        Dim i As New Integer
        Dim j As New Integer
        Dim HeaderTr As New TableRow()
        Dim HeaderTd As New TableCell()
        Dim Tr As New TableRow()
        Dim Td As New TableCell()
        Dim Path As String = String.Empty
        Dim dsRows As New DataSet
        Dim dsCol As New DataSet
        Dim dsFilter As New DataSet
        Dim Des As String = String.Empty
        Dim k As Integer
        Dim dv As New DataView
        Dim dvCol As New DataView
        Dim dt As New DataTable
        Dim YearId As String = ""
        Dim UnitId As String = ""
        Dim dvRptCols1 As New DataView
        Dim dtRptCols1 As New DataTable
        Dim dvRptCols2 As New DataView
        Dim dtRptCols2 As New DataTable
        Dim dsUnitPref As New DataSet
        Dim dsRep As New DataSet
        Dim RowCnt As New Integer
        Dim ColCnt As New Integer
        Dim filterCnt As New Integer
        Dim GrpId As String = ""
        Dim dsGrp As New DataSet
        Dim imgUp As New ImageButton
        Dim imgDn As New ImageButton
        Dim CompId As String = ""
        Dim dsComp As New DataSet
        If hidReport.Value <> "0" Then
            dsRep = Session("dsRep" + REPId.ToString())
            dsUnitPref = Session("dsPref" + REPId.ToString())
            dsRows = Session("dsRows" + REPId.ToString())
            dsCol = Session("dsColumns" + REPId.ToString())
            dsFilter = Session("dsFilters" + REPId.ToString())
        Else
            dsRep = objGetData.GetReportDetails(REPId.ToString())
            dsUnitPref = objGetData.GetPref(REPId.ToString())
            dsRows = objGetData.GetUsersDynamicReportRows(REPId.ToString())
            dsCol = objGetData.GetUsersDynamicReportCols(REPId.ToString())
            dsFilter = objGetData.GetUsersReportFilters(REPId.ToString())

            Session("dsRep" + REPId.ToString()) = dsRep
            Session("dsPref" + REPId.ToString()) = dsUnitPref
            Session("dsRows" + REPId.ToString()) = dsRows
            Session("dsColumns" + REPId.ToString()) = dsCol
            Session("dsFilters" + REPId.ToString()) = dsFilter

            hidReport.Value = "1"
        End If

        filterCnt = dsFilter.Tables(0).Rows.Count
        ColCnt = dsCol.Tables(0).Rows.Count

        For a = 0 To dsCol.Tables(0).Rows.Count - 1
            If dsCol.Tables(0).Rows(a).Item("COLUMNVALUETYPE").ToString() <> "Formula" Then
                YearId = YearId + "" + dsCol.Tables(0).Rows(a).Item("COLUMNVALUEID").ToString() + ","
            End If
        Next
        YearId = YearId.Remove(YearId.Length - 1)

        UnitId = dsRows.Tables(0).Rows(0).Item("UNITID").ToString()
        dvCol = dsCol.Tables(0).DefaultView
        dvRptCols1 = dsCol.Tables(0).DefaultView
        dvRptCols2 = dsCol.Tables(0).DefaultView

        Dim dsTables As New DataSet
        Dim dvTables As New DataView
        Dim dtTables As New DataTable
        Dim ProdId As String = ""
        Dim PackId As String = ""
        Dim dsProd As New DataSet
        Dim dvProd As New DataView
        Dim dtProd As New DataTable
        Dim dsPack As New DataSet
        Dim dvPack As New DataView
        Dim dtPack As New DataTable
        Dim dsOrder As New DataSet()
        Dim funflag As Boolean = False
        Dim MatId As String = ""
        Dim FactId As String = ""
        Dim dsMat As New DataSet
        Dim dsProdMat As New DataSet
        Dim arrRfilt(filterCnt) As String
        Dim filDes As String
        dsTables = objGetData.GetProductDetails(Session("M1SubGroupId"))
        Dim ProdTbl(dsTables.Tables(0).Rows.Count) As String

        dvTables = dsTables.Tables(0).DefaultView
        For a = 0 To dsFilter.Tables(0).Rows.Count - 1
            filDes = Request.Form("ddlfil_" + a.ToString())
            If filDes <> Nothing Then
                'filDes = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                Fils(a) = filDes.ToString()
            Else
                filDes = "0"
                Fils(a) = "0"
                If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() <> "0" Then
                    filDes = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                End If
            End If
            If dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PRODUCT" Then
                arrRfilt(a) = "PRODUCT"
                If filDes = "0" Then
                    For b = 0 To dsTables.Tables(0).Rows.Count - 1
                        ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                        ProdId = ProdId + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                        RowCnt += 1
                    Next
                    dtTables = dsTables.Tables(0)
                    ProdId = ProdId.Remove(ProdId.Length - 1)
                    If MatId <> "" And PackId <> "" Then
                        dsProd = objGetData.GetPivotPackMatProducts(ProdId, PackId, MatId)
                    ElseIf CompId <> "" And PackId <> "" Then
                        dsProd = objGetData.GetPivotPackCompProducts(ProdId, PackId, CompId)
                    ElseIf MatId <> "" Then
                        dsProd = objGetData.GetPivotMatProducts(MatId, ProdId)
                    ElseIf CompId <> "" Then
                        dsProd = objGetData.GetPivotCompProducts(CompId, ProdId)
                    ElseIf PackId <> "" Then
                        dsProd = objGetData.GetPivotPackProducts(ProdId, PackId)
                    Else
                        dsProd = objGetData.GetPivotProductDescription(ProdId)
                    End If
                    ProdId = ""
                    For b = 0 To dsProd.Tables(0).Rows.Count - 1
                        ProdId = ProdId + "" + dsProd.Tables(0).Rows(b).Item("ID").ToString() + ","
                    Next
                    ProdId = ProdId.Remove(ProdId.Length - 1)

                Else
                    ProdId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    dvTables.RowFilter = "ACTUALPRODUCTID=" + ProdId
                    dtTables = dvTables.ToTable()
                    ProdTbl(0) = dtTables.Rows(0).Item("TABLENAME").ToString()
                    RowCnt += 1
                    If MatId <> "" And PackId <> "" Then
                        dsProd = objGetData.GetPivotPackMatProducts(ProdId, PackId, MatId)

                    ElseIf CompId <> "" And PackId <> "" Then
                        dsProd = objGetData.GetPivotPackCompProducts(ProdId, PackId, CompId)

                    ElseIf MatId <> "" Then
                        dsProd = objGetData.GetPivotMatProducts(MatId, ProdId)

                    ElseIf CompId <> "" Then
                        dsProd = objGetData.GetPivotCompProducts(CompId, ProdId)

                    ElseIf PackId <> "" Then
                        dsProd = objGetData.GetPivotPackProducts("", PackId)

                    Else
                        dsProd = objGetData.GetPivotProductDescription(ProdId)
                    End If
                End If

                dsProd.Tables(0).TableName = (a + 1).ToString()
                dsOrder.Tables.Add(dsProd.Tables((a + 1).ToString()).Copy())
                If a = dsFilter.Tables(0).Rows.Count - 1 Then
                    If dsProd.Tables(0).Rows.Count <> 0 Then
                        funflag = True
                    Else
                        funflag = False
                    End If
                End If
            ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PACKAGE" Then
                arrRfilt(a) = "PACKAGE"
                If filDes = "0" Then
                    If ProdId <> "" And MatId <> "" Then
                        dsPack = objGetData.GetPivotProdMatPackages(ProdId, MatId, "")

                    ElseIf ProdId <> "" And CompId <> "" Then
                        dsPack = objGetData.GetProdCompPackages(ProdId, CompId, "")

                    ElseIf ProdId <> "" Then
                        dsPack = objGetData.GetPivotProdPackage(ProdId, "")

                    Else
                        Dim ProdId1 As String = ""
                        For b = 0 To dsTables.Tables(0).Rows.Count - 1
                            ProdId1 = ProdId1 + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                        Next
                        ProdId1 = ProdId1.Remove(ProdId1.Length - 1)
                        If MatId <> "" Then
                            dsPack = objGetData.GetPivotMatAllPackages(ProdId1, MatId)
                        ElseIf CompId <> "" Then
                            dsPack = objGetData.GetPivotCompAllPackages(ProdId1, CompId)
                        Else
                            dsPack = objGetData.GetPackages(ProdId1)
                        End If
                    End If
                    For b = 0 To dsPack.Tables(0).Rows.Count - 1
                        PackId = PackId + "" + dsPack.Tables(0).Rows(b).Item("PACKAGETYPEID").ToString() + ","
                    Next
                    PackId = PackId.Remove(PackId.Length - 1)
                Else
                    PackId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    If ProdId <> "" And MatId <> "" Then
                        dsPack = objGetData.GetPivotProdMatPackages(ProdId, MatId, PackId)

                    ElseIf ProdId <> "" And CompId <> "" Then
                        dsPack = objGetData.GetProdCompPackages(ProdId, CompId, PackId)

                    ElseIf ProdId <> "" Then
                        dsPack = objGetData.GetPivotProdPackage(ProdId, PackId)

                    ElseIf MatId <> "" Then
                        dsPack = objGetData.GetPivotMatPackages(MatId, PackId)

                    ElseIf CompId <> "" Then
                        dsPack = objGetData.GetPivotCompPackages(PackId, CompId)

                    Else
                        dsPack = objGetData.GetPivotPackages(PackId)
                    End If
                End If

                dsPack.Tables(0).TableName = (a + 1).ToString()
                dsOrder.Tables.Add(dsPack.Tables((a + 1).ToString()).Copy())
                If a = dsFilter.Tables(0).Rows.Count - 1 Then
                    If dsPack.Tables(0).Rows.Count <> 0 Then
                        funflag = True
                    Else
                        funflag = False
                    End If
                End If
            ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "MATERIAL" Then
                arrRfilt(a) = "MATERIAL"
                If filDes = "0" Then
                    MatId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                Else
                    MatId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                End If
                If ProdId <> "" And PackId <> "" Then
                    dsMat = objGetData.GetPivotPackProdMaterials(ProdId, PackId, MatId)

                ElseIf ProdId <> "" Then
                    dsMat = objGetData.GetPivotProdMaterials(ProdId, MatId)

                ElseIf PackId <> "" Then
                    Dim ProdId1 As String = ""
                    For b = 0 To dsTables.Tables(0).Rows.Count - 1
                        ProdId1 = ProdId1 + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                    Next
                    ProdId1 = ProdId1.Remove(ProdId1.Length - 1)
                    dsMat = objGetData.GetPivotPackMaterials(ProdId1, PackId, MatId)

                Else
                    dsMat = objGetData.GetPivotMaterials(MatId)
                End If

                dsMat.Tables(0).TableName = (a + 1).ToString()
                dsOrder.Tables.Add(dsMat.Tables((a + 1).ToString()).Copy())
                If a = dsFilter.Tables(0).Rows.Count - 1 Then
                    If dsMat.Tables(0).Rows.Count <> 0 Then
                        funflag = True
                    Else
                        funflag = False
                    End If
                End If

            ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "COMPONENT" Then
                arrRfilt(a) = "COMPONENT"
                If filDes = "0" Then
                    If ProdId <> "" And PackId <> "" Then
                        dsComp = objGetData.GetPivotPackProdComponents(ProdId, PackId, "")

                    ElseIf ProdId <> "" Then
                        dsComp = objGetData.GetPivotProdComponents(ProdId, "")

                    Else
                        Dim ProdId1 As String = ""
                        For b = 0 To dsTables.Tables(0).Rows.Count - 1
                            ProdId1 = ProdId1 + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                        Next
                        ProdId1 = ProdId1.Remove(ProdId1.Length - 1)
                        If PackId <> "" Then
                            dsComp = objGetData.GetPivotPackComponents(ProdId1, PackId, "")
                        Else
                            dsComp = objGetData.GetPivotAllComponents(ProdId1)
                        End If
                    End If
                    For b = 0 To dsComp.Tables(0).Rows.Count - 1
                        CompId = CompId + "" + dsComp.Tables(0).Rows(b).Item("COMPONENTID").ToString() + ","
                    Next
                    CompId = CompId.Remove(CompId.Length - 1)
                Else
                    CompId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    If ProdId <> "" And PackId <> "" Then
                        dsComp = objGetData.GetPivotPackProdComponents(ProdId, PackId, CompId)

                    ElseIf ProdId <> "" Then
                        dsComp = objGetData.GetPivotProdComponents(ProdId, CompId)

                    ElseIf PackId <> "" Then
                        Dim ProdId1 As String = ""
                        For b = 0 To dsTables.Tables(0).Rows.Count - 1
                            ProdId1 = ProdId1 + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                        Next
                        ProdId1 = ProdId1.Remove(ProdId1.Length - 1)
                        dsComp = objGetData.GetPivotPackComponents(ProdId1, PackId, CompId)

                    Else
                        dsComp = objGetData.GetRepComponent(CompId)
                    End If
                End If

                dsComp.Tables(0).TableName = (a + 1).ToString()
                dsOrder.Tables.Add(dsComp.Tables((a + 1).ToString()).Copy())
                If a = dsFilter.Tables(0).Rows.Count - 1 Then
                    If dsComp.Tables(0).Rows.Count <> 0 Then
                        funflag = True
                    Else
                        funflag = False
                    End If
                End If

            ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "GROUP" Then
                arrRfilt(a) = "GROUP"
                'GrpId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                If filDes = "0" Then
                    GrpId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                Else
                    GrpId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                End If
                dsGrp = objGetData.GetPivotGroups(GrpId)

                dsGrp.Tables(0).TableName = (a + 1).ToString()
                dsOrder.Tables.Add(dsGrp.Tables((a + 1).ToString()).Copy())
                If a = dsFilter.Tables(0).Rows.Count - 1 Then
                    If dsGrp.Tables(0).Rows.Count <> 0 Then
                        funflag = True
                    Else
                        funflag = False
                    End If
                End If
            End If
        Next

        Dim dv1 As New DataView
        Dim dv2 As New DataView
        Dim dv3 As New DataView
        Dim dt1 As New DataTable
        Dim dt2 As New DataTable
        Dim dt3 As New DataTable
        Dim dsRptAct As New DataSet
        Dim dvRptAct As New DataView
        Dim dtRptAct As New DataTable
        Dim dvRpt As New DataView
        Dim dtRpt As New DataTable
        Dim strRFilt1 As String = ""
        Dim strRFilt2 As String = ""
        Dim strRFilt3 As String = ""
        Dim dsRows1 As New DataSet
        Dim dvRows1 As New DataView
        Dim dvRows11 As New DataView
        Dim dtRow1 As New DataTable


        If ProdTbl(0) = "" Then
            For b = 0 To dsTables.Tables(0).Rows.Count - 1
                ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                RowCnt += 1
            Next
        End If
        If funflag Then
            If filterCnt = 1 Then
                dv1 = dsOrder.Tables(0).DefaultView
            ElseIf filterCnt = 2 Then
                dv1 = dsOrder.Tables(0).DefaultView
                dv2 = dsOrder.Tables(1).DefaultView
            ElseIf filterCnt = 3 Then
                dv1 = dsOrder.Tables(0).DefaultView
                dv2 = dsOrder.Tables(1).DefaultView
                dv3 = dsOrder.Tables(2).DefaultView
            End If

            dsRows1 = objGetData.GetReportRegionsByRegionSet(dsRep.Tables(0).Rows(0).Item("REGIONSETID").ToString())
            dsRptAct = objGetData.GetPivotReportData_REGION(ProdTbl, MatId, YearId, ProdId, PackId, GrpId, CompId, UnitId, dsTables.Tables(0).Rows.Count, dsRep.Tables(0).Rows(0).Item("REGIONSETID").ToString(), dsRows1)
            dvRptAct = dsRptAct.Tables(0).DefaultView
            dvRpt = dsRptAct.Tables(0).DefaultView
            dtRptAct = dvRptAct.ToTable()

            dvRows1 = dsRows1.Tables(0).DefaultView '
            dvRows11 = dsRows1.Tables(0).DefaultView
            dtRow1 = dvRows1.ToTable()

            ' changes started 
            If dsRptAct.Tables(0).Rows.Count > 0 Then
                '  changes end 


                'CODE FOR GRAPH
                Dim pcScript = ""
                Dim odbutil As New DBUtil()
                Dim Graphtype As String = ""
                Dim GraphName As String = ""

                Dim pref As String = String.Empty
                If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                    pref = "(" + dsRows.Tables(0).Rows(0).Item("UNITSHRT").ToString() + ")"
                Else
                    pref = "(" + dsRows.Tables(0).Rows(0).Item("METRICUNIT").ToString() + ")"
                End If
                Dim Count As String = dtRptAct.Rows.Count
                Dim Str1 As String = ""

                ' Changes Started
                Dim S As String = ""

                For i = 0 To dsCol.Tables(0).Rows.Count - 1
                    Str1 = Str1 + " "
                    If dsCol.Tables(0).Rows(i).Item("COLUMNVALUE").ToString() = "CAGR" Then

                        S = dsCol.Tables(0).Rows(i).Item("COLUMNVALUE").ToString() + "(" + dsCol.Tables(0).Rows(i).Item("INPUTVALUE1").ToString() + "/" + dsCol.Tables(0).Rows(i).Item("INPUTVALUE2").ToString() + ")"
                        Str1 = Str1 + "" + S + ";"
                    Else
                        Str1 = Str1 + dsCol.Tables(0).Rows(i).Item("COLUMNVALUE") + ";"
                    End If

                Next
                ' Changes end

                'For i = 0 To dsCol.Tables(0).Rows.Count - 1
                '    Str1 = Str1 + dsCol.Tables(0).Rows(i).Item("COLUMNVALUE") + "; "
                'Next



                Dim Str2 As String = ""
                For i = 0 To dsRows1.Tables(0).Rows.Count - 1
                    Str2 = Str2 + dsRows1.Tables(0).Rows(i).Item("NAME") + "; "
                Next
                Dim Data As String = String.Empty
                GraphName = "bar"
                Graphtype = "graph"
                ' Dim Count1 As String = dtRptAct.Rows.Count
                dvRpt = dsRptAct.Tables(0).DefaultView

                pcScript &= "" + Graphtype + ".transposed(true)" + Graphtype + ".setCategories(" + Str1.ToString() + ")"
                'single bar multiple data
                'For j = 0 To dsRows1.Tables(0).Rows.Count - 1
                '    pcScript &= "" + Graphtype + ".setSeries(" & dsRows1.Tables(0).Rows(j).Item("NAME") & ";"

                '    For i = 0 To dsCol.Tables(0).Rows.Count - 1
                '        dvRpt.RowFilter = " REGIONID = " + dsRows1.Tables(0).Rows(j).Item("ID").ToString().Replace(" ", " ") + " AND YEARID= " + dsCol.Tables(0).Rows(i).Item("COLUMNVALUEID").ToString().Replace(" ", " ")
                '        dtRpt = dvRpt.ToTable()
                '        Data = FormatNumber(dtRpt.Rows(0).Item("FCT").ToString(), 0)
                '        pcScript &= "" + Data + ";"

                '    Next
                '    pcScript &= ")"
                'Next

                'double bar
                Dim strt(dsCol.Tables(0).Rows.Count) As String

                For j = 0 To dsRows1.Tables(0).Rows.Count - 1
                    For i = 0 To dsCol.Tables(0).Rows.Count - 1
                        If i = 0 Then
                            pcScript &= "" + Graphtype + ".setSeries(" & dsRows1.Tables(0).Rows(j).Item("NAME") & ""
                        End If
                        If dsCol.Tables(0).Rows(i).Item("COLUMNVALUETYPE").ToString() <> "Formula" Then
                            dvRpt.RowFilter = " REGIONID = " + dsRows1.Tables(0).Rows(j).Item("ID").ToString().Replace(" ", " ") + " AND YEARID= " + dsCol.Tables(0).Rows(i).Item("COLUMNVALUEID").ToString().Replace(" ", " ")
                            dtRpt = dvRpt.ToTable()
                            Dim ProdCount As Double = 0.0
                            If dtRpt.Rows.Count > 0 Then
                                For l = 0 To dtRpt.Rows.Count - 1
                                    If dtRpt.Rows(l).Item("FCT").ToString() <> "" Then
                                        If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                                            If UnitId = 1 Then
                                                ProdCount = ProdCount + FormatNumber(dtRpt.Rows(l).Item("FCT").ToString(), 0)
                                            ElseIf UnitId = 2 Then
                                                ProdCount = ProdCount + FormatNumber(dtRpt.Rows(l).Item("FCT").ToString(), 0)
                                            ElseIf UnitId = 4 Then
                                                ProdCount = ProdCount + FormatNumber(dtRpt.Rows(l).Item("FCT").ToString(), 0)
                                            Else
                                                ProdCount = ProdCount + FormatNumber(dtRpt.Rows(l).Item("FCT").ToString(), 0)
                                            End If
                                        Else
                                            If UnitId = 1 Then
                                                ProdCount = ProdCount + FormatNumber(dtRpt.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT"), 0)
                                            ElseIf UnitId = 2 Then
                                                ProdCount = ProdCount + FormatNumber(dtRpt.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL"), 0)
                                            ElseIf UnitId = 3 Then
                                                ProdCount = ProdCount + FormatNumber(dtRpt.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA"), 0)
                                            ElseIf UnitId = 4 Then
                                                ProdCount = ProdCount + FormatNumber(dtRpt.Rows(l).Item("FCT").ToString(), 0)
                                            Else
                                                ProdCount = ProdCount + FormatNumber(dtRpt.Rows(l).Item("FCT").ToString(), 0)
                                            End If
                                        End If
                                    End If
                                Next

                            End If
                            'Data = FormatNumber(dtRpt.Rows(0).Item("FCT").ToString(), 0)
                            pcScript &= ";" + ProdCount.ToString() + ""
                            Data = ProdCount.ToString()
                            strt(i) = "" + ProdCount.ToString()

                        Else
                            Dim count1 As New Double
                            Dim count2 As New Double
                            Dim diff As New Double
                            dvRptCols1.RowFilter = "USERREPORTCOLUMNID=" + dsCol.Tables(0).Rows(i).Item("INPUTVALUETYPE1").ToString()
                            dtRptCols1 = dvRptCols1.ToTable()
                            dvRptCols2.RowFilter = "USERREPORTCOLUMNID=" + dsCol.Tables(0).Rows(i).Item("INPUTVALUETYPE2").ToString()
                            dtRptCols2 = dvRptCols2.ToTable()

                            ' If strRFilt1 <> "" Then
                            dvRptAct.RowFilter = "  REGIONID = " + dsRows1.Tables(0).Rows(j).Item("ID").ToString() + " AND YEARID=" + dtRptCols1.Rows(0).Item("COLUMNVALUEID").ToString()
                            ' Else
                            ' dvRptAct.RowFilter = "YEARID=" + dtRptCols1.Rows(0).Item("COLUMNVALUEID").ToString()
                            'End If
                            dtRptAct = dvRptAct.ToTable()
                            If dtRptAct.Rows.Count > 0 Then
                                For l = 0 To dtRptAct.Rows.Count - 1
                                    If dtRptAct.Rows(l).Item("FCT").ToString() <> "" Then
                                        If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                                            If UnitId = 1 Then
                                                count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                            ElseIf UnitId = 2 Then
                                                count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                            ElseIf UnitId = 4 Then
                                                count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                            Else
                                                count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                            End If
                                        Else
                                            If UnitId = 1 Then
                                                count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT"), 0)
                                            ElseIf UnitId = 2 Then
                                                count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL"), 0)
                                            ElseIf UnitId = 3 Then
                                                count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA"), 0)
                                            ElseIf UnitId = 4 Then
                                                count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                            Else
                                                count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                            End If
                                        End If
                                    End If
                                Next
                            End If

                            ' If strRFilt1 <> "" Then
                            dvRptAct.RowFilter = " REGIONID = " + dsRows1.Tables(0).Rows(j).Item("ID").ToString() + " AND YEARID=" + dtRptCols2.Rows(0).Item("COLUMNVALUEID").ToString()
                            ' Else
                            ' dvRptAct.RowFilter = "YEARID=" + dtRptCols2.Rows(0).Item("COLUMNVALUEID").ToString()
                            ' End If
                            dtRptAct = dvRptAct.ToTable()
                            If dtRptAct.Rows.Count > 0 Then
                                For l = 0 To dtRptAct.Rows.Count - 1
                                    If dtRptAct.Rows(l).Item("FCT").ToString() <> "" Then
                                        If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                                            If UnitId = 1 Then
                                                count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                            ElseIf UnitId = 2 Then
                                                count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                            ElseIf UnitId = 4 Then
                                                count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                            Else
                                                count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                            End If
                                        Else
                                            If UnitId = 1 Then
                                                count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT"), 0)
                                            ElseIf UnitId = 2 Then
                                                count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL"), 0)
                                            ElseIf UnitId = 3 Then
                                                count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA"), 0)
                                            ElseIf UnitId = 4 Then
                                                count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                            Else
                                                count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                            End If
                                        End If
                                    End If
                                Next
                            End If

                            diff = dtRptCols2.Rows(0).Item("COLUMNVALUEID") - dtRptCols1.Rows(0).Item("COLUMNVALUEID")
                            If count1 <> 0 And count2 <> 0 Then
                                Data = FormatNumber((((count2 / count1) ^ (1 / diff)) - 1) * 100, 4)
                            Else
                                Data = FormatNumber(0, 4)
                            End If
                            pcScript &= ";" + Data
                            strt(i) = "" + Data.ToString()

                        End If
                    Next
                    '    pcScript &= ")"
                    '    Dim stra As String = ""
                    '    For k = 0 To dsCol.Tables(0).Rows.Count - 1
                    '        pcScript = pcScript + "" + Graphtype + ".addHoverText(" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString()
                    '        pcScript = pcScript + "" + "," + (j + 1).ToString() + "" ' dsRows1.Tables(0).Rows(j).Item("COUNTRYDES").ToString() + ""
                    '        pcScript = pcScript + "" + ",Year:" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + "<br/>"
                    '        pcScript = pcScript + "" + "Value:" + strt(k).ToString() + ")"
                    '    Next
                    'Next


                    ' Changes Started
                    pcScript &= ")"
                    Dim stra As String = ""
                    Dim S1 As String = ""
                    Dim S2 As String = ""

                    For k = 0 To dsCol.Tables(0).Rows.Count - 1
                        ' pcScript = pcScript + "" + Graphtype + ".addHoverText(" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + ""

                        pcScript = pcScript + "" + Graphtype

                        If dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() = "CAGR" Then

                            S1 = dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + "(" + dsCol.Tables(0).Rows(k).Item("INPUTVALUE1").ToString() + "/" + dsCol.Tables(0).Rows(k).Item("INPUTVALUE2").ToString() + ")"

                            pcScript = pcScript + ".addHoverText(" + S1 + ""
                        Else
                            pcScript = pcScript + ".addHoverText(" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + ""

                        End If


                        pcScript = pcScript + "" + "," + (j + 1).ToString() + "" ' dsRows1.Tables(0).Rows(j).Item("COUNTRYDES").ToString() + ""

                        pcScript = pcScript + ""

                        If dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() = "CAGR" Then

                            S1 = "(" + dsCol.Tables(0).Rows(k).Item("INPUTVALUE1").ToString() + "/" + dsCol.Tables(0).Rows(k).Item("INPUTVALUE2").ToString() + ")"

                            pcScript = pcScript + "" + ",CAGR:" + S1 + "<br/>"


                        Else

                            pcScript = pcScript + "" + ",Year:" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + "<br/>"
                        End If


                        pcScript = pcScript + "" + "Value:" + strt(k).ToString() + ")"


                    Next

                Next


                GenrateChart(pcScript, GraphName, pref, dsRows1.Tables(0).Rows.Count * ColCnt)
                lblNOG.Visible = False

            Else
                MaterialPrice.Visible = False
                lblNOG.Visible = True
                lblNOG.Text = "Values Are Not Available for this Report."
                GetFilters()
            End If
        Else
            MaterialPrice.Visible = False
            lblNOG.Visible = True
            lblNOG.Text = "No Data For this Combination."
            GetFilters()
        End If

    End Sub

    Protected Sub GenrateChart(ByVal PcScript As String, ByVal GraphName As String, ByVal Pref As String, ByVal count As String)
        Try
            lblNOG.Visible = False
            MaterialPrice.Visible = True
            Dim dsChartSetting As New DataTable
            Dim objGetData As New Configration.Selectdata()
            dsChartSetting = objGetData.GetChartSettings().Tables(0)

            Dim myImage As CordaEmbedder = New CordaEmbedder()
            'myImage.externalServerAddress = dsChartSetting.Rows(0)("EXTSERVERADD").ToString()
            Dim currenturl = Request.ServerVariables("HTTP_HOST")
            If currenturl.Contains("www.savvypack.com") Or currenturl.Contains("savvypack.com") Then
                myImage.externalServerAddress = dsChartSetting.Rows(0)("EXTSERVERADD").ToString()
            Else
                myImage.externalServerAddress = "http://192.168.3.31:3001/"
            End If
            myImage.internalCommPortAddress = dsChartSetting.Rows(0)("INTCOMPORTADD").ToString()

            myImage.imageTemplate = "BatNew" + ".itxml" 'adjective bar
            'myImage.imageTemplate = "Sargento_KraftPouchvsTray_Html" + ".itxml"
            ' myImage.imageTemplate = "Sargento_KraftPouchvsTray" + ".itxml" 'single bar
            myImage.userAgent = Request.UserAgent


            myImage.width = count * 80
            If count > 600 Then
                myImage.width = count * 40
                If count > 1200 Then
                    myImage.width = 50000
                End If
            End If
            If count < 10 Then
                myImage.width = 700
            End If
            'myImage.width = 700
            myImage.height = 350
            myImage.returnDescriptiveLink = True
            myImage.language = "EN"
            myImage.pcScript = PcScript + "Y-axis.SetText(" + Pref + ")"
            myImage.outputType = "FLASH"
            myImage.fallback = "STRICT"
            MaterialPrice.InnerHtml = myImage.getEmbeddingHTML
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub SetPivotGrpReportFrameWorkTemp(ByVal RptID As String)
        Dim objGetData As New Selectdata()
        Dim i As New Integer
        Dim j As New Integer
        Dim HeaderTr As New TableRow()
        Dim HeaderTd As New TableCell()
        Dim Tr As New TableRow()
        Dim Td As New TableCell()
        Dim Path As String = String.Empty
        Dim Link As HyperLink
        Dim hyd As HiddenField
        Dim dsRows As New DataSet
        Dim dsCol As New DataSet
        Dim dsFilter As New DataSet
        Dim Flag As Boolean
        Dim Des As String = String.Empty
        Dim k As Integer
        Dim tblFilter As Table
        Dim trRow As TableRow
        Dim trCol As TableCell
        Dim dv As New DataView
        Dim dvCol As New DataView
        Dim dt As New DataTable
        Dim lbl As Label
        Dim YearId As String = ""
        Dim UnitId As String = ""
        Dim dvRptCols1 As New DataView
        Dim dtRptCols1 As New DataTable
        Dim dvRptCols2 As New DataView
        Dim dtRptCols2 As New DataTable
        Dim dsUnitPref As New DataSet
        Dim dsRep As New DataSet
        Dim RowCnt As New Integer
        Dim ColCnt As New Integer
        Dim filterCnt As New Integer
        Dim imgUp As New ImageButton
        Dim imgDn As New ImageButton
        Try
            'Checking for Edit
            If ViewState("Edit") = "Y" Then
                Flag = True
            Else
                Flag = False
            End If

            If hidReport.Value <> "0" Then
                dsRep = Session("dsRep" + REPId.ToString())
                dsUnitPref = Session("dsPref" + REPId.ToString())
                dsRows = Session("dsRows" + REPId.ToString())
                dsCol = Session("dsColumns" + REPId.ToString())
                dsFilter = Session("dsFilters" + REPId.ToString())
            Else
                dsRep = objGetData.GetReportDetails(REPId.ToString())
                dsUnitPref = objGetData.GetPref(REPId.ToString())
                dsRows = objGetData.GetUsersDynamicReportRows(REPId.ToString())
                dsCol = objGetData.GetUsersDynamicReportCols(REPId.ToString())
                dsFilter = objGetData.GetUsersReportFilters(REPId.ToString())

                Session("dsRep" + REPId.ToString()) = dsRep
                Session("dsPref" + REPId.ToString()) = dsUnitPref
                Session("dsRows" + REPId.ToString()) = dsRows
                Session("dsColumns" + REPId.ToString()) = dsCol
                Session("dsFilters" + REPId.ToString()) = dsFilter

                hidReport.Value = "1"
            End If

            filterCnt = dsFilter.Tables(0).Rows.Count
            ColCnt = dsCol.Tables(0).Rows.Count

            For a = 0 To dsCol.Tables(0).Rows.Count - 1
                If dsCol.Tables(0).Rows(a).Item("COLUMNVALUETYPE").ToString() <> "Formula" Then
                    YearId = YearId + "" + dsCol.Tables(0).Rows(a).Item("COLUMNVALUEID").ToString() + ","
                End If
            Next
            YearId = YearId.Remove(YearId.Length - 1)

            UnitId = dsRows.Tables(0).Rows(0).Item("UNITID").ToString()

            dvCol = dsCol.Tables(0).DefaultView
            dvRptCols1 = dsCol.Tables(0).DefaultView
            dvRptCols2 = dsCol.Tables(0).DefaultView

            Dim dsTables As New DataSet
            Dim dvTables As New DataView
            Dim dtTables As New DataTable
            Dim ProdId As String = ""
            Dim PackId As String = ""
            Dim dsProd As New DataSet
            Dim dvProd As New DataView
            Dim dtProd As New DataTable
            Dim dsPack As New DataSet
            Dim dvPack As New DataView
            Dim dtPack As New DataTable
            Dim dsOrder As New DataSet()
            Dim funflag As Boolean = False
            Dim MatId As String = ""
            Dim FactId As String = ""
            Dim dsMat As New DataSet
            Dim dsProdMat As New DataSet
            Dim arrRfilt(filterCnt) As String
            Dim CountryId As String = ""
            Dim RegionId As String = ""
            Dim filDes As String
            dsTables = objGetData.GetProductDetails(Session("M1SubGroupId"))
            dvTables = dsTables.Tables(0).DefaultView
            Dim ProdTbl(dsTables.Tables(0).Rows.Count) As String

            For a = 0 To dsFilter.Tables(0).Rows.Count - 1
                filDes = Request.Form("ddlfil_" + a.ToString())
                If filDes <> Nothing Then
                    'filDes = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    Fils(a) = filDes.ToString()
                Else
                    filDes = "0"
                    Fils(a) = "0"
                    If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() <> "0" Then
                        filDes = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    End If
                End If
                If dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PRODUCT" Then
                    arrRfilt(a) = "PRODUCT"

                    'If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    If filDes = "0" Then

                        For b = 0 To dsTables.Tables(0).Rows.Count - 1
                            ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                            ProdId = ProdId + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                            RowCnt += 1
                        Next
                        dtTables = dsTables.Tables(0)
                        ProdId = ProdId.Remove(ProdId.Length - 1)
                        If PackId <> "" Then
                            dsProd = objGetData.GetPivotPackProducts(ProdId, PackId)

                        Else
                            dsProd = objGetData.GetPivotProductDescription(ProdId)
                        End If
                        ProdId = ""
                        For b = 0 To dsProd.Tables(0).Rows.Count - 1
                            ProdId = ProdId + "" + dsProd.Tables(0).Rows(b).Item("ID").ToString() + ","
                        Next
                        ProdId = ProdId.Remove(ProdId.Length - 1)

                    Else
                        ProdId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                        dvTables.RowFilter = "ACTUALPRODUCTID=" + ProdId
                        dtTables = dvTables.ToTable()
                        ProdTbl(0) = dtTables.Rows(0).Item("TABLENAME").ToString()
                        RowCnt += 1
                        If PackId <> "" Then
                            dsProd = objGetData.GetPivotPackProducts(ProdId, PackId)
                        Else
                            dsProd = objGetData.GetPivotProductDescription(ProdId)
                        End If
                    End If

                    dsProd.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsProd.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsProd.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If
                ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PACKAGE" Then
                    arrRfilt(a) = "PACKAGE"
                    'If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    If filDes = "0" Then
                        If ProdId <> "" Then
                            dsPack = objGetData.GetPivotProdPackage(ProdId, "")
                        Else
                            For b = 0 To dsTables.Tables(0).Rows.Count - 1
                                ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                                ProdId = ProdId + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                                RowCnt += 1
                            Next
                            dtTables = dsTables.Tables(0)
                            ProdId = ProdId.Remove(ProdId.Length - 1)
                            If MatId <> "" Then
                                dsPack = objGetData.GetPivotMatAllPackages(ProdId, MatId)
                            Else
                                dsPack = objGetData.GetPackages(ProdId)
                            End If
                        End If
                        For b = 0 To dsPack.Tables(0).Rows.Count - 1
                            PackId = PackId + "" + dsPack.Tables(0).Rows(b).Item("PACKAGETYPEID").ToString() + ","
                        Next
                        PackId = PackId.Remove(PackId.Length - 1)
                    Else
                        PackId = filDes ' dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                        If ProdId <> "" Then
                            dsPack = objGetData.GetPivotProdPackage(ProdId, PackId)
                        Else
                            dsPack = objGetData.GetPivotPackages(PackId)
                        End If
                    End If

                    dsPack.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsPack.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsPack.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If

                ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "MATERIAL" Then
                    arrRfilt(a) = "MATERIAL"
                    If filDes = "0" Then
                        MatId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    Else
                        MatId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    End If
                    ' MatId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    If PackId <> "" Then
                        dsMat = objGetData.GetPivotPackMaterials(ProdId, PackId, MatId)
                    Else
                        dsMat = objGetData.GetPivotMaterials(MatId)
                    End If

                    dsMat.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsMat.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsMat.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If

                ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "COUNTRY" Then
                    arrRfilt(a) = "COUNTRY"
                    If filDes = "0" Then
                        CountryId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    Else
                        CountryId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    End If
                    'CountryId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    dsMat = objGetData.GetPivotCountriesByRegion(CountryId)
                    dsMat.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsMat.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsMat.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If

                ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "REGION" Then
                    arrRfilt(a) = "REGION"
                    ' RegionId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    If filDes = "0" Then
                        RegionId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    Else
                        RegionId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    End If
                    dsMat = objGetData.GetPivotRegion(RegionId)
                    dsMat.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsMat.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsMat.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If
                End If
            Next

            dv = dsRows.Tables(0).DefaultView

            Dim dv1 As New DataView
            Dim dv2 As New DataView
            Dim dv3 As New DataView
            Dim dt1 As New DataTable
            Dim dt2 As New DataTable
            Dim dt3 As New DataTable
            Dim dsRptAct As New DataSet
            Dim dvRptAct As New DataView
            Dim dtRptAct As New DataTable
            Dim dsRows1 As DataSet
            If ProdTbl(0) = "" Then
                For b = 0 To dsTables.Tables(0).Rows.Count - 1
                    ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                    RowCnt += 1
                Next
            End If

            Dim dvRpt As New DataView
            Dim dtRpt As New DataTable

            If funflag Then
                If filterCnt = 1 Then
                    dv1 = dsOrder.Tables(0).DefaultView
                ElseIf filterCnt = 2 Then
                    dv1 = dsOrder.Tables(0).DefaultView
                    dv2 = dsOrder.Tables(1).DefaultView
                ElseIf filterCnt = 3 Then
                    dv1 = dsOrder.Tables(0).DefaultView
                    dv2 = dsOrder.Tables(1).DefaultView
                    dv3 = dsOrder.Tables(2).DefaultView
                End If

                dsRows1 = objGetData.GetPivotGroupList(dsRep.Tables(0).Rows(0).Item("REGIONSETID").ToString())
                dsRptAct = objGetData.GetPivotReportData_GRP(ProdTbl, YearId, PackId, MatId, CountryId, RegionId, UnitId, dsTables.Tables(0).Rows.Count)
                dvRptAct = dsRptAct.Tables(0).DefaultView
                dvRpt = dsRptAct.Tables(0).DefaultView

                ' changes started
                If dsRptAct.Tables(0).Rows.Count > 0 Then
                    ' changes end


                    'CODE FOR GRAPH
                    Dim pcScript = ""
                    Dim odbutil As New DBUtil()
                    Dim Graphtype As String = ""
                    Dim GraphName As String = ""

                    Dim pref As String = String.Empty
                    If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                        pref = "(" + dsRows.Tables(0).Rows(0).Item("UNITSHRT").ToString() + ")"
                    Else
                        pref = "(" + dsRows.Tables(0).Rows(0).Item("METRICUNIT").ToString() + ")"
                    End If
                    Dim Count As String = dtRptAct.Rows.Count
                    Dim Str1 As String = ""

                    ' changes started
                    Dim S As String = ""
                    For i = 0 To dsCol.Tables(0).Rows.Count - 1
                        Str1 = Str1 + " "
                        If dsCol.Tables(0).Rows(i).Item("COLUMNVALUE").ToString() = "CAGR" Then

                            S = dsCol.Tables(0).Rows(i).Item("COLUMNVALUE").ToString() + "(" + dsCol.Tables(0).Rows(i).Item("INPUTVALUE1").ToString() + "/" + dsCol.Tables(0).Rows(i).Item("INPUTVALUE2").ToString() + ")"
                            Str1 = Str1 + "" + S + ";"
                        Else
                            Str1 = Str1 + dsCol.Tables(0).Rows(i).Item("COLUMNVALUE") + ";"
                        End If
                    Next

                    ' changes end

                    'For i = 0 To dsCol.Tables(0).Rows.Count - 1
                    '    Str1 = Str1 + dsCol.Tables(0).Rows(i).Item("COLUMNVALUE") + "; "
                    'Next


                    Dim Str2 As String = ""
                    For i = 0 To dsRows1.Tables(0).Rows.Count - 1
                        Str2 = Str2 + dsRows1.Tables(0).Rows(i).Item("VALUE") + "; "
                    Next
                    Dim Data As String = String.Empty
                    GraphName = "bar"
                    Graphtype = "graph"
                    'Dim Count1 As String = dtRptAct.Rows.Count
                    dvRpt = dsRptAct.Tables(0).DefaultView

                    pcScript &= "" + Graphtype + ".transposed(true)" + Graphtype + ".setCategories(" + Str1.ToString() + ")"

                    'adjective bar graph
                    Dim strt(dsCol.Tables(0).Rows.Count) As String
                    For j = 0 To dsRows1.Tables(0).Rows.Count - 1
                        For i = 0 To dsCol.Tables(0).Rows.Count - 1
                            If i = 0 Then
                                pcScript &= "" + Graphtype + ".setSeries(" & dsRows1.Tables(0).Rows(j).Item("VALUE") & ""
                            End If
                            If dsCol.Tables(0).Rows(i).Item("COLUMNVALUETYPE").ToString() <> "Formula" Then

                                dvRpt.RowFilter = " SUBGROUPID = " + dsRows1.Tables(0).Rows(j).Item("SUBGROUPID").ToString().Replace(" ", " ") + " AND YEARID= " + dsCol.Tables(0).Rows(i).Item("COLUMNVALUEID").ToString().Replace(" ", " ")
                                dtRpt = dvRpt.ToTable()
                                Dim fct As Double = 0.0
                                Data = ""
                                If dtRpt.Rows.Count > 0 Then
                                    For d = 0 To dtRpt.Rows.Count - 1
                                        fct = fct + FormatNumber(dtRpt.Rows(d).Item("FCT"), 0)
                                    Next
                                    Data = fct.ToString()
                                    'Data = FormatNumber(dtRpt.Rows(0).Item("FCT").ToString(), 0)
                                    pcScript &= ";" + Data + ""
                                    strt(i) = "" + Data.ToString()
                                Else
                                    pcScript &= ";" + fct.ToString() + ""
                                    strt(i) = "0"
                                End If


                            Else
                                Dim count1 As New Double
                                Dim count2 As New Double
                                Dim diff As New Double
                                dvRptCols1.RowFilter = "USERREPORTCOLUMNID=" + dsCol.Tables(0).Rows(i).Item("INPUTVALUETYPE1").ToString()
                                dtRptCols1 = dvRptCols1.ToTable()
                                dvRptCols2.RowFilter = "USERREPORTCOLUMNID=" + dsCol.Tables(0).Rows(i).Item("INPUTVALUETYPE2").ToString()
                                dtRptCols2 = dvRptCols2.ToTable()

                                dvRptAct.RowFilter = " SUBGROUPID = " + dsRows1.Tables(0).Rows(j).Item("SUBGROUPID").ToString() + " AND YEARID=" + dtRptCols1.Rows(0).Item("COLUMNVALUEID").ToString()
                                dtRptAct = dvRptAct.ToTable()
                                If dtRptAct.Rows.Count > 0 Then
                                    For l = 0 To dtRptAct.Rows.Count - 1
                                        If dtRptAct.Rows(l).Item("FCT").ToString() <> "" Then
                                            If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                                                If UnitId = 1 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                ElseIf UnitId = 2 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                ElseIf UnitId = 4 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                Else
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                End If
                                            Else
                                                If UnitId = 1 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT"), 0)
                                                ElseIf UnitId = 2 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL"), 0)
                                                ElseIf UnitId = 3 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA"), 0)
                                                ElseIf UnitId = 4 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                Else
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                End If
                                            End If
                                        End If
                                    Next
                                End If

                                dvRptAct.RowFilter = "SUBGROUPID = " + dsRows1.Tables(0).Rows(j).Item("SUBGROUPID").ToString() + " AND YEARID=" + dtRptCols2.Rows(0).Item("COLUMNVALUEID").ToString()
                                dtRptAct = dvRptAct.ToTable()
                                If dtRptAct.Rows.Count > 0 Then
                                    For l = 0 To dtRptAct.Rows.Count - 1
                                        If dtRptAct.Rows(l).Item("FCT").ToString() <> "" Then
                                            If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                                                If UnitId = 1 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                ElseIf UnitId = 2 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                ElseIf UnitId = 4 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                Else
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                End If
                                            Else
                                                If UnitId = 1 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT"), 0)
                                                ElseIf UnitId = 2 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL"), 0)
                                                ElseIf UnitId = 3 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA"), 0)
                                                ElseIf UnitId = 4 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                Else
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                End If
                                            End If
                                        End If
                                    Next
                                End If

                                diff = dtRptCols2.Rows(0).Item("COLUMNVALUEID") - dtRptCols1.Rows(0).Item("COLUMNVALUEID")
                                If count1 <> 0 And count2 <> 0 Then
                                    pcScript &= ";" + FormatNumber((((count2 / count1) ^ (1 / diff)) - 1) * 100, 4)
                                    Data = FormatNumber((((count2 / count1) ^ (1 / diff)) - 1) * 100, 4)
                                Else
                                    pcScript &= ";" + FormatNumber(0, 4)
                                    Data = "0"
                                End If
                                strt(i) = Data
                            End If
                        Next
                        '    pcScript &= ")"
                        '    If dtRpt.Rows.Count > 0 Then
                        '        For k = 0 To dsCol.Tables(0).Rows.Count - 1
                        '            pcScript = pcScript + "" + Graphtype + ".addHoverText(" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString()
                        '            pcScript = pcScript + "" + "," + (j + 1).ToString() + "" ' dsRows1.Tables(0).Rows(j).Item("COUNTRYDES").ToString() + ""
                        '            pcScript = pcScript + "" + ",Year:" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + "<br/>"
                        '            pcScript = pcScript + "" + "Value:" + strt(k).ToString() + ")"
                        '        Next
                        '    End If

                        'Next

                        ' changes started 
                        pcScript &= ")"
                        Dim stra As String = ""
                        Dim S1 As String = ""
                        Dim S2 As String = ""

                        For k = 0 To dsCol.Tables(0).Rows.Count - 1
                            ' pcScript = pcScript + "" + Graphtype + ".addHoverText(" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + ""

                            pcScript = pcScript + "" + Graphtype

                            If dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() = "CAGR" Then

                                S1 = dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + "(" + dsCol.Tables(0).Rows(k).Item("INPUTVALUE1").ToString() + "/" + dsCol.Tables(0).Rows(k).Item("INPUTVALUE2").ToString() + ")"

                                pcScript = pcScript + ".addHoverText(" + S1 + ""
                            Else
                                pcScript = pcScript + ".addHoverText(" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + ""

                            End If


                            pcScript = pcScript + "" + "," + (j + 1).ToString() + "" ' dsRows1.Tables(0).Rows(j).Item("COUNTRYDES").ToString() + ""

                            pcScript = pcScript + ""

                            If dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() = "CAGR" Then

                                S1 = "(" + dsCol.Tables(0).Rows(k).Item("INPUTVALUE1").ToString() + "/" + dsCol.Tables(0).Rows(k).Item("INPUTVALUE2").ToString() + ")"

                                pcScript = pcScript + "" + ",CAGR:" + S1 + "<br/>"


                            Else

                                pcScript = pcScript + "" + ",Year:" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + "<br/>"
                            End If


                            pcScript = pcScript + "" + "Value:" + strt(k).ToString() + ")"


                        Next

                    Next
                    GenrateChart(pcScript, GraphName, pref, dsRows1.Tables(0).Rows.Count * ColCnt)
                    lblNOG.Visible = False
                Else
                    MaterialPrice.Visible = False
                    lblNOG.Visible = True
                    lblNOG.Text = "Values Are Not Available for this Report."
                    GetFilters()
                End If
            Else
                MaterialPrice.Visible = False
                lblNOG.Visible = True
                lblNOG.Text = "No Data For this Combination."
                GetFilters()
            End If


            ' changes end






            '    GenrateChart(pcScript, GraphName, pref, dsRows1.Tables(0).Rows.Count * ColCnt)
            '    lblNOG.Visible = False
            'Else
            '    MaterialPrice.Visible = False
            '    lblNOG.Visible = True
            '    lblNOG.Text = "No Data For this Combination."

            'End If

        Catch ex As Exception
            ErrorLable.Text = "Error:SetPivotGrpReportFrameWork " + ex.Message.ToString()
        End Try
    End Sub

    Protected Sub SetPivotPackReportFrameWorkTemp(ByVal RptID As String)
        Dim objGetData As New Selectdata()
        Dim i As New Integer
        Dim j As New Integer
        Dim HeaderTr As New TableRow()
        Dim HeaderTd As New TableCell()
        Dim Tr As New TableRow()
        Dim Td As New TableCell()
        Dim Path As String = String.Empty
        Dim Link As HyperLink
        Dim hyd As HiddenField
        Dim dsRows As New DataSet
        Dim dsCol As New DataSet
        Dim dsFilter As New DataSet
        Dim Flag As Boolean
        Dim Des As String = String.Empty
        Dim k As Integer
        Dim tblFilter As Table
        Dim trRow As TableRow
        Dim trCol As TableCell
        Dim dv As New DataView
        Dim dvCol As New DataView
        Dim dt As New DataTable
        Dim lbl As Label
        Dim YearId As String = ""
        Dim UnitId As String = ""
        Dim dvRptCols1 As New DataView
        Dim dtRptCols1 As New DataTable
        Dim dvRptCols2 As New DataView
        Dim dtRptCols2 As New DataTable
        Dim dsUnitPref As New DataSet
        Dim dsRep As New DataSet
        Dim RowCnt As New Integer
        Dim ColCnt As New Integer
        Dim filterCnt As New Integer
        Dim GrpId As String = ""
        Dim dsGrp As New DataSet
        Dim imgUp As New ImageButton
        Dim imgDn As New ImageButton
        Dim CompId As String = ""
        Dim dsComp As New DataSet
        Try
            If hidReport.Value <> "0" Then
                dsRep = Session("dsRep" + REPId.ToString())
                dsUnitPref = Session("dsPref" + REPId.ToString())
                dsRows = Session("dsRows" + REPId.ToString())
                dsCol = Session("dsColumns" + REPId.ToString())
                dsFilter = Session("dsFilters" + REPId.ToString())
            Else
                dsRep = objGetData.GetReportDetails(REPId.ToString())
                dsUnitPref = objGetData.GetPref(REPId.ToString())
                dsRows = objGetData.GetUsersDynamicReportRows(REPId.ToString())
                dsCol = objGetData.GetUsersDynamicReportCols(REPId.ToString())
                dsFilter = objGetData.GetUsersReportFilters(REPId.ToString())

                Session("dsRep" + REPId.ToString()) = dsRep
                Session("dsPref" + REPId.ToString()) = dsUnitPref
                Session("dsRows" + REPId.ToString()) = dsRows
                Session("dsColumns" + REPId.ToString()) = dsCol
                Session("dsFilters" + REPId.ToString()) = dsFilter

                hidReport.Value = "1"
            End If

            filterCnt = dsFilter.Tables(0).Rows.Count
            ColCnt = dsCol.Tables(0).Rows.Count
            For a = 0 To dsCol.Tables(0).Rows.Count - 1
                If dsCol.Tables(0).Rows(a).Item("COLUMNVALUETYPE").ToString() <> "Formula" Then
                    YearId = YearId + "" + dsCol.Tables(0).Rows(a).Item("COLUMNVALUEID").ToString() + ","
                End If
            Next
            YearId = YearId.Remove(YearId.Length - 1)
            UnitId = dsRows.Tables(0).Rows(0).Item("UNITID").ToString()

            dvCol = dsCol.Tables(0).DefaultView
            dvRptCols1 = dsCol.Tables(0).DefaultView
            dvRptCols2 = dsCol.Tables(0).DefaultView

            Dim dsTables As New DataSet
            Dim dvTables As New DataView
            Dim dtTables As New DataTable
            Dim ProdId As String = ""
            Dim PackId As String = ""
            Dim dsProd As New DataSet
            Dim dvProd As New DataView
            Dim dtProd As New DataTable
            Dim dsPack As New DataSet
            Dim dvPack As New DataView
            Dim dtPack As New DataTable
            Dim dsOrder As New DataSet()
            Dim funflag As Boolean = False
            Dim MatId As String = ""
            Dim FactId As String = ""
            Dim dsMat As New DataSet
            Dim dsProdMat As New DataSet
            Dim arrRfilt(filterCnt) As String
            Dim CountryId As String = ""
            Dim RegionId As String = ""
            Dim filDes As String
            dsTables = objGetData.GetProductDetails(Session("M1SubGroupId"))
            dvTables = dsTables.Tables(0).DefaultView
            Dim ProdTbl(dsTables.Tables(0).Rows.Count) As String

            For a = 0 To dsFilter.Tables(0).Rows.Count - 1
                filDes = Request.Form("ddlfil_" + a.ToString())
                If filDes <> Nothing Then
                    'filDes = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    Fils(a) = filDes.ToString()
                Else
                    filDes = "0"
                    Fils(a) = "0"
                    If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() <> "0" Then
                        filDes = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    End If
                End If
                If dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PRODUCT" Then
                    arrRfilt(a) = "PRODUCT"
                    'If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    If filDes = "0" Then
                        For b = 0 To dsTables.Tables(0).Rows.Count - 1
                            ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                            ProdId = ProdId + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                            RowCnt += 1
                        Next
                        dtTables = dsTables.Tables(0)
                        ProdId = ProdId.Remove(ProdId.Length - 1)
                        If MatId <> "" Then
                            dsProd = objGetData.GetPivotMatProducts(MatId, ProdId)
                        ElseIf CompId <> "" Then
                            dsProd = objGetData.GetPivotCompProducts(CompId, ProdId)
                        Else
                            dsProd = objGetData.GetPivotProductDescription(ProdId)
                        End If

                        ProdId = ""
                        For b = 0 To dsProd.Tables(0).Rows.Count - 1
                            ProdId = ProdId + "" + dsProd.Tables(0).Rows(b).Item("ID").ToString() + ","
                        Next
                        ProdId = ProdId.Remove(ProdId.Length - 1)

                    Else
                        ProdId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                        dvTables.RowFilter = "ACTUALPRODUCTID=" + ProdId
                        dtTables = dvTables.ToTable()
                        ProdTbl(0) = dtTables.Rows(0).Item("TABLENAME").ToString()
                        RowCnt += 1
                        If MatId <> "" Then
                            dsProd = objGetData.GetPivotMatProducts(MatId, ProdId)
                        ElseIf CompId <> "" Then
                            dsProd = objGetData.GetPivotCompProducts(CompId, ProdId)
                        Else
                            dsProd = objGetData.GetPivotProductDescription(ProdId)
                        End If
                    End If

                    dsProd.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsProd.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsProd.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If
                ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "MATERIAL" Then
                    arrRfilt(a) = "MATERIAL"
                    'MatId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    If filDes = "0" Then
                        MatId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    Else
                        MatId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    End If
                    If ProdId <> "" Then
                        dsMat = objGetData.GetPivotProdMaterials(ProdId, MatId)
                    Else
                        dsMat = objGetData.GetPivotMaterials(MatId)
                    End If

                    dsMat.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsMat.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsMat.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If
                ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "COMPONENT" Then
                    arrRfilt(a) = "COMPONENT"
                    ' If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    If filDes = "0" Then
                        If ProdId <> "" Then
                            dsComp = objGetData.GetPivotProdComponents(ProdId, "")

                        Else
                            Dim ProdId1 As String = ""
                            For b = 0 To dsTables.Tables(0).Rows.Count - 1
                                ProdId1 = ProdId1 + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                            Next
                            ProdId1 = ProdId1.Remove(ProdId1.Length - 1)
                            dsComp = objGetData.GetPivotAllComponents(ProdId1)
                        End If
                        For b = 0 To dsComp.Tables(0).Rows.Count - 1
                            CompId = CompId + "" + dsComp.Tables(0).Rows(b).Item("COMPONENTID").ToString() + ","
                        Next
                        CompId = CompId.Remove(CompId.Length - 1)
                    Else
                        CompId = filDes ' dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                        If ProdId <> "" Then
                            dsComp = objGetData.GetPivotProdComponents(ProdId, CompId)
                        Else
                            dsComp = objGetData.GetRepComponent(CompId)
                        End If
                    End If

                    dsComp.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsComp.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsComp.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If
                ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "COUNTRY" Then
                    arrRfilt(a) = "COUNTRY"
                    'CountryId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    If filDes = "0" Then
                        CountryId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    Else
                        CountryId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    End If
                    dsMat = objGetData.GetPivotCountriesByRegion(CountryId)
                    dsMat.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsMat.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsMat.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If

                ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "REGION" Then
                    arrRfilt(a) = "REGION"
                    'RegionId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    If filDes = "0" Then
                        RegionId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    Else
                        RegionId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    End If
                    dsMat = objGetData.GetPivotRegion(RegionId)
                    dsMat.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsMat.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsMat.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If

                ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "GROUP" Then
                    arrRfilt(a) = "GROUP"
                    ' GrpId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    If filDes = "0" Then
                        GrpId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    Else
                        GrpId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    End If
                    dsGrp = objGetData.GetPivotGroups(GrpId)
                    dsGrp.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsGrp.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsGrp.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If
                End If
            Next

            dv = dsRows.Tables(0).DefaultView

            Dim dv1 As New DataView
            Dim dv2 As New DataView
            Dim dv3 As New DataView
            Dim dt1 As New DataTable
            Dim dt2 As New DataTable
            Dim dt3 As New DataTable
            Dim dsRptAct As New DataSet
            Dim dvRptAct As New DataView
            Dim dtRptAct As New DataTable
            Dim dsRows1 As DataSet
            Dim strRFilt1 As String = ""
            Dim strRFilt2 As String = ""
            Dim strRFilt3 As String = ""

            If ProdTbl(0) = "" Then
                For b = 0 To dsTables.Tables(0).Rows.Count - 1
                    ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                    ProdId = ProdId + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                    RowCnt += 1
                Next
                ProdId = ProdId.Remove(ProdId.Length - 1)
            End If

            Dim dvRpt As New DataView
            Dim dtRpt As New DataTable

            If funflag Then
                If filterCnt = 1 Then
                    dv1 = dsOrder.Tables(0).DefaultView
                ElseIf filterCnt = 2 Then
                    dv1 = dsOrder.Tables(0).DefaultView
                    dv2 = dsOrder.Tables(1).DefaultView
                ElseIf filterCnt = 3 Then
                    dv1 = dsOrder.Tables(0).DefaultView
                    dv2 = dsOrder.Tables(1).DefaultView
                    dv3 = dsOrder.Tables(2).DefaultView
                End If

                If CompId <> "" Then
                    dsRows1 = objGetData.GetPivotComponentsPackages(ProdId, CompId)
                Else
                    dsRows1 = objGetData.GetPivotPackages(ProdId, MatId)
                End If

                dsRptAct = objGetData.GetPivotReportData_PACK(ProdTbl, YearId, ProdId, MatId, GrpId, CompId, CountryId, RegionId, UnitId, dsTables.Tables(0).Rows.Count)
                dvRptAct = dsRptAct.Tables(0).DefaultView
                dvRpt = dsRptAct.Tables(0).DefaultView

                ' Changes Started 
                If dsRptAct.Tables(0).Rows.Count > 0 Then
                    ' Changes end

                    'CODE FOR GRAPH
                    Dim pcScript = ""
                    Dim odbutil As New DBUtil()
                    Dim Graphtype As String = ""
                    Dim GraphName As String = ""

                    Dim pref As String = String.Empty
                    If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                        pref = "(" + dsRows.Tables(0).Rows(0).Item("UNITSHRT").ToString() + ")"
                    Else
                        pref = "(" + dsRows.Tables(0).Rows(0).Item("METRICUNIT").ToString() + ")"
                    End If
                    Dim Count As String = dtRptAct.Rows.Count
                    Dim Str1 As String = ""
                    'For i = 0 To dsCol.Tables(0).Rows.Count - 1
                    '    Str1 = Str1 + dsCol.Tables(0).Rows(i).Item("COLUMNVALUE") + "; "
                    'Next
                    ' Changes Started 
                    Dim S As String = ""

                    For i = 0 To dsCol.Tables(0).Rows.Count - 1
                        Str1 = Str1 + " "
                        If dsCol.Tables(0).Rows(i).Item("COLUMNVALUE").ToString() = "CAGR" Then

                            S = dsCol.Tables(0).Rows(i).Item("COLUMNVALUE").ToString() + "(" + dsCol.Tables(0).Rows(i).Item("INPUTVALUE1").ToString() + "/" + dsCol.Tables(0).Rows(i).Item("INPUTVALUE2").ToString() + ")"
                            Str1 = Str1 + "" + S + ";"
                        Else
                            Str1 = Str1 + dsCol.Tables(0).Rows(i).Item("COLUMNVALUE") + ";"
                        End If

                    Next
                    '  Changes end




                    Dim Str2 As String = ""
                    For i = 0 To dsRows1.Tables(0).Rows.Count - 1
                        Str2 = Str2 + dsRows1.Tables(0).Rows(i).Item("VALUE") + "; "
                    Next
                    Dim Data As String = String.Empty
                    GraphName = "bar"
                    Graphtype = "graph"
                    'Dim Count1 As String = dtRptAct.Rows.Count
                    dvRpt = dsRptAct.Tables(0).DefaultView

                    pcScript &= "" + Graphtype + ".transposed(true)" + Graphtype + ".setCategories(" + Str1.ToString() + ")"

                    'double bar
                    Dim strt(dsCol.Tables(0).Rows.Count) As String
                    For j = 0 To dsRows1.Tables(0).Rows.Count - 1
                        For i = 0 To dsCol.Tables(0).Rows.Count - 1
                            If i = 0 Then
                                pcScript &= "" + Graphtype + ".setSeries(" & dsRows1.Tables(0).Rows(j).Item("VALUE") & ""
                            End If
                            If dsCol.Tables(0).Rows(i).Item("COLUMNVALUETYPE").ToString() <> "Formula" Then

                                dvRpt.RowFilter = " PACKAGETYPEID = " + dsRows1.Tables(0).Rows(j).Item("PACKAGETYPEID").ToString().Replace(" ", " ") + " AND YEARID= " + dsCol.Tables(0).Rows(i).Item("COLUMNVALUEID").ToString().Replace(" ", " ")
                                dtRpt = dvRpt.ToTable()
                                Dim fct As Double = 0.0
                                Data = ""
                                If dtRpt.Rows.Count > 0 Then
                                    For d = 0 To dtRpt.Rows.Count - 1
                                        fct = fct + FormatNumber(dtRpt.Rows(d).Item("FCT"), 0)
                                    Next
                                    Data = fct.ToString()
                                    'Data = FormatNumber(dtRpt.Rows(0).Item("FCT").ToString(), 0)
                                    pcScript &= ";" + Data + ""
                                    strt(i) = "" + Data.ToString()
                                Else
                                    pcScript &= ";" + fct.ToString() + ""
                                    strt(i) = "0"
                                End If
                            Else
                                Dim count1 As New Double
                                Dim count2 As New Double
                                Dim diff As New Double
                                dvRptCols1.RowFilter = "USERREPORTCOLUMNID=" + dsCol.Tables(0).Rows(i).Item("INPUTVALUETYPE1").ToString()
                                dtRptCols1 = dvRptCols1.ToTable()
                                dvRptCols2.RowFilter = "USERREPORTCOLUMNID=" + dsCol.Tables(0).Rows(i).Item("INPUTVALUETYPE2").ToString()
                                dtRptCols2 = dvRptCols2.ToTable()

                                dvRptAct.RowFilter = " PACKAGETYPEID = " + dsRows1.Tables(0).Rows(j).Item("PACKAGETYPEID").ToString() + " AND YEARID=" + dtRptCols1.Rows(0).Item("COLUMNVALUEID").ToString()
                                dtRptAct = dvRptAct.ToTable()
                                If dtRptAct.Rows.Count > 0 Then
                                    For l = 0 To dtRptAct.Rows.Count - 1
                                        If dtRptAct.Rows(l).Item("FCT").ToString() <> "" Then
                                            If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                                                If UnitId = 1 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                ElseIf UnitId = 2 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                ElseIf UnitId = 4 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                Else
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                End If
                                            Else
                                                If UnitId = 1 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT"), 0)
                                                ElseIf UnitId = 2 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL"), 0)
                                                ElseIf UnitId = 3 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA"), 0)
                                                ElseIf UnitId = 4 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                Else
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                End If
                                            End If
                                        End If
                                    Next
                                End If

                                dvRptAct.RowFilter = " PACKAGETYPEID = " + dsRows1.Tables(0).Rows(j).Item("PACKAGETYPEID").ToString() + " AND YEARID=" + dtRptCols2.Rows(0).Item("COLUMNVALUEID").ToString()
                                dtRptAct = dvRptAct.ToTable()
                                If dtRptAct.Rows.Count > 0 Then
                                    For l = 0 To dtRptAct.Rows.Count - 1
                                        If dtRptAct.Rows(l).Item("FCT").ToString() <> "" Then
                                            If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                                                If UnitId = 1 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                ElseIf UnitId = 2 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                ElseIf UnitId = 4 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                Else
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                End If
                                            Else
                                                If UnitId = 1 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT"), 0)
                                                ElseIf UnitId = 2 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL"), 0)
                                                ElseIf UnitId = 3 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA"), 0)
                                                ElseIf UnitId = 4 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                Else
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                End If
                                            End If
                                        End If
                                    Next
                                End If
                                diff = dtRptCols2.Rows(0).Item("COLUMNVALUEID") - dtRptCols1.Rows(0).Item("COLUMNVALUEID")
                                If count1 <> 0 And count2 <> 0 Then
                                    pcScript &= ";" + FormatNumber((((count2 / count1) ^ (1 / diff)) - 1) * 100, 4)
                                    Data = FormatNumber((((count2 / count1) ^ (1 / diff)) - 1) * 100, 4)
                                Else
                                    pcScript &= ";" + FormatNumber(0, 4)
                                    Data = "0"
                                End If
                                strt(i) = Data
                            End If
                        Next
                        '        pcScript &= ")"
                        '        For k = 0 To dsCol.Tables(0).Rows.Count - 1
                        '            pcScript = pcScript + "" + Graphtype + ".addHoverText(" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString()
                        '            pcScript = pcScript + "" + "," + (j + 1).ToString() + "" ' dsRows1.Tables(0).Rows(j).Item("COUNTRYDES").ToString() + ""
                        '            pcScript = pcScript + "" + ",Year:" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + "<br/>"
                        '            pcScript = pcScript + "" + "Value:" + strt(k).ToString() + ")"
                        '        Next
                        '    Next
                        '    GenrateChart(pcScript, GraphName, pref, dsRows1.Tables(0).Rows.Count * ColCnt)
                        '    lblNOG.Visible = False
                        'Else
                        '    MaterialPrice.Visible = False
                        '    lblNOG.Visible = True
                        '    lblNOG.Text = "No Data For this Combination."
                        'End If



                        '  Changes Started 
                        pcScript &= ")"
                        Dim stra As String = ""
                        Dim S1 As String = ""
                        Dim S2 As String = ""

                        For k = 0 To dsCol.Tables(0).Rows.Count - 1
                            ' pcScript = pcScript + "" + Graphtype + ".addHoverText(" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + ""

                            pcScript = pcScript + "" + Graphtype

                            If dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() = "CAGR" Then

                                S1 = dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + "(" + dsCol.Tables(0).Rows(k).Item("INPUTVALUE1").ToString() + "/" + dsCol.Tables(0).Rows(k).Item("INPUTVALUE2").ToString() + ")"

                                pcScript = pcScript + ".addHoverText(" + S1 + ""
                            Else
                                pcScript = pcScript + ".addHoverText(" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + ""

                            End If


                            pcScript = pcScript + "" + "," + (j + 1).ToString() + "" ' dsRows1.Tables(0).Rows(j).Item("COUNTRYDES").ToString() + ""

                            pcScript = pcScript + ""

                            If dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() = "CAGR" Then

                                S1 = "(" + dsCol.Tables(0).Rows(k).Item("INPUTVALUE1").ToString() + "/" + dsCol.Tables(0).Rows(k).Item("INPUTVALUE2").ToString() + ")"

                                pcScript = pcScript + "" + ",CAGR:" + S1 + "<br/>"


                            Else

                                pcScript = pcScript + "" + ",Year:" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + "<br/>"
                            End If


                            pcScript = pcScript + "" + "Value:" + strt(k).ToString() + ")"


                        Next

                    Next
                    GenrateChart(pcScript, GraphName, pref, dsRows1.Tables(0).Rows.Count * ColCnt)
                    lblNOG.Visible = False
                Else
                    MaterialPrice.Visible = False
                    lblNOG.Visible = True
                    lblNOG.Text = "Values Are Not Available for this Report."
                    GetFilters()
                End If
            Else
                MaterialPrice.Visible = False
                lblNOG.Visible = True
                lblNOG.Text = "No Data For this Combination."
                GetFilters()
            End If
            '  Changes ends

        Catch ex As Exception
            ErrorLable.Text = "Error:SetPivotPackReportFrameWorkTemp " + ex.Message.ToString()
        End Try
    End Sub

    Protected Sub SetPivotCntryReportFrameWorkTemp(ByVal RptID As String)
        Dim objGetData As New Selectdata()
        Dim i As New Integer
        Dim j As New Integer
        Dim HeaderTr As New TableRow()
        Dim HeaderTd As New TableCell()
        Dim Tr As New TableRow()
        Dim Td As New TableCell()
        Dim Path As String = String.Empty
        Dim Link As HyperLink
        Dim hyd As HiddenField
        Dim dsRows As New DataSet
        Dim dsCol As New DataSet
        Dim dsFilter As New DataSet
        Dim Flag As Boolean
        Dim Des As String = String.Empty
        Dim k As Integer
        Dim tblFilter As Table
        Dim trRow As TableRow
        Dim trCol As TableCell
        Dim dv As New DataView
        Dim dvCol As New DataView
        Dim dt As New DataTable
        Dim lbl As Label
        Dim YearId As String = ""
        Dim UnitId As String = ""
        Dim RegionId As String = ""
        Dim dvRptCols1 As New DataView
        Dim dtRptCols1 As New DataTable
        Dim dvRptCols2 As New DataView
        Dim dtRptCols2 As New DataTable
        Dim dsUnitPref As New DataSet
        Dim dsRep As New DataSet
        Dim RowCnt As New Integer
        Dim ColCnt As New Integer
        Dim filterCnt As New Integer
        Dim imgUp As New ImageButton
        Dim imgDn As New ImageButton
        Dim CompId As String = ""
        Dim dsComp As New DataSet
        Try
            If hidReport.Value <> "0" Then
                dsRep = Session("dsRep" + REPId.ToString())
                dsUnitPref = Session("dsPref" + REPId.ToString())
                dsRows = Session("dsRows" + REPId.ToString())
                dsCol = Session("dsColumns" + REPId.ToString())
                dsFilter = Session("dsFilters" + REPId.ToString())
            Else
                dsRep = objGetData.GetUserCustomReportsByRptId(REPId.ToString())
                dsUnitPref = objGetData.GetPref(REPId.ToString())
                dsRows = objGetData.GetUsersDynamicReportRows(REPId.ToString())
                dsCol = objGetData.GetUsersDynamicReportCols(REPId.ToString())
                dsFilter = objGetData.GetUsersReportFilters(REPId.ToString())

                Session("dsRep" + REPId.ToString()) = dsRep
                Session("dsPref" + REPId.ToString()) = dsUnitPref
                Session("dsRows" + REPId.ToString()) = dsRows
                Session("dsColumns" + REPId.ToString()) = dsCol
                Session("dsFilters" + REPId.ToString()) = dsFilter

                hidReport.Value = "1"
            End If

            filterCnt = dsFilter.Tables(0).Rows.Count
            ColCnt = dsCol.Tables(0).Rows.Count

            For a = 0 To dsCol.Tables(0).Rows.Count - 1
                If dsCol.Tables(0).Rows(a).Item("COLUMNVALUETYPE").ToString() <> "Formula" Then
                    YearId = YearId + "" + dsCol.Tables(0).Rows(a).Item("COLUMNVALUEID").ToString() + ","
                End If
            Next
            YearId = YearId.Remove(YearId.Length - 1)
            RegionId = dsRep.Tables(0).Rows(0).Item("REGIONID").ToString()
            UnitId = dsRows.Tables(0).Rows(0).Item("UNITID").ToString()

            dvCol = dsCol.Tables(0).DefaultView
            dvRptCols1 = dsCol.Tables(0).DefaultView
            dvRptCols2 = dsCol.Tables(0).DefaultView

            Dim dsTables As New DataSet
            Dim dvTables As New DataView
            Dim dtTables As New DataTable
            Dim ProdId As String = ""
            Dim PackId As String = ""
            Dim dsProd As New DataSet
            Dim dvProd As New DataView
            Dim dtProd As New DataTable
            Dim dsPack As New DataSet
            Dim dvPack As New DataView
            Dim dtPack As New DataTable
            Dim dsOrder As New DataSet()
            Dim funflag As Boolean = False
            Dim MatId As String = ""
            Dim GrpId As String = ""
            Dim FactId As String = ""
            Dim dsMat As New DataSet
            Dim dsGrp As New DataSet
            Dim dsProdMat As New DataSet
            Dim arrRfilt(filterCnt) As String
            Dim filDes As String
            dsTables = objGetData.GetProductDetails(Session("M1SubGroupId"))
            dvTables = dsTables.Tables(0).DefaultView
            Dim ProdTbl(dsTables.Tables(0).Rows.Count) As String

            For a = 0 To dsFilter.Tables(0).Rows.Count - 1
                filDes = Request.Form("ddlfil_" + a.ToString())
                If filDes <> Nothing Then
                    'filDes = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    Fils(a) = filDes.ToString()
                Else
                    filDes = "0"
                    Fils(a) = "0"
                    If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() <> "0" Then
                        filDes = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    End If
                End If
                If dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PRODUCT" Then
                    arrRfilt(a) = "PRODUCT"

                    'If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    If filDes = "0" Then
                        For b = 0 To dsTables.Tables(0).Rows.Count - 1
                            ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                            ProdId = ProdId + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                            RowCnt += 1
                        Next
                        dtTables = dsTables.Tables(0)
                        ProdId = ProdId.Remove(ProdId.Length - 1)
                        If MatId <> "" And PackId <> "" Then
                            dsProd = objGetData.GetPivotPackMatProducts(ProdId, PackId, MatId)
                        ElseIf CompId <> "" And PackId <> "" Then
                            dsProd = objGetData.GetPivotPackCompProducts(ProdId, PackId, CompId)
                        ElseIf MatId <> "" Then
                            dsProd = objGetData.GetPivotMatProducts(MatId, ProdId)
                        ElseIf CompId <> "" Then
                            dsProd = objGetData.GetPivotCompProducts(CompId, ProdId)
                        ElseIf PackId <> "" Then
                            dsProd = objGetData.GetPivotPackProducts(ProdId, PackId)
                        Else
                            dsProd = objGetData.GetPivotProductDescription(ProdId)
                        End If
                        ProdId = ""
                        For b = 0 To dsProd.Tables(0).Rows.Count - 1
                            ProdId = ProdId + "" + dsProd.Tables(0).Rows(b).Item("ID").ToString() + ","
                        Next
                        ProdId = ProdId.Remove(ProdId.Length - 1)
                    Else
                        ProdId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                        dvTables.RowFilter = "ACTUALPRODUCTID=" + ProdId
                        dtTables = dvTables.ToTable()
                        ProdTbl(0) = dtTables.Rows(0).Item("TABLENAME").ToString()
                        RowCnt += 1
                        If MatId <> "" And PackId <> "" Then
                            dsProd = objGetData.GetPivotPackMatProducts(ProdId, PackId, MatId)
                        ElseIf CompId <> "" And PackId <> "" Then
                            dsProd = objGetData.GetPivotPackCompProducts(ProdId, PackId, CompId)
                        ElseIf MatId <> "" Then
                            dsProd = objGetData.GetPivotMatProducts(MatId, ProdId)
                        ElseIf CompId <> "" Then
                            dsProd = objGetData.GetPivotCompProducts(CompId, ProdId)
                        ElseIf PackId <> "" Then
                            dsProd = objGetData.GetPivotPackProducts(ProdId, PackId)
                        Else
                            dsProd = objGetData.GetPivotProductDescription(ProdId)
                        End If
                    End If
                    dsProd.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsProd.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsProd.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If
                ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PACKAGE" Then
                    arrRfilt(a) = "PACKAGE"

                    ' If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    If filDes = "0" Then
                        If ProdId <> "" And MatId <> "" Then
                            dsPack = objGetData.GetPivotProdMatPackages(ProdId, MatId, "")
                        ElseIf ProdId <> "" And CompId <> "" Then
                            dsPack = objGetData.GetProdCompPackages(ProdId, CompId, "")
                        ElseIf ProdId <> "" Then
                            dsPack = objGetData.GetPivotProdPackage(ProdId, "")
                        Else
                            Dim ProdId1 As String = ""
                            For b = 0 To dsTables.Tables(0).Rows.Count - 1
                                ProdId1 = ProdId1 + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                            Next
                            ProdId1 = ProdId1.Remove(ProdId1.Length - 1)
                            If MatId <> "" Then
                                dsPack = objGetData.GetPivotMatAllPackages(ProdId1, MatId)
                            ElseIf CompId <> "" Then
                                dsPack = objGetData.GetPivotCompAllPackages(ProdId1, CompId)
                            Else
                                dsPack = objGetData.GetPackages(ProdId1)
                            End If
                        End If
                        For b = 0 To dsPack.Tables(0).Rows.Count - 1
                            PackId = PackId + "" + dsPack.Tables(0).Rows(b).Item("PACKAGETYPEID").ToString() + ","
                        Next
                        PackId = PackId.Remove(PackId.Length - 1)
                    Else
                        PackId = filDes ' dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                        If ProdId <> "" And MatId <> "" Then
                            dsPack = objGetData.GetPivotProdMatPackages(ProdId, MatId, PackId)
                        ElseIf ProdId <> "" And CompId <> "" Then
                            dsPack = objGetData.GetProdCompPackages(ProdId, CompId, PackId)
                        ElseIf ProdId <> "" Then
                            dsPack = objGetData.GetPivotProdPackage(ProdId, PackId)
                        ElseIf MatId <> "" Then
                            dsPack = objGetData.GetPivotMatPackages(MatId, PackId)
                        ElseIf CompId <> "" Then
                            dsPack = objGetData.GetPivotCompPackages(PackId, CompId)
                        Else
                            dsPack = objGetData.GetPivotPackages(PackId)
                        End If
                    End If
                    dsPack.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsPack.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsPack.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If
                ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "MATERIAL" Then
                    arrRfilt(a) = "MATERIAL"
                    ' MatId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    If filDes = "0" Then
                        MatId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    Else
                        MatId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    End If
                    If ProdId <> "" And PackId <> "" Then
                        dsMat = objGetData.GetPivotPackProdMaterials(ProdId, PackId, MatId)
                    ElseIf ProdId <> "" Then
                        dsMat = objGetData.GetPivotProdMaterials(ProdId, MatId)
                    ElseIf PackId <> "" Then
                        Dim ProdId1 As String = ""
                        For b = 0 To dsTables.Tables(0).Rows.Count - 1
                            ProdId1 = ProdId1 + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                        Next
                        ProdId1 = ProdId1.Remove(ProdId1.Length - 1)
                        dsMat = objGetData.GetPivotPackMaterials(ProdId1, PackId, MatId)
                    Else
                        dsMat = objGetData.GetPivotMaterials(MatId)
                    End If
                    dsMat.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsMat.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsMat.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If
                ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "COMPONENT" Then
                    arrRfilt(a) = "COMPONENT"

                    'If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    If filDes = "0" Then

                        If ProdId <> "" And PackId <> "" Then
                            dsComp = objGetData.GetPivotPackProdComponents(ProdId, PackId, "")

                        ElseIf ProdId <> "" Then
                            dsComp = objGetData.GetPivotProdComponents(ProdId, "")

                        Else
                            Dim ProdId1 As String = ""
                            For b = 0 To dsTables.Tables(0).Rows.Count - 1
                                ProdId1 = ProdId1 + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                            Next
                            ProdId1 = ProdId1.Remove(ProdId1.Length - 1)
                            If PackId <> "" Then
                                dsComp = objGetData.GetPivotPackComponents(ProdId1, PackId, "")
                            Else
                                dsComp = objGetData.GetPivotAllComponents(ProdId1)
                            End If
                        End If
                        For b = 0 To dsComp.Tables(0).Rows.Count - 1
                            CompId = CompId + "" + dsComp.Tables(0).Rows(b).Item("COMPONENTID").ToString() + ","
                        Next
                        CompId = CompId.Remove(CompId.Length - 1)
                    Else
                        CompId = filDes ' dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                        If ProdId <> "" And PackId <> "" Then
                            dsComp = objGetData.GetPivotPackProdComponents(ProdId, PackId, CompId)

                        ElseIf ProdId <> "" Then
                            dsComp = objGetData.GetPivotProdComponents(ProdId, CompId)

                        ElseIf PackId <> "" Then
                            Dim ProdId1 As String = ""
                            For b = 0 To dsTables.Tables(0).Rows.Count - 1
                                ProdId1 = ProdId1 + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                            Next
                            ProdId1 = ProdId1.Remove(ProdId1.Length - 1)
                            dsComp = objGetData.GetPivotPackComponents(ProdId1, PackId, CompId)

                        Else
                            dsComp = objGetData.GetRepComponent(CompId)
                        End If
                    End If

                    dsComp.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsComp.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsComp.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If

                ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "GROUP" Then
                    arrRfilt(a) = "GROUP"
                    'GrpId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    If filDes = "0" Then
                        GrpId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    Else
                        GrpId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    End If
                    dsGrp = objGetData.GetPivotGroups(GrpId)
                    dsGrp.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsGrp.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsGrp.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If
                End If
            Next

            dv = dsRows.Tables(0).DefaultView

            Dim dv1 As New DataView
            Dim dv2 As New DataView
            Dim dv3 As New DataView
            Dim dt1 As New DataTable
            Dim dt2 As New DataTable
            Dim dt3 As New DataTable
            Dim dsRptAct As New DataSet
            Dim dvRptAct As New DataView
            Dim dtRptAct As New DataTable
            Dim strRFilt1 As String = ""
            Dim strRFilt2 As String = ""
            Dim strRFilt3 As String = ""
            Dim dsRows1 As DataSet

            If ProdTbl(0) = "" Then
                For b = 0 To dsTables.Tables(0).Rows.Count - 1
                    ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                    RowCnt += 1
                Next
            End If

            Dim dvRpt As New DataView
            Dim dtRpt As New DataTable

            If funflag Then
                If filterCnt = 1 Then
                    dv1 = dsOrder.Tables(0).DefaultView
                ElseIf filterCnt = 2 Then
                    dv1 = dsOrder.Tables(0).DefaultView
                    dv2 = dsOrder.Tables(1).DefaultView
                ElseIf filterCnt = 3 Then
                    dv1 = dsOrder.Tables(0).DefaultView
                    dv2 = dsOrder.Tables(1).DefaultView
                    dv3 = dsOrder.Tables(2).DefaultView
                End If

                dsRows1 = objGetData.GetPivotCountry(RegionId)
                dsRptAct = objGetData.GetPivotReportData_COUNTRY(ProdTbl, YearId, ProdId, PackId, MatId, GrpId, CompId, RegionId, UnitId, dsTables.Tables(0).Rows.Count)
                dvRptAct = dsRptAct.Tables(0).DefaultView
                dvRpt = dsRptAct.Tables(0).DefaultView

                ' changes Startd 
                If dsRptAct.Tables(0).Rows.Count > 0 Then
                    ' changes ends

                    'CODE FOR GRAPH
                    Dim pcScript = ""
                    Dim odbutil As New DBUtil()
                    Dim Graphtype As String = ""
                    Dim GraphName As String = ""

                    Dim pref As String = String.Empty
                    If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                        pref = "(" + dsRows.Tables(0).Rows(0).Item("UNITSHRT").ToString() + ")"
                    Else
                        pref = "(" + dsRows.Tables(0).Rows(0).Item("METRICUNIT").ToString() + ")"
                    End If
                    Dim Count As String = dtRptAct.Rows.Count
                    Dim Str1 As String = ""
                    'For i = 0 To dsCol.Tables(0).Rows.Count - 1
                    '    Str1 = Str1 + dsCol.Tables(0).Rows(i).Item("COLUMNVALUE") + "; "
                    'Next

                    '  Changes Started 
                    Dim S As String = ""

                    For i = 0 To dsCol.Tables(0).Rows.Count - 1
                        Str1 = Str1 + " "
                        If dsCol.Tables(0).Rows(i).Item("COLUMNVALUE").ToString() = "CAGR" Then

                            S = dsCol.Tables(0).Rows(i).Item("COLUMNVALUE").ToString() + "(" + dsCol.Tables(0).Rows(i).Item("INPUTVALUE1").ToString() + "/" + dsCol.Tables(0).Rows(i).Item("INPUTVALUE2").ToString() + ")"
                            Str1 = Str1 + "" + S + ";"
                        Else
                            Str1 = Str1 + dsCol.Tables(0).Rows(i).Item("COLUMNVALUE") + ";"
                        End If

                    Next
                    '  Changes end


                    Dim Str2 As String = ""
                    For i = 0 To dsRows1.Tables(0).Rows.Count - 1
                        Str2 = Str2 + dsRows1.Tables(0).Rows(i).Item("COUNTRYDES") + "; "
                    Next
                    Dim Data As String = String.Empty
                    GraphName = "bar"
                    Graphtype = "graph"
                    'GraphName = "Sargento_KraftPouchvsTray"
                    'Graphtype = "graph"
                    'Dim Count1 As String = dtRptAct.Rows.Count
                    dvRpt = dsRptAct.Tables(0).DefaultView

                    pcScript &= "" + Graphtype + ".transposed(true)" + Graphtype + ".setCategories(" + Str1.ToString() + ")"

                    'double bar
                    Dim strt(dsCol.Tables(0).Rows.Count) As String
                    For j = 0 To dsRows1.Tables(0).Rows.Count - 1
                        For i = 0 To dsCol.Tables(0).Rows.Count - 1
                            If i = 0 Then
                                pcScript &= "" + Graphtype + ".setSeries(" & dsRows1.Tables(0).Rows(j).Item("COUNTRYDES") & ""
                            End If
                            If dsCol.Tables(0).Rows(i).Item("COLUMNVALUETYPE").ToString() <> "Formula" Then
                                dvRpt.RowFilter = " COUNTRYID = " + dsRows1.Tables(0).Rows(j).Item("COUNTRYID").ToString().Replace(" ", " ") + " AND YEARID= " + dsCol.Tables(0).Rows(i).Item("COLUMNVALUEID").ToString().Replace(" ", " ")
                                dtRpt = dvRpt.ToTable()
                                Dim fct As Double = 0.0
                                Data = ""

                                If dtRpt.Rows.Count > 0 Then

                                    For d = 0 To dtRpt.Rows.Count - 1
                                        fct = fct + FormatNumber(dtRpt.Rows(d).Item("FCT"), 0)
                                    Next
                                    Data = fct.ToString()
                                    'Data = FormatNumber(dtRpt.Rows(0).Item("FCT").ToString(), 0)
                                    pcScript &= ";" + Data + ""
                                    strt(i) = "" + Data.ToString()
                                Else
                                    pcScript &= ";" + fct.ToString() + ""
                                    strt(i) = "0"
                                End If
                            Else
                                Dim count1 As New Double
                                Dim count2 As New Double
                                Dim diff As New Double
                                dvRptCols1.RowFilter = "USERREPORTCOLUMNID=" + dsCol.Tables(0).Rows(i).Item("INPUTVALUETYPE1").ToString()
                                dtRptCols1 = dvRptCols1.ToTable()
                                dvRptCols2.RowFilter = "USERREPORTCOLUMNID=" + dsCol.Tables(0).Rows(i).Item("INPUTVALUETYPE2").ToString()
                                dtRptCols2 = dvRptCols2.ToTable()

                                If strRFilt1 <> "" Then
                                    dvRptAct.RowFilter = " COUNTRYID = " + dsRows1.Tables(0).Rows(j).Item("COUNTRYID").ToString() + " AND YEARID=" + dtRptCols1.Rows(0).Item("COLUMNVALUEID").ToString()
                                Else
                                    dvRptAct.RowFilter = "COUNTRYID = " + dsRows1.Tables(0).Rows(j).Item("COUNTRYID").ToString() + " AND YEARID=" + dtRptCols1.Rows(0).Item("COLUMNVALUEID").ToString()
                                End If
                                dtRptAct = dvRptAct.ToTable()
                                If dtRptAct.Rows.Count > 0 Then
                                    For l = 0 To dtRptAct.Rows.Count - 1
                                        If dtRptAct.Rows(l).Item("FCT").ToString() <> "" Then
                                            If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                                                If UnitId = 1 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                ElseIf UnitId = 2 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                ElseIf UnitId = 4 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                Else
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                End If
                                            Else
                                                If UnitId = 1 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT"), 0)
                                                ElseIf UnitId = 2 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL"), 0)
                                                ElseIf UnitId = 3 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA"), 0)
                                                ElseIf UnitId = 4 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                Else
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                End If
                                            End If
                                        End If
                                    Next
                                End If

                                If strRFilt1 <> "" Then
                                    dvRptAct.RowFilter = " COUNTRYID = " + dsRows1.Tables(0).Rows(j).Item("COUNTRYID").ToString() + " AND YEARID=" + dtRptCols2.Rows(0).Item("COLUMNVALUEID").ToString()
                                Else
                                    dvRptAct.RowFilter = "COUNTRYID = " + dsRows1.Tables(0).Rows(j).Item("COUNTRYID").ToString() + " AND YEARID=" + dtRptCols2.Rows(0).Item("COLUMNVALUEID").ToString()
                                End If
                                dtRptAct = dvRptAct.ToTable()
                                If dtRptAct.Rows.Count > 0 Then
                                    For l = 0 To dtRptAct.Rows.Count - 1
                                        If dtRptAct.Rows(l).Item("FCT").ToString() <> "" Then
                                            If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                                                If UnitId = 1 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                ElseIf UnitId = 2 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                ElseIf UnitId = 4 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                Else
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                End If
                                            Else
                                                If UnitId = 1 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT"), 0)
                                                ElseIf UnitId = 2 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL"), 0)
                                                ElseIf UnitId = 3 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA"), 0)
                                                ElseIf UnitId = 4 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                Else
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                End If
                                            End If
                                        End If
                                    Next
                                End If

                                diff = dtRptCols2.Rows(0).Item("COLUMNVALUEID") - dtRptCols1.Rows(0).Item("COLUMNVALUEID")
                                If count1 <> 0 And count2 <> 0 Then
                                    pcScript &= ";" + FormatNumber((((count2 / count1) ^ (1 / diff)) - 1) * 100, 4)
                                    strt(i) = FormatNumber((((count2 / count1) ^ (1 / diff)) - 1) * 100, 4).ToString()
                                Else
                                    pcScript &= ";" + FormatNumber(0, 4)
                                    strt(i) = "0"
                                End If
                            End If
                        Next
                        '        pcScript &= ")"

                        '        For k = 0 To dsCol.Tables(0).Rows.Count - 1
                        '            pcScript = pcScript + "" + Graphtype + ".addHoverText(" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString()
                        '            pcScript = pcScript + "" + "," + (j + 1).ToString() + "" ' dsRows1.Tables(0).Rows(j).Item("COUNTRYDES").ToString() + ""
                        '            pcScript = pcScript + "" + ",Year:" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + "<br/>"
                        '            pcScript = pcScript + "" + "Value:" + strt(k).ToString() + ")"
                        '        Next

                        '    Next
                        '    GenrateChart(pcScript, GraphName, pref, dsRows1.Tables(0).Rows.Count * ColCnt)
                        '    lblNOG.Visible = False
                        'Else
                        '    MaterialPrice.Visible = False
                        '    lblNOG.Visible = True
                        '    lblNOG.Text = "No Data For this Combination."
                        'End If

                        '  changes started

                        pcScript &= ")"
                        Dim stra As String = ""
                        Dim S1 As String = ""
                        Dim S2 As String = ""

                        For k = 0 To dsCol.Tables(0).Rows.Count - 1
                            ' pcScript = pcScript + "" + Graphtype + ".addHoverText(" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + ""

                            pcScript = pcScript + "" + Graphtype

                            If dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() = "CAGR" Then

                                S1 = dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + "(" + dsCol.Tables(0).Rows(k).Item("INPUTVALUE1").ToString() + "/" + dsCol.Tables(0).Rows(k).Item("INPUTVALUE2").ToString() + ")"

                                pcScript = pcScript + ".addHoverText(" + S1 + ""
                            Else
                                pcScript = pcScript + ".addHoverText(" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + ""

                            End If


                            pcScript = pcScript + "" + "," + (j + 1).ToString() + "" ' dsRows1.Tables(0).Rows(j).Item("COUNTRYDES").ToString() + ""

                            pcScript = pcScript + ""

                            If dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() = "CAGR" Then

                                S1 = "(" + dsCol.Tables(0).Rows(k).Item("INPUTVALUE1").ToString() + "/" + dsCol.Tables(0).Rows(k).Item("INPUTVALUE2").ToString() + ")"

                                pcScript = pcScript + "" + ",CAGR:" + S1 + "<br/>"


                            Else

                                pcScript = pcScript + "" + ",Year:" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + "<br/>"
                            End If


                            pcScript = pcScript + "" + "Value:" + strt(k).ToString() + ")"


                        Next

                    Next
                    GenrateChart(pcScript, GraphName, pref, dsRows1.Tables(0).Rows.Count * ColCnt)
                    lblNOG.Visible = False
                Else
                    MaterialPrice.Visible = False
                    lblNOG.Visible = True
                    lblNOG.Text = "Values Are Not Available for this Report."
                    GetFilters()
                End If
            Else
                MaterialPrice.Visible = False
                lblNOG.Visible = True
                lblNOG.Text = "No Data For this Combination."
                GetFilters()
            End If
            '  changes  end

        Catch ex As Exception
            ErrorLable.Text = "Error:SetPivotReportFrameWorkTemp " + ex.Message.ToString()
        End Try
    End Sub

    Protected Sub SetPivotProdReportFrameWorkTemp(ByVal RptID As String)
        Dim objGetData As New Selectdata()
        Dim i As New Integer
        Dim j As New Integer
        Dim HeaderTr As New TableRow()
        Dim HeaderTd As New TableCell()
        Dim Tr As New TableRow()
        Dim Td As New TableCell()
        Dim Path As String = String.Empty
        Dim dsRows As New DataSet
        Dim dsCol As New DataSet
        Dim dsFilter As New DataSet
        Dim Flag As Boolean
        Dim Des As String = String.Empty
        Dim dv As New DataView
        Dim dvCol As New DataView
        Dim dt As New DataTable
        Dim YearId As String = ""
        Dim UnitId As String = ""
        Dim dvRptCols1 As New DataView
        Dim dtRptCols1 As New DataTable
        Dim dvRptCols2 As New DataView
        Dim dtRptCols2 As New DataTable
        Dim dsUnitPref As New DataSet
        Dim dsRep As New DataSet
        Dim RowCnt As New Integer
        Dim ColCnt As New Integer
        Dim filterCnt As New Integer
        Dim imgUp As New ImageButton
        Dim imgDn As New ImageButton
        Dim CompId As String = ""
        Dim dsComp As New DataSet
        Try

            If hidReport.Value <> "0" Then
                dsRep = Session("dsRep" + REPId.ToString())
                dsUnitPref = Session("dsPref" + REPId.ToString())
                dsRows = Session("dsRows" + REPId.ToString())
                dsCol = Session("dsColumns" + REPId.ToString())
                dsFilter = Session("dsFilters" + REPId.ToString())
            Else
                dsRep = objGetData.GetReportDetails(REPId.ToString())
                dsUnitPref = objGetData.GetPref(REPId.ToString())
                dsRows = objGetData.GetUsersDynamicReportRows(REPId.ToString())
                dsCol = objGetData.GetUsersDynamicReportCols(REPId.ToString())
                dsFilter = objGetData.GetUsersReportFilters(REPId.ToString())

                Session("dsRep" + REPId.ToString()) = dsRep
                Session("dsPref" + REPId.ToString()) = dsUnitPref
                Session("dsRows" + REPId.ToString()) = dsRows
                Session("dsColumns" + REPId.ToString()) = dsCol
                Session("dsFilters" + REPId.ToString()) = dsFilter

                hidReport.Value = "1"
            End If

            filterCnt = dsFilter.Tables(0).Rows.Count
            ColCnt = dsCol.Tables(0).Rows.Count


            For a = 0 To dsCol.Tables(0).Rows.Count - 1
                If dsCol.Tables(0).Rows(a).Item("COLUMNVALUETYPE").ToString() <> "Formula" Then
                    YearId = YearId + "" + dsCol.Tables(0).Rows(a).Item("COLUMNVALUEID").ToString() + ","
                End If
            Next
            YearId = YearId.Remove(YearId.Length - 1)

            UnitId = dsRows.Tables(0).Rows(0).Item("UNITID").ToString()

            dvCol = dsCol.Tables(0).DefaultView
            dvRptCols1 = dsCol.Tables(0).DefaultView
            dvRptCols2 = dsCol.Tables(0).DefaultView

            Dim dsTables As New DataSet
            Dim dvTables As New DataView
            Dim dtTables As New DataTable
            Dim ProdId As String = ""
            Dim PackId As String = ""
            Dim dsProd As New DataSet
            Dim dvProd As New DataView
            Dim dtProd As New DataTable
            Dim dsPack As New DataSet
            Dim dvPack As New DataView
            Dim dtPack As New DataTable
            Dim dsOrder As New DataSet()
            Dim funflag As Boolean = False
            Dim MatId As String = ""
            Dim FactId As String = ""
            Dim dsMat As New DataSet
            Dim dsProdMat As New DataSet
            Dim arrRfilt(filterCnt) As String
            Dim CountryId As String = ""
            Dim RegionId As String = ""
            Dim GrpId As String = ""
            Dim dsGrp As New DataSet
            Dim filDes As String
            dsTables = objGetData.GetProductDetails(Session("M1SubGroupId"))
            dvTables = dsTables.Tables(0).DefaultView
            Dim ProdTbl(dsTables.Tables(0).Rows.Count) As String

            For b = 0 To dsTables.Tables(0).Rows.Count - 1
                ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                ProdId = ProdId + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                RowCnt += 1
            Next
            ProdId = ProdId.Remove(ProdId.Length - 1)

            For a = 0 To dsFilter.Tables(0).Rows.Count - 1
                filDes = Request.Form("ddlfil_" + a.ToString())
                If filDes <> Nothing Then
                    'filDes = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    Fils(a) = filDes.ToString()
                Else
                    filDes = "0"
                    Fils(a) = "0"
                    If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() <> "0" Then
                        filDes = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    End If
                End If
                If dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PACKAGE" Then
                    arrRfilt(a) = "PACKAGE"
                    '     If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    If filDes = "0" Then
                        If MatId <> "" Then
                            dsPack = objGetData.GetPivotMatAllPackages(ProdId, MatId)
                        ElseIf CompId <> "" Then
                            dsPack = objGetData.GetPivotCompAllPackages(ProdId, CompId)
                        Else
                            dsPack = objGetData.GetPackages(ProdId)
                        End If
                        'End If
                        For b = 0 To dsPack.Tables(0).Rows.Count - 1
                            PackId = PackId + "" + dsPack.Tables(0).Rows(b).Item("PACKAGETYPEID").ToString() + ","
                        Next
                        PackId = PackId.Remove(PackId.Length - 1)
                    Else
                        PackId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                        If MatId <> "" Then
                            dsPack = objGetData.GetPivotMatPackages(MatId, PackId)
                        ElseIf CompId <> "" Then
                            dsPack = objGetData.GetPivotCompPackages(PackId, CompId)
                        Else
                            dsPack = objGetData.GetPivotPackages(PackId)
                        End If
                    End If

                    dsPack.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsPack.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsPack.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If
                ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "MATERIAL" Then
                    arrRfilt(a) = "MATERIAL"
                    'MatId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    If filDes = "0" Then
                        MatId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    Else
                        MatId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    End If
                    If PackId <> "" Then
                        dsMat = objGetData.GetPivotPackMaterials(ProdId, PackId, MatId)
                    Else
                        dsMat = objGetData.GetPivotMaterials(MatId)
                    End If

                    dsMat.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsMat.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsMat.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If
                ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "COMPONENT" Then
                    arrRfilt(a) = "COMPONENT"
                    'If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    If filDes = "0" Then
                        If PackId <> "" Then
                            dsComp = objGetData.GetPivotPackComponents(ProdId, PackId, "")
                        Else
                            dsComp = objGetData.GetPivotAllComponents(ProdId)
                        End If

                        For b = 0 To dsComp.Tables(0).Rows.Count - 1
                            CompId = CompId + "" + dsComp.Tables(0).Rows(b).Item("COMPONENTID").ToString() + ","
                        Next
                        CompId = CompId.Remove(CompId.Length - 1)
                    Else
                        CompId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                        If PackId <> "" Then
                            dsComp = objGetData.GetPivotPackComponents(ProdId, PackId, CompId)
                        Else
                            dsComp = objGetData.GetRepComponent(CompId)
                        End If
                    End If
                    dsComp.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsComp.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsComp.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If

                ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "COUNTRY" Then
                    arrRfilt(a) = "COUNTRY"
                    'CountryId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    If filDes = "0" Then
                        CountryId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    Else
                        CountryId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    End If
                    dsMat = objGetData.GetPivotCountriesByRegion(CountryId)
                    dsMat.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsMat.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsMat.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If

                ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "REGION" Then
                    arrRfilt(a) = "REGION"
                    ' RegionId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    If filDes = "0" Then
                        RegionId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    Else
                        RegionId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    End If
                    dsMat = objGetData.GetPivotRegion(RegionId)
                    dsMat.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsMat.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsMat.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If

                ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "GROUP" Then
                    arrRfilt(a) = "GROUP"
                    'GrpId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    If filDes = "0" Then
                        GrpId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    Else
                        GrpId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    End If
                    dsGrp = objGetData.GetPivotGroups(GrpId)
                    dsGrp.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsGrp.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsGrp.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If
                End If
            Next

            dv = dsRows.Tables(0).DefaultView
            Dim dv1 As New DataView
            Dim dv2 As New DataView
            Dim dv3 As New DataView
            Dim dt1 As New DataTable
            Dim dt2 As New DataTable
            Dim dt3 As New DataTable
            Dim dsRptAct As New DataSet
            Dim dvRptAct As New DataView
            Dim dtRptAct As New DataTable
            Dim strRFilt1 As String = ""
            Dim strRFilt2 As String = ""
            Dim strRFilt3 As String = ""
            Dim dsRows1 As DataSet
            If ProdTbl(0) = "" Then
                For b = 0 To dsTables.Tables(0).Rows.Count - 1
                    ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                    RowCnt += 1
                Next
            End If

            Dim dvRpt As New DataView
            Dim dtRpt As New DataTable

            If funflag Then
                If filterCnt = 1 Then
                    dv1 = dsOrder.Tables(0).DefaultView
                ElseIf filterCnt = 2 Then
                    dv1 = dsOrder.Tables(0).DefaultView
                    dv2 = dsOrder.Tables(1).DefaultView
                ElseIf filterCnt = 3 Then
                    dv1 = dsOrder.Tables(0).DefaultView
                    dv2 = dsOrder.Tables(1).DefaultView
                    dv3 = dsOrder.Tables(2).DefaultView
                End If
                If CompId <> "" Then
                    dsRows1 = objGetData.GetPivotComponentsProducts(ProdId, PackId, CompId)
                Else
                    dsRows1 = objGetData.GetPivotProducts(ProdId, PackId, MatId)
                End If

                dsRptAct = objGetData.GetPivotReportData_PROD(ProdTbl, YearId, PackId, MatId, GrpId, CompId, CountryId, RegionId, UnitId, dsTables.Tables(0).Rows.Count)
                dvRptAct = dsRptAct.Tables(0).DefaultView
                dvRpt = dsRptAct.Tables(0).DefaultView

                ' changes started

                If dsRptAct.Tables(0).Rows.Count > 0 Then
                    ' changes end
                    'CODE FOR GRAPH
                    Dim pcScript = ""
                    Dim odbutil As New DBUtil()
                    Dim Graphtype As String = ""
                    Dim GraphName As String = ""

                    Dim pref As String = String.Empty
                    If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                        pref = "(" + dsRows.Tables(0).Rows(0).Item("UNITSHRT").ToString() + ")"
                    Else
                        pref = "(" + dsRows.Tables(0).Rows(0).Item("METRICUNIT").ToString() + ")"
                    End If
                    Dim Count As String = dtRptAct.Rows.Count
                    Dim Str1 As String = ""


                    'For i = 0 To dsCol.Tables(0).Rows.Count - 1
                    '    Str1 = Str1 + dsCol.Tables(0).Rows(i).Item("COLUMNVALUE") + "; "
                    'Next

                    '  Changes Started 
                    Dim S As String = ""

                    For i = 0 To dsCol.Tables(0).Rows.Count - 1
                        Str1 = Str1 + " "
                        If dsCol.Tables(0).Rows(i).Item("COLUMNVALUE").ToString() = "CAGR" Then

                            S = dsCol.Tables(0).Rows(i).Item("COLUMNVALUE").ToString() + "(" + dsCol.Tables(0).Rows(i).Item("INPUTVALUE1").ToString() + "/" + dsCol.Tables(0).Rows(i).Item("INPUTVALUE2").ToString() + ")"
                            Str1 = Str1 + "" + S + ";"
                        Else
                            Str1 = Str1 + dsCol.Tables(0).Rows(i).Item("COLUMNVALUE") + ";"
                        End If

                    Next
                    '  Changes end




                    Dim Str2 As String = ""
                    For i = 0 To dsRows1.Tables(0).Rows.Count - 1
                        Str2 = Str2 + dsRows1.Tables(0).Rows(i).Item("VALUE") + "; "
                    Next
                    Dim Data As String = String.Empty
                    GraphName = "bar"
                    Graphtype = "graph"
                    ' GraphName = "Sargento_KraftPouchvsTray"
                    'Dim Count1 As String = dtRptAct.Rows.Count
                    dvRpt = dsRptAct.Tables(0).DefaultView

                    pcScript &= "" + Graphtype + ".transposed(true)" + Graphtype + ".setCategories(" + Str1.ToString() + ")"

                    'double bar
                    Dim strt(dsCol.Tables(0).Rows.Count) As String
                    For j = 0 To dsRows1.Tables(0).Rows.Count - 1
                        For i = 0 To dsCol.Tables(0).Rows.Count - 1
                            If i = 0 Then
                                pcScript &= "" + Graphtype + ".setSeries(" & dsRows1.Tables(0).Rows(j).Item("VALUE") & ""
                            End If
                            If dsCol.Tables(0).Rows(i).Item("COLUMNVALUETYPE").ToString() <> "Formula" Then
                                dvRpt.RowFilter = " CATEGORYID = " + dsRows1.Tables(0).Rows(j).Item("CATEGORYID").ToString().Replace(" ", " ") + " AND YEARID= " + dsCol.Tables(0).Rows(i).Item("COLUMNVALUEID").ToString().Replace(" ", " ")
                                dtRpt = dvRpt.ToTable()
                                Dim fct As Double = 0.0
                                Data = ""
                                If dtRpt.Rows.Count > 0 Then
                                    For d = 0 To dtRpt.Rows.Count - 1
                                        fct = fct + FormatNumber(dtRpt.Rows(d).Item("FCT"), 0)
                                    Next
                                    Data = fct.ToString()

                                    'Data = FormatNumber(dtRpt.Rows(0).Item("FCT").ToString(), 0)
                                    pcScript &= ";" + Data + ""
                                    strt(i) = "" + Data.ToString()
                                Else
                                    pcScript &= ";" + fct.ToString() + ""
                                    strt(i) = "0"
                                End If
                            Else
                                Dim count1 As New Double
                                Dim count2 As New Double
                                Dim diff As New Double
                                dvRptCols1.RowFilter = "USERREPORTCOLUMNID=" + dsCol.Tables(0).Rows(i).Item("INPUTVALUETYPE1").ToString()
                                dtRptCols1 = dvRptCols1.ToTable()
                                dvRptCols2.RowFilter = "USERREPORTCOLUMNID=" + dsCol.Tables(0).Rows(i).Item("INPUTVALUETYPE2").ToString()
                                dtRptCols2 = dvRptCols2.ToTable()

                                If strRFilt1 <> "" Then
                                    dvRptAct.RowFilter = strRFilt1 + " AND CATEGORYID = " + dsRows1.Tables(0).Rows(j).Item("CATEGORYID").ToString() + " AND YEARID=" + dtRptCols1.Rows(0).Item("COLUMNVALUEID").ToString()
                                Else
                                    dvRptAct.RowFilter = "CATEGORYID = " + dsRows1.Tables(0).Rows(j).Item("CATEGORYID").ToString() + " AND YEARID=" + dtRptCols1.Rows(0).Item("COLUMNVALUEID").ToString()
                                End If
                                dtRptAct = dvRptAct.ToTable()
                                If dtRptAct.Rows.Count > 0 Then
                                    For l = 0 To dtRptAct.Rows.Count - 1
                                        If dtRptAct.Rows(l).Item("FCT").ToString() <> "" Then
                                            If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                                                If UnitId = 1 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                ElseIf UnitId = 2 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                ElseIf UnitId = 4 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                Else
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                End If
                                            Else
                                                If UnitId = 1 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT"), 0)
                                                ElseIf UnitId = 2 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL"), 0)
                                                ElseIf UnitId = 3 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA"), 0)
                                                ElseIf UnitId = 4 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                Else
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                End If
                                            End If
                                        End If
                                    Next
                                End If

                                If strRFilt1 <> "" Then
                                    dvRptAct.RowFilter = strRFilt1 + " AND CATEGORYID = " + dsRows1.Tables(0).Rows(j).Item("CATEGORYID").ToString() + " AND YEARID=" + dtRptCols2.Rows(0).Item("COLUMNVALUEID").ToString()
                                Else
                                    dvRptAct.RowFilter = "CATEGORYID = " + dsRows1.Tables(0).Rows(j).Item("CATEGORYID").ToString() + " AND YEARID=" + dtRptCols2.Rows(0).Item("COLUMNVALUEID").ToString()
                                End If
                                dtRptAct = dvRptAct.ToTable()
                                If dtRptAct.Rows.Count > 0 Then
                                    For l = 0 To dtRptAct.Rows.Count - 1
                                        If dtRptAct.Rows(l).Item("FCT").ToString() <> "" Then
                                            If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                                                If UnitId = 1 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                ElseIf UnitId = 2 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                ElseIf UnitId = 4 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                Else
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                End If
                                            Else
                                                If UnitId = 1 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT"), 0)
                                                ElseIf UnitId = 2 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL"), 0)
                                                ElseIf UnitId = 3 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA"), 0)
                                                ElseIf UnitId = 4 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                Else
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                End If
                                            End If
                                        End If
                                    Next
                                End If
                                diff = dtRptCols2.Rows(0).Item("COLUMNVALUEID") - dtRptCols1.Rows(0).Item("COLUMNVALUEID")
                                If count1 <> 0 And count2 <> 0 Then
                                    pcScript &= ";" + FormatNumber((((count2 / count1) ^ (1 / diff)) - 1) * 100, 4)
                                    Data = FormatNumber((((count2 / count1) ^ (1 / diff)) - 1) * 100, 4)
                                Else
                                    pcScript &= ";" + FormatNumber(0, 4)
                                    Data = "0"
                                End If
                            End If
                            strt(i) = "" + Data.ToString()
                        Next
                        '        pcScript &= ")"
                        '        For k = 0 To dsCol.Tables(0).Rows.Count - 1
                        '            pcScript = pcScript + "" + Graphtype + ".addHoverText(" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString()
                        '            pcScript = pcScript + "" + "," + (j + 1).ToString() + "" ' dsRows1.Tables(0).Rows(j).Item("COUNTRYDES").ToString() + ""
                        '            pcScript = pcScript + "" + ",Year:" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + "<br/>"
                        '            pcScript = pcScript + "" + "Value:" + strt(k).ToString() + ")"
                        '        Next
                        '    Next
                        '    GenrateChart(pcScript, GraphName, pref, dsRows1.Tables(0).Rows.Count * ColCnt)
                        '    lblNOG.Visible = False
                        'Else
                        '    MaterialPrice.Visible = False
                        '    lblNOG.Visible = True
                        '    lblNOG.Text = "No Data For this Combination."

                        'End If


                        '  Changes Started
                        pcScript &= ")"
                        Dim stra As String = ""
                        Dim S1 As String = ""
                        Dim S2 As String = ""

                        For k = 0 To dsCol.Tables(0).Rows.Count - 1
                            ' pcScript = pcScript + "" + Graphtype + ".addHoverText(" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + ""

                            pcScript = pcScript + "" + Graphtype

                            If dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() = "CAGR" Then

                                S1 = dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + "(" + dsCol.Tables(0).Rows(k).Item("INPUTVALUE1").ToString() + "/" + dsCol.Tables(0).Rows(k).Item("INPUTVALUE2").ToString() + ")"

                                pcScript = pcScript + ".addHoverText(" + S1 + ""
                            Else
                                pcScript = pcScript + ".addHoverText(" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + ""

                            End If


                            pcScript = pcScript + "" + "," + (j + 1).ToString() + "" ' dsRows1.Tables(0).Rows(j).Item("COUNTRYDES").ToString() + ""

                            pcScript = pcScript + ""

                            If dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() = "CAGR" Then

                                S1 = "(" + dsCol.Tables(0).Rows(k).Item("INPUTVALUE1").ToString() + "/" + dsCol.Tables(0).Rows(k).Item("INPUTVALUE2").ToString() + ")"

                                pcScript = pcScript + "" + ",CAGR:" + S1 + "<br/>"


                            Else

                                pcScript = pcScript + "" + ",Year:" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + "<br/>"
                            End If


                            pcScript = pcScript + "" + "Value:" + strt(k).ToString() + ")"


                        Next

                    Next
                    GenrateChart(pcScript, GraphName, pref, dsRows1.Tables(0).Rows.Count * ColCnt)
                    lblNOG.Visible = False
                Else
                    MaterialPrice.Visible = False
                    lblNOG.Visible = True
                    lblNOG.Text = "Values Are Not Available for this Report."
                    GetFilters()
                End If
            Else
                MaterialPrice.Visible = False
                lblNOG.Visible = True
                lblNOG.Text = "No Data For this Combination."
                GetFilters()
            End If
            '  Changes end

        Catch ex As Exception
            ErrorLable.Text = "Error:SetPivotProdReportFrameWorkTemp " + ex.Message.ToString()
        End Try
    End Sub

    Protected Sub SetPivotMatReportFrameWorkTemp(ByVal RptID As String)
        Dim objGetData As New Selectdata()
        Dim i As New Integer
        Dim j As New Integer
        Dim HeaderTr As New TableRow()
        Dim HeaderTd As New TableCell()
        Dim Tr As New TableRow()
        Dim Td As New TableCell()
        Dim Path As String = String.Empty
        Dim dsRows As New DataSet
        Dim dsCol As New DataSet
        Dim dsFilter As New DataSet
        Dim Des As String = String.Empty
        Dim k As Integer
        Dim dv As New DataView
        Dim dvCol As New DataView
        Dim dt As New DataTable
        Dim YearId As String = ""
        Dim UnitId As String = ""
        Dim dvRptCols1 As New DataView
        Dim dtRptCols1 As New DataTable
        Dim dvRptCols2 As New DataView
        Dim dtRptCols2 As New DataTable
        Dim dsUnitPref As New DataSet
        Dim dsRep As New DataSet
        Dim RowCnt As New Integer
        Dim ColCnt As New Integer
        Dim filterCnt As New Integer
        Dim GrpId As String = ""
        Dim dsGrp As New DataSet
        Dim imgUp As New ImageButton
        Dim imgDn As New ImageButton
        Try
            If hidReport.Value <> "0" Then
                dsRep = Session("dsRep" + REPId.ToString())
                dsUnitPref = Session("dsPref" + REPId.ToString())
                dsRows = Session("dsRows" + REPId.ToString())
                dsCol = Session("dsColumns" + REPId.ToString())
                dsFilter = Session("dsFilters" + REPId.ToString())
            Else
                dsRep = objGetData.GetReportDetails(REPId.ToString())
                dsUnitPref = objGetData.GetPref(REPId.ToString())
                dsRows = objGetData.GetUsersDynamicReportRows(REPId.ToString())
                dsCol = objGetData.GetUsersDynamicReportCols(REPId.ToString())
                dsFilter = objGetData.GetUsersReportFilters(REPId.ToString())

                Session("dsRep" + REPId.ToString()) = dsRep
                Session("dsPref" + REPId.ToString()) = dsUnitPref
                Session("dsRows" + REPId.ToString()) = dsRows
                Session("dsColumns" + REPId.ToString()) = dsCol
                Session("dsFilters" + REPId.ToString()) = dsFilter

                hidReport.Value = "1"
            End If

            filterCnt = dsFilter.Tables(0).Rows.Count
            ColCnt = dsCol.Tables(0).Rows.Count

            For a = 0 To dsCol.Tables(0).Rows.Count - 1
                If dsCol.Tables(0).Rows(a).Item("COLUMNVALUETYPE").ToString() <> "Formula" Then
                    YearId = YearId + "" + dsCol.Tables(0).Rows(a).Item("COLUMNVALUEID").ToString() + ","
                End If
            Next
            YearId = YearId.Remove(YearId.Length - 1)
            UnitId = dsRows.Tables(0).Rows(0).Item("UNITID").ToString()

            dvCol = dsCol.Tables(0).DefaultView
            dvRptCols1 = dsCol.Tables(0).DefaultView
            dvRptCols2 = dsCol.Tables(0).DefaultView

            Dim dsTables As New DataSet
            Dim dvTables As New DataView
            Dim dtTables As New DataTable
            Dim ProdId As String = ""
            Dim PackId As String = ""
            Dim dsProd As New DataSet
            Dim dvProd As New DataView
            Dim dtProd As New DataTable
            Dim dsPack As New DataSet
            Dim dvPack As New DataView
            Dim dtPack As New DataTable
            Dim dsOrder As New DataSet()
            Dim funflag As Boolean = False
            Dim MatId As String = ""
            Dim FactId As String = ""
            Dim dsMat As New DataSet
            Dim dsProdMat As New DataSet
            Dim arrRfilt(filterCnt) As String
            Dim CountryId As String = ""
            Dim RegionId As String = ""
            Dim filDes As String
            dsTables = objGetData.GetProductDetails(Session("M1SubGroupId"))
            dvTables = dsTables.Tables(0).DefaultView
            Dim ProdTbl(dsTables.Tables(0).Rows.Count) As String

            For a = 0 To dsFilter.Tables(0).Rows.Count - 1
                filDes = Request.Form("ddlfil_" + a.ToString())
                If filDes <> Nothing Then
                    'filDes = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    Fils(a) = filDes.ToString()
                Else
                    filDes = "0"
                    Fils(a) = "0"
                    If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() <> "0" Then
                        filDes = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    End If
                End If
                If dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PRODUCT" Then
                    arrRfilt(a) = "PRODUCT"
                    'If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    If filDes = "0" Then
                        For b = 0 To dsTables.Tables(0).Rows.Count - 1
                            ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                            ProdId = ProdId + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                            RowCnt += 1
                        Next
                        dtTables = dsTables.Tables(0)
                        ProdId = ProdId.Remove(ProdId.Length - 1)
                        If PackId <> "" Then
                            dsProd = objGetData.GetPivotPackProducts(ProdId, PackId)

                        Else
                            dsProd = objGetData.GetPivotProductDescription(ProdId)
                        End If
                        ProdId = ""
                        For b = 0 To dsProd.Tables(0).Rows.Count - 1
                            ProdId = ProdId + "" + dsProd.Tables(0).Rows(b).Item("ID").ToString() + ","
                        Next
                        ProdId = ProdId.Remove(ProdId.Length - 1)

                    Else
                        ProdId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                        dvTables.RowFilter = "ACTUALPRODUCTID=" + ProdId
                        dtTables = dvTables.ToTable()
                        ProdTbl(0) = dtTables.Rows(0).Item("TABLENAME").ToString()
                        RowCnt += 1
                        If PackId <> "" Then
                            dsProd = objGetData.GetPivotPackProducts(ProdId, PackId)
                        Else
                            dsProd = objGetData.GetPivotProductDescription(ProdId)
                        End If
                    End If

                    dsProd.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsProd.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsProd.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If
                ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PACKAGE" Then
                    arrRfilt(a) = "PACKAGE"
                    'If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    If filDes = "0" Then
                        If ProdId <> "" Then
                            dsPack = objGetData.GetPivotProdPackage(ProdId, "")
                        Else
                            For b = 0 To dsTables.Tables(0).Rows.Count - 1
                                ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                                ProdId = ProdId + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                                RowCnt += 1
                            Next
                            dtTables = dsTables.Tables(0)
                            ProdId = ProdId.Remove(ProdId.Length - 1)
                            dsPack = objGetData.GetPackages(ProdId)
                        End If
                        For b = 0 To dsPack.Tables(0).Rows.Count - 1
                            PackId = PackId + "" + dsPack.Tables(0).Rows(b).Item("PACKAGETYPEID").ToString() + ","
                        Next
                        PackId = PackId.Remove(PackId.Length - 1)
                    Else
                        PackId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                        If ProdId <> "" Then
                            dsPack = objGetData.GetPivotProdPackage(ProdId, PackId)

                        Else
                            dsPack = objGetData.GetPivotPackages(PackId)
                        End If
                    End If

                    dsPack.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsPack.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsPack.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If
                ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "COUNTRY" Then
                    arrRfilt(a) = "COUNTRY"
                    'CountryId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    If filDes = "0" Then
                        CountryId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    Else
                        CountryId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    End If
                    dsMat = objGetData.GetPivotCountriesByRegion(CountryId)
                    dsMat.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsMat.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsMat.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If

                ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "REGION" Then
                    arrRfilt(a) = "REGION"
                    'RegionId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    If filDes = "0" Then
                        RegionId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    Else
                        RegionId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    End If
                    dsMat = objGetData.GetPivotRegion(RegionId)
                    dsMat.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsMat.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsMat.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If

                ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "GROUP" Then
                    arrRfilt(a) = "GROUP"
                    'GrpId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    If filDes = "0" Then
                        GrpId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    Else
                        GrpId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    End If
                    dsGrp = objGetData.GetPivotGroups(GrpId)
                    dsGrp.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsGrp.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsGrp.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If
                End If
            Next

            dv = dsRows.Tables(0).DefaultView

            Dim dv1 As New DataView
            Dim dv2 As New DataView
            Dim dv3 As New DataView
            Dim dt1 As New DataTable
            Dim dt2 As New DataTable
            Dim dt3 As New DataTable
            Dim dsRptAct As New DataSet
            Dim dvRptAct As New DataView
            Dim dtRptAct As New DataTable
            Dim strRFilt As String = ""

            If ProdTbl(0) = "" Then
                For b = 0 To dsTables.Tables(0).Rows.Count - 1
                    ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                    RowCnt += 1
                Next
            End If

            Dim dvRpt As New DataView
            Dim dtRpt As New DataTable
            Dim dsRows1 As DataSet
            If funflag Then
                If filterCnt = 1 Then
                    dv1 = dsOrder.Tables(0).DefaultView
                ElseIf filterCnt = 2 Then
                    dv1 = dsOrder.Tables(0).DefaultView
                    dv2 = dsOrder.Tables(1).DefaultView
                ElseIf filterCnt = 3 Then
                    dv1 = dsOrder.Tables(0).DefaultView
                    dv2 = dsOrder.Tables(1).DefaultView
                    dv3 = dsOrder.Tables(2).DefaultView
                End If
                dsRows1 = objGetData.GetPivotMaterials(ProdId, PackId)
                dsRptAct = objGetData.GetPivotReportData_MAT(ProdTbl, YearId, ProdId, PackId, GrpId, CountryId, RegionId, UnitId, dsTables.Tables(0).Rows.Count)
                dvRptAct = dsRptAct.Tables(0).DefaultView
                dvRpt = dsRptAct.Tables(0).DefaultView

                ' changes Started


                If dsRptAct.Tables(0).Rows.Count > 0 Then
                    ' changes end


                    'CODE FOR GRAPH
                    Dim pcScript = ""
                    Dim odbutil As New DBUtil()
                    Dim Graphtype As String = ""
                    Dim GraphName As String = ""

                    Dim pref As String = String.Empty
                    If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                        pref = "(" + dsRows.Tables(0).Rows(0).Item("UNITSHRT").ToString() + ")"
                    Else
                        pref = "(" + dsRows.Tables(0).Rows(0).Item("METRICUNIT").ToString() + ")"
                    End If
                    Dim Count As String = dtRptAct.Rows.Count
                    Dim Str1 As String = ""


                    'For i = 0 To dsCol.Tables(0).Rows.Count - 1
                    '    Str1 = Str1 + dsCol.Tables(0).Rows(i).Item("COLUMNVALUE") + "; "
                    'Next

                    '  Changes Started 
                    Dim S As String = ""

                    For i = 0 To dsCol.Tables(0).Rows.Count - 1
                        Str1 = Str1 + " "
                        If dsCol.Tables(0).Rows(i).Item("COLUMNVALUE").ToString() = "CAGR" Then

                            S = dsCol.Tables(0).Rows(i).Item("COLUMNVALUE").ToString() + "(" + dsCol.Tables(0).Rows(i).Item("INPUTVALUE1").ToString() + "/" + dsCol.Tables(0).Rows(i).Item("INPUTVALUE2").ToString() + ")"
                            Str1 = Str1 + "" + S + ";"
                        Else
                            Str1 = Str1 + dsCol.Tables(0).Rows(i).Item("COLUMNVALUE") + ";"
                        End If

                    Next
                    '  Changes end


                    Dim Str2 As String = ""
                    For i = 0 To dsRows1.Tables(0).Rows.Count - 1
                        Str2 = Str2 + dsRows1.Tables(0).Rows(i).Item("VALUE") + "; "
                    Next
                    Dim Data As String = String.Empty
                    GraphName = "bar"
                    Graphtype = "graph"
                    'Dim Count1 As String = dtRptAct.Rows.Count
                    dvRpt = dsRptAct.Tables(0).DefaultView

                    pcScript &= "" + Graphtype + ".transposed(true)" + Graphtype + ".setCategories(" + Str1.ToString() + ")"

                    'double bar
                    Dim strt(dsCol.Tables(0).Rows.Count) As String
                    Dim cnt As Integer = 0
                    For j = 0 To dsRows1.Tables(0).Rows.Count - 1
                        dvRpt.RowFilter = " MATERIALID = " + dsRows1.Tables(0).Rows(j).Item("MATERIALID").ToString().Replace(" ", " ")
                        dtRpt = dvRpt.ToTable()
                        If dtRpt.Rows.Count > 0 Then
                            cnt = cnt + 1
                            For i = 0 To dsCol.Tables(0).Rows.Count - 1
                                If i = 0 Then
                                    pcScript &= "" + Graphtype + ".setSeries(" & dsRows1.Tables(0).Rows(j).Item("VALUE") & ""
                                End If
                                If dsCol.Tables(0).Rows(i).Item("COLUMNVALUETYPE").ToString() <> "Formula" Then
                                    dvRpt.RowFilter = " MATERIALID = " + dsRows1.Tables(0).Rows(j).Item("MATERIALID").ToString().Replace(" ", " ") + " AND YEARID= " + dsCol.Tables(0).Rows(i).Item("COLUMNVALUEID").ToString().Replace(" ", " ")
                                    dtRpt = dvRpt.ToTable()
                                    Dim fct As Double = 0.0
                                    Data = ""
                                    If dtRpt.Rows.Count > 0 Then
                                        For d = 0 To dtRpt.Rows.Count - 1
                                            If dtRpt.Rows(d).Item("FCT").ToString() <> "" Then
                                                fct = fct + FormatNumber(dtRpt.Rows(d).Item("FCT"), 0)
                                            End If
                                        Next
                                        Data = fct.ToString()
                                        pcScript &= ";" + Data + ""
                                        strt(i) = "" + Data.ToString()
                                    Else
                                        pcScript &= ";" + fct.ToString() + ""
                                        strt(i) = "0"
                                    End If
                                Else
                                    Dim count1 As New Double
                                    Dim count2 As New Double
                                    Dim diff As New Double
                                    dvRptCols1.RowFilter = "USERREPORTCOLUMNID=" + dsCol.Tables(0).Rows(i).Item("INPUTVALUETYPE1").ToString()
                                    dtRptCols1 = dvRptCols1.ToTable()
                                    dvRptCols2.RowFilter = "USERREPORTCOLUMNID=" + dsCol.Tables(0).Rows(i).Item("INPUTVALUETYPE2").ToString()
                                    dtRptCols2 = dvRptCols2.ToTable()
                                    dvRptAct.RowFilter = " MATERIALID = " + dsRows1.Tables(0).Rows(j).Item("MATERIALID").ToString() + " AND YEARID=" + dtRptCols1.Rows(0).Item("COLUMNVALUEID").ToString()
                                    dtRptAct = dvRptAct.ToTable()
                                    If dtRptAct.Rows.Count > 0 Then
                                        For l = 0 To dtRptAct.Rows.Count - 1
                                            If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                                                If dtRptAct.Rows(l).Item("FCT").ToString() <> "" Then
                                                    If UnitId = 1 Then
                                                        count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                    ElseIf UnitId = 2 Then
                                                        count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                    ElseIf UnitId = 4 Then
                                                        count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                    Else
                                                        count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                    End If
                                                End If
                                            Else
                                                If dtRptAct.Rows(l).Item("FCT").ToString() <> "" Then
                                                    If UnitId = 1 Then
                                                        count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT"), 0)
                                                    ElseIf UnitId = 2 Then
                                                        count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL"), 0)
                                                    ElseIf UnitId = 3 Then
                                                        count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA"), 0)
                                                    ElseIf UnitId = 4 Then
                                                        count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                    Else
                                                        count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                    End If
                                                End If
                                            End If
                                        Next
                                    End If

                                    dvRptAct.RowFilter = " MATERIALID = " + dsRows1.Tables(0).Rows(j).Item("MATERIALID").ToString() + " AND YEARID=" + dtRptCols2.Rows(0).Item("COLUMNVALUEID").ToString()
                                    dtRptAct = dvRptAct.ToTable()
                                    If dtRptAct.Rows.Count > 0 Then
                                        For l = 0 To dtRptAct.Rows.Count - 1
                                            If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                                                If dtRptAct.Rows(l).Item("FCT").ToString() <> "" Then
                                                    If UnitId = 1 Then
                                                        count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                    ElseIf UnitId = 2 Then
                                                        count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                    ElseIf UnitId = 4 Then
                                                        count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                    Else
                                                        count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                    End If
                                                End If
                                            Else
                                                If dtRptAct.Rows(l).Item("FCT").ToString() <> "" Then
                                                    If UnitId = 1 Then
                                                        count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT"), 0)
                                                    ElseIf UnitId = 2 Then
                                                        count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL"), 0)
                                                    ElseIf UnitId = 3 Then
                                                        count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA"), 0)
                                                    ElseIf UnitId = 4 Then
                                                        count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                    Else
                                                        count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                    End If
                                                End If
                                            End If
                                        Next
                                    End If
                                    diff = dtRptCols2.Rows(0).Item("COLUMNVALUEID") - dtRptCols1.Rows(0).Item("COLUMNVALUEID")
                                    pcScript &= ";" + FormatNumber((((count2 / count1) ^ (1 / diff)) - 1) * 100, 4)
                                    Data = FormatNumber((((count2 / count1) ^ (1 / diff)) - 1) * 100, 4)
                                    strt(i) = Data
                                End If
                            Next
                            '            pcScript &= ")"
                            '            For k = 0 To dsCol.Tables(0).Rows.Count - 1
                            '                pcScript = pcScript + "" + Graphtype + ".addHoverText(" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString()
                            '                pcScript = pcScript + "" + "," + (cnt).ToString() + "" ' dsRows1.Tables(0).Rows(j).Item("COUNTRYDES").ToString() + ""
                            '                pcScript = pcScript + "" + ",Year:" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + "<br/>"
                            '                pcScript = pcScript + "" + "Value:" + strt(k).ToString() + ")"
                            '            Next
                            '        End If

                            '    Next
                            '    GenrateChart(pcScript, GraphName, pref, cnt * ColCnt)
                            '    lblNOG.Visible = False
                            'Else
                            '    MaterialPrice.Visible = False
                            '    lblNOG.Visible = True
                            '    lblNOG.Text = "No Data For this Combination."

                            'End If


                            '  changes started
                            pcScript &= ")"
                            Dim stra As String = ""
                            Dim S1 As String = ""
                            Dim S2 As String = ""
                            Dim f = cnt

                            For k = 0 To dsCol.Tables(0).Rows.Count - 1
                                ' pcScript = pcScript + "" + Graphtype + ".addHoverText(" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + ""

                                pcScript = pcScript + "" + Graphtype

                                If dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() = "CAGR" Then

                                    S1 = dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + "(" + dsCol.Tables(0).Rows(k).Item("INPUTVALUE1").ToString() + "/" + dsCol.Tables(0).Rows(k).Item("INPUTVALUE2").ToString() + ")"

                                    pcScript = pcScript + ".addHoverText(" + S1 + ""
                                Else
                                    pcScript = pcScript + ".addHoverText(" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + ""

                                End If


                                pcScript = pcScript + "" + "," + (cnt).ToString() + "" ' dsRows1.Tables(0).Rows(j).Item("COUNTRYDES").ToString() + ""

                                pcScript = pcScript + ""

                                If dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() = "CAGR" Then

                                    S1 = "(" + dsCol.Tables(0).Rows(k).Item("INPUTVALUE1").ToString() + "/" + dsCol.Tables(0).Rows(k).Item("INPUTVALUE2").ToString() + ")"

                                    pcScript = pcScript + "" + ",CAGR:" + S1 + "<br/>"



                                Else

                                    pcScript = pcScript + "" + ",Year:" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + "<br/>"
                                End If


                                pcScript = pcScript + "" + "Value:" + strt(k).ToString() + ")"


                            Next
                        End If

                    Next




                    GenrateChart(pcScript, GraphName, pref, cnt * ColCnt)
                    ' GenrateChart(pcScript, GraphName, pref, dsRows1.Tables(0).Rows.Count * ColCnt)
                    lblNOG.Visible = False
                Else
                    MaterialPrice.Visible = False
                    lblNOG.Visible = True
                    lblNOG.Text = "Values Are Not Available for this Report."
                    GetFilters()
                End If
            Else
                MaterialPrice.Visible = False
                lblNOG.Visible = True
                lblNOG.Text = "No Data For this Combination."
                GetFilters()
            End If
            '  changes end
        Catch ex As Exception
            ErrorLable.Text = "Error:SetPivotMatReportFrameWorkTemp " + ex.Message.ToString()
        End Try
    End Sub

    Protected Sub SetPivotCompReportFrameWorkTemp(ByVal RptID As String)
        Dim objGetData As New Selectdata()
        Dim i As New Integer
        Dim j As New Integer
        Dim HeaderTr As New TableRow()
        Dim HeaderTd As New TableCell()
        Dim Tr As New TableRow()
        Dim Td As New TableCell()
        Dim Path As String = String.Empty
        Dim Link As HyperLink
        Dim hyd As HiddenField
        Dim dsRows As New DataSet
        Dim dsCol As New DataSet
        Dim dsFilter As New DataSet
        Dim Flag As Boolean
        Dim Des As String = String.Empty
        Dim k As Integer
        Dim tblFilter As Table
        Dim trRow As TableRow
        Dim trCol As TableCell
        Dim dv As New DataView
        Dim dvCol As New DataView
        Dim dt As New DataTable
        Dim lbl As Label
        Dim YearId As String = ""
        Dim UnitId As String = ""
        Dim dvRptCols1 As New DataView
        Dim dtRptCols1 As New DataTable
        Dim dvRptCols2 As New DataView
        Dim dtRptCols2 As New DataTable
        Dim dsUnitPref As New DataSet
        Dim dsRep As New DataSet
        Dim RowCnt As New Integer
        Dim ColCnt As New Integer
        Dim filterCnt As New Integer
        Dim GrpId As String = ""
        Dim dsGrp As New DataSet
        Dim imgUp As New ImageButton
        Dim imgDn As New ImageButton
        Try
            If hidReport.Value <> "0" Then
                dsRep = Session("dsRep" + REPId.ToString())
                dsUnitPref = Session("dsPref" + REPId.ToString())
                dsRows = Session("dsRows" + REPId.ToString())
                dsCol = Session("dsColumns" + REPId.ToString())
                dsFilter = Session("dsFilters" + REPId.ToString())
            Else
                dsRep = objGetData.GetReportDetails(REPId.ToString())
                dsUnitPref = objGetData.GetPref(REPId.ToString())
                dsRows = objGetData.GetUsersDynamicReportRows(REPId.ToString())
                dsCol = objGetData.GetUsersDynamicReportCols(REPId.ToString())
                dsFilter = objGetData.GetUsersReportFilters(REPId.ToString())

                Session("dsRep" + REPId.ToString()) = dsRep
                Session("dsPref" + REPId.ToString()) = dsUnitPref
                Session("dsRows" + REPId.ToString()) = dsRows
                Session("dsColumns" + REPId.ToString()) = dsCol
                Session("dsFilters" + REPId.ToString()) = dsFilter

                hidReport.Value = "1"
            End If

            filterCnt = dsFilter.Tables(0).Rows.Count
            ColCnt = dsCol.Tables(0).Rows.Count

            For a = 0 To dsCol.Tables(0).Rows.Count - 1
                If dsCol.Tables(0).Rows(a).Item("COLUMNVALUETYPE").ToString() <> "Formula" Then
                    YearId = YearId + "" + dsCol.Tables(0).Rows(a).Item("COLUMNVALUEID").ToString() + ","
                End If
            Next
            YearId = YearId.Remove(YearId.Length - 1)
            UnitId = dsRows.Tables(0).Rows(0).Item("UNITID").ToString()

            dvCol = dsCol.Tables(0).DefaultView
            dvRptCols1 = dsCol.Tables(0).DefaultView
            dvRptCols2 = dsCol.Tables(0).DefaultView

            Dim dsTables As New DataSet
            Dim dvTables As New DataView
            Dim dtTables As New DataTable
            Dim ProdId As String = ""
            Dim PackId As String = ""
            Dim dsProd As New DataSet
            Dim dvProd As New DataView
            Dim dtProd As New DataTable
            Dim dsPack As New DataSet
            Dim dvPack As New DataView
            Dim dtPack As New DataTable
            Dim dsOrder As New DataSet()
            Dim funflag As Boolean = False
            Dim MatId As String = ""
            Dim FactId As String = ""
            Dim dsMat As New DataSet
            Dim dsProdMat As New DataSet
            Dim arrRfilt(filterCnt) As String
            Dim CountryId As String = ""
            Dim RegionId As String = ""
            Dim filDes As String
            dsTables = objGetData.GetProductDetails(Session("M1SubGroupId"))
            dvTables = dsTables.Tables(0).DefaultView
            Dim ProdTbl(dsTables.Tables(0).Rows.Count) As String

            For a = 0 To dsFilter.Tables(0).Rows.Count - 1
                filDes = Request.Form("ddlfil_" + a.ToString())
                If filDes <> Nothing Then
                    'filDes = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    Fils(a) = filDes.ToString()
                Else
                    filDes = "0"
                    Fils(a) = "0"
                    If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() <> "0" Then
                        filDes = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    End If
                End If
                If dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PRODUCT" Then
                    arrRfilt(a) = "PRODUCT"
                    ' If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    If filDes = "0" Then

                        For b = 0 To dsTables.Tables(0).Rows.Count - 1
                            ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                            ProdId = ProdId + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                            RowCnt += 1
                        Next
                        dtTables = dsTables.Tables(0)
                        ProdId = ProdId.Remove(ProdId.Length - 1)
                        If PackId <> "" Then
                            dsProd = objGetData.GetPivotPackProducts(ProdId, PackId)

                        Else
                            dsProd = objGetData.GetPivotProductDescription(ProdId)
                        End If
                        ProdId = ""
                        For b = 0 To dsProd.Tables(0).Rows.Count - 1
                            ProdId = ProdId + "" + dsProd.Tables(0).Rows(b).Item("ID").ToString() + ","
                        Next
                        ProdId = ProdId.Remove(ProdId.Length - 1)

                    Else
                        ProdId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                        dvTables.RowFilter = "ACTUALPRODUCTID=" + ProdId
                        dtTables = dvTables.ToTable()
                        ProdTbl(0) = dtTables.Rows(0).Item("TABLENAME").ToString()
                        RowCnt += 1
                        If PackId <> "" Then
                            dsProd = objGetData.GetPivotPackProducts(ProdId, PackId)
                        Else
                            dsProd = objGetData.GetPivotProductDescription(ProdId)
                        End If
                    End If

                    dsProd.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsProd.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsProd.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If
                ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PACKAGE" Then
                    arrRfilt(a) = "PACKAGE"
                    ' If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    If filDes = "0" Then

                        If ProdId <> "" Then
                            dsPack = objGetData.GetPivotProdPackage(ProdId, "")
                        Else
                            For b = 0 To dsTables.Tables(0).Rows.Count - 1
                                ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                                ProdId = ProdId + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                                RowCnt += 1
                            Next
                            dtTables = dsTables.Tables(0)
                            ProdId = ProdId.Remove(ProdId.Length - 1)
                            dsPack = objGetData.GetPackages(ProdId)
                        End If
                        For b = 0 To dsPack.Tables(0).Rows.Count - 1
                            PackId = PackId + "" + dsPack.Tables(0).Rows(b).Item("PACKAGETYPEID").ToString() + ","
                        Next
                        PackId = PackId.Remove(PackId.Length - 1)
                    Else
                        PackId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                        If ProdId <> "" Then
                            dsPack = objGetData.GetPivotProdPackage(ProdId, PackId)

                        Else
                            dsPack = objGetData.GetPivotPackages(PackId)
                        End If
                    End If

                    dsPack.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsPack.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsPack.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If
                ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "COUNTRY" Then
                    arrRfilt(a) = "COUNTRY"
                    'CountryId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    If filDes = "0" Then
                        CountryId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    Else
                        CountryId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    End If
                    dsMat = objGetData.GetPivotCountriesByRegion(CountryId)
                    dsMat.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsMat.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsMat.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If

                ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "REGION" Then
                    arrRfilt(a) = "REGION"
                    ' RegionId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    If filDes = "0" Then
                        RegionId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    Else
                        RegionId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    End If
                    dsMat = objGetData.GetPivotRegion(RegionId)
                    dsMat.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsMat.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsMat.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If

                ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "GROUP" Then
                    arrRfilt(a) = "GROUP"
                    'GrpId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    If filDes = "0" Then
                        GrpId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    Else
                        GrpId = filDes 'dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()

                    End If
                    dsGrp = objGetData.GetPivotGroups(GrpId)
                    dsGrp.Tables(0).TableName = (a + 1).ToString()
                    dsOrder.Tables.Add(dsGrp.Tables((a + 1).ToString()).Copy())
                    If a = dsFilter.Tables(0).Rows.Count - 1 Then
                        If dsGrp.Tables(0).Rows.Count <> 0 Then
                            funflag = True
                        Else
                            funflag = False
                        End If
                    End If
                End If
            Next

            dv = dsRows.Tables(0).DefaultView
            Dim dsRows1 As DataSet
            Dim dv1 As New DataView
            Dim dv2 As New DataView
            Dim dv3 As New DataView
            Dim dt1 As New DataTable
            Dim dt2 As New DataTable
            Dim dt3 As New DataTable
            Dim dsRptAct As New DataSet
            Dim dvRptAct As New DataView
            Dim dtRptAct As New DataTable
            Dim strRFilt As String = ""

            If ProdTbl(0) = "" Then
                For b = 0 To dsTables.Tables(0).Rows.Count - 1
                    ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                    RowCnt += 1
                Next
            End If

            Dim dvRpt As New DataView
            Dim dtRpt As New DataTable

            If funflag Then
                If filterCnt = 1 Then
                    dv1 = dsOrder.Tables(0).DefaultView
                ElseIf filterCnt = 2 Then
                    dv1 = dsOrder.Tables(0).DefaultView
                    dv2 = dsOrder.Tables(1).DefaultView
                ElseIf filterCnt = 3 Then
                    dv1 = dsOrder.Tables(0).DefaultView
                    dv2 = dsOrder.Tables(1).DefaultView
                    dv3 = dsOrder.Tables(2).DefaultView
                End If
                dsRows1 = objGetData.GetPivotComponents(ProdId, PackId)
                dsRptAct = objGetData.GetPivotReportData_COMP(ProdTbl, YearId, ProdId, PackId, GrpId, CountryId, RegionId, UnitId, dsTables.Tables(0).Rows.Count)
                dvRptAct = dsRptAct.Tables(0).DefaultView
                dvRpt = dsRptAct.Tables(0).DefaultView

                '  Changes started 
                If dsRptAct.Tables(0).Rows.Count > 0 Then
                    '  Changes end
                    'CODE FOR GRAPH
                    Dim pcScript = ""
                    Dim odbutil As New DBUtil()
                    Dim Graphtype As String = ""
                    Dim GraphName As String = ""

                    Dim pref As String = String.Empty
                    If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                        pref = "(" + dsRows.Tables(0).Rows(0).Item("UNITSHRT").ToString() + ")"
                    Else
                        pref = "(" + dsRows.Tables(0).Rows(0).Item("METRICUNIT").ToString() + ")"
                    End If
                    Dim Count As String = dtRptAct.Rows.Count
                    Dim Str1 As String = ""
                    'For i = 0 To dsCol.Tables(0).Rows.Count - 1
                    '    Str1 = Str1 + dsCol.Tables(0).Rows(i).Item("COLUMNVALUE") + "; "
                    'Next

                    '  Changes Started 
                    Dim S As String = ""

                    For i = 0 To dsCol.Tables(0).Rows.Count - 1
                        Str1 = Str1 + " "
                        If dsCol.Tables(0).Rows(i).Item("COLUMNVALUE").ToString() = "CAGR" Then

                            S = dsCol.Tables(0).Rows(i).Item("COLUMNVALUE").ToString() + "(" + dsCol.Tables(0).Rows(i).Item("INPUTVALUE1").ToString() + "/" + dsCol.Tables(0).Rows(i).Item("INPUTVALUE2").ToString() + ")"
                            Str1 = Str1 + "" + S + ";"
                        Else
                            Str1 = Str1 + dsCol.Tables(0).Rows(i).Item("COLUMNVALUE") + ";"
                        End If

                    Next
                    '  Changes end




                    Dim Str2 As String = ""
                    For i = 0 To dsRows1.Tables(0).Rows.Count - 1
                        Str2 = Str2 + dsRows1.Tables(0).Rows(i).Item("VALUE") + "; "
                    Next
                    Dim Data As String = String.Empty
                    GraphName = "bar"
                    Graphtype = "graph"
                    'Dim Count1 As String = dtRptAct.Rows.Count
                    dvRpt = dsRptAct.Tables(0).DefaultView

                    pcScript &= "" + Graphtype + ".transposed(true)" + Graphtype + ".setCategories(" + Str1.ToString() + ")"

                    'double bar
                    Dim strt(dsCol.Tables(0).Rows.Count) As String
                    For j = 0 To dsRows1.Tables(0).Rows.Count - 1
                        For i = 0 To dsCol.Tables(0).Rows.Count - 1
                            If i = 0 Then
                                pcScript &= "" + Graphtype + ".setSeries(" & dsRows1.Tables(0).Rows(j).Item("VALUE") & ""
                            End If
                            If dsCol.Tables(0).Rows(i).Item("COLUMNVALUETYPE").ToString() <> "Formula" Then

                                dvRpt.RowFilter = " COMPONENTID = " + dsRows1.Tables(0).Rows(j).Item("COMPONENTID").ToString().Replace(" ", " ") + " AND YEARID= " + dsCol.Tables(0).Rows(i).Item("COLUMNVALUEID").ToString().Replace(" ", " ")
                                dtRpt = dvRpt.ToTable()
                                Dim fct As Double = 0.0
                                Data = ""
                                If dtRpt.Rows.Count > 0 Then
                                    For d = 0 To dtRpt.Rows.Count - 1
                                        fct = fct + FormatNumber(dtRpt.Rows(d).Item("FCT"), 0)
                                    Next
                                    Data = fct.ToString()

                                    ' Data = FormatNumber(dtRpt.Rows(0).Item("FCT").ToString(), 0)
                                    pcScript &= ";" + Data + ""
                                    strt(i) = "" + Data.ToString()
                                Else
                                    pcScript &= ";" + fct.ToString() + ""
                                    strt(i) = "0"
                                End If
                            Else
                                Dim count1 As New Double
                                Dim count2 As New Double
                                Dim diff As New Double
                                dvRptCols1.RowFilter = "USERREPORTCOLUMNID=" + dsCol.Tables(0).Rows(i).Item("INPUTVALUETYPE1").ToString()
                                dtRptCols1 = dvRptCols1.ToTable()
                                dvRptCols2.RowFilter = "USERREPORTCOLUMNID=" + dsCol.Tables(0).Rows(i).Item("INPUTVALUETYPE2").ToString()
                                dtRptCols2 = dvRptCols2.ToTable()
                                dvRptAct.RowFilter = " COMPONENTID = " + dsRows1.Tables(0).Rows(j).Item("COMPONENTID").ToString() + " AND YEARID=" + dtRptCols1.Rows(0).Item("COLUMNVALUEID").ToString()

                                dtRptAct = dvRptAct.ToTable()
                                If dtRptAct.Rows.Count > 0 Then
                                    For l = 0 To dtRptAct.Rows.Count - 1
                                        If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                                            If dtRptAct.Rows(l).Item("FCT").ToString() <> "" Then
                                                If UnitId = 1 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                ElseIf UnitId = 2 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                ElseIf UnitId = 4 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                Else
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                End If
                                            End If
                                        Else
                                            If dtRptAct.Rows(l).Item("FCT").ToString() <> "" Then
                                                If UnitId = 1 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT"), 0)
                                                ElseIf UnitId = 2 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL"), 0)
                                                ElseIf UnitId = 3 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA"), 0)
                                                ElseIf UnitId = 4 Then
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                Else
                                                    count1 = count1 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                End If
                                            End If
                                        End If
                                    Next
                                End If


                                dvRptAct.RowFilter = " COMPONENTID = " + dsRows1.Tables(0).Rows(j).Item("COMPONENTID").ToString() + " AND YEARID=" + dtRptCols2.Rows(0).Item("COLUMNVALUEID").ToString()

                                dtRptAct = dvRptAct.ToTable()
                                If dtRptAct.Rows.Count > 0 Then
                                    For l = 0 To dtRptAct.Rows.Count - 1
                                        If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
                                            If dtRptAct.Rows(l).Item("FCT").ToString() <> "" Then
                                                If UnitId = 1 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                ElseIf UnitId = 2 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                ElseIf UnitId = 4 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                Else
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                End If
                                            End If
                                        Else
                                            If dtRptAct.Rows(l).Item("FCT").ToString() <> "" Then
                                                If UnitId = 1 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTWT"), 0)
                                                ElseIf UnitId = 2 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTVOL"), 0)
                                                ElseIf UnitId = 3 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT") * dsUnitPref.Tables(0).Rows(0).Item("CONVTAREA"), 0)
                                                ElseIf UnitId = 4 Then
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                Else
                                                    count2 = count2 + FormatNumber(dtRptAct.Rows(l).Item("FCT").ToString(), 0)
                                                End If
                                            End If
                                        End If
                                    Next
                                End If
                                diff = dtRptCols2.Rows(0).Item("COLUMNVALUEID") - dtRptCols1.Rows(0).Item("COLUMNVALUEID")
                                Data = FormatNumber((((count2 / count1) ^ (1 / diff)) - 1) * 100, 4)
                                strt(i) = Data
                            End If
                        Next
                        '        pcScript &= ")"
                        '        For k = 0 To dsCol.Tables(0).Rows.Count - 1
                        '            pcScript = pcScript + "" + Graphtype + ".addHoverText(" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString()
                        '            pcScript = pcScript + "" + "," + (j + 1).ToString() + "" ' dsRows1.Tables(0).Rows(j).Item("COUNTRYDES").ToString() + ""
                        '            pcScript = pcScript + "" + ",Year:" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + "<br/>"
                        '            pcScript = pcScript + "" + "Value:" + strt(k).ToString() + ")"
                        '        Next
                        '    Next
                        '    GenrateChart(pcScript, GraphName, pref, dsRows1.Tables(0).Rows.Count * ColCnt)
                        '    lblNOG.Visible = False
                        'Else
                        '    MaterialPrice.Visible = False
                        '    lblNOG.Visible = True
                        '    lblNOG.Text = "No Data For this Combination."

                        'End If

                        '  Changes started

                        pcScript &= ")"
                        Dim stra As String = ""
                        Dim S1 As String = ""
                        Dim S2 As String = ""

                        For k = 0 To dsCol.Tables(0).Rows.Count - 1
                            ' pcScript = pcScript + "" + Graphtype + ".addHoverText(" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + ""

                            pcScript = pcScript + "" + Graphtype

                            If dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() = "CAGR" Then

                                S1 = dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + "(" + dsCol.Tables(0).Rows(k).Item("INPUTVALUE1").ToString() + "/" + dsCol.Tables(0).Rows(k).Item("INPUTVALUE2").ToString() + ")"

                                pcScript = pcScript + ".addHoverText(" + S1 + ""
                            Else
                                pcScript = pcScript + ".addHoverText(" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + ""

                            End If


                            pcScript = pcScript + "" + "," + (j + 1).ToString() + "" ' dsRows1.Tables(0).Rows(j).Item("COUNTRYDES").ToString() + ""

                            pcScript = pcScript + ""

                            If dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() = "CAGR" Then

                                S1 = "(" + dsCol.Tables(0).Rows(k).Item("INPUTVALUE1").ToString() + "/" + dsCol.Tables(0).Rows(k).Item("INPUTVALUE2").ToString() + ")"

                                pcScript = pcScript + "" + ",CAGR:" + S1 + "<br/>"


                            Else

                                pcScript = pcScript + "" + ",Year:" + dsCol.Tables(0).Rows(k).Item("COLUMNVALUE").ToString() + "<br/>"
                            End If


                            pcScript = pcScript + "" + "Value:" + strt(k).ToString() + ")"


                        Next

                    Next



                    GenrateChart(pcScript, GraphName, pref, dsRows1.Tables(0).Rows.Count * ColCnt)
                    lblNOG.Visible = False
                Else
                    MaterialPrice.Visible = False
                    lblNOG.Visible = True
                    lblNOG.Text = "Values Are Not Available for this Report."
                    GetFilters()
                End If
            Else
                MaterialPrice.Visible = False
                lblNOG.Visible = True
                lblNOG.Text = "No Data For this Combination."
                GetFilters()
            End If

            '  Changes end
        Catch ex As Exception
            ErrorLable.Text = "Error:SetPivotCompReportFrameWorkTemp " + ex.Message.ToString()
        End Try
    End Sub

    'SetUniformReportProdPackMat
    Private Sub SetUniformReportProdPackMat(ByVal reportDes As String, ByVal rptType As String)
        Dim objGetData As New Selectdata()
        Dim dsRptRws As New DataSet()

        Dim dsRptFilter As New DataSet()

        Dim dsRptFilterValue As New DataSet()
        Dim dsUnitPref As New DataSet
        Dim ColCnt As New Integer
        Dim RowCnt As New Integer
        Dim FiltCnt As New Integer
        Dim groupId As String = String.Empty
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
        Dim k1 As Integer = 0
        Dim ProdId As String = ""
        Dim PackId As String = ""
        Dim Unit As String = ""
        Dim YearId As String = ""
        Dim dValue As Integer = 1
        Dim dsTables As New DataSet
        Dim dvTables As New DataView
        Dim dtTables As New DataTable
        Dim dsProd As New DataSet
        Dim dvProd As New DataView
        Dim dtProd As New DataTable
        Dim dsRptAct As New DataSet()
        Dim dvRptAct As New DataView()
        Dim dtRptAct As New DataTable()
        Dim dsRow As New DataSet
        Dim dvRow As New DataView
        Dim dtRow As New DataTable
        Dim dsPack As New DataSet
        Dim dvPack As New DataView
        Dim dtPack As New DataTable
        Dim dsRptCols As New DataSet()
        Dim dvRptCols1 As New DataView
        Dim dtRptCols1 As New DataTable
        Dim dvRptCols2 As New DataView
        Dim dtRptCols2 As New DataTable




        If hidReport.Value <> "0" Then
            dsUnitPref = Session("dsPref" + REPId.ToString())
            dsRptRws = Session("dsRows" + REPId.ToString())
            dsRptCols = Session("dsColumns" + REPId.ToString())
            dsRptFilter = Session("dsFilters" + REPId.ToString())
        Else
            dsUnitPref = objGetData.GetPref(REPId.ToString())
            dsTables = objGetData.GetProductDetails(Session("M1SubGroupId"))
            dvTables = dsTables.Tables(0).DefaultView
            dsRptCols = objGetData.GetUsersDynamicReportCols(REPId.ToString())
            dsRptFilter = objGetData.GetUsersReportFilters(REPId.ToString())

            Session("dsPref" + REPId.ToString()) = dsUnitPref
            Session("dsRows" + REPId.ToString()) = dsRptRws
            Session("dsColumns" + REPId.ToString()) = dsRptCols
            Session("dsFilters" + REPId.ToString()) = dsRptFilter

            hidReport.Value = "1"
        End If
        Unit = "1"
        Dim ProdTbl(dsTables.Tables(0).Rows.Count) As String

        For a = 0 To dsRptCols.Tables(0).Rows.Count - 1
            If dsRptCols.Tables(0).Rows(a).Item("COLUMNVALUETYPE").ToString() <> "Formula" Then
                YearId = YearId + "" + dsRptCols.Tables(0).Rows(a).Item("COLUMNVALUEID").ToString() + ","
            End If
        Next
        YearId = YearId.Remove(YearId.Length - 1)

        For a = 0 To dsRptFilter.Tables(0).Rows.Count - 1
            If dsRptFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PRODUCT" Then
                If dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    For b = 0 To dsTables.Tables(0).Rows.Count - 1
                        ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                        ProdId = ProdId + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                        RowCnt += 1
                    Next
                    dtTables = dsTables.Tables(0)
                    ProdId = ProdId.Remove(ProdId.Length - 1)
                Else
                    ProdId = dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    dvTables.RowFilter = "ACTUALPRODUCTID=" + ProdId
                    dtTables = dvTables.ToTable()
                    ProdTbl(0) = dtTables.Rows(0).Item("TABLENAME").ToString()
                    RowCnt += 1
                End If
            ElseIf dsRptFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PACKAGE" Then
                If dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    If ProdId <> "" Then
                        dsPack = objGetData.GetPivotPackage(ProdId)
                    Else

                    End If
                    For b = 0 To dsPack.Tables(0).Rows.Count - 1
                        PackId = PackId + "" + dsPack.Tables(0).Rows(b).Item("ID").ToString() + ","
                        'RowCnt += 1
                    Next
                    PackId = PackId.Remove(PackId.Length - 1)
                Else
                    PackId = dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    dsPack = objGetData.GetProductTypeByPack(PackId)
                End If

            End If
        Next
        Dim dsRows1 As DataSet
        groupId = Session("M1SubGroupId").ToString()
        dsRptAct = objGetData.GetPivotReportData(ProdTbl, "MAT", YearId, PackId, Unit, dsTables.Tables(0).Rows.Count)
        dsProd = objGetData.GetProductDescription(ProdId)
        dsRows1 = objGetData.GetPivotMaterialss(ProdId, PackId)
        Unit = objGetData.GetPivotUnit(REPId.ToString())

        If dsRptCols.Tables.Count > 0 Then
            ColCnt = dsRptCols.Tables(0).Rows.Count
        End If
        If dsRptFilter.Tables.Count > 0 Then
            FiltCnt = dsRptFilter.Tables(0).Rows.Count
        End If

        Dim count As Integer = FiltCnt + ColCnt


        ''CODE FOR GRAPH
        'Dim pcScript = ""
        'Dim odbutil As New DBUtil()
        'Dim Graphtype As String = ""
        'Dim GraphName As String = ""

        'Dim pref As String = String.Empty
        'If dsUnitPref.Tables(0).Rows(0).Item("UNITS").ToString() = 0 Then
        '    pref = "(" + dsRows.Tables(0).Rows(0).Item("UNITSHRT").ToString() + ")"
        'Else
        '    pref = "(" + dsRows.Tables(0).Rows(0).Item("METRICUNIT").ToString() + ")"
        'End If
        'Dim Count As String = dtRptAct.Rows.Count
        'Dim Str1 As String = ""
        'For i = 0 To dsCol.Tables(0).Rows.Count - 1
        '    Str1 = Str1 + dsCol.Tables(0).Rows(i).Item("COLUMNVALUE") + "; "
        'Next
        'Dim Str2 As String = ""
        'For i = 0 To dsRows1.Tables(0).Rows.Count - 1
        '    Str2 = Str2 + dsRows1.Tables(0).Rows(i).Item("VALUE") + "; "
        'Next
        'Dim Data As String = String.Empty
        'GraphName = "bar"
        'Graphtype = "graph"
        'Dim Count1 As String = dtRptAct.Rows.Count
        'dvRpt = dsRptAct.Tables(0).DefaultView

        'pcScript &= "" + Graphtype + ".transposed(true)" + Graphtype + ".setCategories(" + Str1.ToString() + ")"

        ''double bar
        'Dim strt(dsCol.Tables(0).Rows.Count) As String
        'For j = 0 To dsRows1.Tables(0).Rows.Count - 1
        '    For i = 0 To dsCol.Tables(0).Rows.Count - 1
        '        If i = 0 Then
        '            pcScript &= "" + Graphtype + ".setSeries(" & dsRows1.Tables(0).Rows(j).Item("VALUE") & ""
        '        End If
        '        dvRpt.RowFilter = " COMPONENTID = " + dsRows1.Tables(0).Rows(j).Item("COMPONENTID").ToString().Replace(" ", " ") + " AND YEARID= " + dsCol.Tables(0).Rows(i).Item("COLUMNVALUEID").ToString().Replace(" ", " ")
        '        dtRpt = dvRpt.ToTable()
        '        Data = FormatNumber(dtRpt.Rows(0).Item("FCT").ToString(), 0)
        '        pcScript &= ";" + Data + ""
        '        strt(i) = "" + Data.ToString()
        '    Next
        '    pcScript &= ")"
        '    For k = 0 To dsCol.Tables(0).Rows.Count - 1
        '        'pcScript = pcScript + "" + Graphtype + ".addHoverText(" + k.ToString() + "," + (k + 4).ToString() + ",Value:" + strt(k).ToString() + "<br/>)"
        '    Next
        'Next
        'GenrateChart(pcScript, GraphName, pref)


    End Sub

    'SetUniformReportProdPackMatEMEA
    Private Sub SetUniformReportProdPackMatEMEA(ByVal reportDes As String, ByVal rptType As String)
        Dim objGetData As New Selectdata()
        Dim dsRptRws As New DataSet()

        Dim dsRptFilter As New DataSet()

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
        Dim k1 As Integer = 0
        Dim ProdId As String = ""
        Dim ProdTbl(11) As String
        Dim PackId As String = ""
        Dim Unit As String = ""
        Dim YearId As String = ""
        Dim dValue As Integer = 1
        Dim dsTables As New DataSet
        Dim dvTables As New DataView
        Dim dtTables As New DataTable
        Dim dsProd As New DataSet
        Dim dvProd As New DataView
        Dim dtProd As New DataTable
        Dim dsRptAct As New DataSet()
        Dim dvRptAct As New DataView()
        Dim dtRptAct As New DataTable()
        Dim dsRow As New DataSet
        Dim dvRow As New DataView
        Dim dtRow As New DataTable
        Dim dsPack As New DataSet
        Dim dvPack As New DataView
        Dim dtPack As New DataTable
        Dim dsRptCols As New DataSet()
        Dim dvRptCols1 As New DataView
        Dim dtRptCols1 As New DataTable
        Dim dvRptCols2 As New DataView
        Dim dtRptCols2 As New DataTable
        Dim RegID As String = ""


        dsUnitPref = objGetData.GetPref(REPId.ToString())
        dsTables = objGetData.GetProductDetails(Session("M1SubGroupId"))
        dvTables = dsTables.Tables(0).DefaultView
        dsRptCols = objGetData.GetUsersDynamicReportCols(REPId.ToString())
        dsRptFilter = objGetData.GetUsersReportFilters(REPId.ToString())

        Session("dsPref" + REPId.ToString()) = dsUnitPref
        Session("dsRows" + REPId.ToString()) = dsRptRws
        Session("dsColumns" + REPId.ToString()) = dsRptCols
        Session("dsFilters" + REPId.ToString()) = dsRptFilter

        hidReport.Value = "1"
        ' End If
        Unit = "1"
        For a = 0 To dsRptCols.Tables(0).Rows.Count - 1
            If dsRptCols.Tables(0).Rows(a).Item("COLUMNVALUETYPE").ToString() <> "Formula" Then
                YearId = YearId + "" + dsRptCols.Tables(0).Rows(a).Item("COLUMNVALUEID").ToString() + ","
            End If
        Next
        YearId = YearId.Remove(YearId.Length - 1)

        For a = 0 To dsRptFilter.Tables(0).Rows.Count - 1
            If dsRptFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PRODUCT" Then
                If dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    For b = 0 To dsTables.Tables(0).Rows.Count - 1
                        ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                        ProdId = ProdId + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                        RowCnt += 1
                    Next
                    dtTables = dsTables.Tables(0)
                    ProdId = ProdId.Remove(ProdId.Length - 1)
                Else
                    ProdId = dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    dvTables.RowFilter = "ACTUALPRODUCTID=" + ProdId
                    dtTables = dvTables.ToTable()
                    ProdTbl(0) = dtTables.Rows(0).Item("TABLENAME").ToString()
                    RowCnt += 1
                End If
            ElseIf dsRptFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PACKAGE" Then
                If dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    If ProdId <> "" Then
                        dsPack = objGetData.GetPivotPackage(ProdId)
                    Else

                    End If
                    For b = 0 To dsPack.Tables(0).Rows.Count - 1
                        PackId = PackId + "" + dsPack.Tables(0).Rows(b).Item("ID").ToString() + ","
                        'RowCnt += 1
                    Next
                    PackId = PackId.Remove(PackId.Length - 1)
                Else
                    PackId = dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    dsPack = objGetData.GetProductTypeByPack(PackId)
                End If
            ElseIf dsRptFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "REGION" Then
                RegID = dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
            End If
        Next

        groupId = Session("M1SubGroupId").ToString()
        dsRptAct = objGetData.GetPivotReportDataEMEA(ProdTbl, "MAT", YearId, PackId, Unit, RowCnt, RegID)
        dsProd = objGetData.GetProductDescription(ProdId)
        dsRow = objGetData.GetPivotMaterialss(ProdId, PackId)
        Unit = objGetData.GetPivotUnit(REPId.ToString())

        If dsRptCols.Tables.Count > 0 Then
            ColCnt = dsRptCols.Tables(0).Rows.Count
        End If
        If dsRptFilter.Tables.Count > 0 Then
            FiltCnt = dsRptFilter.Tables(0).Rows.Count
        End If

        Dim count As Integer = FiltCnt + ColCnt


        dvProd = dsProd.Tables(0).DefaultView
        dvRptAct = dsRptAct.Tables(0).DefaultView
        dvRow = dsRow.Tables(0).DefaultView
        dvPack = dsPack.Tables(0).DefaultView
        dvRptCols1 = dsRptCols.Tables(0).DefaultView
        dvRptCols2 = dsRptCols.Tables(0).DefaultView

    End Sub

    'SetUniformReport_MAT_PACK_PROD
    Private Sub SetUniformReport_MAT_PACK_PROD(ByVal reportDes As String, ByVal rptType As String)
        Dim objGetData As New Selectdata()
        Dim dsRptRws As New DataSet()
        Dim dsRptCols As New DataSet()
        Dim dsRptFilter As New DataSet()

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
        Dim k1 As Integer = 0
        Dim ProdId As String = ""
        Dim ProdTbl(11) As String
        Dim PackId As String = ""
        Dim Unit As String = ""
        Dim YearId As String = ""
        Dim dValue As Integer = 1
        Dim dsTables As New DataSet
        Dim dvTables As New DataView
        Dim dtTables As New DataTable
        Dim dsProd As New DataSet
        Dim dvProd As New DataView
        Dim dtProd As New DataTable
        Dim dsRptAct As New DataSet()
        Dim dvRptAct As New DataView()
        Dim dtRptAct As New DataTable()
        Dim dsRow As New DataSet
        Dim dvRow As New DataView
        Dim dtRow As New DataTable
        Dim dsPack As New DataSet
        Dim dvPack As New DataView
        Dim dtPack As New DataTable

        If hidReport.Value <> "0" Then
            dsUnitPref = Session("dsPref" + REPId.ToString())
            dsRptRws = Session("dsRows" + REPId.ToString())
            dsRptCols = Session("dsColumns" + REPId.ToString())
            dsRptFilter = Session("dsFilters" + REPId.ToString())
        Else
            dsUnitPref = objGetData.GetPref(REPId.ToString())
            dsTables = objGetData.GetProductDetails(Session("M1SubGroupId"))
            dvTables = dsTables.Tables(0).DefaultView
            dsRptCols = objGetData.GetUsersDynamicReportCols(REPId.ToString())
            dsRptFilter = objGetData.GetUsersReportFilters(REPId.ToString())

            Session("dsPref" + REPId.ToString()) = dsUnitPref
            Session("dsRows" + REPId.ToString()) = dsRptRws
            Session("dsColumns" + REPId.ToString()) = dsRptCols
            Session("dsFilters" + REPId.ToString()) = dsRptFilter

            hidReport.Value = "1"
        End If
        Unit = "1"
        For a = 0 To dsRptCols.Tables(0).Rows.Count - 1
            YearId = YearId + "" + dsRptCols.Tables(0).Rows(a).Item("COLUMNVALUEID").ToString() + ","
        Next
        YearId = YearId.Remove(YearId.Length - 1)
        Dim packCNT As Integer = 0
        For a = 0 To dsRptFilter.Tables(0).Rows.Count - 1
            If dsRptFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PRODUCT" Then
                If dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    For b = 0 To dsTables.Tables(0).Rows.Count - 1
                        ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                        ProdId = ProdId + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                        RowCnt += 1
                    Next
                    ProdId = ProdId.Remove(ProdId.Length - 1)
                Else
                    ProdId = dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    dvTables.RowFilter = "ACTUALPRODUCTID=" + ProdId
                    dtTables = dvTables.ToTable()
                    ProdTbl(0) = dtTables.Rows(0).Item("TABLENAME").ToString()
                    RowCnt += 1
                End If
            ElseIf dsRptFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PACKAGE" Then
                If dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    If ProdId <> "" Then
                        dsPack = objGetData.GetPackageTypeByFact(ProdId)
                    Else

                    End If
                    For b = 0 To dsPack.Tables(0).Rows.Count - 1
                        PackId = PackId + "" + dsPack.Tables(0).Rows(b).Item("ID").ToString() + ","
                        packCNT += 1
                    Next
                    PackId = PackId.Remove(PackId.Length - 1)
                Else
                    PackId = dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    dsPack = objGetData.GetProductTypeByPack(PackId)
                    packCNT = 1
                End If

            End If
        Next

        groupId = Session("M1SubGroupId").ToString()
        dsRptAct = objGetData.GetPivotReportData(ProdTbl, "MAT", YearId, PackId, Unit, dsTables.Tables(0).Rows.Count)
        dsProd = objGetData.GetProductDescription(ProdId)
        dsRow = objGetData.GetPivotMaterialss(ProdId, PackId)
        Dim dsMatPiv As New DataSet
        dsMatPiv = objGetData.GetMedicalDeviceMaterials(ProdId)
        Unit = objGetData.GetPivotUnit(REPId.ToString())

        If dsRptCols.Tables.Count > 0 Then
            ColCnt = dsRptCols.Tables(0).Rows.Count
        End If
        If dsRptFilter.Tables.Count > 0 Then
            FiltCnt = dsRptFilter.Tables(0).Rows.Count
        End If

        Dim count As Integer = FiltCnt + ColCnt


        dvProd = dsProd.Tables(0).DefaultView
        dvRptAct = dsRptAct.Tables(0).DefaultView
        dvRow = dsRow.Tables(0).DefaultView

        Dim dvMatD As New DataView
        Dim dtMatD As New DataTable
        dvMatD = dsMatPiv.Tables(0).DefaultView

    End Sub

    'SetUniformReportPACKPRODMAT_CNTRY
    Private Sub SetUniformReportPACKPRODMAT_CNTRY(ByVal reportDes As String, ByVal rptType As String)
        Dim objGetData As New Selectdata()
        Dim dsRptRws As New DataSet()
        Dim dsRptCols As New DataSet()
        Dim dsRptFilter As New DataSet()

        Dim dsRptFilterValue As New DataSet()
        Dim dsUnitPref As New DataSet
        Dim ColCnt As New Integer
        Dim RowCnt As New Integer
        Dim FiltCnt As New Integer
        Dim groupId As String = String.Empty
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
        Dim ProdId As String = ""
        Dim ProdTbl(11) As String
        Dim PackId As String = ""
        Dim Unit As String = ""
        Dim YearId As String = ""
        Dim dValue As Integer = 1
        Dim dsTables As New DataSet
        Dim dvTables As New DataView
        Dim dtTables As New DataTable
        Dim dsProd As New DataSet
        Dim dvProd As New DataView
        Dim dtProd As New DataTable
        Dim dsRptAct As New DataSet()
        Dim dvRptAct As New DataView()
        Dim dtRptAct As New DataTable()
        Dim dsRow As New DataSet
        Dim dvRow As New DataView
        Dim dtRow As New DataTable
        Dim dsPack As New DataSet
        Dim dvPack As New DataView
        Dim dtPack As New DataTable
        Dim cntryID As String = ""

        If hidReport.Value <> "0" Then
            dsUnitPref = Session("dsPref" + REPId.ToString())
            dsRptRws = Session("dsRows" + REPId.ToString())
            dsRptCols = Session("dsColumns" + REPId.ToString())
            dsRptFilter = Session("dsFilters" + REPId.ToString())
        Else
            dsUnitPref = objGetData.GetPref(REPId.ToString())
            dsTables = objGetData.GetProductDetails(Session("M1SubGroupId"))
            dvTables = dsTables.Tables(0).DefaultView
            dsRptCols = objGetData.GetUsersDynamicReportCols(REPId.ToString())
            dsRptFilter = objGetData.GetUsersReportFilters(REPId.ToString())

            Session("dsPref" + REPId.ToString()) = dsUnitPref
            Session("dsRows" + REPId.ToString()) = dsRptRws
            Session("dsColumns" + REPId.ToString()) = dsRptCols
            Session("dsFilters" + REPId.ToString()) = dsRptFilter

            hidReport.Value = "1"
        End If
        Unit = "1"
        For a = 0 To dsRptCols.Tables(0).Rows.Count - 1
            YearId = YearId + "" + dsRptCols.Tables(0).Rows(a).Item("COLUMNVALUEID").ToString() + ","
        Next
        YearId = YearId.Remove(YearId.Length - 1)

        For a = 0 To dsRptFilter.Tables(0).Rows.Count - 1
            If dsRptFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PRODUCT" Then
                If dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    For b = 0 To dsTables.Tables(0).Rows.Count - 1
                        ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                        ProdId = ProdId + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                        RowCnt += 1
                    Next
                    ProdId = ProdId.Remove(ProdId.Length - 1)
                Else
                    ProdId = dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    dvTables.RowFilter = "ACTUALPRODUCTID=" + ProdId
                    dtTables = dvTables.ToTable()
                    ProdTbl(0) = dtTables.Rows(0).Item("TABLENAME").ToString()
                    RowCnt += 1
                End If
            ElseIf dsRptFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PACKAGE" Then
                If dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    If ProdId <> "" Then
                        dsPack = objGetData.GetPackageTypeByFact(ProdId)
                    Else

                    End If
                    For b = 0 To dsPack.Tables(0).Rows.Count - 1
                        PackId = PackId + "" + dsPack.Tables(0).Rows(b).Item("ID").ToString() + ","
                        'RowCnt += 1
                    Next
                    PackId = PackId.Remove(PackId.Length - 1)
                Else
                    PackId = dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                End If
            ElseIf dsRptFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "COUNTRY" Then
                cntryID = dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
            End If
        Next

        groupId = Session("M1SubGroupId").ToString()
        dsRptAct = objGetData.GetPivotReportDataCNTRY(ProdTbl, "MAT", YearId, PackId, Unit, dsTables.Tables(0).Rows.Count, cntryID)
        dsProd = objGetData.GetProductDescription(ProdId)
        dsRow = objGetData.GetPivotMaterialss(ProdId, PackId)
        Dim dsMatPiv As New DataSet
        dsMatPiv = objGetData.GetMedicalDeviceMaterials(ProdId)

        Dim dsPackTPiv As New DataSet
        Dim dvPackTPiv As New DataView
        Dim dtPackTPiv As New DataTable
        dsPackTPiv = objGetData.GetPivotPackageTYP(ProdId)
        Unit = objGetData.GetPivotUnit(REPId.ToString())

        If dsRptCols.Tables.Count > 0 Then
            ColCnt = dsRptCols.Tables(0).Rows.Count
        End If
        If dsRptFilter.Tables.Count > 0 Then
            FiltCnt = dsRptFilter.Tables(0).Rows.Count
        End If
        Dim count As Integer = FiltCnt + ColCnt
        dvProd = dsProd.Tables(0).DefaultView
        dvRptAct = dsRptAct.Tables(0).DefaultView
        dvRow = dsRow.Tables(0).DefaultView


        dvPackTPiv = dsPackTPiv.Tables(0).DefaultView

    End Sub

    'SetUniformReportPACKPRODMAT_REGION
    Private Sub SetUniformReportPACKPRODMAT_REGION(ByVal reportDes As String, ByVal rptType As String)
        Dim objGetData As New Selectdata()
        Dim dsRptRws As New DataSet()
        Dim dsRptCols As New DataSet()
        Dim dsRptFilter As New DataSet()

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
        Dim ProdId As String = ""
        Dim ProdTbl(11) As String
        Dim PackId As String = ""
        Dim Unit As String = ""
        Dim YearId As String = ""
        Dim dValue As Integer = 1
        Dim dsTables As New DataSet
        Dim dvTables As New DataView
        Dim dtTables As New DataTable
        Dim dsProd As New DataSet
        Dim dvProd As New DataView
        Dim dtProd As New DataTable
        Dim dsRptAct As New DataSet()
        Dim dvRptAct As New DataView()
        Dim dtRptAct As New DataTable()
        Dim dsRow As New DataSet
        Dim dvRow As New DataView
        Dim dtRow As New DataTable
        Dim dsPack As New DataSet
        Dim dvPack As New DataView
        Dim dtPack As New DataTable
        Dim RegID As String = ""

        If hidReport.Value <> "0" Then
            dsUnitPref = Session("dsPref" + REPId.ToString())
            dsRptRws = Session("dsRows" + REPId.ToString())
            dsRptCols = Session("dsColumns" + REPId.ToString())
            dsRptFilter = Session("dsFilters" + REPId.ToString())
        Else
            dsUnitPref = objGetData.GetPref(REPId.ToString())
            dsTables = objGetData.GetProductDetails(Session("M1SubGroupId"))
            dvTables = dsTables.Tables(0).DefaultView
            dsRptCols = objGetData.GetUsersDynamicReportCols(REPId.ToString())
            dsRptFilter = objGetData.GetUsersReportFilters(REPId.ToString())

            Session("dsPref" + REPId.ToString()) = dsUnitPref
            Session("dsRows" + REPId.ToString()) = dsRptRws
            Session("dsColumns" + REPId.ToString()) = dsRptCols
            Session("dsFilters" + REPId.ToString()) = dsRptFilter

            hidReport.Value = "1"
        End If
        Unit = "1"
        For a = 0 To dsRptCols.Tables(0).Rows.Count - 1
            YearId = YearId + "" + dsRptCols.Tables(0).Rows(a).Item("COLUMNVALUEID").ToString() + ","
        Next
        YearId = YearId.Remove(YearId.Length - 1)

        For a = 0 To dsRptFilter.Tables(0).Rows.Count - 1
            If dsRptFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PRODUCT" Then
                If dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    For b = 0 To dsTables.Tables(0).Rows.Count - 1
                        ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                        ProdId = ProdId + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                        RowCnt += 1
                    Next
                    ProdId = ProdId.Remove(ProdId.Length - 1)
                Else
                    ProdId = dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    dvTables.RowFilter = "ACTUALPRODUCTID=" + ProdId
                    dtTables = dvTables.ToTable()
                    ProdTbl(0) = dtTables.Rows(0).Item("TABLENAME").ToString()
                    RowCnt += 1
                End If
            ElseIf dsRptFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PACKAGE" Then
                If dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    If ProdId <> "" Then
                        dsPack = objGetData.GetPackageTypeByFact(ProdId)
                    Else

                    End If
                    For b = 0 To dsPack.Tables(0).Rows.Count - 1
                        PackId = PackId + "" + dsPack.Tables(0).Rows(b).Item("ID").ToString() + ","
                        'RowCnt += 1
                    Next
                    PackId = PackId.Remove(PackId.Length - 1)
                Else
                    PackId = dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                End If
            ElseIf dsRptFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "REGION" Then
                RegID = dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
            End If
        Next

        groupId = Session("M1SubGroupId").ToString()
        dsRptAct = objGetData.GetPivotReportDataREGION(ProdTbl, "MAT", YearId, PackId, Unit, dsTables.Tables(0).Rows.Count, RegID)
        dsProd = objGetData.GetProductDescription(ProdId)
        dsRow = objGetData.GetPivotMaterialss(ProdId, PackId)
        Dim dsMatPiv As New DataSet
        dsMatPiv = objGetData.GetMedicalDeviceMaterials(ProdId)
        Unit = objGetData.GetPivotUnit(REPId.ToString())

        Dim dsPackTPiv As New DataSet
        Dim dvPackTPiv As New DataView
        Dim dtPackTPiv As New DataTable
        dsPackTPiv = objGetData.GetPivotPackageTYP(ProdId)

        If dsRptCols.Tables.Count > 0 Then
            ColCnt = dsRptCols.Tables(0).Rows.Count
        End If
        If dsRptFilter.Tables.Count > 0 Then
            FiltCnt = dsRptFilter.Tables(0).Rows.Count
        End If

        Dim count As Integer = FiltCnt + ColCnt

        dvProd = dsProd.Tables(0).DefaultView
        dvRptAct = dsRptAct.Tables(0).DefaultView
        dvRow = dsRow.Tables(0).DefaultView
        dvPackTPiv = dsPackTPiv.Tables(0).DefaultView

    End Sub

    Private Sub SetUniformReportByRegion(ByVal reportDes As String, ByVal rptType As String)
        Dim objGetData As New Selectdata()
        Dim dsRptRws As New DataSet()

        Dim dsRptFilter As New DataSet()

        Dim dsRptFilterValue As New DataSet()
        Dim dsUnitPref As New DataSet
        Dim ColCnt As New Integer
        Dim RowCnt As New Integer
        Dim FiltCnt As New Integer
        Dim groupId As String = String.Empty
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
        Dim k1 As Integer = 0
        Dim ProdId As String = ""
        Dim ProdTbl(11) As String
        Dim PackId As String = ""
        Dim Unit As String = ""
        Dim YearId As String = ""
        Dim dValue As Integer = 1
        Dim dsTables As New DataSet
        Dim dvTables As New DataView
        Dim dtTables As New DataTable
        Dim dsProd As New DataSet
        Dim dvProd As New DataView
        Dim dtProd As New DataTable
        Dim dsRptAct As New DataSet()
        Dim dvRptAct As New DataView()
        Dim dtRptAct As New DataTable()
        Dim dsRow As New DataSet
        Dim dvRow As New DataView
        Dim dtRow As New DataTable
        Dim dsPack As New DataSet
        Dim dvPack As New DataView
        Dim dtPack As New DataTable
        Dim dsRptCols As New DataSet()
        Dim dvRptCols1 As New DataView
        Dim dtRptCols1 As New DataTable
        Dim dvRptCols2 As New DataView
        Dim dtRptCols2 As New DataTable
        Dim dsCoun As New DataSet
        Dim dsReg As New DataSet
        Dim RegionSet As New Integer

        If hidReport.Value <> "0" Then
            dsUnitPref = Session("dsPref" + REPId.ToString())
            dsRptRws = Session("dsRows" + REPId.ToString())
            dsRptCols = Session("dsColumns" + REPId.ToString())
            dsRptFilter = Session("dsFilters" + REPId.ToString())
        Else
            dsUnitPref = objGetData.GetPref(REPId.ToString())
            dsTables = objGetData.GetProductDetails(Session("M1SubGroupId"))
            dvTables = dsTables.Tables(0).DefaultView
            dsRptCols = objGetData.GetUsersDynamicReportCols(REPId.ToString())
            dsRptFilter = objGetData.GetUsersReportFilters(REPId.ToString())

            Session("dsPref" + REPId.ToString()) = dsUnitPref
            Session("dsRows" + REPId.ToString()) = dsRptRws
            Session("dsColumns" + REPId.ToString()) = dsRptCols
            Session("dsFilters" + REPId.ToString()) = dsRptFilter

            hidReport.Value = "1"
        End If
        Unit = objGetData.GetPivotUnit(REPId.ToString())
        RegionSet = objGetData.GetPivotRegionset(REPId.ToString())
        dsReg = objGetData.GetPivotRegions(RegionSet.ToString())

        For a = 0 To dsRptCols.Tables(0).Rows.Count - 1
            If dsRptCols.Tables(0).Rows(a).Item("COLUMNVALUETYPE").ToString() <> "Formula" Then
                YearId = YearId + "" + dsRptCols.Tables(0).Rows(a).Item("COLUMNVALUEID").ToString() + ","
            End If
        Next
        YearId = YearId.Remove(YearId.Length - 1)

        For a = 0 To dsRptFilter.Tables(0).Rows.Count - 1
            If dsRptFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PRODUCT" Then
                If dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    For b = 0 To dsTables.Tables(0).Rows.Count - 1
                        ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                        ProdId = ProdId + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                        RowCnt += 1
                    Next
                    dtTables = dsTables.Tables(0)
                    ProdId = ProdId.Remove(ProdId.Length - 1)
                Else
                    ProdId = dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    dvTables.RowFilter = "ACTUALPRODUCTID=" + ProdId
                    dtTables = dvTables.ToTable()
                    ProdTbl(0) = dtTables.Rows(0).Item("TABLENAME").ToString()
                    RowCnt += 1
                End If
            ElseIf dsRptFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PACKAGE" Then
                If dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    If ProdId <> "" Then
                        dsPack = objGetData.GetPivotPackage(ProdId)
                    Else
                        For b = 0 To dsTables.Tables(0).Rows.Count - 1
                            ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                            ProdId = ProdId + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                            RowCnt += 1
                        Next
                        dtTables = dsTables.Tables(0)
                        ProdId = ProdId.Remove(ProdId.Length - 1)
                        dsPack = objGetData.GetPivotPackage(ProdId)
                    End If
                    For b = 0 To dsPack.Tables(0).Rows.Count - 1
                        PackId = PackId + "" + dsPack.Tables(0).Rows(b).Item("ID").ToString() + ","
                        'RowCnt += 1
                    Next
                    PackId = PackId.Remove(PackId.Length - 1)
                Else
                    PackId = dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    dsPack = objGetData.GetProductTypeByPack(PackId)
                End If

            End If
        Next

        groupId = Session("M1SubGroupId").ToString()
        dsRptAct = objGetData.GetPivotReportRegData(ProdTbl, YearId, PackId, Unit.ToString(), dsTables.Tables(0).Rows.Count, RegionSet.ToString())
        dsProd = objGetData.GetProductDescription(ProdId)
        dsRow = objGetData.GetPivotMaterialss(ProdId, PackId)
        dsCoun = objGetData.GetPivotCountry()

        If dsRptCols.Tables.Count > 0 Then
            ColCnt = dsRptCols.Tables(0).Rows.Count
        End If
        If dsRptFilter.Tables.Count > 0 Then
            FiltCnt = dsRptFilter.Tables(0).Rows.Count
        End If

        Dim count As Integer = FiltCnt + ColCnt

        dvProd = dsProd.Tables(0).DefaultView
        dvRptAct = dsRptAct.Tables(0).DefaultView
        dvRow = dsRow.Tables(0).DefaultView
        dvPack = dsPack.Tables(0).DefaultView
        dvRptCols1 = dsRptCols.Tables(0).DefaultView
        dvRptCols2 = dsRptCols.Tables(0).DefaultView

    End Sub

    Private Sub SetUniformReportByCountry(ByVal reportDes As String, ByVal rptType As String)
        Dim objGetData As New Selectdata()
        Dim dsRptRws As New DataSet()

        Dim dsRptFilter As New DataSet()

        Dim dsRptFilterValue As New DataSet()
        Dim dsUnitPref As New DataSet
        Dim ColCnt As New Integer
        Dim RowCnt As New Integer
        Dim FiltCnt As New Integer
        Dim groupId As String = String.Empty
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
        Dim k1 As Integer = 0
        Dim ProdId As String = ""
        Dim ProdTbl(11) As String
        Dim PackId As String = ""
        Dim Unit As String = ""
        Dim YearId As String = ""
        Dim dValue As Integer = 1
        Dim dsTables As New DataSet
        Dim dvTables As New DataView
        Dim dtTables As New DataTable
        Dim dsProd As New DataSet
        Dim dvProd As New DataView
        Dim dtProd As New DataTable
        Dim dsRptAct As New DataSet()
        Dim dvRptAct As New DataView()
        Dim dtRptAct As New DataTable()
        Dim dsRow As New DataSet
        Dim dvRow As New DataView
        Dim dtRow As New DataTable
        Dim dsPack As New DataSet
        Dim dvPack As New DataView
        Dim dtPack As New DataTable
        Dim dsRptCols As New DataSet()
        Dim dvRptCols1 As New DataView
        Dim dtRptCols1 As New DataTable
        Dim dvRptCols2 As New DataView
        Dim dtRptCols2 As New DataTable
        Dim dsCoun As New DataSet

        If hidReport.Value <> "0" Then
            dsUnitPref = Session("dsPref" + REPId.ToString())
            dsRptRws = Session("dsRows" + REPId.ToString())
            dsRptCols = Session("dsColumns" + REPId.ToString())
            dsRptFilter = Session("dsFilters" + REPId.ToString())
        Else
            dsUnitPref = objGetData.GetPref(REPId.ToString())
            dsTables = objGetData.GetProductDetails(Session("M1SubGroupId"))
            dvTables = dsTables.Tables(0).DefaultView
            dsRptCols = objGetData.GetUsersDynamicReportCols(REPId.ToString())
            dsRptFilter = objGetData.GetUsersReportFilters(REPId.ToString())

            Session("dsPref" + REPId.ToString()) = dsUnitPref
            Session("dsRows" + REPId.ToString()) = dsRptRws
            Session("dsColumns" + REPId.ToString()) = dsRptCols
            Session("dsFilters" + REPId.ToString()) = dsRptFilter

            hidReport.Value = "1"
        End If
        Unit = objGetData.GetPivotUnit(REPId.ToString())
        For a = 0 To dsRptCols.Tables(0).Rows.Count - 1
            If dsRptCols.Tables(0).Rows(a).Item("COLUMNVALUETYPE").ToString() <> "Formula" Then
                YearId = YearId + "" + dsRptCols.Tables(0).Rows(a).Item("COLUMNVALUEID").ToString() + ","
            End If
        Next
        YearId = YearId.Remove(YearId.Length - 1)

        For a = 0 To dsRptFilter.Tables(0).Rows.Count - 1
            If dsRptFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PRODUCT" Then
                If dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    For b = 0 To dsTables.Tables(0).Rows.Count - 1
                        ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                        ProdId = ProdId + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                        RowCnt += 1
                    Next
                    dtTables = dsTables.Tables(0)
                    ProdId = ProdId.Remove(ProdId.Length - 1)
                Else
                    ProdId = dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    dvTables.RowFilter = "ACTUALPRODUCTID=" + ProdId
                    dtTables = dvTables.ToTable()
                    ProdTbl(0) = dtTables.Rows(0).Item("TABLENAME").ToString()
                    RowCnt += 1
                End If
            ElseIf dsRptFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PACKAGE" Then
                If dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    If ProdId <> "" Then
                        dsPack = objGetData.GetPivotPackage(ProdId)
                    Else

                    End If
                    For b = 0 To dsPack.Tables(0).Rows.Count - 1
                        PackId = PackId + "" + dsPack.Tables(0).Rows(b).Item("ID").ToString() + ","
                        'RowCnt += 1
                    Next
                    PackId = PackId.Remove(PackId.Length - 1)
                Else
                    PackId = dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    dsPack = objGetData.GetProductTypeByPack(PackId)
                End If

            End If
        Next

        groupId = Session("M1SubGroupId").ToString()
        dsRptAct = objGetData.GetPivotReportDataREGION(ProdTbl, "COUNTRY", YearId, PackId, Unit.ToString(), dsTables.Tables(0).Rows.Count, 639)
        dsProd = objGetData.GetProductDescription(ProdId)
        dsRow = objGetData.GetPivotMaterialss(ProdId, PackId)
        dsCoun = objGetData.GetPivotCountry()

        If dsRptCols.Tables.Count > 0 Then
            ColCnt = dsRptCols.Tables(0).Rows.Count
        End If
        If dsRptFilter.Tables.Count > 0 Then
            FiltCnt = dsRptFilter.Tables(0).Rows.Count
        End If

        Dim count As Integer = FiltCnt + ColCnt
        dvProd = dsProd.Tables(0).DefaultView
        dvRptAct = dsRptAct.Tables(0).DefaultView
        dvRow = dsRow.Tables(0).DefaultView
        dvPack = dsPack.Tables(0).DefaultView
        dvRptCols1 = dsRptCols.Tables(0).DefaultView
        dvRptCols2 = dsRptCols.Tables(0).DefaultView

    End Sub

    'SetUniformReportProdRegionBUYER
    Private Sub SetUniformReportProdRegBuyers(ByVal reportDes As String, ByVal rptType As String)
        Dim objGetData As New Selectdata()
        Dim dsRptRws As New DataSet()

        Dim dsRptFilter As New DataSet()

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
        Dim ProdId As String = ""
        Dim ProdTbl(11) As String
        Dim PackId As String = ""
        Dim RegID As String = ""
        Dim Unit As String = ""
        Dim YearId As String = ""
        Dim dValue As Integer = 1
        Dim dsTables As New DataSet
        Dim dvTables As New DataView
        Dim dtTables As New DataTable
        Dim dsProd As New DataSet
        Dim dvProd As New DataView
        Dim dtProd As New DataTable
        Dim dsRptAct As New DataSet()
        Dim dvRptAct As New DataView()
        Dim dtRptAct As New DataTable()
        Dim dsRow As New DataSet
        Dim dvRow As New DataView
        Dim dtRow As New DataTable
        Dim dsPack As New DataSet
        Dim dvPack As New DataView
        Dim dtPack As New DataTable
        Dim dsRptCols As New DataSet()
        Dim dvRptCols1 As New DataView
        Dim dtRptCols1 As New DataTable
        Dim dvRptCols2 As New DataView
        Dim dtRptCols2 As New DataTable
        Dim dsBuyers As New DataSet
        Dim dsReg As New DataSet

        Dim dtBuyers As New DataTable
        Dim dtReg As New DataTable

        Dim dvBuyers As New DataView
        Dim dvReg As New DataView

        dsUnitPref = objGetData.GetPref(REPId.ToString())
        dsTables = objGetData.GetProductDetails(Session("M1SubGroupId"))
        dvTables = dsTables.Tables(0).DefaultView
        dsRptCols = objGetData.GetUsersDynamicReportCols(REPId.ToString())
        dsRptFilter = objGetData.GetUsersReportFilters(REPId.ToString())

        Session("dsPref" + REPId.ToString()) = dsUnitPref
        Session("dsRows" + REPId.ToString()) = dsRptRws
        Session("dsColumns" + REPId.ToString()) = dsRptCols
        Session("dsFilters" + REPId.ToString()) = dsRptFilter

        hidReport.Value = "1"
        'End If
        Unit = "1"
        For a = 0 To dsRptCols.Tables(0).Rows.Count - 1
            If dsRptCols.Tables(0).Rows(a).Item("COLUMNVALUETYPE").ToString() <> "Formula" Then
                YearId = YearId + "" + dsRptCols.Tables(0).Rows(a).Item("COLUMNVALUEID").ToString() + ","
            End If
        Next
        YearId = YearId.Remove(YearId.Length - 1)

        For a = 0 To dsRptFilter.Tables(0).Rows.Count - 1
            If dsRptFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PRODUCT" Then
                If dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    For b = 0 To dsTables.Tables(0).Rows.Count - 1
                        ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                        ProdId = ProdId + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                        RowCnt += 1
                    Next
                    dtTables = dsTables.Tables(0)
                    ProdId = ProdId.Remove(ProdId.Length - 1)
                Else
                    ProdId = dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    dvTables.RowFilter = "ACTUALPRODUCTID=" + ProdId
                    dtTables = dvTables.ToTable()
                    ProdTbl(0) = dtTables.Rows(0).Item("TABLENAME").ToString()
                    RowCnt += 1
                End If
            ElseIf dsRptFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PACKAGE" Then
                If dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    If ProdId <> "" Then
                        dsPack = objGetData.GetPivotPackage(ProdId)
                    Else

                    End If
                    For b = 0 To dsPack.Tables(0).Rows.Count - 1
                        PackId = PackId + "" + dsPack.Tables(0).Rows(b).Item("ID").ToString() + ","
                        'RowCnt += 1
                    Next
                    PackId = PackId.Remove(PackId.Length - 1)
                Else
                    PackId = dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    dsPack = objGetData.GetProductTypeByPack(PackId)
                End If
            ElseIf dsRptFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "REGION" Then
                RegID = dsRptFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
            End If
        Next

        groupId = Session("M1SubGroupId").ToString()
        dsRptAct = objGetData.GetPivotBuyersReportData(ProdTbl, "BUYER", YearId, RegID, 5, RowCnt)
        dsProd = objGetData.GetProductDescription(ProdId)
        dsRow = objGetData.GetPivotMaterialss(ProdId, PackId)
        dsBuyers = objGetData.GetPivotBuyers(ProdId)
        dsReg = objGetData.GetPivotRegionn(RegID)
        Unit = objGetData.GetPivotUnit(REPId.ToString())
        If dsRptCols.Tables.Count > 0 Then
            ColCnt = dsRptCols.Tables(0).Rows.Count
        End If
        If dsRptFilter.Tables.Count > 0 Then
            FiltCnt = dsRptFilter.Tables(0).Rows.Count
        End If

        Dim count As Integer = FiltCnt + ColCnt

        dvProd = dsProd.Tables(0).DefaultView
        dvRptAct = dsRptAct.Tables(0).DefaultView
        dvRptCols1 = dsRptCols.Tables(0).DefaultView
        dvRptCols2 = dsRptCols.Tables(0).DefaultView

    End Sub

    ' Changes Started
    Protected Sub GetFilters()
        Dim objGetData As New Selectdata()
        Dim dsFilter As New DataSet()
        Dim dsProd As New DataSet()
        Dim dsPack As New DataSet()
        Dim dsComp As New DataSet()

        Dim dsTables As New DataSet()
        Dim dvTables As New DataView
        Dim dtTables As New DataTable
        Dim RowCnt As New Integer
        Dim dvProd As New DataView
        Dim dtProd As New DataTable
        Dim dvPack As New DataView
        Dim dtPack As New DataTable
        Dim dsOrder As New DataSet()
        Dim ProdId As String = ""
        Dim PackId As String = ""
        Dim MatId As String = ""
        Dim CompId As String = ""
        Dim FactId As String = ""
        Dim dsMat As New DataSet
        Dim GrpId As String = ""
        Dim dsGrp As New DataSet
        Dim dsProdMat As New DataSet
        Dim CountryId As String = ""
        Dim RegionId As String = ""
        dsFilter = objGetData.GetUsersReportFilters(REPId.ToString())

        tblFil.Controls.Clear()
        Dim tr As TableRow
        Dim td As TableCell
        Dim ddlfil As DropDownList
        Dim lblfil As Label
        dsTables = objGetData.GetProductDetails(Session("M1SubGroupId"))
        dvTables = dsTables.Tables(0).DefaultView
        Dim ProdTbl(dsTables.Tables(0).Rows.Count) As String

        For a = 0 To dsFilter.Tables(0).Rows.Count - 1
            ddlfil = New DropDownList
            lblfil = New Label
            tr = New TableRow
            ddlfil.ID = "ddlfil_" + a.ToString()
            ddlfil.Width = 150
            If hidfil.Value = "0" Then
                Fils(a) = ""
            End If
            If dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PRODUCT" Then
                td = New TableCell
                lblfil.Text = "<b>Select Product: </b>"
                td.Controls.Add(lblfil)
                tr.Controls.Add(td)

                td = New TableCell

                If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    Dim lst As ListItem = New ListItem("All Product", "0")
                    ddlfil.Items.Add(lst)
                    ddlfil.AppendDataBoundItems = True
                    For b = 0 To dsTables.Tables(0).Rows.Count - 1
                        ProdTbl(b) = dsTables.Tables(0).Rows(b).Item("TABLENAME").ToString()
                        ProdId = ProdId + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                        RowCnt += 1
                    Next
                    dtTables = dsTables.Tables(0)
                    ProdId = ProdId.Remove(ProdId.Length - 1)
                    If MatId <> "" And PackId <> "" Then
                        dsProd = objGetData.GetPivotPackMatProductsDistinct(ProdId, PackId, MatId)
                    ElseIf CompId <> "" And PackId <> "" Then
                        dsProd = objGetData.GetPivotPackCompProductsDistinct(ProdId, PackId, CompId)
                    ElseIf MatId <> "" Then
                        dsProd = objGetData.GetPivotMatProductsDistinct(MatId, ProdId)
                    ElseIf CompId <> "" Then
                        dsProd = objGetData.GetPivotCompProductsDistinct(CompId, ProdId)
                    ElseIf PackId <> "" Then
                        dsProd = objGetData.GetPivotPackProductsDistinct(ProdId, PackId)
                    Else
                        dsProd = objGetData.GetPivotProductDescription(ProdId)
                    End If
                    ProdId = ""
                    For b = 0 To dsProd.Tables(0).Rows.Count - 1
                        ProdId = ProdId + "" + dsProd.Tables(0).Rows(b).Item("ID").ToString() + ","
                    Next
                    ProdId = ProdId.Remove(ProdId.Length - 1)
                Else
                    ProdId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    dvTables.RowFilter = "ACTUALPRODUCTID=" + ProdId
                    dtTables = dvTables.ToTable()
                    ProdTbl(0) = dtTables.Rows(0).Item("TABLENAME").ToString()
                    RowCnt += 1
                    If MatId <> "" And PackId <> "" Then
                        dsProd = objGetData.GetPivotPackMatProductsDistinct(ProdId, PackId, MatId)
                    ElseIf CompId <> "" And PackId <> "" Then
                        dsProd = objGetData.GetPivotPackCompProductsDistinct(ProdId, PackId, CompId)
                    ElseIf MatId <> "" Then
                        dsProd = objGetData.GetPivotMatProductsDistinct(MatId, ProdId)
                    ElseIf CompId <> "" Then
                        dsProd = objGetData.GetPivotCompProductsDistinct(CompId, ProdId)
                    ElseIf PackId <> "" Then
                        dsProd = objGetData.GetPivotPackProductsDistinct(ProdId, PackId)
                    Else
                        dsProd = objGetData.GetPivotProductDescription(ProdId)
                    End If
                End If
                ' dsProd.Tables(0).TableName = (a + 1).ToString()
                '  dsOrder.Tables.Add(dsProd.Tables((a + 1).ToString()).Copy())


                With ddlfil
                    .DataSource = dsProd
                    .DataTextField = "VALUE"
                    .DataValueField = "ID"
                    .DataBind()
                    .Enabled = False
                End With
                If Fils(a).ToString() <> "" Then
                    ddlfil.SelectedValue = Fils(a).ToString()
                Else
                    If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID") = "0" Then
                        ddlfil.SelectedValue = 0 'dsProd.Tables(0).Rows(0).Item("ID").ToString()
                    Else
                        ddlfil.SelectedValue = dsProd.Tables(0).Rows(0).Item("ID").ToString()
                    End If
                End If
                td = New TableCell
                td.Controls.Add(ddlfil)
                tr.Controls.Add(td)

            ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "PACKAGE" Then
                td = New TableCell
                lblfil.Text = "<b>Select Package: </b>"
                td.Controls.Add(lblfil)
                tr.Controls.Add(td)

                td = New TableCell
                If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    If ProdId <> "" And MatId <> "" Then
                        dsPack = objGetData.GetPivotProdMatPackagesDistinct(ProdId, MatId, "")
                    ElseIf ProdId <> "" And CompId <> "" Then
                        dsPack = objGetData.GetProdCompPackagesDistinct(ProdId, CompId, "")
                    ElseIf ProdId <> "" Then
                        dsPack = objGetData.GetPivotProdPackageDistinct(ProdId, "")
                    Else
                        Dim ProdId1 As String = ""
                        For b = 0 To dsTables.Tables(0).Rows.Count - 1
                            ProdId1 = ProdId1 + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                        Next
                        ProdId1 = ProdId1.Remove(ProdId1.Length - 1)
                        If MatId <> "" Then
                            dsPack = objGetData.GetPivotMatAllPackagesDistinct(ProdId1, MatId)
                        ElseIf CompId <> "" Then
                            dsPack = objGetData.GetPivotCompAllPackagesDistinct(ProdId1, CompId)
                        Else
                            dsPack = objGetData.GetPackages(ProdId1)
                        End If
                    End If
                    For b = 0 To dsPack.Tables(0).Rows.Count - 1
                        PackId = PackId + "" + dsPack.Tables(0).Rows(b).Item("PACKAGETYPEID").ToString() + ","
                    Next
                    PackId = PackId.Remove(PackId.Length - 1)
                    Dim lst As ListItem = New ListItem("All Package", "0")
                    ddlfil.Items.Add(lst)
                    'ddlfil.Items.Insert(0, "All Package")
                    ddlfil.AppendDataBoundItems = True

                Else
                    PackId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    If ProdId <> "" And MatId <> "" Then
                        dsPack = objGetData.GetPivotProdMatPackagesDistinct(ProdId, MatId, PackId)
                    ElseIf ProdId <> "" And CompId <> "" Then
                        dsPack = objGetData.GetProdCompPackagesDistinct(ProdId, CompId, PackId)
                    ElseIf ProdId <> "" Then
                        dsPack = objGetData.GetPivotProdPackageDistinct(ProdId, PackId)
                    ElseIf MatId <> "" Then
                        dsPack = objGetData.GetPivotMatPackages(MatId, PackId)
                    ElseIf CompId <> "" Then
                        dsPack = objGetData.GetPivotCompPackages(PackId, CompId)
                    Else
                        dsPack = objGetData.GetPivotPackages(PackId)
                    End If
                End If
                'dsPack.Tables(0).TableName = (a + 1).ToString()
                'dsOrder.Tables.Add(dsPack.Tables((a + 1).ToString()).Copy())

                If dsPack.Tables(0).Rows.Count > 0 Then
                    With ddlfil
                        .DataSource = dsPack
                        .DataTextField = "VALUE"
                        .DataValueField = "PACKAGETYPEID"
                        .DataBind()
                        .Enabled = False
                    End With
                    If Fils(a).ToString() <> "" Then
                        ddlfil.SelectedValue = Fils(a).ToString()
                    Else
                        If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID") = "0" Then
                            ddlfil.SelectedValue = 0
                        Else
                            ddlfil.SelectedValue = dsPack.Tables(0).Rows(0).Item("PACKAGETYPEID").ToString()

                        End If
                    End If
                    td.Controls.Add(ddlfil)
                    tr.Controls.Add(td)
                Else

                    For i = 0 To dsFilter.Tables(0).Rows.Count - 1
                        If PackId = dsFilter.Tables(0).Rows(i).Item("FILTERVALUEID").ToString() Then
                            ddlfil.SelectedValue = dsFilter.Tables(0).Rows(i).Item("FILTERVALUEID").ToString()
                            With ddlfil

                                .DataSource = dsFilter
                                .SelectedValue = dsFilter.Tables(0).Rows(i).Item("FILTERVALUEID").ToString()
                                .DataTextField = "FILTERVALUE"
                                .DataValueField = "FILTERVALUEID"
                                .DataBind()
                                .Enabled = False
                            End With


                        End If
                    Next
                    lblNOG.Text = "No Data For this Combination."
                End If
                td.Controls.Add(ddlfil)
                tr.Controls.Add(td)


            ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "MATERIAL" Then
                td = New TableCell
                lblfil.Text = "<b>Select Material: </b>"
                td.Controls.Add(lblfil)
                tr.Controls.Add(td)
                td = New TableCell


                MatId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                If ProdId <> "" And PackId <> "" Then
                    dsMat = objGetData.GetPivotPackProdMaterialsDistinct(ProdId, PackId, MatId)
                ElseIf ProdId <> "" Then
                    dsMat = objGetData.GetPivotProdMaterialsDistinct(ProdId, MatId)
                ElseIf PackId <> "" Then
                    Dim ProdId1 As String = ""
                    For b = 0 To dsTables.Tables(0).Rows.Count - 1
                        ProdId1 = ProdId1 + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                    Next
                    ProdId1 = ProdId1.Remove(ProdId1.Length - 1)
                    dsMat = objGetData.GetPivotPackMaterialsDistinct(ProdId1, PackId, MatId)
                Else
                    dsMat = objGetData.GetPivotMaterials(MatId)
                End If
                'dsMat.Tables(0).TableName = (a + 1).ToString()
                'dsOrder.Tables.Add(dsMat.Tables((a + 1).ToString()).Copy())
                If dsMat.Tables(0).Rows.Count > 0 Then

                    td = New TableCell
                    lblfil.Text = "<b>Select Material: </b>"
                    td.Controls.Add(lblfil)
                    tr.Controls.Add(td)
                    td = New TableCell
                    With ddlfil
                        .DataSource = dsMat
                        .DataTextField = "VALUE"
                        .DataValueField = "MATERIALID"
                        .DataBind()
                        .Enabled = False
                    End With
                    If Fils(a).ToString() <> "" Then
                        ddlfil.SelectedValue = Fils(a).ToString()
                    Else
                        ddlfil.SelectedValue = dsMat.Tables(0).Rows(0).Item("MATERIALID").ToString()
                    End If

                    td.Controls.Add(ddlfil)
                    tr.Controls.Add(td)
                Else

                    For i = 0 To dsFilter.Tables(0).Rows.Count - 1
                        If MatId = dsFilter.Tables(0).Rows(i).Item("FILTERVALUEID").ToString() Then
                            ddlfil.SelectedValue = dsFilter.Tables(0).Rows(i).Item("FILTERVALUEID").ToString()
                            With ddlfil

                                .DataSource = dsFilter
                                .SelectedValue = dsFilter.Tables(0).Rows(i).Item("FILTERVALUEID").ToString()
                                .DataTextField = "FILTERVALUE"
                                .DataValueField = "FILTERVALUEID"
                                .DataBind()
                                .Enabled = False
                            End With


                        End If
                    Next

                    lblNOG.Text = "No Data For this Combination."
                End If
                td.Controls.Add(ddlfil)
                tr.Controls.Add(td)







            ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "COMPONENT" Then
                td = New TableCell
                lblfil.Text = "<b>Select Component: </b>"
                td.Controls.Add(lblfil)
                tr.Controls.Add(td)

                td = New TableCell
                If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString() = "0" Then
                    Dim lst As ListItem = New ListItem("All Component", "0")
                    ddlfil.Items.Add(lst)
                    ddlfil.AppendDataBoundItems = True

                    If ProdId <> "" And PackId <> "" Then
                        dsComp = objGetData.GetPivotPackProdComponentsDistinct(ProdId, PackId, "")

                    ElseIf ProdId <> "" Then
                        dsComp = objGetData.GetPivotProdComponentsDistict(ProdId, "")

                    Else
                        Dim ProdId1 As String = ""
                        For b = 0 To dsTables.Tables(0).Rows.Count - 1
                            ProdId1 = ProdId1 + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                        Next
                        ProdId1 = ProdId1.Remove(ProdId1.Length - 1)
                        If PackId <> "" Then
                            dsComp = objGetData.GetPivotPackProdComponentsDistinct(ProdId1, PackId, "")
                        Else
                            dsComp = objGetData.GetPivotAllComponents(ProdId1)
                        End If
                    End If
                    For b = 0 To dsComp.Tables(0).Rows.Count - 1
                        CompId = CompId + "" + dsComp.Tables(0).Rows(b).Item("COMPONENTID").ToString() + ","
                    Next
                    CompId = CompId.Remove(CompId.Length - 1)
                Else
                    CompId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                    If ProdId <> "" And PackId <> "" Then
                        dsComp = objGetData.GetPivotPackProdComponents(ProdId, PackId, CompId)

                    ElseIf ProdId <> "" Then
                        dsComp = objGetData.GetPivotProdComponents(ProdId, CompId)

                    ElseIf PackId <> "" Then
                        Dim ProdId1 As String = ""
                        For b = 0 To dsTables.Tables(0).Rows.Count - 1
                            ProdId1 = ProdId1 + "" + dsTables.Tables(0).Rows(b).Item("ACTUALPRODUCTID").ToString() + ","
                        Next
                        ProdId1 = ProdId1.Remove(ProdId1.Length - 1)
                        dsComp = objGetData.GetPivotPackComponents(ProdId1, PackId, CompId)

                    Else
                        dsComp = objGetData.GetRepComponent(CompId)
                    End If
                End If

                'dsComp.Tables(0).TableName = (a + 1).ToString()
                'dsOrder.Tables.Add(dsComp.Tables((a + 1).ToString()).Copy())

                With ddlfil
                    .DataSource = dsComp
                    .DataTextField = "VALUE"
                    .DataValueField = "COMPONENTID"
                    .DataBind()
                    .Enabled = False
                End With
                If Fils(a).ToString() <> "" Then
                    ddlfil.SelectedValue = Fils(a).ToString()
                Else
                    'ddlfil.SelectedValue = "0" 'dsComp.Tables(0).Rows(0).Item("COMPONENTID").ToString()
                    If dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID") = "0" Then
                        ddlfil.SelectedValue = 0
                    Else
                        ddlfil.SelectedValue = dsComp.Tables(0).Rows(0).Item("COMPONENTID").ToString()
                    End If
                End If

                td.Controls.Add(ddlfil)
                tr.Controls.Add(td)


            ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "GROUP" Then
                td = New TableCell
                lblfil.Text = "<b>Select Group: </b>"
                td.Controls.Add(lblfil)
                tr.Controls.Add(td)

                td = New TableCell ''
                GrpId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                dsGrp = objGetData.GetPivotGroups(GrpId)
                'dsGrp.Tables(0).TableName = (a + 1).ToString()
                'dsOrder.Tables.Add(dsGrp.Tables((a + 1).ToString()).Copy())
                With ddlfil
                    .DataSource = dsGrp
                    .DataTextField = "VALUE"
                    .DataValueField = "SUBGROUPID"
                    .DataBind()
                    .Enabled = False
                End With
                If Fils(a).ToString() <> "" Then
                    ddlfil.SelectedValue = Fils(a).ToString()
                Else
                    ddlfil.SelectedValue = dsGrp.Tables(0).Rows(0).Item("SUBGROUPID").ToString()

                End If
                td.Controls.Add(ddlfil)
                tr.Controls.Add(td)




            ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "COUNTRY" Then
                td = New TableCell
                lblfil.Text = "<b>Select Country: </b>"
                td.Controls.Add(lblfil)
                tr.Controls.Add(td)
                td = New TableCell
                CountryId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                dsMat = objGetData.GetPivotCountriesByRegion(CountryId)
                'dsMat.Tables(0).TableName = (a + 1).ToString()
                'dsOrder.Tables.Add(dsMat.Tables((a + 1).ToString()).Copy())
                With ddlfil
                    .DataSource = dsMat
                    .DataTextField = "VALUE"
                    .DataValueField = "COUNTRYID"
                    .DataBind()
                    .Enabled = False
                End With
                If Fils(a).ToString() <> "" Then
                    ddlfil.SelectedValue = Fils(a).ToString()
                Else
                    ddlfil.SelectedValue = dsMat.Tables(0).Rows(0).Item("COUNTRYID").ToString()

                End If

                td.Controls.Add(ddlfil)
                tr.Controls.Add(td)

            ElseIf dsFilter.Tables(0).Rows(a).Item("FILTERTYPE").ToString().ToUpper() = "REGION" Then
                td = New TableCell
                lblfil.Text = "<b>Select Region: </b>"
                td.Controls.Add(lblfil)
                tr.Controls.Add(td)
                td = New TableCell
                RegionId = dsFilter.Tables(0).Rows(a).Item("FILTERVALUEID").ToString()
                dsMat = objGetData.GetPivotRegion(RegionId)
                'dsMat.Tables(0).TableName = (a + 1).ToString()
                'dsOrder.Tables.Add(dsMat.Tables((a + 1).ToString()).Copy())
                With ddlfil
                    .DataSource = dsMat
                    .DataTextField = "VALUE"
                    .DataValueField = "REGIONID"
                    .DataBind()
                    .Enabled = False

                End With

                If Fils(a).ToString() <> "" Then
                    ddlfil.SelectedValue = Fils(a).ToString()
                Else
                    ddlfil.SelectedValue = dsMat.Tables(0).Rows(0).Item("REGIONID").ToString()
                End If
                td.Controls.Add(ddlfil)
                tr.Controls.Add(td)

            End If
            tblFil.Controls.Add(tr)
        Next
        hidfil.Value = "1"
        btnSumit.Enabled = False

    End Sub

    ' changes ends



    Protected Sub btnSumit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSumit.Click
        tblFil.Visible = True
        GetPageDetails()
        GetFiltersDropDown()
    End Sub
End Class
