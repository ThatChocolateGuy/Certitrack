USE [certitrack]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Nem Ekpunobi
-- Create date: April 10, 2019
-- Description:	Get/Create Promotion
--	Returns Promotion Creation Status
-- =============================================
CREATE PROCEDURE stpGetCreatePromo
	@discount int = 0,
	@id int output
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @status int = 0
	
	-- CREATE PROMOTION IF DOESN'T EXIST --
	IF NOT EXISTS (
		SELECT 1 FROM [promotion]
		WHERE [discount] LIKE @discount )
	BEGIN
		INSERT INTO [promotion] ([discount])
		VALUES (@discount)

		SET @status = 1 -- creation successful
	END

	-- GET PROMOTION ID --
	SELECT @id = [id] FROM [promotion]
	WHERE [discount] LIKE @discount

	RETURN @status
END
GO
