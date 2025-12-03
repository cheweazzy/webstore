using System.ComponentModel.DataAnnotations;
namespace WebStore.ViewModels.VM
{
    public class AddOrUpdateProductVm
    {
        public int? Id { get; set; }
        [Required]
        public string Description { get; set; } = default!;
        [Required]
        public byte[] ImageBytes { get; set; } = default!;
        [Required]
        public string Name { get; set; } = default!;
        [Required]
        public decimal Price { get; set; }
        [Required]
        public float Weight { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int SupplierId { get; set; }
    }
}