using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WeatherForecast.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // ViewData: 不可跨Action (ViewData跟ViewBag如果同名會被後面覆蓋)
            // ViewData["Demo"] = "ViewDataDemo";
            ViewData["Demo"] = "ViewDataDemo123";
            // ViewBag: 不可跨Action(動態dynamic類似var會自動轉型)
            ViewBag.Demo2 = "ViewBagDemo456";
            // TempData 可跨Action(用一次就消失，ex: 警告，錯誤訊息)
            TempData["Demo"] = "TempDataDemo789";
            TempData["DemoPass"] = "TempDataDemoPass";
            // @Model (傳List)
            // var dataQuery = db.Data.Include(e => e.id>0)
            // return View(dataQuery);

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}