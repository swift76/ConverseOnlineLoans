if exists (select * from sys.objects where name='sp_GetApplicationInformation' and type='P')
	drop procedure dbo.sp_GetApplicationInformation
GO

create procedure dbo.sp_GetApplicationInformation(@APPLICATION_ID	uniqueidentifier)
AS
	select	STATUS_ID,
			LOAN_TYPE_ID,
			REFUSAL_REASON,
			MANUAL_REASON,
			dbo.f_GetApprovedAmount(ID, LOAN_TYPE_ID) + ' ' + CURRENCY_CODE as APPROVED_AMOUNT
	from dbo.APPLICATION
	where ID = @APPLICATION_ID
GO
