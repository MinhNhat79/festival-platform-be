﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Request
{
    public class RoleCreateRequest
    {
        public string RoleName { get; set; } = null!;
        public string? Permissions { get; set; }
    }
}
