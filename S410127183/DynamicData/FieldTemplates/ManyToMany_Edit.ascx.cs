using System;
using System.Data;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using System.Web.DynamicData;

namespace S410127183_1
{
    public partial class ManyToMany_EditField : System.Web.DynamicData.FieldTemplateUserControl
    {
        public void Page_Load(object sender, EventArgs e)
        {
            // 註冊 DataSource 的更新事件
            EntityDataSource ds = (EntityDataSource)this.FindDataSourceControl();

            // 此欄位範本同時用於編輯和插入作業
            ds.Updating += new EventHandler<EntityDataSourceChangingEventArgs>(DataSource_UpdatingOrInserting);
            ds.Inserting += new EventHandler<EntityDataSourceChangingEventArgs>(DataSource_UpdatingOrInserting);
        }

        void DataSource_UpdatingOrInserting(object sender, EntityDataSourceChangingEventArgs e)
        {
            MetaTable childTable = ChildrenColumn.ChildTable;

            // 註解以員工/地區加以假設說明，但程式碼為泛型

            // 取得此員工的地區集合
            RelatedEnd entityCollection = (RelatedEnd)Column.EntityTypeProperty.GetValue(e.Entity, null);

            // 在編輯模式中，確認它已被載入 (這在插入模式中沒有意義)
            if (Mode == DataBoundControlMode.Edit && !entityCollection.IsLoaded)
            {
                entityCollection.Load();
            }

            // 從中取得 IList (例如目前員工的地區清單)
            // 檢閱: 我們應該直接使用 EntityCollection，但是 EF 沒有
            // 其非泛型型別。它們會將此項目加到 vnext
            IList entityList = ((IListSource)entityCollection).GetList();

            // 處理所有的地區 (而不只是此員工的地區)
            foreach (object childEntity in childTable.GetQuery(e.Context))
            {

                // 檢查員工目前是否有此地區
                bool isCurrentlyInList = entityList.Contains(childEntity);

                // 尋找此地區的核取方塊，讓我們了解新狀態
                string pkString = childTable.GetPrimaryKeyString(childEntity);
                ListItem listItem = CheckBoxList1.Items.FindByValue(pkString);
                if (listItem == null)
                    continue;

                // 如果狀態不同，請進行適當的新增/移除變更
                if (listItem.Selected)
                {
                    if (!isCurrentlyInList)
                        entityList.Add(childEntity);
                }
                else
                {
                    if (isCurrentlyInList)
                        entityList.Remove(childEntity);
                }
            }
        }

        protected void CheckBoxList1_DataBound(object sender, EventArgs e)
        {
            MetaTable childTable = ChildrenColumn.ChildTable;

            // 註解以員工/地區加以假設說明，但程式碼為泛型

            IList entityList = null;
            ObjectContext objectContext = null;

            if (Mode == DataBoundControlMode.Edit)
            {
                object entity;
                ICustomTypeDescriptor rowDescriptor = Row as ICustomTypeDescriptor;
                if (rowDescriptor != null)
                {
                    // 從包裝函式取得真正的實體
                    entity = rowDescriptor.GetPropertyOwner(null);
                }
                else
                {
                    entity = Row;
                }

                // 取得此員工的地區集合並確認它已被載入
                RelatedEnd entityCollection = Column.EntityTypeProperty.GetValue(entity, null) as RelatedEnd;
                if (entityCollection == null)
                {
                    throw new InvalidOperationException(String.Format("ManyToMany 範本不支援 '{1}' 資料表的 '{0}' 資料行的集合型別。", Column.Name, Table.Name));
                }
                if (!entityCollection.IsLoaded)
                {
                    entityCollection.Load();
                }

                // 從中取得 IList (例如目前員工的地區清單)
                // 檢閱: 我們應該直接使用 EntityCollection，但是 EF 沒有
                // 其非泛型型別。它們會將此項目加到 vnext
                entityList = ((IListSource)entityCollection).GetList();

                // 取得目前的 ObjectContext
                // 檢閱: 這是一個不適合的處理方法。請尋找更好的替代辦法
                ObjectQuery objectQuery = (ObjectQuery)entityCollection.GetType().GetMethod(
                    "CreateSourceQuery").Invoke(entityCollection, null);
                objectContext = objectQuery.Context;
            }

            // 處理所有的地區 (而不只是此員工的地區)
            foreach (object childEntity in childTable.GetQuery(objectContext))
            {
                MetaTable actualTable = MetaTable.GetTable(childEntity.GetType());
                // 為它建立核取方塊
                ListItem listItem = new ListItem(
                    actualTable.GetDisplayString(childEntity),
                    actualTable.GetPrimaryKeyString(childEntity));

                // 讓它處於選取狀態 (如果目前的員工沒有該地區的話)
                if (Mode == DataBoundControlMode.Edit)
                {
                    listItem.Selected = entityList.Contains(childEntity);
                }

                CheckBoxList1.Items.Add(listItem);
            }
        }

        public override Control DataControl
        {
            get
            {
                return CheckBoxList1;
            }
        }

    }
}
