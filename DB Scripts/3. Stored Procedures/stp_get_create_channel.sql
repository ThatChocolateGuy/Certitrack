USE [certitrack]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Nem Ekpunobi
-- Create date: April 10, 2019
-- Description:	Get/Create Channel
--	Returns Channel Creation Status
-- =============================================
CREATE PROCEDURE stpGetCreateChannel 
	@channel varchar(30),
	@id int output
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @status int = 0
	
	-- CREATE CHANNEL IF DOESN'T EXIST --
	IF NOT EXISTS (
		SELECT 1 FROM [channel]
		WHERE [channel] LIKE @channel )
	BEGIN
		INSERT INTO [channel] ([channel])
		VALUES (@channel)

		SET @status = 1 -- creation successful
	END

	-- GET CHANNEL ID --
	SELECT @id = [id] FROM [channel]
	WHERE [channel] LIKE @channel

	RETURN @status
END
GO
