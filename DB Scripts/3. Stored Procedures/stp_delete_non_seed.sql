USE [certitrack]
GO
/****** Object:  StoredProcedure [dbo].[stpDeleteNonSeed]    Script Date: 4/11/2019 2:19:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[stpDeleteNonSeed] 
AS
BEGIN
	SET NOCOUNT ON;

	----------------------------------------
	-- DELETES CREATED CERTIFICATE LINKS
	----------------------------------------

	-- DELETE ANYTHING THAT ISN'T PART OF SEED SET
	DELETE FROM [certificate_link] WHERE [certificate_id] > '3'

	-- DISPLAY TABLE POST-DELETION
	SELECT * FROM [certificate_link];

	----------------------------------------
	-- DELETES GENERATED CERTIFICATES
	----------------------------------------

	-- DELETE ANYTHING THAT ISN'T PART OF SEED SET
	DELETE FROM [certificate] WHERE [id] > '3'

	-- DISPLAY TABLE POST-DELETION
	SELECT * FROM [certificate];

	----------------------------------------
	-- DELETES GENERATED ORDERS & ITEMS
	----------------------------------------

	-- DELETE ANYTHING THAT ISN'T PART OF SEED SET
	DELETE FROM [order_item] WHERE [id] > '3'
	DELETE FROM [order] WHERE [id] > '2'	

	-- DISPLAY TABLE POST-DELETION
	SELECT * FROM [order_item];
	SELECT * FROM [order];	

	----------------------------------------
	-- DELETES CREATED CUSTOMERS
	----------------------------------------

	-- DELETE ANYTHING THAT ISN'T PART OF SEED SET
	DELETE FROM [customer] WHERE [id] > '3'

	-- DISPLAY TABLE POST-DELETION
	SELECT * FROM [customer];

	----------------------------------------
	-- DELETES CREATED STAFF LINKS
	----------------------------------------

	-- DELETE ANYTHING THAT ISN'T PART OF SEED SET
	DELETE FROM [staff_link] WHERE [staff_id] > '3'

	-- DISPLAY TABLE POST-DELETION
	SELECT * FROM [staff_link];

	----------------------------------------
	-- DELETES CREATED STAFF
	----------------------------------------

	-- DELETE ANYTHING THAT ISN'T PART OF SEED SET
	DELETE FROM [staff] WHERE [id] > '3'

	-- DISPLAY TABLE POST-DELETION
	SELECT * FROM [staff];

	----------------------------------------
	-- DELETES CREATED STAFF TYPES
	----------------------------------------

	-- DELETE ANYTHING THAT ISN'T PART OF SEED SET
	DELETE FROM [staff_type] WHERE [id] > '3'

	-- DISPLAY TABLE POST-DELETION
	SELECT * FROM [staff_type];	

	----------------------------------------
	-- DELETES CREATED ROLES
	----------------------------------------

	-- DELETE ANYTHING THAT ISN'T PART OF SEED SET
	DELETE FROM [role] WHERE [id] > '2'

	-- DISPLAY TABLE POST-DELETION
	SELECT * FROM [role];

	----------------------------------------
	-- DELETES CREATED CHANNELS
	----------------------------------------

	-- DELETE ANYTHING THAT ISN'T PART OF SEED SET
	DELETE FROM [channel] WHERE [id] > '4'

	-- DISPLAY TABLE POST-DELETION
	SELECT * FROM [channel];

	----------------------------------------
	-- DELETES CREATED PROMOS
	----------------------------------------

	-- DELETE ANYTHING THAT ISN'T PART OF SEED SET
	DELETE FROM [promotion] WHERE [id] > '4'

	-- DISPLAY TABLE POST-DELETION
	SELECT * FROM [promotion];
END
