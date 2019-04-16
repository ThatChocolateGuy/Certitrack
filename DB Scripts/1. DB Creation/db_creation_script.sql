-- =============================================
-- Author:		Nem Ekpunobi
-- Create date: April 1, 2019
-- Description:	Creates New CertiTrack Database
-- =============================================
--
----------------------------------------
-- CREATE DB
----------------------------------------
/*
	If DB creation fails, simply create it manually on server then run this script
*/

IF NOT EXISTS (SELECT name FROM master.sys.databases WHERE name = N'certitrack')
BEGIN
	CREATE DATABASE [certitrack];
	IF DB_ID('certitrack') IS NOT NULL
		PRINT 'CertiTrack DB Created';
	ELSE
		PRINT 'CertiTrack DB Creation Failed'
END
GO

USE [certitrack];
GO

----------------------------------------
-- PRIMARY TABLES
----------------------------------------

IF NOT EXISTS (
	SELECT * FROM [certitrack].INFORMATION_SCHEMA.TABLES   
    WHERE TABLE_SCHEMA = N'dbo' AND TABLE_NAME = N'staff')
BEGIN
	PRINT 'Creating [staff] Table';
	CREATE TABLE [staff] (
		[id] 		INT 			NOT NULL 	IDENTITY 	PRIMARY KEY,
		[name] 		VARCHAR(50) 	NOT NULL,
		[email] 	VARCHAR(50) 	NOT NULL,
		[password] 	VARCHAR(255) 	NOT NULL,
		[created] 	DATETIME 		NOT NULL 	DEFAULT CURRENT_TIMESTAMP
	); -- ALL WHO WILL HAVE ACCESS TO DB
END

IF NOT EXISTS (
	SELECT * FROM [certitrack].INFORMATION_SCHEMA.TABLES   
    WHERE TABLE_SCHEMA = N'dbo' AND TABLE_NAME = N'role')
BEGIN
	PRINT 'Creating [role] Table';
	CREATE TABLE [role] (
		[id]          	INT          	NOT NULL 	IDENTITY 	PRIMARY KEY,
		[title]       	VARCHAR (30) 	NOT NULL,
		[description] 	TEXT        	NOT NULL
	); -- ADMIN, THERAPIST, ETC.
END

IF NOT EXISTS (
	SELECT * FROM [certitrack].INFORMATION_SCHEMA.TABLES   
    WHERE TABLE_SCHEMA = N'dbo' AND TABLE_NAME = N'staff_type')
BEGIN
	PRINT 'Creating [staff_type] Table';
	CREATE TABLE [staff_type] (
		[id] 	INT 			NOT NULL 	IDENTITY 	PRIMARY KEY,
		[type] 	VARCHAR(45) 	NOT NULL
	); -- THERAPISTS, STUDENT THERAPISTS, OTHER STAFF
END

IF NOT EXISTS (
	SELECT * FROM [certitrack].INFORMATION_SCHEMA.TABLES   
    WHERE TABLE_SCHEMA = N'dbo' AND TABLE_NAME = N'certificate')
BEGIN
	PRINT 'Creating [certificate] Table';
	CREATE TABLE [certificate] (
		[id] 				INT 	NOT NULL 	IDENTITY 	PRIMARY KEY,
		[certificate_no] 	DECIMAL NOT NULL, 	-- generate unique no with date
		[DATE_issued] 		DATE 	NOT NULL 	DEFAULT CONVERT (DATE, GETDATE()),
		[DATE_redeemed] 	DATE 		NULL, 	-- must be after date issued (blank till redeemed)
		[expiry_DATE] 		DATE 	NOT NULL,
		[price] 			DECIMAL NOT NULL 	-- certificate value
	); -- ALL CERTIFICATES CREATED AND REDEEMED - DYNAMICALLY GENERATED ON ORDER CREATION
END

IF NOT EXISTS (
	SELECT * FROM [certitrack].INFORMATION_SCHEMA.TABLES   
    WHERE TABLE_SCHEMA = N'dbo' AND TABLE_NAME = N'channel')
BEGIN
	PRINT 'Creating [channel] Table';
	CREATE TABLE [channel] (
		[id]          INT           NOT NULL 	IDENTITY 	PRIMARY KEY,
		[channel]     VARCHAR (30)	NOT NULL
	); -- TRACKS CHANNEL OF DISTRIBUTION
END

IF NOT EXISTS (
	SELECT * FROM [certitrack].INFORMATION_SCHEMA.TABLES   
    WHERE TABLE_SCHEMA = N'dbo' AND TABLE_NAME = N'promotion')
BEGIN
	PRINT 'Creating [promotion] Table';
	CREATE TABLE [promotion] (
		[id] 		INT 	NOT NULL 	IDENTITY 	PRIMARY KEY,
		[discount] 	INT 	NOT NULL, -- default value is $0 or id of 1 in code
	); -- PROMOTION DISCOUNTS ($0, $10, $20, $30)
END

IF NOT EXISTS (
	SELECT * FROM [certitrack].INFORMATION_SCHEMA.TABLES   
    WHERE TABLE_SCHEMA = N'dbo' AND TABLE_NAME = N'customer')
