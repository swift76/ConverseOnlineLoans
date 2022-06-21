create or alter procedure sp_GetGenderSetting(
	@SCORECARD_CODE	varchar(3)
	,@GENDER bit
)
AS
	select SCORE
	from GENDER_SETTING
	where SCORECARD_CODE=@SCORECARD_CODE
		and GENDER=@GENDER
GO
