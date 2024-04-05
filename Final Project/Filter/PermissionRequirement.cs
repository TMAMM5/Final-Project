﻿using Microsoft.AspNetCore.Authorization;

namespace Final_Project.Filter
{
    public class PermissionRequirement:IAuthorizationRequirement
    {
        public string Permission { get;private set; }
        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}
