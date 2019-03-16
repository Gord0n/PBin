USE PBin;

SELECT * FROM [Post] ORDER BY DateCreated DESC;

SELECT * FROM [User];

SELECT * FROM [Role];

SELECT * FROM [UserRole];


/*
INSERT INTO [Role] VALUES (
	NEWID(),
	'Administrator'
);

INSERT INTO [Role] VALUES (
	NEWID(),
	'User'
);

--Admin
INSERT INTO [UserRole] VALUES (
	NEWID(),
	'6660EFFA-3884-44A6-8D12-D4E29B7D03D2',
	(SELECT [Id] FROM [User] WHERE EMAIL = 'cox.gordon@gmail.com')
);

--USER
INSERT INTO [UserRole] VALUES (
	NEWID(),
	'A3210815-B77D-4488-8D95-8191B4D90CB2',
	(SELECT [Id] FROM [User] WHERE EMAIL = 'cox.gordon@gmail.com')
);
	
*/