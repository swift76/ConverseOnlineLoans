if exists (select * from sys.objects where name='sp_SendLoanApplicationEmailSMSNotification' and type='P')
	drop procedure dbo.sp_SendLoanApplicationEmailSMSNotification
GO

create procedure sp_SendLoanApplicationEmailSMSNotification(@EmailSMS	bit,
															@Address	varchar(100),
															@Subject	nvarchar(100),
															@Body		nvarchar(max))
AS
GO
