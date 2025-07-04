﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.DTOs.Response
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string AccessToken { get; set; }
        public string Message { get; set; }
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public int StatusCode { get; set; }
    }
}
