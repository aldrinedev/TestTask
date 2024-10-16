using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestTask.Models;

namespace TestTaskAPI.Models
{
    public class ValidationResult
    {
        public int TotalRows { get; set; }
        public OrderDataRow? PreviewRow { get; set; }
    }
}