﻿using dotnetCore_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCore_API.Services.Interfaces
{
    public interface ITypeOfLeaveServices
    {
        public List<TypeOfLeaveModel> GetTypeOfLeave();
    }
}
