if exists (select * from sys.objects where name='sp_SaveWorkingExperience' and type='P')
	drop procedure dbo.sp_SaveWorkingExperience
GO

create procedure dbo.sp_SaveWorkingExperience(@CODE		char(1),
											 @NAME_AM	nvarchar(50),
											 @NAME_EN	varchar(50))
AS
	insert into dbo.WORKING_EXPERIENCE (CODE,NAME_AM,NAME_EN)
		values (@CODE,dbo.ahf_ANSI2Unicode(@NAME_AM),@NAME_EN)
GO
