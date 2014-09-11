namespace MiniProject1
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("urlcontentTable")]
    public partial class urlcontentTable
    {
        [Key]
        [StringLength(1000)]
        public string url { get; set; }

        [Column(TypeName = "text")]
        public string content { get; set; }
    }
}
