USE [certitrack]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Nem Ekpunobi
-- Create date: April 11, 2019
-- Description:	Get/Create Role
--	Returns Role Creation Status
-- =============================================
CREATE PROCEDURE stpGetCreateRole 
	@title varchar(30),
	@description varchar(255),
	@id int output
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @status int = 0

    -- CREATE ROLE IF DOESN'T EXIST --
	IF NOT EXISTS (
		SELECT 1 FROM [role]
		WHERE [title] LIKE @title
		AND @description IS NOT NULL )
	BEGIN
		INSERT INTO [role] ([title], [description])
		VALUES (@title, @description)

		SET @status = 1 -- creation successful
	END

	-- GET ROLE ID MATCHING ROLE DETAILS --
	SELECT @id = [id] FROM [role]
	WHERE [title] LIKE @title

	RETURN @status
END
GO
