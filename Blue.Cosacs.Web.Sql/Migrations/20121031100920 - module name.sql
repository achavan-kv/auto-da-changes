ALTER TABLE Admin.AdditionalProfile
ADD Module VARCHAR(50)
CONSTRAINT moduledefault DEFAULT '' NOT NULL



ALTER TABLE Admin.AdditionalProfile
DROP CONSTRAINT moduledefault 
