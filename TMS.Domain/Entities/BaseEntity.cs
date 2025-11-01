using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public abstract class BaseEntity<TId>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // ✅ Auto-increment
    public TId Id { get; set; }

    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public string LastModifiedBy { get; set; }
}
