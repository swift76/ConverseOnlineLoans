if exists (select * from sys.objects where name='sp_FinalizeDueApplications' and type='P')
	drop procedure dbo.sp_FinalizeDueApplications
GO

create procedure dbo.sp_FinalizeDueApplications(@GLDueDays	int)
AS
	BEGIN TRY

		declare @CurrentDate date = convert(date,getdate())

		if @GLDueDays>0
			update dbo.APPLICATION
			set STATUS_ID=55,
				TO_BE_SYNCHRONIZED=1
			where LOAN_TYPE_ID<>'00'
				and STATUS_ID in (1,2,3,5,7,8,10,11,12,13,15,19,20,99)
				and datediff(day, convert(date,CREATION_DATE), @CurrentDate)>@GLDueDays
	END TRY
	BEGIN CATCH

	END CATCH
GO
