using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace YouKuCrawler
{
    public class YouKu
    {
        public string id { get; set; }

        public int videoNum { get; set; }

        public List<string> videoCountry { get; set; }
        public List<string> videoType { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ///爬虫的制作主要分为三个方面
            ///1、加载网页结构
            ///2、解析网页结构，转变为符合需求的数据实体
            ///3、保存数据实体（数据库，文本等）


            /*
             * 在实际的编码过程中，找到了一个好的类库“HtmlAgilityPack”。
             * 介绍：
             * 官网：http://html-agility-pack.net/?z=codeplex
             * Html Agility Pack源码中的类大概有28个左右，其实不算一个很复杂的类库，但它的功能确不弱，为解析DOM已经提供了足够强大的功能支持，可以跟jQuery操作DOM媲美)
             * 使用说明：
             * Html Agility Pack（XPath 定位）,在实际使用过程中，发现有部分内容如果通过Css进行定位会比XPath更加方便，所以通过查找找到了另外一个CSS的解析了类库 ScrapySharp（Css 定位）
             * 整理：
             * Nuget包需要引用的库
             * 1、Html Agility Pack（XPath 定位）
             * 2、ScrapySharp（Css 定位）
             */


            //第一点——加载网页结构,Html Agility Pack封装了加载内容的方法，使用doc.Load(arguments),具有多种重载方式，以下列举官网的三个实例
            //LoadDocment();

            //第二点——解析网页结构，转变为符合需求的数据实体
            //ParsingWebStructure();

            //第三点——保存数据实体，转变为符合需求的数据实体
            SavaData();
            Console.Read();
        }


        private static string getJsonByObject(Object obj)
        {
            //实例化DataContractJsonSerializer对象，需要待序列化的对象类型
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            //实例化一个内存流，用于存放序列化后的数据
            MemoryStream stream = new MemoryStream();
            //使用WriteObject序列化对象
            serializer.WriteObject(stream, obj);
            //写入内存流中
            byte[] dataBytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(dataBytes, 0, (int)stream.Length);
            //通过UTF8格式转换为字符串
            return Encoding.UTF8.GetString(dataBytes);
        }

        /// <summary>
        /// 保存数据实体
        /// </summary>
        private static void SavaData()
        {
            var model = ParsingWebStructure();
            var path = "youku.txt";

            if (!File.Exists(path))
            {
                File.Create(path);
            }

            File.WriteAllText(path, getJsonByObject(model));
        }

        /// <summary>
        /// 解析网页结构
        /// </summary>
        private static YouKu ParsingWebStructure()
        {
            /*选用优酷片库列表
             地址：http://list.youku.com/category/show/c_96_s_1_d_1_p_{index}.html 
            */

            //首先加载web内容
            var url = "http://list.youku.com/category/show/c_96_s_1_d_1_p_1.html";
            var web = new HtmlWeb();
            var doc = web.Load(url);

            //输出WebHtml内容
            //Console.WriteLine(doc.DocumentNode.InnerHtml);

            /* HtmlAgilityPack 解析方式官网提供的有俩种示例*/
            //1、 With XPath	
            var value = doc.DocumentNode.SelectSingleNode("//*[@id='total_videonum']").Attributes["id"].Value;
            var resultCount = doc.DocumentNode.SelectSingleNode("//*[@id='total_videonum']").InnerText;

            Console.WriteLine($"id='{value}' 筛选结果:{resultCount}个");
            // 2、With LINQ	
            var linqNodes = doc.DocumentNode.SelectSingleNode("//*[@id='filterPanel']/div[2]/ul").Descendants("li").ToList();

            Console.WriteLine("电影产地:");
            List<string> videoCountry = new List<string>();
            foreach (var node in linqNodes)
            {
                videoCountry.Add(node.InnerText);
                Console.Write($"{node.InnerText} \t");
            }

            //3、使用ScrapySharp进行Css定位
            var cssNodes = doc.DocumentNode.CssSelect("#filterPanel > div > label");
            Console.WriteLine();

            List<string> videoType = new List<string>();
            foreach (var node in cssNodes)
            {
                videoType.Add(node.InnerText);
                Console.Write($"{node.InnerText} \t");
            }

            //构造实体
            YouKu model = new YouKu()
            {
                id = value,
                videoNum = int.Parse(resultCount),
                videoCountry = videoCountry,
                videoType = videoType
            };

            return model;
        }

        /// <summary>
        /// 加载网页结构
        /// </summary>
        private static void LoadDocment()
        {
            // 从文件中加载
            var docFile = new HtmlDocument();
            docFile.Load("file path");

            // 从字符串中加载
            var docHtml = new HtmlDocument();
            docHtml.LoadHtml("html");

            // 从网站中加载
            var url = "http://html-agility-pack.net/";
            var web = new HtmlWeb();
            var docWeb = web.Load(url);
        }
    }
}
