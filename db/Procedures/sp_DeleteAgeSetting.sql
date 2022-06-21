create or alter procedure sp_DeleteAgeSetting(@SCORECARD_CODE	varchar(3))
AS
	delete from AGE_SETTING
	where SCORECARD_CODE=@SCORECARD_CODE
GO
