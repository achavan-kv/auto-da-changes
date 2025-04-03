INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
SELECT  1603, 'Search Service Requests', 16, 'Allow user to search the service requests via the Search Service Request screen.' UNION ALL
SELECT  1604, 'View Service Request', 16, 'Allow user to view a service request in the Service Request screen.' UNION ALL
SELECT  1605, 'Save Service Requests', 16, 'Allow user to save Service Request.' UNION ALL
SELECT  1606, 'Enable Food Loss', 16, 'Allow user to enter Food Loss in the Service Request screen.' UNION ALL
SELECT  1607, 'Print Service Request', 16, 'Allow user to print the service request.' UNION ALL
SELECT  1608, 'Enable Allocation', 16, 'Allow user to change the allocation section in Service Request.' UNION ALL
SELECT  1609, 'Enable Charge To', 16, 'Allow user to change the charge to section in Service Request.' UNION ALL
SELECT  1610, 'Enable Labour Cost', 16, 'Allow user to change the Labour Cost section in Service Request.' UNION ALL
SELECT  1611, 'Enable Resolution', 16, 'Allow user to change the resolution section in Service Request.' UNION ALL
SELECT  1612, 'Enable Finalize', 16, 'Allow user to change the Finalize section in Service Request.' UNION ALL
SELECT  1613, 'Print Invoice', 16, 'Allow user to print associated Invoice Service Request screen.' UNION ALL
SELECT  1614, 'Enable Estimate Update', 16, 'Allow user to change Estimates after save in Service Request Screen.' UNION ALL
SELECT  1615, 'View Technician Diary', 16, 'Allow user to view Techician Diary Screen.'