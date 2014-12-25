namespace SignalrSample.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CenterModel
    {
        public int CenterId { get; set; }
        [Required]
        public string CenterName { get; set; }
        [Required]
        public string CenterAddress { get; set; }
        [Required]
        public int Quota { get; set; }
        public DateTime CreateOn { get; set; }
    }
}