CREATE TABLE [dbo].[Leaves] (
    [LeaveID]       BIGINT           IDENTITY (1, 1) NOT NULL,
    [CorrelationID] UNIQUEIDENTIFIER NULL,
    [Category]      TINYINT          NULL,
    [Employee]      VARCHAR (50)     NULL,
    [StartDate]     DATETIME         NULL,
    [EndDate]       DATETIME         NULL,
    [Description]   VARCHAR (255)    NULL,
    [Duration]      TINYINT          NULL,
    [Status]        TINYINT          NULL,
    [IsCompleted]   BIT              NULL,
    [Remarks]       VARCHAR (255)    NULL,
    [DateSubmitted] DATETIME         NULL,
    CONSTRAINT [PK_Leaves] PRIMARY KEY CLUSTERED ([LeaveID] ASC)
);

