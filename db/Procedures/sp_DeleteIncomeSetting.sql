create or alter procedure sp_DeleteIncomeSetting(@SCORECARD_CODE	varchar(3))
AS
	delete from INCOME_SETTING
	where SCORECARD_CODE=@SCORECARD_CODE
GO
