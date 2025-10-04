using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Application.DTOs.Operator
{
    public class CreateOperatorDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string OperatorCode { get; set; }
    }

    public class UpdateOperatorDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string OperatorCode { get; set; }
    }

    public class OperatorDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string OperatorCode { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

