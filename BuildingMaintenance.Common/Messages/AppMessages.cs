using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingMaintenance.Common.Messages
{
    public static class AppMessages
    {
        #region Authentication        
        public const string RoleExist = "Role already exists.";
        public const string RoleSuccess = "Role created successfully.";
        public const string RoleFailed = "Role creation failed.";

        public const string UserExist = "User already exists.";
        public const string UserFailed = "User creation failed.";
        public const string UserSuccess = "User created successfully.";

        public const string AdminExist = "Admin already exists.";
        public const string AdminFailed = "Admin creation failed.";
        public const string AdminSuccess = "Admin created successfully.";
        #endregion
    }
}
