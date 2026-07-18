using System;

namespace SHOP.CO.Domain.Entities
{
    public class InteractionLog
    {
        public int LogId { get; set; }
        public int? UserId { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public int? VariantId { get; set; }
        public string? SessionId { get; set; }
        public string LogType { get; set; } = null!; 
        public string? Title { get; set; }
        public string? Message { get; set; }
        public string? SenderType { get; set; }
        public string? IntentName { get; set; }
        public string? EntitiesJson { get; set; }
        public string? ActionName { get; set; }
        public string? OldValueJson { get; set; }
        public string? NewValueJson { get; set; }
        public int? QuantityChanged { get; set; }
        public string? ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
        public bool IsRead { get; set; } 
        public DateTime? ReadAt { get; set; }
        public string? Status { get; set; }
        public string? PayloadJson { get; set; }
        public DateTime CreatedAt { get; set; } 

        public virtual User? User { get; set; }
        public virtual Order? Order { get; set; }
        public virtual Product? Product { get; set; }
        public virtual ProductVariant? ProductVariant { get; set; }
    }
}
