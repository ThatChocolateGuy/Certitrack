SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Nem Ekpunobi
-- Create date: April 4, 2019
-- Description:	Creates New Customer
--				Returns Success Status
-- =============================================
USE certitrack
GO

CREATE PROCEDURE stpGetCreateCustomer
	@name varchar(50),
	@email varchar(50),
	@phone varchar(15),
	@id int output -- Pass Back Customer Id
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @status int = 0;

	-- CREATES CUSTOMER IF DOESN'T EXIST --
	IF NOT EXISTS (
		SELECT 1 FROM [customer]
		WHERE [email] LIKE @email
		OR [phone] LIKE @phone
	)
	BEGIN
		INSERT INTO [customer] ([name], email, phone)
		VALUES (@name, @email, @phone)

		SET @status = 1 -- Validates Customer Creation
	END

	-- GETS CUSTOMER ID IF NO INFO MISMATCH --
	DECLARE @tempId1 int, @tempId2 int;

	SET @tempId1 = (
		SELECT [id] FROM [customer]
		WHERE [email] LIKE @email)
	SET @tempId2 = (
		SELECT [id] FROM [customer]
		WHERE [phone] LIKE @phone)

	IF @tempId1 NOT LIKE @tempId2
	OR @tempId1 IS NULL OR @tempId2 IS NULL
	BEGIN
		DECLARE @message varchar(50)

		IF @tempId1 IS NOT NULL AND @tempId2 IS NULL
			SET @message = 'Phone Does Not Match Existing Email'

		ELSE IF @tempId2 IS NOT NULL AND @tempId1 IS NULL
			SET @message = 'Email Does Not Match Existing Phone'

		ELSE SET @message = 'Both Email and Phone Exist for Different Customers'

		SELECT TOP(2) [id] AS 'Customer IDs Detected' FROM [customer]
		WHERE [email] LIKE @email
		OR [phone] LIKE @phone

		SELECT @message AS 'Error Message'

		RETURN @status
	END

	SELECT @id = [id] FROM [customer]
	WHERE [email] LIKE @email
	OR [phone] LIKE @phone

	SELECT * FROM [customer]
	SELECT @id AS 'Customer ID - stpCreateCustomer'

	RETURN @status
END
GO
