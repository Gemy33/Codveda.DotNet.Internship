namespace Level2_ASPNetCore_MVC_TaskManager.Models
{
   
        public class HomeViewModel
        {
            public string WelcomeMessage { get; set; } = string.Empty;
            public int TotalProducts { get; set; }
            public int TotalCategories { get; set; }
            public List<Product> FeaturedProducts { get; set; } = new();
            public List<string> Categories { get; set; } = new();
        
    }
}
