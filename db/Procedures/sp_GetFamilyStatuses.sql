if exists (select * from sys.objects where name='sp_GetFamilyStatuses' and type='P')
	drop procedure dbo.sp_GetFamilyStatuses
GO

create procedure dbo.sp_GetFamilyStatuses(@LANGUAGE_CODE	char(2))
AS
	select CODE,
		case upper(@LANGUAGE_CODE)
			when 'AM' then NAME_AM
			else NAME_EN
		end as NAME
	from dbo.FAMILY_STATUS
GO
