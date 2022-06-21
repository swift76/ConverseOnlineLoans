if exists (select * from sys.objects where name='ol0sp_GetInterestAmount' and type='P')
	drop procedure ol0sp_GetInterestAmount
GO

CREATE PROCEDURE ol0sp_GetInterestAmount(
	@TERM_MONTH int,
	@AGREEMENT_FROM datetime,
	@FIRST_REPAYMENT datetime,
	@AGREEMENT_TO datetime,
	@TEMPLATE_CODE varchar(10),
	@SUBSYSTEM_CODE varchar(10),
	@AMOUNT decimal,
	@INTEREST decimal,
	@REPAY_DAY int
) AS
select 10;
GO

