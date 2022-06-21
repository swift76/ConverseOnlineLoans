if exists (select * from sys.objects where name='ol0sp_GetPassHolidayDate' and type='P')
	drop procedure ol0sp_GetPassHolidayDate
GO

CREATE PROCEDURE ol0sp_GetPassHolidayDate(
	@SubsystemCode varchar(10),
	@TemplateCode varchar(10),
	@Date datetime
) AS
select GETDATE();
GO
