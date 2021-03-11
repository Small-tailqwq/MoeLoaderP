﻿using System.Threading;
using System.Threading.Tasks;

namespace MoeLoaderP.Core.Sites
{
    /// <summary>
    /// yuriimg.com Last change 20200428
    /// </summary>
    public class YuriimgSite : MoeSite
    {
        public override string HomeUrl => "http://yuriimg.com";
        public override string ShortName => "yuriimg";
        public override string DisplayName => "Yuriimg（百合居）";

        public YuriimgSite()
        {
            SupportState.IsSupportAutoHint = false;
            SupportState.IsSupportRating = true;

            DownloadTypes.Add("原图", 4);
        }

        public async Task GetDetailTask(MoeItem img, string id, CancellationToken token = new CancellationToken())
        {
            var api = $"https://api.yuriimg.com/post/{id}";
            var json = await Net.GetJsonAsync(api, token);
            if (json == null) return;
            img.Score = $"{json.praise}".ToInt();
            img.Date = $"{json.format_date}".ToDateTime();
            img.Urls.Add( 4, $"https://i.yuriimg.com/{json.src}");
            img.Artist = $"{json.artist?.name}";
            img.Uploader = $"{json.user?.name}";
            img.UploaderId = $"{json.user?.id}";

            foreach (var tag in Extend.GetList(json.tags.general))
            {
                var t = $"{tag.tags?.cn}";
                if (!t.IsEmpty())
                {
                    img.Tags.Add(t);
                }
            }

            if ($"{json.page_count}".ToInt() > 1)
            {
                var q = $"{api}/multi";
                var json2 = await Net.GetJsonAsync(q, token);

                var child1 = new MoeItem(this,img.Para);
                child1.Width = img.Width;
                child1.Height = img.Height;
                foreach (var urlInfo in img.Urls)
                {
                    child1.Urls.Add(urlInfo);
                }
                img.ChildrenItems.Add(child1);

                foreach (var jitem in Extend.GetList(json2))
                {
                    var childImg = new MoeItem(this,img.Para);
                    childImg.Width = $"{jitem.width}".ToInt();
                    childImg.Height = $"{jitem.height}".ToInt();
                    //childImg.Urls.Add(4, $"https://i.yuriimg.com/{post.src}/yuriimg.com%20{post.id}%20contain.jpg");
                    //childImg.Urls.Add(4,null,null,null, ResolveUrlFunc);
                    img.ChildrenItems.Add(childImg);
                }
            }

        }

        public override async Task<MoeItems> GetRealPageImagesAsync(SearchPara para, CancellationToken token)
        {
            if (Net == null) Net = new NetOperator(Settings, HomeUrl);
            const string api = "https://api.yuriimg.com/posts";
            var pairs = new Pairs
            {
                {"page", $"{para.PageIndex}"},
                {"tags",para.Keyword.ToEncodedUrl() }
            };
            var json = await Net.GetJsonAsync(api, token, pairs);
            if (json?.posts == null) return null;
            var imgs = new MoeItems();
            foreach (var post in json.posts)
            {
                var img = new MoeItem(this, para);
                img.IsExplicit = $"{post.rating}" == "e";
                if (CurrentSiteSetting.LoginCookie.IsEmpty() && img.IsExplicit) continue;
                img.Id = $"{post.pid}".ToInt();
                img.Sid = $"{post.id}";
                img.Width = $"{post.width}".ToInt();
                img.Height = $"{post.height}".ToInt();
                img.Urls.Add( 1, $"https://i.yuriimg.com/{post.src}/yuriimg.com%20{post.id}%20contain.jpg");
                img.DetailUrl = $"{HomeUrl}/show/{post.id}";
                img.GetDetailTaskFunc = async () => await GetDetailTask(img, $"{post.id}", token);
                img.OriginString = $"{post}";
                
                imgs.Add(img);
            }

            return imgs;
        }
    }
}
