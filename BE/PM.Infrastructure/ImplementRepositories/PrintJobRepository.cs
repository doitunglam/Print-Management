using PM.Domain.InterfaceRepositories;
using PM.Infrastructure.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Infrastructure.ImplementRepositories
{
    public class PrintJobRepository : IPrintJobRepository
    {
        private readonly AppDbContext _context;

        public PrintJobRepository(AppDbContext context)
        {
            _context = context;
        }
    }
}
