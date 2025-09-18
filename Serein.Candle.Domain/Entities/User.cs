using System;
using System.Collections.Generic;

namespace Serein.Candle.Domain.Entities;

public partial class User
{
    public int UserId { get; set; }

    public int RoleId { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string FullName { get; set; } = null!;

    public string? PasswordHash { get; set; }

    public bool IsGuest { get; set; }

    public DateOnly? Dob { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<CustomerAddress> CustomerAddresses { get; set; } = new List<CustomerAddress>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

    public virtual RoleType Role { get; set; } = null!;

    public virtual Staff? Staff { get; set; }

    public virtual ICollection<VoucherUser> VoucherUsers { get; set; } = new List<VoucherUser>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
