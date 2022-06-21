if exists (select * from sys.objects where name='ol0sp_GetTemplateData' and type='P')
	drop procedure ol0sp_GetTemplateData
GO

CREATE PROCEDURE ol0sp_GetTemplateData(
	@SubsystemCode varchar(10),
	@TemplateCode varchar(10),
	@Date datetime,
	@PeriodMonthFrom int output,
	@PeriodMonthTo int output,
	@Interest decimal output,
	@RepayDay int output
) AS
set @RepayDay = 5
GO

