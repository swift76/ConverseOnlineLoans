if exists (select * from sys.objects where name='sp_SaveCommunicationType' and type='P')
	drop procedure dbo.sp_SaveCommunicationType
GO

create procedure dbo.sp_SaveCommunicationType(@CODE		char(4),
											  @NAME_AM	nvarchar(50),
											  @NAME_EN	varchar(50))
AS
	insert into dbo.COMMUNICATION_TYPE (CODE,NAME_AM,NAME_EN)
		values (@CODE,dbo.ahf_ANSI2Unicode(@NAME_AM),@NAME_EN)
GO
