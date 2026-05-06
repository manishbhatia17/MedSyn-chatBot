


/* ======= This is 202005141924_updateRoleTableAddArchive.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/


IF COL_LENGTH('Role', 'IsArchived') IS NULL
	ALTER TABLE dbo.Role
		ADD IsArchived bit NOT NULL
		CONSTRAINT D_Role_IsArchived DEFAULT(0)


/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



