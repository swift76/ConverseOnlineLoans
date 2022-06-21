if exists (select * from sys.objects where name='sp_GetCities' and type='P')
	drop procedure dbo.sp_GetCities
GO

create procedure dbo.sp_GetCities (@LANGUAGE_CODE	char(2),
								   @STATE_CODE		char(3))
AS
	select CODE,
		case upper(@LANGUAGE_CODE)
			when 'AM' then NAME_AM
			else NAME_EN
		end as NAME
	from dbo.CITY
	where STATE_CODE=@STATE_CODE
GO
