SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Nem Ekpunobi
-- Create date: April 2, 2019
-- Description:	Redeems Certificate
-- =============================================
USE certitrack;
GO

CREATE PROCEDURE stpRedeemCertificate 
	@redeemed BIT OUTPUT,	-- BOOLEAN TO RETURN REDEMPTION STATUS
	@cert_no DECIMAL		-- CERTIFICATE NO.
AS
BEGIN
	SET NOCOUNT ON;
	-- DISPLAY PRE-OP TABLE
	SELECT * FROM [certificate];

	-- UPDATES [date_redeemed] FIELD WITH CURRENT DATE
	UPDATE [certificate]
	SET [date_redeemed] = CONVERT(DATE, GETDATE())
	WHERE [certificate_no] LIKE @cert_no

	-- SET REDEMPTION STATUS TO 1 (TRUE) IF UPDATE SUCCESSFUL
	IF EXISTS (
		SELECT 1 FROM [certificate]
		WHERE [date_redeemed] = CONVERT(DATE, GETDATE())
		AND	[certificate_no] LIKE @cert_no)
		BEGIN
			SET @redeemed = 1;
		END

	-- DISPLAY POST-OP TABLE
	SELECT * FROM [certificate];
	-- DISPLAY REDEMPTION STATUS WITH CERT NO.
	SELECT @cert_no AS CertNo,  @redeemed AS IsRedeemed;

	RETURN @redeemed;
END
GO