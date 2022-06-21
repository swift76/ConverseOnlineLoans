using Dapper;
using System.Collections.Generic;
using IntelART.OnlineLoans.Entities;

namespace IntelART.OnlineLoans.Repositories
{
    public class ShopRepository : BaseRepository
    {
        public ShopRepository(string connectionString) : base(connectionString)
        {
        }

        public IEnumerable<Shop> GetShops()
        {
            return GetList<Shop>(new DynamicParameters(), "IL.sp_GetShops");
        }
    }
}
