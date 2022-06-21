if exists (select * from sys.objects where name='sp_SaveApplicationScanType' and type='P')
	drop procedure dbo.sp_SaveApplicationScanType
GO

create procedure dbo.sp_SaveApplicationScanType(@CODE		char(1),
												@NAME_AM	nvarchar(50),
												@NAME_EN	varchar(50))
AS
	insert into dbo.APPLICATION_SCAN_TYPE (CODE,NAME_AM,NAME_EN)
		values (@CODE,dbo.ahf_ANSI2Unicode(@NAME_AM),@NAME_EN)
GO
