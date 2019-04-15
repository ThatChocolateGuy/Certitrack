-- =============================================
-- Author:		Nem Ekpunobi
-- Create date: April 1, 2019
-- Description:	Seeds CertiTrack Database
-- =============================================

USE [certitrack];
GO

IF NOT EXISTS (SELECT 1 FROM [staff])
BEGIN
	INSERT INTO [staff] ([name], [email], [password])
	VALUES
	('Admin', 'admin@certitrack.com', 'admin'),					-- 1
	('Therapist1', 'therapist1@certitrack.com', 'therapist'),	-- 2
	('Therapist2', 'therapist2@certitrack.com', 'therapist');	-- 3
END

IF NOT EXISTS (SELECT 1 FROM [role])
BEGIN
	INSERT INTO [role] ([title], [description])
	VALUES 
	('Admin', 'Can create, edit, and delete all users; roles; and certificates'),	-- 1
	('Therapist/Staff', 'Can view users, create and modify certificates');			-- 2
END

IF NOT EXISTS (SELECT 1 FROM [staff_type])
BEGIN
	INSERT INTO [staff_type] ([type])
	VALUES
	('Therapist'),			-- 1
	('Student Therapist'),	-- 2
	('Other Staff');		-- 3
END

IF NOT EXISTS (SELECT 1 FROM [certificate])
BEGIN
	INSERT INTO [certificate]
	([certificate_no], [expiry_date], [price])
	VALUES
	(20190331001, '2019-04-30', 30), -- 1
	(20190331002, '2019-04-30', 30), -- 2
	(20190331003, '2019-04-30', 30); -- 3
END

IF NOT EXISTS (SELECT 1 FROM [channel])
BEGIN
	INSERT INTO [channel] ([channel])
	VALUES
	('Assembly'),					-- 1
	('Clinic'),						-- 2
	('Friend'),						-- 3
	('New Student Orientation');	-- 4
END

IF NOT EXISTS (SELECT 1 FROM [promotion])
BEGIN
	INSERT INTO [promotion] ([discount])
	VALUES
	(0),	-- 1
	(10),	-- 2
	(20),	-- 3
	(30);	-- 4
END

IF NOT EXISTS (SELECT 1 FROM [customer])
BEGIN
	INSERT INTO [customer] ([name], email, phone)
	VALUES
	('customer1', 'customer1@gmail.com', '555-555-5555'), -- 1
	('customer2', 'customer2@gmail.com', '555-555-5556'), -- 2
	('customer3', 'customer3@gmail.com', '555-555-5557'); -- 3
END

IF NOT EXISTS (SELECT 1 FROM [staff_link])
BEGIN
	INSERT INTO [staff_link] ([staff_id], [role_id], [staff_type_id])
	VALUES
	(1, 1, 1), -- 1
	(2, 2, 1), -- 2
	(3, 2, 2); -- 3
END

IF NOT EXISTS (SELECT 1 FROM [certificate_link])
BEGIN
	INSERT INTO [certificate_link]
	([certificate_id], [staff_id], [customer_id], [promotion_id], [channel_id])
	VALUES
	(1, 1, 1, 4, 1), -- 1
	(2, 2, 1, 3, 1), -- 2
	(3, 3, 2, 4, 4); -- 3
END

IF NOT EXISTS (SELECT 1 FROM [order])
BEGIN
	INSERT INTO [order] ([customer_id])
	VALUES
	(1), -- 1
	(2); -- 2
END

IF NOT EXISTS (SELECT 1 FROM [order_item])
BEGIN
	INSERT INTO [order_item]
	([order_id], [certificate_id])
	VALUES
	(1, 1), -- 1
	(1, 2), -- 2
	(2, 3); -- 3
END