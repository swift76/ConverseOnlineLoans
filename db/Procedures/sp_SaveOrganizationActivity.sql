if exists (select * from sys.objects where name='sp_SaveOrganizationActivity' and type='P')
	drop procedure dbo.sp_SaveOrganizationActivity
GO

create procedure dbo.sp_SaveOrganizationActivity(@CODE		char(2),
												 @NAME_AM	nvarchar(50),
												 @NAME_EN	varchar(50))
AS
	insert into dbo.ORGANIZATION_ACTIVITY (CODE,NAME_AM,NAME_EN)
		values (@CODE,dbo.ahf_ANSI2Unicode(@NAME_AM),@NAME_EN)
GO
