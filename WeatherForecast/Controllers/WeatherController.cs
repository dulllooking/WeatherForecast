using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using WeatherForecast.Models;

namespace WeatherForecast.Controllers
{
    public class WeatherController : Controller
    {
        // GET: Weather
        public ActionResult Index()
        {
            // 宣告 List<T> 先整理資料
            List<CityArea> cityAreaList = new List<CityArea>();

            string downloadLink = "https://opendata.cwb.gov.tw/fileapi/v1/opendataapi/F-D0047-067?Authorization=CWB-BC2E6A08-130F-4DCA-B26C-8E5ADF09F133&downloadType=WEB&format=XML";
            string fileName = "高雄市未來1週天氣預報.xml";
            // 取得刷新下載檔案
            string filePath = Server.MapPath("~/File/");
            GetDownloadFile(downloadLink, fileName, filePath);
            // 取得 xml 資料
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(filePath + fileName);
            XmlNode xmlNodeRoot = xmlDocument.DocumentElement["dataset"];
            XmlNodeList xmlNodeList = xmlNodeRoot.ChildNodes;
            XmlNode xmlNodeLocations = xmlNodeList[1];
            XmlNodeList xmlNodeLocationsList = xmlNodeLocations.ChildNodes;

            // ViewData: 不可跨Action
            // ViewBag: 不可跨Action(動態dynamic類似var會自動轉型)
            // TempData: 可跨Action(用一次就消失，ex: 警告，錯誤訊息)
            // @model: 只能放一個

            // 資料來源在請求資料時，早上6點前當天會多一筆6點前(3個時間)，超過18:00會只剩一筆(畫面直接從明天開始)，都會多於7(一週)*2(日夜)的14筆資料變15筆
            // 判斷當天的資料筆數有幾筆來決定存取的資料
            int countTime = 0;
            foreach (XmlNode nodeWeatherElement in xmlNodeLocationsList[1].ChildNodes[4].ChildNodes) {
                if (nodeWeatherElement.Name.Equals("time")) {
                    countTime++;
                }
            }

            // 取得區域名列表
            int countArea = 0;
            int count = 0;
            int dataNode = 4;
            // 修正開始取資料的位置
            int adjust = countTime == 14 ? 0 : 1;
            foreach (XmlNode nodeLocations in xmlNodeLocationsList) {
                if (nodeLocations.Name.Equals("location")) {
                    // 取得區域天氣細節資料列表
                    XmlNodeList xmlNodeLocationDataList = nodeLocations.ChildNodes;
                    List<WeatherData> weatherDataList = new List<WeatherData>();
                    for (int i = 2; i < (2 + 7 * 2); i++) {
                        count++;
                        // 降雨機率超過3天之後會是空字串需做轉換
                        var popStr = xmlNodeLocationDataList[dataNode + 9].ChildNodes[i + adjust]["elementValue"]["value"].InnerText;
                        // 紫外線指數沒分白天晚上，存相同值用
                        int uvIndex = Convert.ToInt16(Math.Floor((float)i / 2)) + 1;
                        var Id = count;
                        weatherDataList.Add(new WeatherData
                        {
                            Id = count,
                            Date = Convert.ToDateTime(xmlNodeLocationDataList[dataNode].ChildNodes[i + adjust]["startTime"].InnerText),
                            Time = count % 2 == 0 ? "晚上" : "白天",
                            AverageTemperature = Convert.ToInt32(xmlNodeLocationDataList[dataNode].ChildNodes[i + adjust]["elementValue"]["value"].InnerText),
                            AverageDewPointTemperature = Convert.ToInt32(xmlNodeLocationDataList[dataNode + 1].ChildNodes[i + adjust]["elementValue"]["value"].InnerText),
                            AverageRelativeHumidity = Convert.ToInt32(xmlNodeLocationDataList[dataNode + 2].ChildNodes[i + adjust]["elementValue"]["value"].InnerText),
                            MaxTemperature = Convert.ToInt32(xmlNodeLocationDataList[dataNode + 3].ChildNodes[i + adjust]["elementValue"]["value"].InnerText),
                            MinTemperature = Convert.ToInt32(xmlNodeLocationDataList[dataNode + 4].ChildNodes[i + adjust]["elementValue"]["value"].InnerText),
                            MaxApparentTemperature = Convert.ToInt32(xmlNodeLocationDataList[dataNode + 5].ChildNodes[i + adjust]["elementValue"]["value"].InnerText),
                            MinApparentTemperature = Convert.ToInt32(xmlNodeLocationDataList[dataNode + 6].ChildNodes[i + adjust]["elementValue"]["value"].InnerText),
                            MaxComfortIndex = Convert.ToInt32(xmlNodeLocationDataList[dataNode + 7].ChildNodes[i + adjust].ChildNodes[2]["value"].InnerText),
                            MaxComfortText = xmlNodeLocationDataList[dataNode + 7].ChildNodes[i + adjust].ChildNodes[3]["value"].InnerText,
                            MinComfortIndex = Convert.ToInt32(xmlNodeLocationDataList[dataNode + 8].ChildNodes[i + adjust].ChildNodes[2]["value"].InnerText),
                            MinComfortText = xmlNodeLocationDataList[dataNode + 8].ChildNodes[i + adjust].ChildNodes[3]["value"].InnerText,
                            ProbabilityOfPrecipitation12h = Convert.ToInt32(String.IsNullOrWhiteSpace(popStr) ? "-1" : popStr),
                            WindDirection = xmlNodeLocationDataList[dataNode + 10].ChildNodes[i + adjust]["elementValue"]["value"].InnerText,
                            MaxWindSpeed = Convert.ToInt32(xmlNodeLocationDataList[dataNode + 11].ChildNodes[i + adjust].ChildNodes[2]["value"].InnerText),
                            MaxBeaufortScale = xmlNodeLocationDataList[dataNode + 11].ChildNodes[i + adjust].ChildNodes[3]["value"].InnerText,
                            WeatherText = xmlNodeLocationDataList[dataNode + 12].ChildNodes[i + adjust].ChildNodes[2]["value"].InnerText,
                            WeatherImage = xmlNodeLocationDataList[dataNode + 12].ChildNodes[i + adjust].ChildNodes[3]["value"].InnerText,
                            UVindex = Convert.ToInt32(xmlNodeLocationDataList[dataNode + 13].ChildNodes[uvIndex].ChildNodes[2]["value"].InnerText),
                            UVindexText = xmlNodeLocationDataList[dataNode + 13].ChildNodes[uvIndex].ChildNodes[3]["value"].InnerText,
                            WeatherDescription = xmlNodeLocationDataList[dataNode + 14].ChildNodes[i + adjust]["elementValue"]["value"].InnerText
                        });
                    }
                    // 加入區域天氣資料
                    countArea++;
                    cityAreaList.Add(new CityArea
                    {
                        Id = countArea,
                        LocationName = nodeLocations["locationName"].InnerText,
                        WeatherDataList = weatherDataList
                    });
                }
            }

            // 送出起始日期
            ViewBag.DateNow = Convert.ToDateTime(xmlNodeLocationsList[1].ChildNodes[4].ChildNodes[2 + adjust]["startTime"].InnerText);

            List<WeatherData> weatherDatas = cityAreaList.FirstOrDefault(x => x.Id == 1).WeatherDataList.ToList();
            return View(weatherDatas);
        }

        /// <summary>
        /// 刷新下載檔案
        /// </summary>
        /// <param name="downloadLink">csv檔案下載連結</param>
        /// <param name="fileName">檔案名稱.csv</param>
        /// <param name="filePath">檔案存放路徑</param>
        private static void GetDownloadFile(string downloadLink, string fileName, string filePath)
        {
            // 清空資料夾
            DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
            FileInfo[] files = directoryInfo.GetFiles();
            foreach (FileInfo file in files) {
                file.Delete();
            }
            // 下載檔案
            try {
                WebClient mywebClient = new WebClient();
                mywebClient.DownloadFile(downloadLink, filePath + fileName);
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }

    }
}