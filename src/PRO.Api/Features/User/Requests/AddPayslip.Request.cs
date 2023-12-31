﻿using System;
using System.ComponentModel.DataAnnotations;

namespace PRO.Api.Features.User.Requests
{
    public class AddPayslipRequest
    {
        [Required]
        public DateTime? Date { get; set; }

        [Required]
        public int? UserId { get; set; }

        [Required]
        public float? WorkingDays { get; set; }

        public decimal Bonus { get; set; }

        [Required]
        public bool? IsPaid { get; set; }
    }
}