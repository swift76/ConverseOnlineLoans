if exists (select * from sys.objects where name='sp_SaveCardReceivingOptions' and type='P')
	drop procedure dbo.sp_SaveCardReceivingOptions
GO

create procedure dbo.sp_SaveCardReceivingOptions(@CODE		char(2),
												 @NAME_AM	nvarchar(50),
												 @NAME_EN	varchar(50))
AS
	insert into dbo.CARD_RECEIVING_OPTIONS (CODE,NAME_AM,NAME_EN)
		values (@CODE,dbo.ahf_ANSI2Unicode(@NAME_AM),@NAME_EN)
GO
