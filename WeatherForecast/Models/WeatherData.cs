using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WeatherForecast.Models
{
    public class CityArea
    {
        [Display(Name = "編號")]
        public int Id { get; set; }

        [Display(Name = "區域")]
        public string LocationName { get; set; }

        [Display(Name = "區域天氣資料")]
        public List<WeatherData> WeatherDataList { get; set; }
    }

    public class WeatherData
    {
        [Display(Name = "編號")]
        public int Id { get; set; }

        [Display(Name = "日期")]
        public DateTime Date { get; set; }

        [Display(Name = "時間")]
        public string Time { get; set; }

        [Display(Name = "平均溫度")]
        public int AverageTemperature { get; set; }

        [Display(Name = "平均露點溫度")]
        public int AverageDewPointTemperature { get; set; }

        [Display(Name = "平均相對濕度")]
        public int AverageRelativeHumidity { get; set; }

        [Display(Name = "最高溫度")]
        public int MaxTemperature { get; set; }

        [Display(Name = "最低溫度")]
        public int MinTemperature { get; set; }

        [Display(Name = "最高體感溫度")]
        public int MaxApparentTemperature { get; set; }

        [Display(Name = "最低體感溫度")]
        public int MinApparentTemperature { get; set; }

        [Display(Name = "最大舒適度指數")]
        public int MaxComfortIndex { get; set; }

        [Display(Name = "最大舒適度描述")]
        public string MaxComfortText { get; set; }

        [Display(Name = "最小舒適度指數")]
        public int MinComfortIndex { get; set; }

        [Display(Name = "最小舒適度描述")]
        public string MinComfortText { get; set; }

        [Display(Name = "12小時降雨機率")]
        public int ProbabilityOfPrecipitation12h { get; set; }

        [Display(Name = "風向")]
        public string WindDirection { get; set; }

        [Display(Name = "最大風速")]
        public int MaxWindSpeed { get; set; }

        // 蒲福風級設為string，因為會有"<=1"
        [Display(Name = "蒲福風級")]
        public string MaxBeaufortScale { get; set; }

        [Display(Name = "天氣現象文字")]
        public string WeatherText { get; set; }

        [Display(Name = "天氣現象圖片")]
        public string WeatherImage { get; set; }

        [Display(Name = "紫外線指數")]
        public int UVindex { get; set; }

        [Display(Name = "紫外線曝曬級數")]
        public string UVindexText { get; set; }

        [Display(Name = "天氣預報綜合描述")]
        public string WeatherDescription { get; set; }
    }
}