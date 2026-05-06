


/* ======= This is 202005191123_addRoleEditKeyToSuperAdmin.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/


IF NOT EXISTS( SELECT 1 FROM RoleSecurityKey WHERE RoleId = 1 AND SecurityKeyId = 39)
	INSERT INTO RoleSecurityKey VALUES (1, 38), (1, 39)



/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



