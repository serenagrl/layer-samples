CREATE TABLE [dbo].[LeaveStatusLogs] (
    [LogID]   BIGINT   IDENTITY (1, 1) NOT NULL,
    [LeaveID] BIGINT   NULL,
    [Status]  TINYINT  NULL,
    [Date]    DATETIME NULL,
    CONSTRAINT [PK_LeaveStatusLogs] PRIMARY KEY CLUSTERED ([LogID] ASC),
    CONSTRAINT [FK_LeaveStatusLogs_Leaves] FOREIGN KEY ([LeaveID]) REFERENCES [dbo].[Leaves] ([LeaveID])
);

