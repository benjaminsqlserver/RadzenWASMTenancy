using System;
using System.Net;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace RadzenSchoolTenants.Server.Controllers.ConData
{
    public partial class AddNewUserToAppRolesController : ODataController
    {
        private RadzenSchoolTenants.Server.Data.ConDataContext context;

        public AddNewUserToAppRolesController(RadzenSchoolTenants.Server.Data.ConDataContext context)
        {
            this.context = context;
        }


        [HttpGet]
        [Route("odata/ConData/AddNewUserToAppRolesFunc(UserId={UserId},RoleId={RoleId})")]
        public IActionResult AddNewUserToAppRolesFunc([FromODataUri] string UserId, [FromODataUri] string RoleId)
        {
            this.OnAddNewUserToAppRolesDefaultParams(ref UserId, ref RoleId);


            SqlParameter[] @params =
            {
                new SqlParameter("@returnVal", SqlDbType.Int) {Direction = ParameterDirection.Output},
              new SqlParameter("@UserId", SqlDbType.NVarChar) {Direction = ParameterDirection.Input, Value = UserId},
              new SqlParameter("@RoleId", SqlDbType.NVarChar) {Direction = ParameterDirection.Input, Value = RoleId},

            };

            foreach(var _p in @params)
            {
                if(_p.Direction == ParameterDirection.Input && _p.Value == null)
                {
                    _p.Value = DBNull.Value;
                }
            }

            this.context.Database.ExecuteSqlRaw("EXEC @returnVal=[dbo].[AddNewUserToAppRole] @UserId, @RoleId", @params);

            int result = Convert.ToInt32(@params[0].Value);

            this.OnAddNewUserToAppRolesInvoke(ref result);

            return Ok(result);
        }

        partial void OnAddNewUserToAppRolesDefaultParams(ref string UserId, ref string RoleId);
      partial void OnAddNewUserToAppRolesInvoke(ref int result);
    }
}
