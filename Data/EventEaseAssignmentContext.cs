using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;

namespace EventEaseAssignment.Data
{
    public class EventEaseAssignmentContext : DbContext
    {
        public EventEaseAssignmentContext (DbContextOptions<EventEaseAssignmentContext> options)
            : base(options)
        {
        }

        public DbSet<EventEase.Models.Venues> Venues { get; set; } = default!;
        public DbSet<EventEase.Models.Bookings> Bookings { get; set; } = default!;
        public DbSet<EventEase.Models.Events> Events { get; set; } = default!;
    }
}
