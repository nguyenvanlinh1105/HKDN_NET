using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineERP.Application.Dtos.Employees
{
    public class EmployeeContractFileDto
    {
        public string FileName { get; set; } = default!;
        public string ContentType { get; set; } = "application/pdf";
        public byte[] Content { get; set; } = default!;
    }
}
