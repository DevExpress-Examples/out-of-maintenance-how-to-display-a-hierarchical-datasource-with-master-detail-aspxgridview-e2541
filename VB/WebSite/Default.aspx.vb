Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports DevExpress.Web
Imports System.Data

Partial Public Class _Default
	Inherits System.Web.UI.Page
	Public Function GetData(ByVal reportsTo As Integer) As DataTable
		If reportsTo = -1 Then
			sds.SelectCommand = "SELECT [EmployeeID], [LastName], [FirstName], [Country] FROM [Employees] WHERE ([ReportsTo] IS NULL)"
		Else
			sds.SelectCommand = String.Format("SELECT [EmployeeID], [LastName], [FirstName], [Country] FROM [Employees] WHERE ([ReportsTo] = {0})", reportsTo)
		End If

		Dim dataTable As DataTable = (TryCast(sds.Select(DataSourceSelectArguments.Empty), DataView)).Table

		Return dataTable
	End Function

	Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs)
		grid.Templates.DetailRow = New MyDetailTemplate()
		grid.DataBind()
	End Sub

	Protected Sub grid_BeforePerformDataSelect(ByVal sender As Object, ByVal e As EventArgs)
		Dim grid As ASPxGridView = TryCast(sender, ASPxGridView)
		grid.DataSource = GetData(-1)
	End Sub

	Protected Sub grid_DetailRowGetButtonVisibility(ByVal sender As Object, ByVal e As ASPxGridViewDetailRowButtonEventArgs)
		Dim grid As ASPxGridView = TryCast(sender, ASPxGridView)
		Dim container As GridViewDetailRowTemplateContainer = TryCast(grid.NamingContainer, GridViewDetailRowTemplateContainer)

		Dim dataTable As DataTable = GetData(Convert.ToInt32(e.KeyValue))
		If dataTable.Rows.Count = 0 Then
			e.ButtonState = GridViewDetailRowButtonState.Hidden
		End If
	End Sub
End Class

Public Class MyDetailTemplate
	Implements ITemplate
	Protected Sub grid_DetailRowGetButtonVisibility(ByVal sender As Object, ByVal e As ASPxGridViewDetailRowButtonEventArgs)
		Dim grid As ASPxGridView = TryCast(sender, ASPxGridView)
		Dim container As GridViewDetailRowTemplateContainer = TryCast(grid.NamingContainer, GridViewDetailRowTemplateContainer)

		Dim dataTable As DataTable = (TryCast(container.Grid.Page, _Default)).GetData(Convert.ToInt32(e.KeyValue))
		If dataTable.Rows.Count = 0 Then
			e.ButtonState = GridViewDetailRowButtonState.Hidden
		End If
	End Sub

	Private Sub InstantiateIn(ByVal _container As Control) Implements ITemplate.InstantiateIn
		Dim container As GridViewDetailRowTemplateContainer = TryCast(_container, GridViewDetailRowTemplateContainer)

		Dim grid As New ASPxGridView()
		grid.ID = "grid"

		_container.Controls.Add(grid)

		grid.Width = Unit.Percentage(100)
		grid.Styles.DetailCell.Paddings.Padding = Unit.Pixel(0)

		grid.DataSource = (TryCast(container.Grid.Page, _Default)).GetData(Convert.ToInt32(container.KeyValue))

		grid.KeyFieldName = "EmployeeID"
		grid.SettingsDetail.ShowDetailRow = True
		grid.Templates.DetailRow = Me

		AddHandler grid.DetailRowGetButtonVisibility, AddressOf grid_DetailRowGetButtonVisibility
	End Sub
End Class