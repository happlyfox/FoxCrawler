namespace YouKuCrawlerAsync
{
    /// <summary>
    ///     视频内容实体
    /// </summary>
    public class VideoContent
    {
        /// <summary>
        ///     页码
        /// </summary>
        public string PageIndex { get; set; }

        /// <summary>
        ///     标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     链接地址
        /// </summary>
        public string Href { get; set; }

        /// <summary>
        ///     图片链接地址
        /// </summary>
        public string ImgHref { get; set; }

        /// <summary>
        ///     播放量
        /// </summary>
        public string Hits { get; set; }

        /// <summary>
        ///     编码
        /// </summary>
        public string Code { get; set; }
    }
}