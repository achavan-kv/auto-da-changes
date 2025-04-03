if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetMenusForRoleSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].DN_GetMenusForRoleSP
GO

CREATE PROCEDURE  dbo.DN_GetMenusForRoleSP  
   @userId int,  
   @screen varchar(50),  
   @return int OUTPUT  
  
AS  
  
 SET  @return = 0   --initialise return code  
  
  
    -- The disabled controls should be disabled in design view but this is  
    -- not always the case. So retrieve the full set of controls that can have  
    -- role permissions on this screen. Then update the ones that should be  
    -- enabled for this role.  
         
 SELECT  Screen,   
   Control,  
   Max(Visible) AS Visible,  
   max(Enabled) AS Enabled  
 FROM control c  
 INNER JOIN admin.UserPermissionsView up on c.Taskid = up.PermissionId  
 INNER JOIN Admin.[User] u ON u.Id = up.UserId  
 WHERE u.id = @userId  
 AND  c.Screen = @screen  
 GROUP BY screen, control  
 ORDER BY Screen, Control  
  
  
 IF (@@error != 0)  
 BEGIN  
  SET @return = @@error  
 END  
  