BEGIN
	PRINT 'Creating [customer] Table';
	CREATE TABLE [customer] (
		[id] 		INT 			NOT NULL 	IDENTITY 	PRIMARY KEY,
		[name] 		VARCHAR(50) 	NOT NULL,
		[email] 	VARCHAR(50) 	NOT NULL,
		[phone] 	VARCHAR(15) 	NOT NULL
	); -- CERTIFICATE RECIPIENTS
END

----------------------------------------
-- RELATIONSHIP TABLES
----------------------------------------

IF NOT EXISTS (
	SELECT * FROM [certitrack].INFORMATION_SCHEMA.TABLES   
    WHERE TABLE_SCHEMA = N'dbo' AND TABLE_NAME = N'staff_link')
BEGIN
	PRINT 'Creating [staff_link] Table';
	CREATE TABLE [staff_link] (
		[staff_id] 		INT 	NOT NULL 	PRIMARY KEY,
		[role_id] 		INT 	NOT NULL,
		[staff_type_id] INT 	NOT NULL,

		CONSTRAINT FK_staff_sLink 		FOREIGN KEY ([staff_id]) 		REFERENCES [staff]([id]),
		CONSTRAINT FK_role_sLink 		FOREIGN KEY ([role_id]) 		REFERENCES [role]([id]),
		CONSTRAINT FK_staff_type_sLink 	FOREIGN KEY ([staff_type_id]) 	REFERENCES [staff_type]([id])

		-- these clauses ensure changes in [certificates] table are auto propagated to this table
		ON DELETE CASCADE
		ON UPDATE CASCADE
	); -- RESPONSIBLE FOR [staff] TABLE RELATIONSHIPS
END

IF NOT EXISTS (
	SELECT * FROM [certitrack].INFORMATION_SCHEMA.TABLES   
    WHERE TABLE_SCHEMA = N'dbo' AND TABLE_NAME = N'certificate_link')
BEGIN
	PRINT 'Creating [certificate_link] Table';
	CREATE TABLE [certificate_link] (
		[certificate_id] 	INT 	NOT NULL 	PRIMARY KEY,
		[staff_id] 			INT 	NULL, 		-- staff who issues cert
		[customer_id] 		INT 	NULL, 		-- recipient
		[promotion_id] 		INT 	NULL, 		-- promo table
		[channel_id] 		INT 	NULL, 		-- channel table

		CONSTRAINT FK_certificate_cLink FOREIGN KEY ([certificate_id]) 	REFERENCES [certificate]([id]),
		CONSTRAINT FK_staff_cLink 		FOREIGN KEY ([staff_id]) 		REFERENCES [staff]([id]),
		CONSTRAINT FK_customer_cLink 	FOREIGN KEY ([customer_id]) 	REFERENCES [customer]([id]),
		CONSTRAINT FK_promotion_cLink 	FOREIGN KEY ([promotion_id]) 	REFERENCES [promotion]([id]),
		CONSTRAINT FK_channel_cLink 	FOREIGN KEY ([channel_id]) 		REFERENCES [channel]([id])

		ON DELETE CASCADE
		ON UPDATE CASCADE
	); -- RESPONSIBLE FOR [certificate] TABLE RELATIONSHIPS & POPULATED SIMULTANEOUSLY WITH [certificate] TABLE
END

----------------------------------------
-- ORDER-HANDLING TABLES
----------------------------------------

IF NOT EXISTS (
	SELECT * FROM [certitrack].INFORMATION_SCHEMA.TABLES   
    WHERE TABLE_SCHEMA = N'dbo' AND TABLE_NAME = N'order')
BEGIN
	PRINT 'Creating [order] Table';
	CREATE TABLE [order] (
		[id] 			INT 	NOT NULL 	IDENTITY 	PRIMARY KEY,
		[customer_id]	INT 	NOT NULL,

		CONSTRAINT FK_customer_order FOREIGN KEY ([customer_id]) REFERENCES [customer]([id])

		ON DELETE CASCADE
		ON UPDATE CASCADE
	); -- WHEN CREATING ORDERS, CERTIFICATES WILL FIRST BE GENERATED (issued) THEN ASSIGNED TO CUSTOMER FOR EACH CERT PURCHASED
END

IF NOT EXISTS (
	SELECT * FROM [certitrack].INFORMATION_SCHEMA.TABLES
    WHERE TABLE_SCHEMA = N'dbo' AND TABLE_NAME = N'order_item')
BEGIN
	PRINT 'Creating [order_item] Table';
	CREATE TABLE [order_item] (
		[id] 				INT 	NOT NULL 	IDENTITY 	PRIMARY KEY,
		[order_id] 			INT 	NOT NULL,
		[certificate_id] 	INT 	NOT NULL

		CONSTRAINT FK_order_oItem 		FOREIGN KEY ([order_id]) 		REFERENCES [order]([id]),
		CONSTRAINT FK_certificate_oItem FOREIGN KEY ([certificate_id]) 	REFERENCES [certificate]([id])

		ON DELETE CASCADE
		ON UPDATE CASCADE
	); -- EACH CERTIFICATE ORDERED BY CUSTOMER
END