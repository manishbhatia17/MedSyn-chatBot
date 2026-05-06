/*
	Keep this script in the solution to create the
	DB Version Table to keep track of the database versioning.
*/

CREATE TABLE [dbo].[_DBVersion] (
	[DBVersionId] INTEGER IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Version] VARCHAR(16),
	[RunDate] DATETIME NOT NULL DEFAULT GETDATE()
);