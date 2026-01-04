using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VilaManagement.Application.Common
{
    public static class BookingOrderStatus
    {
        public const string Pending = nameof(Pending);
        public const string Approved = nameof(Approved);
        public const string CheckededIn = nameof(CheckededIn);
        public const string Completed = nameof(Completed);
        public const string Cancelled = nameof(Cancelled);
        public const string Refunded = nameof(Refunded);
    }
}
