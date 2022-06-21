if exists (select * from sys.objects where name='sp_SetPrintExists' and type='P')
	drop procedure dbo.sp_SetPrintExists
GO

create procedure dbo.sp_SetPrintExists(@APPLICATION_ID		uniqueidentifier,
									   @PRINT_EXISTS		bit,
									   @AGREEMENT_NUMBER	nvarchar(14))
AS
	update APPLICATION
	set PRINT_EXISTS = @PRINT_EXISTS
		,AGREEMENT_NUMBER = dbo.ahf_ANSI2Unicode(@AGREEMENT_NUMBER)
	where ID = @APPLICATION_ID
GO
