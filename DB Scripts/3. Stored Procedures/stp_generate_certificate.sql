SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Nem Ekpunobi
-- Create date: April 1, 2019
-- Description:	Generates New Certificates
-- =============================================
USE certitrack;
GO

CREATE PROCEDURE stpGenerateCertificate
	@cert_qty int = 1,
	@price money = 30
AS
BEGIN
	SET NOCOUNT ON;

	-- GETS ONLY DATE PORTION OF GETDATE() FUNCTION
	DECLARE @current_date DATE = CONVERT(DATE, GETDATE())

	-- REMOVES HYPENS FROM DATE TO PREP RESULT FOR OPERATIONS
	DECLARE @clean_date DECIMAL = REPLACE (
			CONVERT(VARCHAR, @current_date), '-', '');

	
	-- CHECKS MOST RECENT CERT NO. - DEFAULTING TO [@current_date]000
	DECLARE @latest_no DECIMAL = CONCAT(@clean_date, '000');
	WHILE (@cert_qty > 0)
	BEGIN
		-- GETS MOST RECENT CERTIFICATE NO. IF IT EXISTS
		IF EXISTS (
			SELECT 1 FROM [certificate]
			WHERE [certificate_no] like CONCAT(@clean_date, '%'))
		BEGIN
			SET @latest_no = (
				SELECT TOP(1) certificate_no FROM [certificate]
				WHERE [certificate_no] like CONCAT(@clean_date, '%')
				ORDER BY certificate_no DESC);
		END

		-- INCREMENTS CERT NO. BY ONE TO GENERATE NEW CERT
		DECLARE @new_no DECIMAL = @latest_no + 1;

		-- GENERATES EXPIRY DATE ONE MONTH FROM CURRENT DATE
		DECLARE @expiry_date DATE = (SELECT DATEADD(MONTH, 1, @current_date));

		-- INJECTS NEW VALUES INTO NEW CERTIFICATE RECORD
		INSERT INTO [certificate] (certificate_no, [expiry_date], price)
		VALUES (@new_no, @expiry_date, @price);

		-- DECREMENT CERT QTY VALUE BY 1 IN WHILE LOOP
		SET @cert_qty -= 1;
	END


	-- DISPLAYS PREVIOUS AND NEW VALUES FOR CERT NO.
	SELECT @latest_no AS PreviousCertNo, @new_no AS NewCertNo;

	-- DISPLAY THE RESULTING TABLE
	SELECT * FROM [certificate];


	-- RETURN MOST RECENT CERT ID
	RETURN (
		SELECT [id] FROM [certificate]
		WHERE [certificate_no] LIKE @new_no)
END

GO
