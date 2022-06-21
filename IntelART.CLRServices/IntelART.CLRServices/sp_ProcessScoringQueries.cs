using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using IntelART.CLRServices;

public partial class StoredProcedures
{
    [SqlProcedure]
    public static void sp_ProcessScoringQueries(SqlInt32 queryTimeout)
    {
        DataHelper.CommandTimeoutInterval = queryTimeout.Value;
        ServiceHelper.QueryTimeout = queryTimeout.Value;
        using (DataHelper helper = new DataHelper())
        {
            try
            {
                DoNORQQueries(helper, helper.GetApplicationsForNORQRequest());
            }
            catch (Exception ex)
            {
                helper.LogError("NORQ Query", ex.ToString());
            }

            try
            {
                DoACRAQueries(helper, helper.GetApplicationsForACRARequest());
            }
            catch (Exception ex)
            {
                helper.LogError("ACRA Query", ex.ToString());
            }
        }
    }

    [SqlProcedure]
    public static void sp_ProcessScoringQueriesByID(SqlInt32 queryTimeout, SqlGuid id)
    {
        DataHelper.CommandTimeoutInterval = queryTimeout.Value;
        ServiceHelper.QueryTimeout = queryTimeout.Value;
        using (DataHelper helper = new DataHelper())
        {
            try
            {
                DoNORQQueries(helper, helper.GetApplicationForNORQRequestByID(id.Value));
            }
            catch (Exception ex)
            {
                helper.LogError("NORQ Query", ex.ToString());
            }

            try
            {
                DoACRAQueries(helper, helper.GetApplicationForACRARequestByID(id.Value));
            }
            catch (Exception ex)
            {
                helper.LogError("ACRA Query", ex.ToString());
            }
        }
    }

    [SqlProcedure]
    public static void sp_ProcessScoringQueriesByISN(SqlInt32 queryTimeout, SqlInt32 isn)
    {
        DataHelper.CommandTimeoutInterval = queryTimeout.Value;
        ServiceHelper.QueryTimeout = queryTimeout.Value;
        using (DataHelper helper = new DataHelper())
        {
            try
            {
                DoNORQQueries(helper, helper.GetApplicationForNORQRequestByISN(isn.Value));
            }
            catch (Exception ex)
            {
                helper.LogError("NORQ Query", ex.ToString());
            }

            try
            {
                DoACRAQueries(helper, helper.GetApplicationForACRARequestByISN(isn.Value));
            }
            catch (Exception ex)
            {
                helper.LogError("ACRA Query", ex.ToString());
            }
        }
    }

    [SqlProcedure]
    public static void sp_ProcessNORQQueryByID(SqlInt32 queryTimeout, SqlGuid id)
    {
        DataHelper.CommandTimeoutInterval = queryTimeout.Value;
        ServiceHelper.QueryTimeout = queryTimeout.Value;
        using (DataHelper helper = new DataHelper())
        {
            try
            {
                DoNORQQueries(helper, helper.GetApplicationForNORQRequestByID(id.Value));
            }
            catch (Exception ex)
            {
                helper.LogError("NORQ Query", ex.ToString());
            }
        }
    }

    [SqlProcedure]
    public static void sp_ProcessACRAQueryByID(SqlInt32 queryTimeout, SqlGuid id)
    {
        DataHelper.CommandTimeoutInterval = queryTimeout.Value;
        ServiceHelper.QueryTimeout = queryTimeout.Value;
        using (DataHelper helper = new DataHelper())
        {
            try
            {
                DoACRAQueries(helper, helper.GetApplicationsForACRARequest());
            }
            catch (Exception ex)
            {
                helper.LogError("ACRA Query", ex.ToString());
            }
        }
    }

    private static void DoNORQQueries(DataHelper helper, List<NORQRequest> entities_NORQ)
    {
        ServiceConfig config = helper.GetServiceConfig("NORQ");
        if (entities_NORQ.Count > 0)
        {
            string sessionID = ServiceHelper.NORQ_Login(config);
            NORQQuery norqQuery = new NORQQuery();
            foreach (NORQRequest entity in entities_NORQ)
            {
                try
                {
                    norqQuery.GetResponse(helper, entity, config.URL, sessionID);
                }
                catch (Exception ex)
                {
                    helper.LogError("NORQ Query", ex.ToString(), entity.ID);
                }
            }
        }
    }

    private static void DoACRAQueries(DataHelper helper, List<ACRARequest> entities_ACRA)
    {
        ServiceConfig config = helper.GetServiceConfig("ACRA");
        if (entities_ACRA.Count > 0)
        {
            string cookie = null;
            string sessionID = ServiceHelper.ACRA_Login(config, ref cookie);

            ACRAQuery acraQuery = new ACRAQuery();
            foreach (ACRARequest entity in entities_ACRA)
                try
                {
                    acraQuery.GetResponse(helper, entity, config.URL, sessionID, cookie);
                }
                catch (Exception ex)
                {
                    helper.LogError("ACRA Query", ex.ToString(), entity.ID);
                }
        }
    }
};
