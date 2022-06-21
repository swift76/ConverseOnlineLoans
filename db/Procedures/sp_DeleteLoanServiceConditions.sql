if exists (select * from sys.objects where name='sp_DeleteLoanServiceConditions' and type='P')
	drop procedure dbo.sp_DeleteLoanServiceConditions
GO

create procedure dbo.sp_DeleteLoanServiceConditions
AS
	truncate table dbo.LOAN_SERVICE_CONDITION
GO
