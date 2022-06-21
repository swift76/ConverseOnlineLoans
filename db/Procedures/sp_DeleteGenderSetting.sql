create or alter procedure sp_DeleteGenderSetting(@SCORECARD_CODE	varchar(3))
AS
	delete from GENDER_SETTING
	where SCORECARD_CODE=@SCORECARD_CODE
GO
