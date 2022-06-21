if exists (select * from sys.objects where name='ol0sp_GetLastRepaymentDate' and type='P')
	drop procedure ol0sp_GetLastRepaymentDate
GO

CREATE PROCEDURE ol0sp_GetLastRepaymentDate(
	@SubsystemCode varchar(10),
	@TemplateCode varchar(10),
	@DateAgreementFrom datetime,
	@DateFirstRepayment datetime,
	@DateAgreementTo datetime,
	@LoanDuration int,
	@Amount money,
	@Interest money,
	@RepayDay tinyint
) AS
select GETDATE();
GO
