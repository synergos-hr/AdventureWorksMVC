-- CREATE SCHEMA Users;

CREATE VIEW Users.Users
AS
/******************************************************************************************************
  Name: Users
*******************************************************************************************************
  Version history:
    2018-07-18 Created
*******************************************************************************************************/
SELECT
  UserId = u.Id,
  u.UserName,
  u.Email,
  up.FirstName,
  up.LastName,
  FullName = rtrim(up.FirstName + ' ' + up.LastName),
  DisplayName = case when len(rtrim(isnull(up.FirstName + ' ' + up.LastName, ''))) = 0 then u.UserName else rtrim(up.FirstName + ' ' + up.LastName) end,
  up.Gender,
  TestUser = cast(isnull(up.TestUser, 0) as bit),
  Locked = cast(CASE WHEN isnull(u.LockoutEndDateUtc, getdate()) >= '2100-01-01' THEN 1 ELSE 0 END as bit),
  Roles = roles.RolesId,
  RolesTxt = roles.RolesTxt,
  MinRoleCode = roleCodes.MinCode,
  MaxRoleCode = roleCodes.MaxCode
FROM
  dbo.AspNetUsers u
  LEFT JOIN dbo.UserProfiles up ON up.UserId = u.Id
  OUTER APPLY 
  (
    SELECT 
      RolesId =
      (
        SELECT 
          CASE row_number() OVER (ORDER BY rl.[Code]) WHEN 1 THEN '' ELSE ',' END + cast(rl.[Id] as varchar) AS 'text()' 
        FROM 
          dbo.AspNetRoles rl 
          INNER JOIN dbo.AspNetUserRoles ur ON ur.RoleId = rl.Id 
        WHERE 
          ur.UserId = u.Id
        ORDER BY
          rl.Code
        FOR XML PATH('')
      ),
      RolesTxt =
      (
        SELECT 
          CASE row_number() OVER (ORDER BY rl.[Code]) WHEN 1 THEN '' ELSE ', ' END + rl.[Translation] AS 'text()' 
        FROM 
          dbo.AspNetRoles rl 
          INNER JOIN dbo.AspNetUserRoles ur ON ur.RoleId = rl.Id 
        WHERE 
          ur.UserId = u.Id
        ORDER BY
          rl.Code
        FOR XML PATH('')
      )
  ) roles
  OUTER APPLY 
  (
    SELECT 
      MinCode = min(rl.Code),
      MaxCode = max(rl.Code)
    FROM 
      dbo.AspNetRoles rl 
      INNER JOIN dbo.AspNetUserRoles ur ON ur.RoleId = rl.Id 
    WHERE 
      ur.UserId = u.Id
  ) roleCodes
