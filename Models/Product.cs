using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название товара обязательно.")]
        [StringLength(100, ErrorMessage = "Название не может превышать 100 символов.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Описание товара обязательно.")]
        [StringLength(1000, ErrorMessage = "Описание не может превышать 1000 символов.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Цена товара обязательна.")]
        [Range(0.01, 1000000, ErrorMessage = "Цена должна быть больше 0.")]
        public decimal Price { get; set; }

        [StringLength(200, ErrorMessage = "URL изображения не может превышать 200 символов.")]
        [DataType(DataType.ImageUrl)]
        public string? ImageUrl { get; set; }
    }
}
