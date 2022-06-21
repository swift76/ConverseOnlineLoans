create or alter procedure sp_GetIncomeSetting(@VALUE	money)
AS
	select SCORE
	from INCOME_SETTING
	where @VALUE between VALUE_FROM and VALUE_TO
GO
