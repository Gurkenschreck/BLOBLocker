﻿DECLARE @CurrentMigration [nvarchar](max)

IF object_id('[dbo].[__MigrationHistory]') IS NOT NULL
    SELECT @CurrentMigration =
        (SELECT TOP (1) 
        [Project1].[MigrationId] AS [MigrationId]
        FROM ( SELECT 
        [Extent1].[MigrationId] AS [MigrationId]
        FROM [dbo].[__MigrationHistory] AS [Extent1]
        WHERE [Extent1].[ContextKey] = N'BLOBLocker.Entities.Models.Migrations.AT.BLATConfiguration'
        )  AS [Project1]
        ORDER BY [Project1].[MigrationId] DESC)

IF @CurrentMigration IS NULL
    SET @CurrentMigration = '0'

IF @CurrentMigration < '201604232304324_ATInitial'
BEGIN
    CREATE TABLE [dbo].[Accounts] (
        [ID] [int] NOT NULL IDENTITY,
        [Alias] [nvarchar](max),
        [Email] [nvarchar](max),
        [DerivedPassword] [varbinary](max),
        [LastLogin] [datetime],
        [Salt] [varbinary](max),
        [IsActive] [bit] NOT NULL,
        CONSTRAINT [PK_dbo.Accounts] PRIMARY KEY ([ID])
    )
    CREATE TABLE [dbo].[RoleLinks] (
        [ID] [int] NOT NULL IDENTITY,
        [Account_ID] [int],
        [Role_ID] [int],
        CONSTRAINT [PK_dbo.RoleLinks] PRIMARY KEY ([ID])
    )
    CREATE INDEX [IX_Account_ID] ON [dbo].[RoleLinks]([Account_ID])
    CREATE INDEX [IX_Role_ID] ON [dbo].[RoleLinks]([Role_ID])
    CREATE TABLE [dbo].[Roles] (
        [ID] [int] NOT NULL IDENTITY,
        [Definition] [nvarchar](max),
        CONSTRAINT [PK_dbo.Roles] PRIMARY KEY ([ID])
    )
    ALTER TABLE [dbo].[RoleLinks] ADD CONSTRAINT [FK_dbo.RoleLinks_dbo.Accounts_Account_ID] FOREIGN KEY ([Account_ID]) REFERENCES [dbo].[Accounts] ([ID])
    ALTER TABLE [dbo].[RoleLinks] ADD CONSTRAINT [FK_dbo.RoleLinks_dbo.Roles_Role_ID] FOREIGN KEY ([Role_ID]) REFERENCES [dbo].[Roles] ([ID])
    CREATE TABLE [dbo].[__MigrationHistory] (
        [MigrationId] [nvarchar](150) NOT NULL,
        [ContextKey] [nvarchar](300) NOT NULL,
        [Model] [varbinary](max) NOT NULL,
        [ProductVersion] [nvarchar](32) NOT NULL,
        CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY ([MigrationId], [ContextKey])
    )
    INSERT [dbo].[__MigrationHistory]([MigrationId], [ContextKey], [Model], [ProductVersion])
    VALUES (N'201604232304324_ATInitial', N'BLOBLocker.Entities.Models.Migrations.AT.BLATConfiguration',  0x1F8B0800000000000400ED5A4B6FE33610BE17E87F10746A8BAC95642F6D60EF22CFC2A8F340EC2C7A5BD012ED104B912A49A5368AFEB21EFA93FA173AD4839228C9961C27BB582C724928F29BF7708693FFFEF977F87E1552E7090B49381BB9478343D7C1CCE70161CB911BABC59B9FDDF7EFBEFF6E7819842BE743BEEFADDE0727991CB98F4A45279E27FD471C223908892FB8E40B35F079E8A1807BC78787BF7847471E060817B01C67781F3345429CFC017F9E73E6E348C5885EF3005399ADC3976982EADCA010CB08F978E49E4D6ECF26DCFF84C5E0124014C172909E1A9C06216133CEA9EB9C528280B729A60BD7418C718514707EF220F15409CE96D30816109DAD230CFB16884A9C4974526CEF2ADCE1B116CE2B0EE6507E2C150F7B021EBDCDB4E5D9C777D2B96BB409FA4C54B6D652273A1DB9A7BECFC118AE63D33A39A742EFEBA6F1418673E034EC3E308E03FEA57F0E9CF398AA58E011C3B112881E3877F19C12FF37BC9EF14F988D584C699971601DBE551660E94EF0080BB5BEC78B4C9CF185EB78D5739E7DD01C2B9D49251D33F5F6D8756E80389A536CFCA2A495A9E202FF8A191648E1E00E29850598751CE044B335EA16ADC42F7372E0891066AE738D5613CC96EA71E4C2AFAE73455638C85732161E1881A884434AC4781B95CB1011FAE2542EB0204F5A0752FEC94590D33B230C8975377A9B094C905413BE242C87BE009DCF206F6C3B384554ED9F9DB13CF515486C90C1EF31620DFE62E1DCA027B24CDCC742BC0700F0867B4C93AFF2914469CE1AE82F13C23E7D34F1792578A857D353E56F1F67482CB11698B76C98F258F8165F43AFC8051B33448EF6EC1491037DCB115DFDC318FFB91E923B40AB87E42ED4C77337B395EE68E049FFD2CE50F2B5899B5EFEBA175FFDE6A7DBF2FF823092D6257BBD6ADA4C7D2A25F749C2BB959B4C105419BE6481B32D2252C6CBAE0BFC8345490436042646EE4F35456CC035B9B8C035015A853D1C0C8E6CB94B1276143CF1F6ADDC55636E4F225742D5027D86B0A9EDA1255088806B66029F4D4E677A0DAF9A6A5528EBB3F8969943D9DC6BCC2956D5C40A776FE169B6B56A0AA842E46A68C228D4DA01A40DA076B8A4AF3A1BC53550DAD67655D8B9A7439C18CE2B927B3D90724F29211566B0F35C55D63E7A4855B74109F588E91033BB8B5F89120BA68FE0795234815134C95EDA25E7DDB4D7D24E0FAF5114417E2EB5D7D98A334D7BEBF337D3FE2D66986278BE6CE8340DB78612DC396889ADAF401A38BD22422AA8F1D11CE91BE23C086BDBCA69A025B4724A56A4D74D95C75C7E40FF9E279BEE1DAF0D5C68F50A040DE12A4D64C675B7AF9F4C1E3C1045A2E1563FE7340E595B65B0E974D6739601B2A5EE18594759C6C896BA63D4FAC5325AED6377DC529B58462C2D77C74A3BC7324CBAD21DA1E8122B1633AB75A4A167398CEDAA5ECD57ADA461FB7EA7C828F2D8DE43C3DC7FFD63A3FDE8F382E3332AF98514BCA3725F2BEB94BB836AA017EBAF6FA6EAF5DA1C104529B5A3DB17005DBD5B170E0D1D7653FD54575927E3E5284D46D44A33E477E02C2B9276E4AC374770FF0789033963A9DB5BD3527693D7AEB176F793B4D4DCD549D2D3FBF0907289B9A31112887DFA46B97AFEA21CA345D2ED5E512BC0ED2D26779942DC2AB88759F1BB7DC855AB86D32DAE03A23F914057C2D3B554381CE80D83E91FF49C12AC334EBEE11A31B2C052A54F50EEF1E1D1B13515FB7226549E9401ED36A67AF5673462D2F8A687B26D13804D5320F68484FF88C40F215AFD5886EA3DE9791652CB340720E7C9046527D0DA04270005AA6482D30FA83CD1791E4BF614674ED416FBF59F947C255E5ABA424B646A8F6F6316E0D5C8FD2B3976E28C7FFF589C3C706E0564A313E7D0F9BBAFA54CA6EE473C3BD69D72BFC9C2D761DBFADB7D8FE4F102EFF39592BBEFBB79E3C3DB8EEFDC60352CB45211858B5E2A01577DAD09B91384F92442D4E2BE5EE8747106AD5083687FB9C01166DACAB6785D686D2EFA0DB4E59ADB54F022638A1D26089FDDEC2D5DF8E7B5F9864A7EEF066F1ED5D49F723B0D6336CD62D2F217CA87390703A779A97578D036A9D938A869A2D0FAD2DF48A2758CD3065D877D8D098FA5EDCA8374978146E32CE44B1BE0D4987C15D17A8C68EA9D208461E99F22210748B22C20746FCBB05F0940B367CC163C4F041647F916EBFABFC60A41298E4E85220BE42BF8EC632993E1FD0744E3A4B398E360CC6E6315C50A44C6E19C56FE2540E7934DF493395495E7E16D94FCBBC83E44003689EE266ED9594C6860F8BE6A28E05B2074A2CA6A2A6D4BA56BABE5DA20DD70D61128539FC9AF331C4614C0E42D9B22DD6AF4E7ED41E2095E227F9D37F4ED20DB0D5155FBF082A0A540A1CC308AF3F027F87010AEDEFD0F42F5E33B1B2C0000 , N'6.1.3-40302')
END

