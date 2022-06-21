if exists (select * from sys.objects where name='sp_ChangeApplicationStatus' and type='P')
	drop procedure dbo.sp_ChangeApplicationStatus
GO

create procedure dbo.sp_ChangeApplicationStatus(@APPLICATION_ID	uniqueidentifier,
												@STATUS_ID		tinyint,
												@TO_SYNCHRONIZE	bit = null)
AS
	declare @TO_BE_SYNCHRONIZED bit
	if @STATUS_ID in (2,3,12,13,16,21)
		set @TO_BE_SYNCHRONIZED=1
	else
	begin
		if @STATUS_ID=10
		begin
			set @STATUS_ID=13
			set @TO_BE_SYNCHRONIZED=1
		end
		else if @STATUS_ID=15
		begin
			declare @CUSTOMER_STATUS_ID tinyint,@LOAN_GETTING_OPTION_CODE char(1),@CUSTOMER_USER_ID int
			select
				@CUSTOMER_STATUS_ID=CUSTOMER_STATUS_ID,
				@LOAN_GETTING_OPTION_CODE=LOAN_GETTING_OPTION_CODE,
				@CUSTOMER_USER_ID = CUSTOMER_USER_ID
			from dbo.APPLICATION with (UPDLOCK) where ID=@APPLICATION_ID

			if (@CUSTOMER_STATUS_ID<>3 or @LOAN_GETTING_OPTION_CODE='1') and not @CUSTOMER_USER_ID is null
				set @STATUS_ID=12
			set @TO_BE_SYNCHRONIZED=1
		end
		else
			set @TO_BE_SYNCHRONIZED=null
	end

	update dbo.APPLICATION set
		STATUS_ID=@STATUS_ID,
		TO_BE_SYNCHRONIZED=
			case
				when @TO_SYNCHRONIZE is null then isnull(@TO_BE_SYNCHRONIZED,TO_BE_SYNCHRONIZED)
				else @TO_SYNCHRONIZE
			end
	where ID=@APPLICATION_ID
GO
