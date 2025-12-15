namespace PJ_P_Installation_Management_System.ViewModel
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Price { get; set; }

        public int AvailableStock { get; set; }
    }
}