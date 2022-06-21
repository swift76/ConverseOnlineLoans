if exists (select * from sys.objects where name='sp_ProcessScoringQueriesByISN' and type in ( N'P', N'PC' ))
	drop procedure dbo.sp_ProcessScoringQueriesByISN
GO



if exists (select * from sys.objects where name='sp_ProcessScoringQueriesByID' and type in ( N'P', N'PC' ))
	drop procedure dbo.sp_ProcessScoringQueriesByID
GO



if exists (select * from sys.objects where name='sp_ProcessScoringQueries' and type in ( N'P', N'PC' ))
	drop procedure dbo.sp_ProcessScoringQueries
GO



CREATE PROCEDURE dbo.sp_ProcessScoringQueries
@queryTimeout INT
AS
GO



CREATE PROCEDURE dbo.sp_ProcessScoringQueriesByID(
	@queryTimeout	INT,
	@ID				uniqueidentifier
)
AS
GO



CREATE PROCEDURE dbo.sp_ProcessScoringQueriesByISN(
	@queryTimeout	INT,
	@ID				INT
)
AS
GO
