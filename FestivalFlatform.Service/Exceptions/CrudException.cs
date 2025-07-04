﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FestivalFlatform.Service.Exceptions
{
    public class CrudException : Exception
    {
        public HttpStatusCode Status { get; private set; }
        public string Error { get; set; }

        public CrudException(HttpStatusCode status, string msg, string error) : base(msg)
        {
            Status = status;
            Error = error;
        }
        public CrudException(HttpStatusCode statusCode, string message)
        : base(message)
        {
            Status = statusCode;
        }
    }
}
