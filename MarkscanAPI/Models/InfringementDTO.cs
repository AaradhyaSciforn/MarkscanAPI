namespace MarkscanAPI.Models
{
    public class InfringementDTO
    {
        public string? Platform { get; set; }               // e.g., YouTube, Facebook, etc.
        public string? AssetName { get; set; }

        // URLs
        //public string? SourceURL { get; set; }
        //public string? SourceURLLink { get; set; }
        public string? VideoURL { get; set; }
        public string? ProfileURL { get; set; }
        public string? ChannelURL { get; set; }
      //  public string? ChannelOrProfileURL { get; set; }
        public string? SignPostURLs { get; set; }
       
        // Metadata
        public string? VideoTitle { get; set; }
      //  public string? Title { get; set; }
        public string? Caption { get; set; }
      //  public string? Keywords { get; set; }
        public string? Keyword { get; set; }
        public string? InfringementType { get; set; }
        public string? QualityOfPrint { get; set; } 
      //  public string? Quality { get; set; }

        // User / Profile Info
        public string? ProfileName { get; set; }
        public string? ChannelName { get; set; }
        public string? ChannelId { get; set; }
        public string? Username { get; set; }
       // public string? UserName { get; set; }
        public string? UserFullName { get; set; }
      //  public string? ChannelOrProfileName { get; set; }

        // Engagement Metrics
     //   public string? Views { get; set; }
        public string? ViewCount { get; set; }
        public string? LikeCount { get; set; }
     //   public string? Likes { get; set; }
        public string? dislikeCount { get; set; }
        public string? FavouriteCount { get; set; }
        public string? Subscriber { get; set; }
       // public string? Subscribers { get; set; }
        public string? SubscriberCount { get; set; }
        public string? FollowersCount { get; set; }
      //  public string? CommentsCount { get; set; }
        public string? CommentCount { get; set; }
        public string? RetweetCount { get; set; }
     //   public string? like_count { get; set; }
     //   public string? comment_count { get; set; }

        // Video Info
        public string? VideoId { get; set; }
        public string? VideoName { get; set; }
     //   public string? Duration { get; set; }
        public string? VideoDuration { get; set; }
    //    public string? VideoLength { get; set; }

        // Date Info
        public DateTime? UploadDate { get; set; }
     //   public DateTime? PublishedOn { get; set; }
    //    public DateTime? publishedDate { get; set; }
        public DateTime? ChannelCreationDate { get; set; }
    //    public DateTime? PostUploadDateTime { get; set; }
    //    public DateTime? PostDate { get; set; }
    //    public DateTime? PostUploadDate { get; set; }

        // Media Categorization
        public string? Category { get; set; }
        public string? Season { get; set; }
        public string? Episode { get; set; }

        // Languages
     //   public string? AudioLanguage { get; set; }
        public string? Language { get; set; }
        public string? Language1 { get; set; }
        public string? Language2 { get; set; }
        public string? Language3 { get; set; }
        public string? Language4 { get; set; }

        // Others
        public string? Country { get; set; }
        public string? RemovalStatus { get; set; }
        public bool? IsChannelSuspended { get; set; }


        public static InfringementDTO MapToDTO(dynamic item, string platform)
        {
            var dto = new InfringementDTO
            {
                Platform = platform,
                AssetName = SafeGet<string>(item, "AssetName"),
                Country = SafeGet<string>(item, "Country"),
               // RemovalStatus = SafeGet<string>(item, "RemovalStatus"),
               // QualityOfPrint= SafeGet<string>(item, "QualityOfPrint"),
                Season= SafeGet<string>(item, "Season"),
                Episode=SafeGet<string>(item, "Episode"),
                InfringementType= SafeGet<string>(item, "InfringementType"),


            };

            switch (platform.Trim().ToLower())
            {
                case "youtube":
                case "yt":
                    dto.VideoURL = SafeGet<string>(item, "SourceURL");
                  //  dto.ChannelOrProfileURL = SafeGet<string>(item, "ChannelOrProfileURL");
                    dto.ChannelName = SafeGet<string>(item, "ChannelName"); 
                    dto.ChannelId = SafeGet<string>(item, "ChannelId"); 
                   // dto.Username = SafeGet<string>(item, "UserName");
                  //  dto.ChannelOrProfileName = SafeGet<string>(item, "ChannelOrProfileName");
                  //  dto.SignPostURLs = SafeGet<string>(item, "SignPostURLs");
                    dto.VideoId = SafeGet<string>(item, "VideoId");
                    dto.VideoTitle = SafeGet<string>(item, "VideoName");
                    dto.VideoDuration = SafeGet<string>(item, "VideoDuration");
                    dto.ViewCount = SafeGet<string>(item, "ViewCount");
                    dto.LikeCount = SafeGet<string>(item, "LikeCount"); 
                    dto.FavouriteCount = SafeGet<string>(item, "FavouriteCount");
                    dto.CommentCount = SafeGet<string>(item, "CommentsCount"); 
                    dto.SubscriberCount = SafeGet<string>(item, "SubscriberCount");
                    dto.UploadDate = SafeGet<DateTime?>(item, "UploadDate");
                    dto.Keyword = SafeGet<string>(item, "Keywords");
                    dto.IsChannelSuspended = SafeGet<bool?>(item, "IsChannelSuspended");
                    dto.RemovalStatus = SafeGet<string>(item, "RemovalStatus");
                    dto.Language = SafeGet<string?>(item, "Language"); 
                   // dto.AudioLanguage= SafeGet<bool?>(item, "AudioLanguage");
                    //dto.PostUploadDate = SafeGet<DateTime?>(item, "PostUploadDate");

                    break;

                case "facebook":
                case "fb":
                    dto.VideoURL = SafeGet<string>(item, "VideoURL"); 
                    dto.VideoTitle = SafeGet<string>(item, "VideoTitle");
                    dto.VideoDuration = SafeGet<string>(item, "VideoLength");
                    dto.ProfileURL = SafeGet<string>(item, "ProfileURL");
                    dto.ProfileName = SafeGet<string>(item, "ProfileName");
                  //  dto.Caption = SafeGet<string>(item, "Caption");
                    dto.ViewCount = SafeGet<string>(item, "Views");
                    dto.LikeCount = SafeGet<string>(item, "like_count");
                    dto.CommentCount = SafeGet<string>(item, "comment_count");
                    dto.Language = SafeGet<string>(item, "AudioLanguage");
                    dto.UploadDate = SafeGet<DateTime?>(item, "publishedDate");
                    dto.Keyword = SafeGet<string>(item, "Keywords");
                    dto.SignPostURLs = SafeGet<string>(item, "SignPostURLs");
                    dto.QualityOfPrint = SafeGet<string>(item, "QualityOfPrint");
                    break;

                case "instagram":
                case "ig":
                    dto.VideoURL = SafeGet<string>(item, "VideoURL");
                    dto.ProfileURL = SafeGet<string>(item, "ProfileURL");
                    dto.ProfileName = SafeGet<string>(item, "UserName");
                    dto.UserFullName = SafeGet<string>(item, "UserFullName");
                    dto.Caption = SafeGet<string>(item, "Caption");
                    dto.LikeCount = SafeGet<string>(item, "LikeCount");
                    dto.ViewCount = SafeGet<string>(item, "ViewCount");
                    dto.FollowersCount = SafeGet<string>(item, "FollowersCount");              
                    dto.CommentCount = SafeGet<string>(item, "CommentCount");
                    dto.UploadDate = SafeGet<DateTime?>(item, "PostDate");
                    dto.SignPostURLs = SafeGet<string>(item, "SignPostURLs");
                    dto.Language = SafeGet<string>(item, "AudioLanguage");
                    dto.Keyword = SafeGet<string>(item, "Keywords");
                    dto.QualityOfPrint = SafeGet<string>(item, "QualityOfPrint");
                    break;

                case "telegram":
                case "tg":
                    dto.VideoURL = SafeGet<string>(item, "SourceURL");
                    dto.ChannelURL = SafeGet<string>(item, "ChannelURL");
                    dto.ChannelName = SafeGet<string>(item, "ChannelName");
                    dto.Caption = SafeGet<string>(item, "Caption");
                    dto.ViewCount = SafeGet<string>(item, "Views"); 
                    dto.VideoDuration = SafeGet<string>(item, "Duration"); 
                    dto.QualityOfPrint = SafeGet<string>(item, "Quality");  
                    dto.Category = SafeGet<string>(item, "Category"); 
                    dto.Subscriber = SafeGet<string>(item, "Subscribers");
                    dto.Language1 = SafeGet<string>(item, "Language1");
                    dto.Language2 = SafeGet<string>(item, "Language2");
                    dto.Language3 = SafeGet<string>(item, "Language3");
                    dto.Language4 = SafeGet<string>(item, "Language4");
                    dto.UploadDate = SafeGet<DateTime?>(item, "PostUploadDate");
                    dto.ChannelCreationDate = SafeGet<DateTime?>(item, "ChannelCreationDate");
                    dto.SignPostURLs = SafeGet<string>(item, "SignPostURLs");
                    break;

                case "twitter":
                case "tw":
                    dto.VideoURL = SafeGet<string>(item, "SourceURLLink"); 
                    dto.Caption = SafeGet<string>(item, "Title"); 
                    dto.ProfileURL = SafeGet<string>(item, "ProfileURL");
                    dto.Username = SafeGet<string>(item, "Username"); 
                    dto.UserFullName = SafeGet<string>(item, "UserFullName");
                  //  dto.Caption = SafeGet<string>(item, "Caption");
                    dto.LikeCount = SafeGet<string>(item, "LikeCount");
                    dto.ViewCount = SafeGet<string>(item, "ViewCount"); 
                    dto.VideoDuration = SafeGet<string>(item, "VideoDuration");
                   // dto.comment_count = SafeGet<string>(item, "comment_count");
                    dto.RetweetCount = SafeGet<string>(item, "RetweetCount");
                    dto.UploadDate = SafeGet<DateTime?>(item, "PublishedOn");
                    dto.SignPostURLs = SafeGet<string>(item, "SignPostURLs");
                    dto.Language = SafeGet<string>(item, "AudioLanguage");
                    dto.Keyword = SafeGet<string>(item, "Keywords");
                    dto.QualityOfPrint = SafeGet<string>(item, "QualityOfPrint");
                    break;

                case "ugc":
                case "ugcandothersocialmedia":
                    dto.VideoURL = SafeGet<string>(item, "VideoURL");
                    dto.VideoDuration = SafeGet<string>(item, "Duration");
                    dto.ProfileURL = SafeGet<string>(item, "ChannelOrProfileURL");
                    dto.ProfileName = SafeGet<string>(item, "ChannelOrProfileName");
                    dto.Subscriber= SafeGet<string>(item, "Subscriber");
                    dto.VideoTitle = SafeGet<string>(item, "VideoTitle");
                    //dto.Caption = SafeGet<string>(item, "Caption");
                    dto.SignPostURLs = SafeGet<string>(item, "SignPostURL");
                    dto.ViewCount = SafeGet<string>(item, "Views");
                    dto.LikeCount = SafeGet<string>(item, "Likes");
                    dto.CommentCount = SafeGet<string>(item, "CommentCount");
                    dto.UploadDate = SafeGet<DateTime?>(item, "PostUploadDate");
                    dto.Keyword= SafeGet<string>(item, "Keyword");
                    dto.Language= SafeGet<string>(item, "Language");
                    dto.QualityOfPrint = SafeGet<string>(item, "QualityOfPrint");
                    break;
            }

            return dto;
        }


        private static T SafeGet<T>(dynamic item, string propertyName)
        {
            try
            {
                var dict = item as IDictionary<string, object>;
                if (dict != null && dict.ContainsKey(propertyName))
                    return (T)dict[propertyName];

                var prop = item.GetType().GetProperty(propertyName);
                if (prop != null)
                    return (T)prop.GetValue(item);
            }
            catch { }

            return default;
        }



    }

}