using System;
using System.ComponentModel.DataAnnotations;
using System.Web.DynamicData;

namespace S410127183_1
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Collections.IList visibleTables = Global.DefaultModel.VisibleTables;
            if (visibleTables.Count == 0)
            {
                throw new InvalidOperationException("沒有可存取的資料表。至少要在 Global.asax 中註冊一個資料模型，而且要啟用 Scaffolding 或實作自訂頁面。");
            }
            Menu1.DataSource = visibleTables;
            Menu1.DataBind();
        }

    }
}
