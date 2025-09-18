using System;
using System.Collections.Generic;

namespace Serein.Candle.Domain.Entities;

public partial class Product
{
    public int ProductId { get; set; }

    public int? CategoryId { get; set; }

    public string? Sku { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? ShortDescription { get; set; }

    public string? Description { get; set; }

    public string? Ingredients { get; set; }

    public string? BurnTime { get; set; }

    public decimal Price { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Category? Category { get; set; }

    public virtual Inventory? Inventory { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<ProductAttributeValue> ProductAttributeValues { get; set; } = new List<ProductAttributeValue>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
