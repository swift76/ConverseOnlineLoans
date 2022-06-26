create or alter procedure sp_SaveFicoSettingParameters(
	@PAYMENT_HISTORY_YEAR	tinyint,
	@HAD_LOAN_YEAR			tinyint,
	@NEW_LOAN_MONTH			tinyint,
	@NEW_QUERY_MONTH		tinyint,
	@NEW_RATIO_MONTH		tinyint,
	@NON_BANK_INTEREST		money
)
AS
	delete from FICO_SETTING_PARAMETER
	insert into FICO_SETTING_PARAMETER(PAYMENT_HISTORY_YEAR, HAD_LOAN_YEAR, NEW_LOAN_MONTH, NEW_QUERY_MONTH, NEW_RATIO_MONTH, NON_BANK_INTEREST)
	values (@PAYMENT_HISTORY_YEAR, @HAD_LOAN_YEAR, @NEW_LOAN_MONTH, @NEW_QUERY_MONTH, @NEW_RATIO_MONTH, @NON_BANK_INTEREST)
GO
