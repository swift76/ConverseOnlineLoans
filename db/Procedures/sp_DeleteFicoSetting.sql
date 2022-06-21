create or alter procedure sp_DeleteFicoSetting(@SCORECARD_CODE	varchar(3))
AS
	delete from FICO_SETTING
	where SCORECARD_CODE=@SCORECARD_CODE
GO
