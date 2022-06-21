if exists (select * from sys.objects where name='sp_ResetRequestsTryCount' and type='P')
	drop procedure dbo.sp_ResetRequestsTryCount
GO

create procedure dbo.sp_ResetRequestsTryCount(@ISN	int)
AS
	update dbo.APPLICATION set
		NORQ_TRY_COUNT=0,
		ACRA_TRY_COUNT=0
	where ISN=@ISN
GO
