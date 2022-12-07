using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using System.Data;

public partial class _Default : System.Web.UI.Page {
    public DataTable GetData(int reportsTo) {
        if (reportsTo == -1)
            sds.SelectCommand = "SELECT [EmployeeID], [LastName], [FirstName], [Country] FROM [Employees] WHERE ([ReportsTo] IS NULL)";
        else
            sds.SelectCommand = String.Format("SELECT [EmployeeID], [LastName], [FirstName], [Country] FROM [Employees] WHERE ([ReportsTo] = {0})", reportsTo);

        DataTable dataTable = (sds.Select(DataSourceSelectArguments.Empty) as DataView).Table;

        return dataTable;
    }

    protected void Page_Init(object sender, EventArgs e) {
        grid.Templates.DetailRow = new MyDetailTemplate();
        grid.DataBind();
    }

    protected void grid_BeforePerformDataSelect(object sender, EventArgs e) {
        ASPxGridView grid = sender as ASPxGridView;
        grid.DataSource = GetData(-1);
    }

    protected void grid_DetailRowGetButtonVisibility(object sender, ASPxGridViewDetailRowButtonEventArgs e) {
        ASPxGridView grid = sender as ASPxGridView;
        GridViewDetailRowTemplateContainer container = grid.NamingContainer as GridViewDetailRowTemplateContainer;

        DataTable dataTable = GetData(Convert.ToInt32(e.KeyValue));
        if (dataTable.Rows.Count == 0)
            e.ButtonState = GridViewDetailRowButtonState.Hidden;
    }
}

public class MyDetailTemplate : ITemplate {
    protected void grid_DetailRowGetButtonVisibility(object sender, ASPxGridViewDetailRowButtonEventArgs e) {
        ASPxGridView grid = sender as ASPxGridView;
        GridViewDetailRowTemplateContainer container = grid.NamingContainer as GridViewDetailRowTemplateContainer;

        DataTable dataTable = (container.Grid.Page as _Default).GetData(Convert.ToInt32(e.KeyValue));
        if (dataTable.Rows.Count == 0)
            e.ButtonState = GridViewDetailRowButtonState.Hidden;
    }

    void ITemplate.InstantiateIn(Control _container) {
        GridViewDetailRowTemplateContainer container = _container as GridViewDetailRowTemplateContainer;

        ASPxGridView grid = new ASPxGridView();
        grid.ID = "grid";

        _container.Controls.Add(grid);

        grid.Width = Unit.Percentage(100);
        grid.Styles.DetailCell.Paddings.Padding = Unit.Pixel(0);

        grid.DataSource = (container.Grid.Page as _Default).GetData(Convert.ToInt32(container.KeyValue));

        grid.KeyFieldName = "EmployeeID";
        grid.SettingsDetail.ShowDetailRow = true;
        grid.Templates.DetailRow = this;

        grid.DetailRowGetButtonVisibility += new ASPxGridViewDetailRowButtonEventHandler(grid_DetailRowGetButtonVisibility);
    }
}