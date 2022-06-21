if exists (select * from sys.objects where name='ol0sp_GetSchedule' and type='P')
	drop procedure dbo.ol0sp_GetSchedule
GO

create procedure ol0sp_GetSchedule(@SubsystemCode		char(2),
								   @TemplateCode		char(4),
								   @DateAgreementFrom	date,
								   @DateFirstRepayment	date,
								   @DateAgreementTo		date,
								   @LoanDuration		int,
								   @Amount				money,
								   @Interest			money,
								   @RepayDay			tinyint)
AS
	Select getdate() as Date,0 as MainAmount,0 as InterestAmount
GO
