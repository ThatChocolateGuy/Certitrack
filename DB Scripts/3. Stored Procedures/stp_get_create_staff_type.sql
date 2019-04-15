USE [certitrack]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Nem Ekpunobi
-- Create date: April 11, 2019
-- Description:	Get/Create Staff Type
--	Returns Staff Type Creation Status
-- =============================================
CREATE PROCEDURE stpGetCreateStaffType
	@type varchar(45),
	@id int output
AS
BEGIN
	SET NOCOUNT ON;

    DECLARE @status int = 0

	-- CREATE STAFF TYPE IF DOESN'T EXIST --
	IF NOT EXISTS (
		SELECT 1 FROM [staff_type]
		WHERE [type] LIKE @type )
	BEGIN
		INSERT INTO [staff_type] ([type])
		VALUES (@type)

		SET @status = 1 -- creation successful
	END

	-- GET STAFF TYPE ID MATCHING SUPPLIED DETAILS --
	SELECT @id = [id] FROM [staff_type]
	WHERE [type] LIKE @type

	RETURN @status
END
GO
