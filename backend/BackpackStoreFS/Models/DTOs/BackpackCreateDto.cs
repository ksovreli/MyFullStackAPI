namespace BackpackStoreFS.Models.DTOs
{
    public class BackpackCreateDto
    {
        public string Name { get; set; } = null!;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal? SalePrice { get; set; }
        public int CategoryId { get; set; }
        public bool IsNew { get; set; }
        public decimal Rating { get; set; }
    }

    public class BackpackReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Image { get; set; } = null!;
        public decimal Price { get; set; }
        public string Description { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal? SalePrice { get; set; }
        public bool IsNew { get; set; }
        public decimal Rating { get; set; }
        public int ReviewCount { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
    }
}