using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;

namespace FestivalFlatform.Service.Helpers
{
    public static class OrderQueryableExtensions
    {
        public static IQueryable<Order> OnlyValidOrders(this IQueryable<Order> query)
        {
            return query.Where(o => o.Status.ToLower() == "paid" || o.Status.ToLower() == "completed");
        }
    }
}
