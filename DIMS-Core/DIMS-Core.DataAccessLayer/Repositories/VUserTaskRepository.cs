﻿using System;
using System.Collections.Generic;
using System.Text;
using DIMS_Core.DataAccessLayer.Context;
using DIMS_Core.DataAccessLayer.Interfaces;
using DIMS_Core.DataAccessLayer.Entities;

namespace DIMS_Core.DataAccessLayer.Repositories
{
    public class VUserTaskRepository:Repository<VUserTask>, IVUserTaskRepository
    {
        public VUserTaskRepository(DIMSCoreDatabaseContext dbContext) : base(dbContext) { }
    }
}
