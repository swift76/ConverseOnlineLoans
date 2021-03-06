CREATE PROCEDURE sp_UpdateArcaOrderById (
	@FORM_URL nvarchar(1024),
	@ARCA_ORDER_ID nvarchar(64),
	@ERROR_CODE NVARCHAR(16),
	@ID BIGINT
)
AS
BEGIN
	UPDATE ARCA_PAYMENT_ORDER
	SET FORM_URL = ISNULL(@FORM_URL, FORM_URL),
	ARCA_ORDER_ID = ISNULL(@ARCA_ORDER_ID, ARCA_ORDER_ID),
	ERROR_CODE = ISNULL(@ERROR_CODE, ERROR_CODE)
	WHERE ID = @ID
END
GO