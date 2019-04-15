SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Nem Ekpunobi
-- Create date: April 9, 2019
-- Description:	Gets or Creates Staff
--				Returns Creation Status
-- =============================================
USE [certitrack]
GO

CREATE PROCEDURE stpGetCreateStaff 
	@name varchar(50),
	@email varchar(50),
	@password varchar(20),
	@id int output -- pass back staff id
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @status int = 0 -- staff creation status

	-- CREATE STAFF IF DOESN'T EXIST --
	IF NOT EXISTS (
		SELECT 1 FROM [staff]
		WHERE [email] LIKE @email )
	BEGIN
		INSERT INTO [staff] ([name], [email], [password])
		VALUES (@name, @email, @password)

		SET @status = 1 -- staff creation successful
	END

	-- GET STAFF iD --
	SELECT @id = [id] FROM [staff]
	WHERE [name] LIKE @name
	OR [email] LIKE @email

	SELECT @id AS 'Staff ID - stpGetCreateStaff'
	SELECT @status AS 'Staff Creation Status - stpGetCreateStaff'

	-- RETURN CREATION STATUS --
	RETURN @status
END
GO
