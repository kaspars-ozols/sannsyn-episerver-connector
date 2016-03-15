namespace Sannsyn.Episerver.Commerce
{
    public static class Constants
    {
        public static class ServiceMethod
        {
            public const string Recommend = "recommend";
            public const string MipRecommend = "miprecommend";
            public const string Merge = "merge";
            public const string Update = "update";
            public const string Start = "start";
            public const string Stop = "stop";
            public const string Servicestatus = "servicestatus";
        }

        public static class Recommenders
        {
            public const string ItemItemClickBuy = "ItemItemClickBuy";
            public const string UserItemClickBuy = "UserItemClickBuy";
            public const string UserItemCategoryClickBuy = "UserItemCategoryClickBuy";
            public const string ScoredItems = "ScoredItems";
        }

        public static class Tags
        {
            public const string Buy = "buy";
            public const string ItemCat = "itemcat";
        }
    }
}
