﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.DAL.Entities {
    public class Player {
        public string Salt { get; set; }
        public string HashPassword { get; set; }
        public string ErrorMessage { get; set; }
    }
}
