using System;
using System.Collections.Generic;

namespace Identity.Data.Dtos.Request.Auth
{
    public class RolePermission
    {
        public List<Guid> Permissions { get; set; }
    }
}