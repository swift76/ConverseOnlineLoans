﻿if exists (select * from sys.objects where name='sp_SaveCity' and type='P')
	drop procedure dbo.sp_SaveCity
GO

create procedure dbo.sp_SaveCity(@CODE		varchar(30),
									@STATE_CODE	char(3),
									@NAME_AM	nvarchar(50),
									@NAME_EN	varchar(50))
AS
	insert into dbo.CITY (CODE,STATE_CODE,NAME_AM,NAME_EN)
		values (@CODE,@STATE_CODE,dbo.ahf_ANSI2Unicode(@NAME_AM),@NAME_EN)
GO
