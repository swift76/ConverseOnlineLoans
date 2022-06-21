create or alter procedure sp_GetFicoSetting(@VALUE int)
AS
	select SCORE
	from FICO_SETTING
	where @VALUE between VALUE_FROM and VALUE_TO
GO
