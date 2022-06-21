using System.Data;
using System.Data.SqlClient;
using IntelART.Utilities;
using IntelART.IdentityManagement;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace IntelART.OnlineLoans.SqlMembershipProvider
{
    public class SqlMembershipProvider : IMembershipProvider
    {
        private readonly string schemaName;
        private readonly string getUserByUsernameSpName;
        private readonly string getUserByIdSpName;
        private readonly string authenticateApplicationUserSpName;
        private readonly string changeApplicationUserPasswordSpName;
        private readonly string logClientIpAddressSpName;

        private string connectionString;

        public SqlMembershipProvider(string connectionString)
        {
            this.schemaName = "dbo";
            this.getUserByUsernameSpName = string.Format("{0}.{1}", this.schemaName, "sp_GetApplicationUser");
            this.getUserByIdSpName = string.Format("{0}.{1}", this.schemaName, "sp_GetApplicationUserByID");
            this.authenticateApplicationUserSpName = string.Format("{0}.{1}", this.schemaName, "sp_AuthenticateApplicationUser");
            this.changeApplicationUserPasswordSpName = string.Format("{0}.{1}", this.schemaName, "sp_ChangeApplicationUserPasswordByID");
            this.logClientIpAddressSpName = string.Format("{0}.{1}", this.schemaName, "sp_LogClientIpAddress");
            this.connectionString = connectionString;
        }

        public UserInfo GetUserByUsername(string username)
        {
            UserInfo userInfo = null;
            using (SqlConnection connection = this.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(this.getUserByUsernameSpName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("LOGIN", username.Trim()));
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        userInfo = this.ReadSingleUser(reader);
                    }
                }
            }
            return userInfo;
        }

        public void LogClientIpAddress(string ipAddress, int? userId, string userLogin, string operation)
        {
            using (SqlConnection connection = this.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(this.logClientIpAddressSpName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("IP_ADDRESS", ipAddress.Trim()));
                    command.Parameters.Add(new SqlParameter("USER_LOGIN", userLogin));
                    command.Parameters.Add(new SqlParameter("USER_ID", userId));
                    command.Parameters.Add(new SqlParameter("OPERATION_TYPE", operation));
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool ValidatePassword(string username, string password, string ipAddress = null)
        {
            bool result = false;
            int? userId;
            using (SqlConnection connection = this.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(this.authenticateApplicationUserSpName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("LOGIN", username.Trim()));
                    command.Parameters.Add(new SqlParameter("HASH", Crypto.HashString(password)));
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (reader.Read())
                        {
                            result = true;
                            userId = reader.GetInt32(0);
                        }
                        else
                        {
                            result = false;
                            userId = null;
                        }
                    }
                }
            }
            if (ipAddress != null)
                LogClientIpAddress(ipAddress, userId, username, "LOGIN");

            return result;
        }

        //public Task ActivateUser(string id)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task AddUser(ApplicationUser user)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task ChangeUserPassword(string id, string oldPassword, string newPassword)
        {
            using (SqlConnection connection = this.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(this.changeApplicationUserPasswordSpName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("ID", id));
                    command.Parameters.Add(new SqlParameter("HASH", Crypto.HashString(newPassword)));
                    command.Parameters.Add(new SqlParameter("PASSWORD_EXPIRY_DATE", DateTime.Now.Date.AddDays(90)));
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        //public Task DeactivateUser(string id)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task DeleteUser(string id)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<IEnumerable<ApplicationUser>> GetAllUsers()
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<IEnumerable<ApplicationUser>> GetAllUsers(int from, int count)
        //{
        //    throw new NotImplementedException();
        //}

        public UserInfo GetUserById(string userId)
        {
            UserInfo userInfo = null;

            int id;
            if (int.TryParse(userId, out id))
            {
                using (SqlConnection connection = this.GetConnection())
                {
                    using (SqlCommand command = new SqlCommand(this.getUserByIdSpName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("ID", id));
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow))
                        {
                            userInfo = this.ReadSingleUser(reader);
                        }
                    }
                }
            }
            return userInfo;
        }

        //public Task<IEnumerable<ApplicationUser>> SearchUsers(string criteria)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task UpdateUserInfo(ApplicationUser user)
        //{
        //    throw new NotImplementedException();
        //}

        private UserInfo ReadSingleUser(SqlDataReader reader)
        {
            UserInfo result = null;
            if (reader.Read())
            {
                result = this.ReadUser(reader);
            }
            return result;
        }

        private UserInfo ReadUser(SqlDataReader reader)
        {
            UserInfo result = null;
            result = new UserInfo();
            result.Id = reader.GetInt32(reader.GetOrdinal("ID"));
            result.Username = reader.GetString(reader.GetOrdinal("LOGIN"));
            int emailOrdinal = reader.GetOrdinal("EMAIL");
            if (!reader.IsDBNull(emailOrdinal))
            {
                result.Email = reader.GetString(emailOrdinal);
            }
            result.FullName = reader.GetString(reader.GetOrdinal("FIRST_NAME")) + reader.GetString(reader.GetOrdinal("LAST_NAME"));
            return result;
        }

        private SqlConnection GetConnection()
        {
            SqlConnection connection = new SqlConnection(this.connectionString);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            return connection;
        }

        private async Task<SqlConnection> GetConnectionAsync()
        {
            SqlConnection connection = new SqlConnection(this.connectionString);
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            return connection;
        }

        public async Task<IEnumerable<string>> GetUserRolesById(string userId)
        {
            List<string> roles = new List<string>();
            int id;
            if (int.TryParse(userId, out id))
            {
                using (SqlConnection connection = await this.GetConnectionAsync())
                {
                    using (SqlCommand command = new SqlCommand("select dbo.f_GetApplicationUserRoleName(@id)", connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@id", id));
                        using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow))
                        {
                            if (reader.Read())
                            {
                                string roleName = reader.GetString(0);
                                if (roleName != null)
                                {
                                    roles.Add(roleName);
                                    if (roleName == "BankPowerUser")
                                    {
                                        roles.Add("BankUser");
                                    }
                                    else if (roleName == "ShopPowerUser")
                                    {
                                        roles.Add("ShopUser");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return roles;
        }
    }
}
