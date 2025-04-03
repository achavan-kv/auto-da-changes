-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 

INSERT INTO [Admin].Permission (id, Name, CategoryId, [Description])
VALUES (386, 'List User Sessions', 12, 'Allows access to the page that lists currently active user sessions')

INSERT INTO [Admin].Permission (id, Name, CategoryId, [Description])
VALUES (387, 'Kill User Session', 12, 'Allows killing/terminating an active user session')
