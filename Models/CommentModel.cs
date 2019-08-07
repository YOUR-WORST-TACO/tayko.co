using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Tayko.co.Models
{
    public class CommentModel
    {
        public int Id { get; set; }
        
        [Required]
        [HiddenInput]
        public string Article { get; set; }
        
        [Required]
        [StringLength(2000)]
        public string Text { get; set; }
        
        [Required]
        [StringLength(56)]
        public string Author { get; set; }
        
        [StringLength(128)]
        public string AuthorEmail { get; set; }
        
        public DateTime PostDate { get; set; }
        public string PostIp { get; set; }
    }
}