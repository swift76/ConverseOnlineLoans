if exists (select * from sys.objects where name='sp_GetAddressCountries' and type='P')
	drop procedure dbo.sp_GetAddressCountries
GO

create procedure dbo.sp_GetAddressCountries(@LANGUAGE_CODE	char(2))
AS
	select CODE,
		case upper(@LANGUAGE_CODE)
			when 'AM' then NAME_AM
			else NAME_EN
		end as NAME
	from dbo.COUNTRY
	where CODE = 'AM'
/*
	union all

	select CODE,
		case upper(@LANGUAGE_CODE)
			when 'AM' then NAME_AM
			else NAME_EN
		end as NAME
	from dbo.COUNTRY
	where CODE = 'NK'
*/
GO