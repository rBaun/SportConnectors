﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.DataTransferObjects {
    public class ResetPasswordRequest {
        public string Password { get; set; }
        public string Url { get; set; }
    }
}
