using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using J3OMotors_v1._0.Models.Seguro;

namespace J3OMotors_v1._0.Data
{
    public class J3OMotors_v1_0Context : DbContext
    {
        public J3OMotors_v1_0Context (DbContextOptions<J3OMotors_v1_0Context> options)
            : base(options)
        {
        }

        public DbSet<J3OMotors_v1._0.Models.Seguro.SegurosViewModel> SegurosViewModel { get; set; } = default!;
    }
}
