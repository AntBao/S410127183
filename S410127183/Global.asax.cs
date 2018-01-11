using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Web.Script.Services;


namespace S410127183
{
    /// <summary>
    /// WebService1 的摘要描述
    /// </summary>
    [WebService(Namespace = "", Description = "test")]
    //[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下一行。
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void HelloWorld()
        {


            List<contacts> people = new List<contacts>{
                new contacts( "Scott", "0922123456"),
                new contacts( "Scott", "0922123456"),
};

            string jsonString = people.ToJSON();

            jsonString = "{contacts:" + jsonString + "}";

            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(jsonString);

            // return jsonString;
        }


    }

    public class contacts
    {
        public string name;
        public string phone;

        public contacts(string a, string b)
        {
            name = a;
            phone = b;

        }

    }

    public static class JSONHelper
    {
        public static string ToJSON(this object obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }

        public static string ToJSON(this object obj, int recursionDepth)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.RecursionLimit = recursionDepth;
            return serializer.Serialize(obj);
        }
    }
}