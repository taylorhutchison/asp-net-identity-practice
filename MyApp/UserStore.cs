using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using System;


namespace MyApp {
    public class UserStore : IUserStore<User>, IUserPasswordStore<User>
    {
        public async static Task<SqliteConnection> GetConnection() {
            //var connectionStringBuilder = new SqliteConnectionStringBuilder();
            //connectionStringBuilder.DataSource = "./data.db";
            var connection = new SqliteConnection("Filename=./data.db");
            await connection.OpenAsync();
            return connection;
        }
        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            using(var connection = await GetConnection()) {
                var transaction = await connection.BeginTransactionAsync();
                var command = connection.CreateCommand();
                command.CommandText = @"INSERT INTO USERS 
                (ID, USERNAME, NORMALIZEDUSERNAME, PASSWORDHASH) 
                VALUES (:ID, :UN, :NUM, :PH);";
                command.Parameters.AddWithValue(":ID", user.Id);
                command.Parameters.AddWithValue(":UN", user.UserName);
                command.Parameters.AddWithValue(":NUM", user.NormalizedUserName);
                command.Parameters.AddWithValue(":PH", user.PasswordHash);

                await command.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
            }
            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            using(var connection = await GetConnection()) {
                var command = connection.CreateCommand();
                command.CommandText = 
                @"
                SELECT ID, USERNAME, NORMALIZEDUSERNAME, PASSWORDHASH
                FROM USERS WHERE ID = :ID;
                ";
                command.Parameters.AddWithValue(":ID", userId);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        return new User {
                            Id = reader.GetString(0),
                            UserName = reader.GetString(1),
                            NormalizedUserName = reader.GetString(2),
                            PasswordHash = reader.GetString(3)
                        };
                    }
                }
                return null;
            }
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
             using(var connection = await GetConnection()) {
                var command = connection.CreateCommand();
                command.CommandText = 
                @"
                SELECT ID, USERNAME, NORMALIZEDUSERNAME, PASSWORDHASH
                FROM USERS WHERE NORMALIZEDUSERNAME = :NUM;
                ";
                command.Parameters.AddWithValue(":NUM", normalizedUserName);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        return new User {
                            Id = reader.GetString(0),
                            UserName = reader.GetString(1),
                            NormalizedUserName = reader.GetString(2),
                            PasswordHash = reader.GetString(3)
                        };
                    }
                }
                return null;
            }
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
			return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            user.UserName = normalizedName;
			return Task.CompletedTask;
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
			return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
             using(var connection = await GetConnection()) {
                var transaction = await connection.BeginTransactionAsync();
                var command = connection.CreateCommand();
                command.CommandText = @"UPDATE USERS SET
                (ID, USERNAME, NORMALIZEDUSERNAME, PASSWORDHASH) 
                VALUES (:ID, :UN, :NUM, :PH) WHERE ID = :ID;";
                command.Parameters.AddWithValue(":ID", user.Id);
                command.Parameters.AddWithValue(":UN", user.UserName);
                command.Parameters.AddWithValue(":NUM", user.NormalizedUserName);
                command.Parameters.AddWithValue(":PH", user.PasswordHash);
                
                await command.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
            }
            return IdentityResult.Success;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UserStore()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }
        #endregion
    }
}
