-- Create Menu table(11)
CREATE TABLE IF NOT EXISTS Menu (
MenuId SERIAL PRIMARY KEY,
Name VARCHAR(50) NOT NULL,
AccountType SMALLINT NOT NULL CHECK (AccountType IN (1, 2)),
SortOrder INT
);

INSERT INTO Menu (Name, AccountType, SortOrder) VALUES
('Regions', 1, 1),
('Vendorsinto', 1, 2),
('Scheduling', 1, 3),
('Profession', 1, 4),
('ProviderLocation', 1, 5),
('SMSLogs', 1, 6),
('History', 1, 7),
('SendOrder', 1, 8),
('HaloEmployee', 1, 9),
('Accounts', 1, 10),
('EmailLogs', 1, 11),
('HaloWorkPlace', 1, 12),
('MyProfile', 1, 13),
('Dashboard', 1, 14),
('HaloAdministrators', 1, 15),
('Chat', 1, 16),
('PatientRecords', 1, 17),
('Role', 1, 18),
('HaloUsers', 1, 19),
('Provider', 1, 20),
('Blocked History', 1, 21),
('RequestData', 1, 22),
('Concelled History', 1, 23),
('Invoicing', 1, 24);

INSERT INTO Menu (Name, AccountType, SortOrder) VALUES
('Dashboard', 2, 1),
('MySchedule', 2, 2),
('MyProfile', 2, 3),
('SendOrder', 2, 4),
('Chat', 2, 5),
('Invoicing', 2, 6);