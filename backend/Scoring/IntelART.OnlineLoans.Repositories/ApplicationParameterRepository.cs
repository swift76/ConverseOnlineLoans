using System.Threading.Tasks;
using Dapper;
using IntelART.OnlineLoans.Entities;
using System.Collections.Generic;

namespace IntelART.OnlineLoans.Repositories
{
    public class ApplicationParameterRepository : BaseRepository
    {
        public ApplicationParameterRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<LoanLimits> GetLoanLimits(string loanTypeCode, string currency)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("LOAN_TYPE_CODE", loanTypeCode);
            parameters.Add("CURRENCY", currency);
            return await GetSingleAsync<LoanLimits>(parameters, "dbo.sp_GetLoanLimits");
        }

        public async Task<LoanParameters> GetLoanParameters(string loanTypeCode)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("LOAN_TYPE_CODE", loanTypeCode);
            return await GetSingleAsync<LoanParameters>(parameters, "dbo.sp_GetLoanParameters");
        }

        public async Task<IEnumerable<Shop>> GetParentShops()
        {
            return await GetListAsync<Shop>(new DynamicParameters(), "IL.sp_GetParentShops");
        }

        public int GetFileMaxSize()
        {
            int fileMaxSize = int.Parse(GetSetting("FILE_MAX_SIZE"));
            return fileMaxSize;
        }

        public async Task<GeneralLoanSetting> GetGeneralLoanSettings()
        {
            return await GetSingleAsync<GeneralLoanSetting>(new DynamicParameters(), "dbo.sp_GetGeneralLoanSettings");
        }
    }
}
