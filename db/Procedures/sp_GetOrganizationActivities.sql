if exists (select * from sys.objects where name='sp_GetOrganizationActivities' and type='P')
	drop procedure dbo.sp_GetOrganizationActivities
GO

create procedure dbo.sp_GetOrganizationActivities(@LANGUAGE_CODE	char(2))
AS
	select CODE,
		case upper(@LANGUAGE_CODE)
			when 'AM' then NAME_AM
			else NAME_EN
		end as NAME
	from dbo.ORGANIZATION_ACTIVITY
GO
