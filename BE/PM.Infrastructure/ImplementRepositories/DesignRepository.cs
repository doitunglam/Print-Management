using PM.Domain.InterfaceRepositories;
using PM.Infrastructure.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Infrastructure.ImplementRepositories
{
    public class DesignRepository : IDesignRepository
    {
        private readonly AppDbContext _context;

        public DesignRepository(AppDbContext context)
        {
            _context = context;
        }
    }
}
