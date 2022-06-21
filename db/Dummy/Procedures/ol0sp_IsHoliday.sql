if exists (select * from sys.objects where name='ol0sp_IsHoliday' and type='P')
	drop procedure ol0sp_IsHoliday
GO

CREATE PROCEDURE ol0sp_IsHoliday(
	@Date datetime
) AS
select 0;
GO

