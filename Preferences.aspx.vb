Imports System.Data
Imports System
Imports M1SubGetData
Imports M1SubUpInsData
Partial Class Pages_Market1_CAGR_Preferences
    Inherits System.Web.UI.Page
#Region "Get Set Variables"
    Dim _lErrorLble As Label
    Dim _iUserId As Integer
    Dim _btnUpdate As ImageButton
    Dim _divMainHeading As HtmlGenericControl
    Dim _ctlContentPlaceHolder As ContentPlaceHolder

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



    Public DataCnt As Integer
    Public CaseDesp As New ArrayList
#End Region
#Region "MastePage Content Variables"

    Protected Sub GetMasterPageControls()
        GetErrorLable()
        GetUpdatebtn()
        GetContentPlaceHolder()
    End Sub

    Protected Sub GetErrorLable()
        ErrorLable = Page.Master.FindControl("lblError")
    End Sub

    Protected Sub GetUpdatebtn()
        Updatebtn = Page.Master.FindControl("imgUpdate")
        Updatebtn.Visible = True
        AddHandler Updatebtn.Click, AddressOf Update_Click
    End Sub

    Protected Sub GetContentPlaceHolder()
        ctlContentPlaceHolder = Page.Master.FindControl("Content1")
    End Sub

#End Region
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            GetMasterPageControls()
            REPId = Request.QueryString("RepID").ToString()
            If Not IsPostBack Then
                GetPageDetails()
            End If
        Catch ex As Exception

        End Try
    End Sub
    Protected Sub GetPageDetails()
        Dim ds As New DataSet
        Dim objGetData As New Selectdata()
        Try

            ds = objGetData.GetPref(REPId.ToString())
            If ds.Tables(0).Rows.Count > 0 Then
                If ds.Tables(0).Rows(0).Item("UNITS").ToString() = "1" Then
                    rdMetric.Checked = True
                    rdEnglish.Checked = False
                Else
                    rdMetric.Checked = False
                    rdEnglish.Checked = True
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub
    Protected Sub Update_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
        Dim Unit As String = String.Empty
        Dim ObjUpIns As New UpdateInsert()
        Try
            If rdEnglish.Checked Then
                Unit = "0"
            Else
                Unit = "1"
            End If
            ObjUpIns.PrefrencesUpdate(REPId, Unit)
            Session("PrefChange" + REPId.ToString()) = "1"
        Catch ex As Exception

        End Try
    End Sub
End Class
