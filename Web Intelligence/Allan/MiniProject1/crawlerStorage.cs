namespace MiniProject1
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("crawlerStorage")]
    public partial class crawlerStorage
    {
        [Key]
        [StringLength(1000)]
        public string url { get; set; }

        [Column(TypeName = "text")]
        public string content { get; set; }
    }
}
