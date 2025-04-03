DELETE FROM Service.Request

ALTER TABLE Service.Request
ADD LastUpdatedUser INT NOT NULL

ALTER TABLE Service.Request
ADD LastUpdatedUserName VARCHAR(100) NOT NULL

ALTER TABLE Service.Request
ADD LastUpdatedDate datetime NOT NULL
