if exists (select * from sys.objects where name='sp_GetCountries' and type='P')
	drop procedure dbo.sp_GetCountries
GO

create procedure dbo.sp_GetCountries(@LANGUAGE_CODE	char(2))
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
	union all

	select CODE,
		case upper(@LANGUAGE_CODE)
			when 'AM' then NAME_AM
			else NAME_EN
		end as NAME
	from dbo.COUNTRY
	where not CODE in ('AM','NK')
GO